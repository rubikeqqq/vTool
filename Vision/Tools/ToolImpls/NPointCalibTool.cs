using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.ID;
using Cognex.VisionPro.ToolBlock;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [GroupInfo("标定工具", 1)]
    [ToolName("9点标定", 0)]
    [Description("标准9点标定工具")]
    public class NPointCalibTool : ToolBase, IImageIn, IVpp, IImageOut
    {
        //vp的9点标定会直接在图像中转换为实际坐标
        //所以输出的坐标为实际机械手标定时的对应的机械坐标
        [NonSerialized]
        private Station _station;

        public string ImageInName { get; set; }

        [field: NonSerialized]
        public ICogImage ImageIn { get; set; }

        [field: NonSerialized]
        public CogToolBlock ToolBlock { get; set; }

        [field: NonSerialized]
        public bool IsLoaded { get; set; }

        [field: NonSerialized]
        public ICogImage ImageOut { get; private set; }

        [field: NonSerialized]
        public UcNineCalibTool UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            if (UI == null)
            {
                UI = new UcNineCalibTool(station, this);
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

        #region 【vpp相关】

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
                    var toolPath = Path.Combine(
                        ProjectManager.ProjectDir,
                        _station.StationName,
                        $"{ToolName}.vpp"
                    );
                    ToolBlock = CogSerializer.LoadObjectFromFile(toolPath) as CogToolBlock;
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
                CogSerializer.SaveObjectToFile(ToolBlock, toolPath);
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

        /// <summary>
        /// 添加vpp工具
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

            //设置ID的一些参数
            idTool.RunParams.NumToFind = 20;
            idTool.RunParams.DisableAllCodes();
            idTool.RunParams.DataMatrix.Enabled = true;

            CogCalibNPointToNPointTool nTool = new CogCalibNPointToNPointTool();
            nTool.Name = "CogCalibNPointToNPointTool1";
            string[] s4 = new string[1];
            string[] s5 = new string[1];
            s4[0] = "|InputImage|InputImage";
            s5[0] = "|OutputImage|OutputImage";

            nTool.UserData.Add("_ToolInputTerminals", s4); //添加终端-InputImage
            nTool.UserData.Add("_ToolOutputTerminals", s5);

            tb.Tools.Add(acqTool);
            tb.Tools.Add(idTool);
            tb.Tools.Add(nTool);
        }
        #endregion

        #region 工具相关

        public override void Run()
        {
            RunTime = TimeSpan.Zero;
            if (!Enable)
                return;
            GetImageIn();
            if (ImageIn != null)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                if (ToolBlock != null)
                {
                    ToolBlock.Inputs["InputImage"].Value = ImageIn;
                    ToolBlock.Run();
                    if (ToolBlock.RunStatus.Result != CogToolResultConstants.Accept)
                    {
                        throw new Exception($"工具[{ToolName}]运行失败！");
                    }
                    ImageOut = (ICogImage)ToolBlock.Outputs["OutputImage"].Value;
                    _station.ShowImage = ImageOut;
                }
                stopwatch.Stop();
                RunTime = stopwatch.Elapsed;
            }
            else
            {
                LogNet.Log($"[{ToolName}]没有输入图像");
                throw new Exception($"[{ToolName}]没有输入图像");
            }
        }

        public override void RunDebug()
        {
            Run();
        }

        /// <summary>
        /// 获取输入图像
        /// </summary>
        private bool GetImageIn()
        {
            if (_station == null || ImageInName == null)
                return false;

            var tool = _station[ImageInName];

            if (tool == null)
                return false;

            ImageIn = ((IImageOut)tool).ImageOut;
            return true;
        }

        public void RegisterStation(Station station)
        {
            _station = station;
        }

        public void CloseCam()
        {
            if (ToolBlock.Tools.Contains("CogAcqFifoTool1"))
            {
                var acqTool = ToolBlock.Tools["CogAcqFifoTool1"] as CogAcqFifoTool;

                if (acqTool.Operator == null)
                {
                    return;
                }

                if (
                    acqTool != null
                    && acqTool.Operator != null
                    && acqTool.Operator.FrameGrabber != null
                )
                {
                    acqTool.Operator.FrameGrabber.Disconnect(true);
                    acqTool.Dispose();
                }
            }
        }

        public override void Close()
        {
            CloseCam();
            base.Close();
        }
        #endregion
    }
}
