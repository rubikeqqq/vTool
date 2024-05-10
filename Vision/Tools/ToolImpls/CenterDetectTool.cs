using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;

using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [GroupInfo(name: "视觉工具",index: 2)]
    [ToolName("旋转检测",1)]
    [Description("主检测流程,旋转中心使用")]
    public class CenterDetectTool : ToolBase, IVpp, IImageIn
    {
        private Station _station;

        public string ImageInName { get; set; }

        public CogToolBlock ToolBlock { get; set; }

        public ICogImage ImageIn { get; set; }

        public bool IsLoaded { get; set; }

        /// <summary>
        /// 模板输出点位
        /// </summary>
        public PointA ModelPoint { get; set; }

        public UcCenterDetectTool UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            if(UI == null)
            {
                UI = new UcCenterDetectTool(station,this);
            }
            else
            {
                //刷新图像源
                UI.GetImageIn();
            }

            return UI;
        }

        public override void Save()
        {
            SaveVpp();
            base.Save();
        }

        #region Vpp相关

        public void CreateVpp()
        {
            if(!IsLoaded)
            {
                var toolPath = Path.Combine(ProjectManager.ProjectDir,_station.StationName,$"{ToolName}.vpp");
                if(string.IsNullOrEmpty(toolPath))
                {
                    throw new Exception("vpp的路径不存在");
                }

                ToolBlock = new CogToolBlock();
                ToolBlock.Inputs.Add(new CogToolBlockTerminal("InputImage",typeof(ICogImage)));
                //加上旋转标定计算得到的点位
                ToolBlock.Outputs.Add(new CogToolBlockTerminal("X",typeof(double)));
                ToolBlock.Outputs.Add(new CogToolBlockTerminal("Y",typeof(double)));
                ToolBlock.Outputs.Add(new CogToolBlockTerminal("Angle",typeof(double)));
                IsLoaded = true;
            }
        }

        public void LoadVpp()
        {
            if(!IsLoaded)
            {
                try
                {
                    var toolPath = Path.Combine(ProjectManager.ProjectDir,_station.StationName,$"{ToolName}.vpp");
                    ToolBlock = CogSerializer.LoadObjectFromFile(toolPath) as CogToolBlock;
                    IsLoaded = true;
                }
                catch(Exception ex)
                {
                    throw new Exception($"工具vpp加载失败.\r\n{ex.Message}");
                }
            }
        }

        public void SaveVpp()
        {
            if(IsLoaded)
            {
                var toolPath = Path.Combine(ProjectManager.ProjectDir,_station.StationName,$"{ToolName}.vpp");
                CogSerializer.SaveObjectToFile(ToolBlock,toolPath);
                //LogUI.AddLog("vpp保存后需检查结果编辑工具！");
            }
        }

        public void RemoveVpp()
        {
            if(!IsLoaded)
            {
                try
                {
                    var toolPath = Path.Combine(ProjectManager.ProjectDir,_station.StationName,$"{ToolName}.vpp");
                    if(File.Exists(toolPath))
                    {
                        File.Delete(toolPath);
                        IsLoaded = false;
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception($"工具vpp删除失败.\r\n{ex.Message}");
                }
            }
        }

        #endregion

        #region 工具相关

        public override void Run()
        {
            RunTime = TimeSpan.Zero;
            if(!Enable) return;
            GetImageIn();
            if(ImageIn != null)
            {
                Stopwatch sw = Stopwatch.StartNew();
                if(ToolBlock != null)
                {
                    ToolBlock.Inputs["InputImage"].Value = ImageIn;

                    //复位结果数据
                    ResetOutput();

                    //运行
                    ToolBlock.Run();
                    if(ToolBlock.RunStatus.Result != CogToolResultConstants.Accept)
                    {
                        LogNet.Log($"[{ToolName}] NG!");
                        throw new Exception($"[{ToolName}] NG!");
                    }

                    //获取运行结果
                    if(ToolBlock.Outputs["X"].Value != null &&
                        ToolBlock.Outputs["Y"].Value != null &&
                        ToolBlock.Outputs["Angle"].Value != null)
                    {
                        ModelPoint = new PointA(
                            (double)ToolBlock.Outputs["X"].Value,
                            (double)ToolBlock.Outputs["Y"].Value,
                            (double)ToolBlock.Outputs["Angle"].Value);
                    }

                    //计算机械手旋转后的坐标
                    ModelPoint = GetRobotPoint();

                    if (ToolBlock.CreateLastRunRecord().SubRecords.Count > 0)
                    {
                        if (string.IsNullOrEmpty(_station.LastRecordName))
                        {
                            _station.ShowImage = ToolBlock.CreateLastRunRecord().SubRecords[0];
                        }
                        else
                        {
                            _station.ShowImage = ToolBlock.CreateLastRunRecord().SubRecords[_station.LastRecordName];
                        }
                    }
                }
                sw.Stop();
                RunTime = sw.Elapsed;
            }
            else
            {   
                LogNet.Log($"[{ToolName}] 输入图像不存在！");
                throw new Exception($"[{ToolName}] 输入图像不存在！");
            }
        }

        public override void RunDebug()
        {
            Run();
        }

        /// <summary>
        /// 根据名称获取结果
        /// 给结果配置工具使用
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        public object GetValue(string sourceName)
        {
            return ToolBlock.Outputs[sourceName].Value;
        }

        public void RegisterStation(Station station)
        {
            _station = station;
        }

        /// <summary>
        /// 获取输入图像
        /// </summary>
        private bool GetImageIn()
        {
            if(_station == null || ImageInName == null) return false;

            var tool = _station[ImageInName];

            if(tool == null) return false;

            ImageIn = ((IImageOut)tool).ImageOut;
            return true;
        }

        /// <summary>
        /// 复位vpp输出
        /// </summary>
        private void ResetOutput()
        {
            var terminals = ToolBlock.Outputs;
            foreach(CogToolBlockTerminal terminal in terminals)
            {
                var type = terminal.ValueType;
                switch(type.Name)
                {
                    case nameof(Boolean):
                        terminal.Value = false;
                        break;
                    case nameof(String):
                        terminal.Value = "";
                        break;
                    case nameof(Int32):
                    case nameof(Int16):
                        terminal.Value = 0;
                        break;
                    case nameof(Double):
                        terminal.Value = 0.0;
                        break;
                }
            }
        }

        /// <summary>
        /// 计算当机械手旋转后的坐标
        /// </summary>
        /// <returns></returns>
        private PointA GetRobotPoint()
        {
            try
            {
                //看看旋转中心有没有标定
                var centerX = _station.DataConfig.CalibConfig.CenterPoint.X;
                var centerY = _station.DataConfig.CalibConfig.CenterPoint.Y;

                if(centerX == 0 && centerY == 0)
                {
                    LogUI.AddLog($"[{_station.StationName}]旋转中心未标定！");
                    return null;
                }

                //====================================== 旋转中心带入计算机械手移动之后的位置 ========================================

                //相当于将机械手从原模板位移动到现在的位置
                var a1 = ModelPoint.Angle;  //现在找到的模板角度
                var a2 = _station.DataConfig.CalibConfig.ModelOriginPoint.Angle;  //保存的模板角度
                //if (a2 < -Math.PI / 2)
                //{
                //    a2 = a2 + Math.PI;
                //}

                var deltaAngle = a1 - a2;    //角度插值


                //获取旋转中心的固定偏差
                PointD _centerDelta = new PointD();
                _centerDelta.X = _station.DataConfig.CalibConfig.RobotOriginPosition.X - _station.DataConfig.CalibConfig.CenterCalibRobotPoint.X;
                _centerDelta.Y = _station.DataConfig.CalibConfig.RobotOriginPosition.Y - _station.DataConfig.CalibConfig.CenterCalibRobotPoint.Y;

                //计算模板点的实际坐标绕旋转中心旋转后得到的新的坐标
                RotatedAffine.Math_Transfer(_station.DataConfig.CalibConfig.ModelOriginPoint.X,_station.DataConfig.CalibConfig.ModelOriginPoint.Y,deltaAngle,
                    _station.DataConfig.CalibConfig.CenterPoint.X + _centerDelta.X,_station.DataConfig.CalibConfig.CenterPoint.Y + _centerDelta.Y,
                   out var rotatedX,out var rotatedY);

                // 模板的坐标 - 旋转后的坐标 = delta
                var deltaX = ModelPoint.X - rotatedX;
                var deltaY = ModelPoint.Y - rotatedY;


                //系统补偿
                var offset = _station.DataConfig.OffsetConfig;

                //KK移动的偏差
                PointD _robotOffset = new PointD();
                var _robotTool = (KkRobotCalibTool)_station.GetRobotCalibTool(0);
                _robotOffset = _robotTool.RobotDelta ?? new PointD();


                PointA point = new PointA();

                //机械手示教位 + 系统补偿 + kk机械手的偏移量+ delta 
                point.X = (_station.DataConfig.CalibConfig.RobotOriginPosition.X + offset.OffsetX) + _robotOffset.X + deltaX;
                point.Y = (_station.DataConfig.CalibConfig.RobotOriginPosition.Y + offset.OffsetY) + _robotOffset.Y + deltaY;
                //角度就是当前角度 - 模板角度
                point.Angle = deltaAngle * 180 / Math.PI + _station.DataConfig.CalibConfig.RobotOriginPosition.Angle;

                LogUI.AddLog($"=>{point}");
                return point;
                
            }
            catch(Exception ex)
            {
                ex.Message.MsgBox();
                return null;
            }
        }
        #endregion

        #region ISerializable
        public override void LoadFromStream(SerializationInfo info,string toolName)
        {
            base.LoadFromStream(info,toolName);
            string imageInName = $"{toolName}.imageInName";

            ImageInName = info.GetString(imageInName);
        }

        public override void SaveToStream(SerializationInfo info,string toolName)
        {
            base.SaveToStream(info,toolName);

            string imageInName = $"{toolName}.imageInName";

            info.AddValue(imageInName,ImageInName);
        }
        #endregion
    }
}