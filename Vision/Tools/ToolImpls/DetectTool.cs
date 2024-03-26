using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [GroupInfo(name: "视觉工具", index: 2)]
    [ToolName("检测工具", 0)]
    [Description("主检测流程")]
    public class DetectTool : ToolBase, IVpp, IImageIn
    {

        public DetectTool()
        {
        }

        [NonSerialized]
        private Station _station;

        /// <summary>
        /// 输入图像名称
        /// </summary>
        public string ImageInName { get; set; }

        /// <summary>
        /// toolblock
        /// </summary>
        [field: NonSerialized]
        public CogToolBlock ToolBlock { get; set; }

        /// <summary>
        /// 输入图像
        /// </summary>
        [field: NonSerialized]
        public ICogImage ImageIn { get; set; }

        /// <summary>
        /// vpp加载
        /// </summary>
        [field: NonSerialized]
        public bool IsLoaded { get; set; }

        [field: NonSerialized]
        public UcDetectTool UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            if (UI == null)
            {
                UI = new UcDetectTool(station, this);
            }
            UI.GetImageIn();
            return UI;
        }

        public override void Save()
        {
            SaveVpp();
            base.Save();
        }

        #region 【Vpp相关】
        /// <summary>
        /// 创建vpp
        /// </summary>
        /// <exception cref="Exception"></exception>
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
                IsLoaded = true;
            }
        }

        /// <summary>
        /// 加载vpp
        /// </summary>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// 保存vpp
        /// </summary>
        public void SaveVpp()
        {
            if (IsLoaded)
            {
                var toolPath = Path.Combine(ProjectManager.ProjectDir, _station.StationName, $"{ToolName}.vpp");
                CogSerializer.SaveObjectToFile(ToolBlock, toolPath);
            }
        }

        /// <summary>
        /// 删除Vpp
        /// </summary>
        /// <exception cref="Exception"></exception>
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
        /// <summary>
        /// 运行工具
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override void Run()
        {
            if (!Enable) return;
            GetImageIn();
            if (ImageIn != null)
            {
                if (ToolBlock != null)
                {
                    ToolBlock.Inputs["InputImage"].Value = ImageIn;

                    //复位结果数据
                    ResetOutput();

                    ToolBlock.Run();
                    if (ToolBlock.RunStatus.Result != CogToolResultConstants.Accept)
                    {
                        throw new ToolException($"[{ToolName}]");
                    }
                }
            }
            else
            {
                throw new ToolException($"[{ToolName}] 输入图像不存在！")
                {
                    ImageInNull = true,
                };
            }
        }

        /// <summary>
        /// 关闭工具
        /// </summary>
        public override void Close()
        {
            base.Close();
            _station.StationNameChangedEvent -= Station_StationNameChanged;
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

        /// <summary>
        /// 注册station
        /// </summary>
        /// <param name="station"></param>
        public void RegisterStation(Station station)
        {
            _station = station;
            station.StationNameChangedEvent += Station_StationNameChanged;
        }

        /// <summary>
        /// station名称改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Station_StationNameChanged(object sender, StationEventArgs e)
        {
            //ToolPath = Path.Combine(e.Station.StationPath, $"{Name}.vpp");
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
        /// 复位vpp输出
        /// </summary>
        private void ResetOutput()
        {
            var terminals = ToolBlock.Outputs;
            foreach (CogToolBlockTerminal terminal in terminals)
            {
                var type = terminal.ValueType;
                switch (type.Name)
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
        #endregion
    }
}
