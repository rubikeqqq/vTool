using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.ID;
using Cognex.VisionPro.ToolBlock;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [ToolName("旋转标定", 1)]
    [GroupInfo(name: "标定工具", index: 1)]
    [Description("旋转中心标定工具")]
    public class CenterCalibTool : ToolBase, IVpp, IImageIn, IImageOut
    {
        //DetectTool工具完成得到的点位 传入此旋转工具进行计算
        [NonSerialized]
        private Station _station;

        [NonSerialized]
        private CenterDetectTool _detectTool;

        [NonSerialized]
        private KkRobotCalibTool _robotTool;

        /// <summary>
        /// 是否已经标定
        /// </summary>
        public bool IsCalibed { get; set; }

        [field: NonSerialized]
        public bool IsLoaded { get; set; }

        /// <summary>
        /// 标定vpp 标定时使用
        /// </summary>
        [field: NonSerialized]
        public CogToolBlock ToolBlock { get; set; }

        /// <summary>
        /// 输入点位
        /// </summary>
        [field: NonSerialized]
        public PointA PointIn { get; set; }

        /// <summary>
        /// 由KK移动 计算得到的机械手的 偏移量
        /// </summary>
        [field: NonSerialized]
        public PointD RobotDelta { get; set; }

        /// <summary>
        /// 机械手旋转中心的偏移值
        /// </summary>
        [field: NonSerialized]
        public PointD CenterRobotDelta { get; set; }

        public string ImageInName { get; set; }

        [field: NonSerialized]
        public ICogImage ImageIn { get; set; }

        /// <summary>
        /// 数据输出的点
        /// </summary>
        [field: NonSerialized]
        public PointA PointOut { get; set; }

        [field: NonSerialized]
        public UcCenterCalibTool UI { get; set; }

        [field: NonSerialized]
        public ICogImage ImageOut {  get; set; }

        public override UserControl GetToolControl(Station station)
        {
            if (UI == null)
            {
                UI = new UcCenterCalibTool(station, this);
            }
            else
            {
                UI.GetImageIn();
            }
            
            return UI;
        }

        public override void Save()
        {
            SaveVpp();
            base.Save();
        }

        #region 工具运行相关

        public override void Run()
        {
            if (!Enable) return;
            GetImageIn();
            if (ImageIn != null)
            {
                if (ToolBlock != null)
                {
                    ToolBlock.Inputs["InputImage"].Value = ImageIn;
                    ToolBlock.Run();
                    if (ToolBlock.RunStatus.Result != CogToolResultConstants.Accept)
                    {
                        throw new ToolException($"工具[{ToolName}]运行失败！")
                        {
                            ToolName = ToolName,
                        };
                    }
                    ImageOut = (ICogImage)ToolBlock.Outputs["OutputImage"].Value;
                }
            }
            else
            {
                throw new ToolException($"[{ToolName}]没有输入图像");
            }


            if (_robotTool == null)
            {
                _robotTool = (KkRobotCalibTool)_station.GetRobotCalibTool(0);
                if (_robotTool == null)
                    throw new ToolException("旋转检测不存在！");
            }
            if (_detectTool == null)
            {
                _detectTool = (CenterDetectTool)_station.GetCenterDetectTool(0);
                if (_detectTool == null)
                    throw new ToolException("KK机械手标定不存在！");
            }
            GetRobotCenterData();
            PointOut = GetCenterCalibPoint();
        }

        public override void RunDebug()
        {
            Run();
        }

        public override void Close()
        {
            base.Close();
            CloseCam();
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
            if (_station == null || ImageInName == null) return false;

            var tool = _station[ImageInName];

            if (tool == null) return false;

            ImageIn = ((IImageOut)tool).ImageOut;
            return true;
        }

        /// <summary>
        /// 计算当机械手旋转后的坐标
        /// </summary>
        /// <returns></returns>
        private PointA GetCenterCalibPoint()
        {
            try
            {
                if (!IsCalibed)
                {
                    LogNet.Log($"[{_station.StationName}]旋转中心未标定！");
                    return null;
                }

                #region 带旋转标定


                //相当于将机械手从原模板位移动到现在的位置
                var a1 = PointIn.Angle;
                var a2 = _station.DataConfig.CalibConfig.ModelOriginPoint.Angle;
                //if (a2 < -Math.PI / 2)
                //{
                //    a2 = a2 + Math.PI;
                //}

                var deltaAngle = a1 - a2;


                //计算模板点的实际坐标绕旋转中心旋转后得到的新的坐标
                //RotatedAffine.Math_Transfer(PointIn.X, PointIn.Y, deltaAngle,
                //    CenterPoint.X + CenterRobotDelta.X, CenterPoint.Y + CenterRobotDelta.Y,
                //    out var rotatedX, out var rotatedY);

                // 旋转后的坐标 - 模板的坐标 = delta
                //var deltaX = rotatedX - _station.ModelPosition.X ;
                //var deltaY = rotatedY - _station.ModelPosition.Y ;


                RotatedAffine.Math_Transfer(_station.DataConfig.CalibConfig.ModelOriginPoint.X, _station.DataConfig.CalibConfig.ModelOriginPoint.Y, deltaAngle,
                    _station.DataConfig.CalibConfig.CenterPoint.X + CenterRobotDelta.X, _station.DataConfig.CalibConfig.CenterPoint.Y + CenterRobotDelta.Y,
                   out var rotatedX, out var rotatedY);

                var deltaX = PointIn.X - rotatedX;
                var deltaY = PointIn.Y - rotatedY;


                //系统补偿
                var offset = GetOffset();

                PointA point = new PointA();

                //机械手示教位 + 系统补偿 + kk机械手的偏移量+ delta 
                point.X = (_station.DataConfig.CalibConfig.RobotOriginPosition.X + offset.X) + RobotDelta.X + deltaX;
                point.Y = (_station.DataConfig.CalibConfig.RobotOriginPosition.Y + offset.Y) + RobotDelta.Y + deltaY;
                //角度就是当前角度 - 模板角度
                point.Angle = deltaAngle * 180 / Math.PI + _station.DataConfig.CalibConfig.RobotOriginPosition.Angle;

                LogUI.AddLog($"=>{point}");
                return point;
                #endregion
            }
            catch (Exception ex)
            {
                ex.Message.MsgBox();
                return null;
            }
        }

        /// <summary>
        /// 获取此前工具的输出数据 和机械手偏差值 
        /// </summary>
        private void GetRobotCenterData()
        {
            PointIn = _detectTool.ModelPoint ?? new PointA();
            RobotDelta = _robotTool.RobotDelta ?? new PointD();

            CenterRobotDelta.X = _station.DataConfig.CalibConfig.RobotOriginPosition.X - _station.DataConfig.CalibConfig.CenterCalibRobotPoint.X;
            CenterRobotDelta.Y = _station.DataConfig.CalibConfig.RobotOriginPosition.Y - _station.DataConfig.CalibConfig.CenterCalibRobotPoint.Y;
        }

        /// <summary>
        /// 系统补偿
        /// </summary>
        /// <returns></returns>
        private PointD GetOffset()
        {
            var point = _station.DataConfig.OffsetConfig;
            return new PointD(point.OffsetX, point.OffsetY);
        }

        #endregion 

        #region vpp相关

        public void CreateVpp()
        {
            if (!IsLoaded)
            {
                var toolPath = Path.Combine(ProjectManager.ProjectDir, _station.StationName, $"{ToolName}.vpp");
                if (string.IsNullOrEmpty(toolPath))
                {
                    throw new Exception("vpp的路径不存在");
                }

                ToolBlock = new CogToolBlock();
                ToolBlock.Inputs.Add(new CogToolBlockTerminal("InputImage", typeof(ICogImage)));
                ToolBlock.Outputs.Add(new CogToolBlockTerminal("OutputImage", typeof(ICogImage)));
                AddTools(ToolBlock);
                IsLoaded = true;
            }
        }

        public void LoadVpp()
        {
            if (!IsLoaded)
            {
                try
                {
                    var toolPath = Path.Combine(ProjectManager.ProjectDir, _station.StationName, $"{ToolName}.vpp");
                    ToolBlock = CogSerializer.LoadObjectFromFile(toolPath) as CogToolBlock;
                    IsLoaded = true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"工具vpp加载失败.\r\n{ex.Message}");
                }
            }
        }

        public void RemoveVpp()
        {
            if (!IsLoaded)
            {
                try
                {
                    var toolPath = Path.Combine(ProjectManager.ProjectDir, _station.StationName, $"{ToolName}.vpp");
                    if (File.Exists(toolPath))
                    {
                        File.Delete(toolPath);
                        IsLoaded = false;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"工具vpp删除失败.\r\n{ex.Message}");
                }
            }
        }

        public void SaveVpp()
        {
            if (IsLoaded)
            {
                var toolPath = Path.Combine(ProjectManager.ProjectDir, _station.StationName, $"{ToolName}.vpp");
                CogSerializer.SaveObjectToFile(ToolBlock, toolPath);
            }
        }

        /// <summary>
        /// 关闭相机
        /// </summary>
        public void CloseCam()
        {
            if (ToolBlock.Tools.Contains("CogAcqFifoTool1"))
            {
                var acqTool = ToolBlock.Tools["CogAcqFifoTool1"] as CogAcqFifoTool;
                if (acqTool != null && acqTool.Operator != null && acqTool.Operator.FrameGrabber != null)
                {
                    acqTool.Operator.FrameGrabber.Disconnect(true);
                    acqTool.Dispose();
                }
            }
        }

        /// <summary>
        /// 添加工具
        /// </summary>
        /// <param name="tb"></param>
        private void AddTools(CogToolBlock tb)
        {
            CogAcqFifoTool acqTool = new CogAcqFifoTool();
            acqTool.Name = "CogAcqFifoTool1";
            string[] s1 = new string[1];
            s1[0] = "|OutputImage|OutputImage";

            acqTool.UserData.Add("_ToolOutputTerminals", s1);

            CogIDTool idTool = new CogIDTool();
            idTool.Name = "CogIDTool1";
            string[] s2 = new string[1];
            s2[0] = "|InputImage|InputImage";
            string[] s3 = new string[1];
            s3[0] = "|Result.Count|Result.Count";

            idTool.UserData.Add("_ToolOutputTerminals", s2);
            idTool.UserData.Add("_ToolOutputTerminals", s3);



            CogCalibNPointToNPointTool nTool = new CogCalibNPointToNPointTool();
            nTool.Name = "CogCalibNPointToNPointTool1";
            string[] s4 = new string[1];
            string[] s5 = new string[1];
            s4[0] = "|InputImage|InputImage";
            s5[0] = "|OutputImage|OutputImage";

            nTool.UserData.Add("_ToolInputTerminals", s4);//添加终端-InputImage
            nTool.UserData.Add("_ToolOutputTerminals", s5);

            CogFitCircleTool fitCircleTool = new CogFitCircleTool();
            fitCircleTool.Name = "CogFitCircleTool1";
            string[] s8 = new string[1];
            string[] s9 = new string[4];
            s8[0] = "|InputImage|InputImage";

            s9[0] = "|Result.GetCircle()|Result.GetCircle()";
            s9[1] = "|Result.GetCircle().CenterX|Result.GetCircle().CenterX";
            s9[2] = "|Result.GetCircle().CenterY|Result.GetCircle().CenterY";
            s9[3] = "|Result.GetCircle().Radius|Result.GetCircle().Radius";

            fitCircleTool.UserData.Add("_ToolInputTerminals", s8);//添加终端-InputImage
            fitCircleTool.UserData.Add("_ToolOutputTerminals", s9);

            tb.Tools.Add(acqTool);
            tb.Tools.Add(idTool);
            tb.Tools.Add(nTool);
            tb.Tools.Add(fitCircleTool);
        }

        #endregion

    }
}
