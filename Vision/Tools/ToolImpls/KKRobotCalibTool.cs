using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [GroupInfo("标定工具", 1)]
    [ToolName("kk标定", 2)]
    [Description("kk轴和机械手的标定工具")]
    public class KkRobotCalibTool : ToolBase, IVpp
    {
        [NonSerialized]
        private Station _station;

        [field: NonSerialized]
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// 机械手偏移值
        /// </summary>
        [field: NonSerialized]
        public PointD RobotDelta { get; set; }

        /// <summary>
        /// 9点标定vpp
        /// </summary>
        [field: NonSerialized]
        public CogCalibNPointToNPointTool CalibTool { get; set; }

        [field: NonSerialized]
        public UcKkRobotTool UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            return UI ?? (UI = new UcKkRobotTool(this));
        }

        public override void Save()
        {
            SaveVpp();
            base.Save();
        }

        #region 【工具相关】

        public override void Run()
        {
            RunTime = TimeSpan.Zero;
            if (!Enable)
                return;
            if (CalibTool == null)
            {
                LogNet.Log("CogCalibNPointToNPointTool不存在！");
                throw new Exception("CogCalibNPointToNPointTool不存在！");
            }

            //先获取kk的原始坐标
            var kkOrigin = _station.DataConfig.KKConfig.KKOriginPosition;

            //读取现在的kk坐标
            if (
                string.IsNullOrEmpty(_station.DataConfig.KKConfig.AddressX)
                || string.IsNullOrEmpty(_station.DataConfig.KKConfig.AddressY)
            )
            {
                LogNet.Log("kk坐标的地址不存在！");
                throw new Exception("kk坐标的地址不存在！");
            }

            var plc = ProjectManager.Instance.Plc;
            if (!plc.IsOpened)
            {
                LogNet.Log("plc未连接！");
                throw new Exception("plc未连接！");
            }

            Stopwatch sw = Stopwatch.StartNew();
            plc.ReadDouble(_station.DataConfig.KKConfig.AddressX, out double x);
            plc.ReadDouble(_station.DataConfig.KKConfig.AddressY, out double y);

            //LogUI.AddLog($"KK当前坐标 x:{x} y:{y}");

            //根据当前的KK坐标 计算 机械手坐标
            var cogtranform2DLinear =
                CalibTool.Calibration.GetComputedUncalibratedFromCalibratedTransform();
            cogtranform2DLinear.InvertBase().MapPoint(x, y, out var currentRx, out var currentRy);

            //初始点kk坐标 计算 机械手坐标
            cogtranform2DLinear
                .InvertBase()
                .MapPoint(kkOrigin.X, kkOrigin.Y, out var originRx, out var originRy);

            //机械手deta值
            double deltaRx = currentRx - originRx;
            double deltaRy = currentRy - originRy;

            //LogUI.AddLog($"机械手delta值 x:{deltaRx.ToString("f3")} y:{deltaRy.ToString("f3")}");

            RobotDelta = new PointD(deltaRx, deltaRy);
            sw.Stop();
            RunTime = sw.Elapsed;
        }

        public override void RunDebug()
        {
            Run();
        }

        public void RegisterStation(Station station)
        {
            _station = station;
        }

        #endregion

        #region 【vpp相关】

        public void LoadVpp()
        {
            if (!IsLoaded)
            {
                try
                {
                    var toolPath = Path.Combine(
                        ProjectManager.ProjectDir,
                        _station.StationName,
                        $"{ToolName}.vpp"
                    );
                    CalibTool =
                        CogSerializer.LoadObjectFromFile(toolPath) as CogCalibNPointToNPointTool;
                    IsLoaded = true;
                }
                catch (Exception ex)
                {
                    throw new Exception($"工具vpp加载失败.\r\n{ex.Message}");
                }
            }
        }

        public void SaveVpp()
        {
            if (IsLoaded)
            {
                var toolPath = Path.Combine(
                    ProjectManager.ProjectDir,
                    _station.StationName,
                    $"{ToolName}.vpp"
                );
                CogSerializer.SaveObjectToFile(CalibTool, toolPath);
            }
        }

        public void CreateVpp()
        {
            if (!IsLoaded)
            {
                var toolPath = Path.Combine(
                    ProjectManager.ProjectDir,
                    _station.StationName,
                    $"{ToolName}.vpp"
                );
                if (string.IsNullOrEmpty(toolPath))
                {
                    throw new Exception("标定文件的路径不存在");
                }
                CalibTool = new CogCalibNPointToNPointTool();
                IsLoaded = true;
            }
        }

        public void RemoveVpp()
        {
            if (!IsLoaded)
            {
                try
                {
                    var toolPath = Path.Combine(
                        ProjectManager.ProjectDir,
                        _station.StationName,
                        $"{ToolName}.vpp"
                    );
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
        #endregion
    }
}
