using Cognex.VisionPro;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [GroupInfo("图像工具", 0)]
    [ToolName("相机采集",0)]
    [Description("通过相机进行采集的康耐视工具")]
    public class ImageAcqTool : ToolBase, IImageOut, IVpp
    {
        [NonSerialized]
        private Station _station;

        [field: NonSerialized]
        public ICogImage ImageOut { get; private set; }

        /// <summary>
        /// 采集工具
        /// </summary>
        [field: NonSerialized]
        public CogAcqFifoTool AcqFifoTool { get; set; }

        [field: NonSerialized]
        public bool IsLoaded { get;private set; }

        [field: NonSerialized]
        public UcAcqTool UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            return UI ?? (UI = new UcAcqTool(this));
        }

        public override void Save()
        {
            SaveVpp();
            base.Save();
        }

        #region 【vpp相关】
        
        public void LoadVpp()
        {
            if(!IsLoaded)
            {
                try
                {
                    var toolPath = Path.Combine(ProjectManager.ProjectDir, _station.StationName, $"{ToolName}.vpp");
                    AcqFifoTool = CogSerializer.LoadObjectFromFile(toolPath) as CogAcqFifoTool;
                    IsLoaded = true;
                }
                catch(Exception ex)
                {
                    throw new Exception($"相机加载失败！" + ex.Message);
                }
            }
        }

        public void SaveVpp()
        {
            if(IsLoaded)
            {
                try
                {
                    var toolPath = Path.Combine(ProjectManager.ProjectDir, _station.StationName, $"{ToolName}.vpp");
                    CogSerializer.SaveObjectToFile(AcqFifoTool,toolPath);
                }
                catch(Exception ex)
                {
                    throw new Exception($"相机保存失败！" + ex.Message);
                }
            }
        }

        public void CreateVpp()
        {
            if(!IsLoaded)
            {
                var toolPath = Path.Combine(ProjectManager.ProjectDir, _station.StationName, $"{ToolName}.vpp");
                if (string.IsNullOrEmpty(toolPath))
                {
                    throw new Exception("相机的路径不存在");
                }
                AcqFifoTool = new CogAcqFifoTool();
                IsLoaded = true;
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
        #endregion

        #region 【工具相关】

        public override void Run()
        {
            if (!Enable) return;
            if (AcqFifoTool == null)
                throw new ToolException("AcqFifoTool为null")
                {
                    ImageInNull = true,
                };
            AcqFifoTool.Run();
            if (AcqFifoTool.RunStatus.Result != CogToolResultConstants.Accept)
            {
                throw new ToolException("相机取像失败！");
            }
            ImageOut = AcqFifoTool.OutputImage;
        }

        public override void RunDebug()
        {
            Run();
        }

        public override void Close()
        {
            base.Close();
            if(AcqFifoTool != null && AcqFifoTool.Operator != null &&
               AcqFifoTool.Operator.FrameGrabber != null)
            {
                AcqFifoTool.Operator.FrameGrabber.Disconnect(true);
                AcqFifoTool.Dispose();
                AcqFifoTool = null;
            }
        }

        public void RegisterStation(Station station)
        {
            _station = station;
        }

        #endregion
    }
}