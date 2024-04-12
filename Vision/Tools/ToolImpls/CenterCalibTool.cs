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

        [field: NonSerialized]
        public bool IsLoaded { get; set; }

        /// <summary>
        /// 标定vpp 标定时使用
        /// </summary>
        [field: NonSerialized]
        public CogToolBlock ToolBlock { get; set; }

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
                        throw new Exception($"工具[{ToolName}]运行失败！"); 
                    }
                    ImageOut = (ICogImage)ToolBlock.Outputs["OutputImage"].Value;
                    _station.ShowImage = ImageOut;
                }
            }
            else
            {
                throw new Exception($"[{ToolName}]没有输入图像");
            }
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
