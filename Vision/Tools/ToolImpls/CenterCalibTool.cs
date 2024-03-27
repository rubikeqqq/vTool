using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.PMAlign;
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
    public class CenterCalibTool : ToolBase, IVpp, ICenterCalib, IPointIn, IPointOut
    {
        //DetectTool工具完成得到的点位 传入此旋转工具进行计算
        [NonSerialized]
        private Station _station;

        [NonSerialized]
        private CenterDetectTool _detectTool;

        [NonSerialized]
        private KkRobotCalibTool _robotTool;

        public bool IsCalibed { get; set; }

        public PointD CenterPoint { get; set; }

        [field: NonSerialized]
        public bool IsLoaded { get; set; }

        /// <summary>
        /// 标定vpp 标定时使用
        /// </summary>
        [field: NonSerialized]
        public CogToolBlock ToolBlock { get; set; }

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

        [field: NonSerialized]
        public PointA PointOut { get; set; }

        /// <summary>
        /// 旋转标定时机械手点位
        /// </summary>
        public PointD CenterCalibRobotPoint { get; set; } = new PointD();

        /// <summary>
        /// 机械手的示教位
        /// </summary>
        public PointA RobotOriginPosition { get; set; } = new PointA();

        [field: NonSerialized]
        public UserControl UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            return UI ?? (UI = new UcCenterCalibTool(station, this));
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
            if (_robotTool == null)
            {
                _robotTool = (KkRobotCalibTool)_station.GetRobotTool(0);
                if (_robotTool == null)
                    throw new ToolException("旋转检测不存在！");
            }
            if (_detectTool == null)
            {
                _detectTool = (CenterDetectTool)_station.GetModelTool(0);
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
                var a2 = _detectTool.ModelOriginPoint.Angle;
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


                RotatedAffine.Math_Transfer(_detectTool.ModelOriginPoint.X, _detectTool.ModelOriginPoint.Y, deltaAngle,
                   CenterPoint.X + CenterRobotDelta.X, CenterPoint.Y + CenterRobotDelta.Y,
                   out var rotatedX, out var rotatedY);

                var deltaX = PointIn.X - rotatedX;
                var deltaY = PointIn.Y - rotatedY;


                //系统补偿
                var offset = GetOffset();

                PointA point = new PointA();

                //机械手示教位 + 系统补偿 + kk机械手的偏移量+ delta 
                point.X = (RobotOriginPosition.X + offset.X) + RobotDelta.X + deltaX;
                point.Y = (RobotOriginPosition.Y + offset.Y) + RobotDelta.Y + deltaY;
                //角度就是当前角度 - 模板角度
                point.Angle = deltaAngle * 180 / Math.PI + RobotOriginPosition.Angle;

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
            PointIn = ((IModelPoint)_detectTool).ModelPoint ?? new PointA();
            RobotDelta = ((IRobotDeltaPoint)_robotTool).RobotDelta ?? new PointD();

            if (CenterRobotDelta == null)
            {
                CenterRobotDelta = new PointD();
            }
            if (CenterCalibRobotPoint == null)
            {
                CenterCalibRobotPoint = new PointD();
            }
            CenterRobotDelta.X = RobotOriginPosition.X - CenterCalibRobotPoint.X;
            CenterRobotDelta.Y = RobotOriginPosition.Y - CenterCalibRobotPoint.Y;
        }

        /// <summary>
        /// 系统补偿
        /// </summary>
        /// <returns></returns>
        private PointD GetOffset()
        {
            var point = ProjectManager.Instance.ProjectData.Offset;
            return point;
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

        public void AddToolBlock()
        {
            if (IsLoaded)
            {
                AddTools(ToolBlock);
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

            if (!tb.Tools.Contains(acqTool.Name))
            {
                tb.Tools.Add(acqTool);
            }


            //自动寻找之前的9点标定
            var tool = _station.GetNPointTool();
            if (tool != null)
            {
                foreach (CogToolBase t in tool.ToolBlock.Tools)
                {
                    if (t is CogCalibNPointToNPointTool cTool)
                    {
                        if (!tb.Tools.Contains(cTool.Name))
                        {
                            tb.Tools.Add(cTool);
                        }
                    }
                }
                foreach (CogToolBase t in tool.ToolBlock.Tools)
                {
                    if (t is CogPMAlignTool pTool)
                    {
                        if (!tb.Tools.Contains(pTool.Name))
                        {
                            tb.Tools.Add(pTool);
                        }
                    }
                }
                foreach (CogToolBase t in tool.ToolBlock.Tools)
                {
                    if (t is CogFindCircleTool fcTool)
                    {
                        if (!tb.Tools.Contains(fcTool.Name))
                        {
                            tb.Tools.Add(fcTool);
                        }
                    }
                }
            }
            else
            {
                "请先完成9点标定！".MsgBox();
            }

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

            if (!tb.Tools.Contains(fitCircleTool.Name))
            {
                tb.Tools.Add(fitCircleTool);
            }
        }

        #endregion

    }
}
