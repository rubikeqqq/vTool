using Cognex.VisionPro;
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
    [GroupInfo(name: "视觉工具", index: 2)]
    [ToolName("旋转检测工具", 1)]
    [Description("主检测流程,旋转中心使用")]
    public class CenterDetectTool : ToolBase, IVpp, IImageIn, IModelPoint
    {
        public CenterDetectTool()
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

        /// <summary>
        /// 模版点 给下面的旋转标定计算
        /// </summary>
        [field: NonSerialized]
        public PointA ModelPoint { get; set; }

        /// <summary>
        /// 示教时模板的点位
        /// </summary>
        public PointA ModelOriginPoint { get; set; }

        [field: NonSerialized]
        public UcCenterDetectTool UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            if (UI == null)
            {
                UI = new UcCenterDetectTool(station, this);
            }
            //刷新图像源
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
                ToolBlock.Outputs.Add(new CogToolBlockTerminal("ModelX", typeof(double)));
                ToolBlock.Outputs.Add(new CogToolBlockTerminal("ModelY", typeof(double)));
                ToolBlock.Outputs.Add(new CogToolBlockTerminal("ModelAngle", typeof(double)));
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
                //LogUI.AddLog("vpp保存后需检查结果编辑工具！");
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

                    if (ModelPoint == null)
                    {
                        ModelPoint = new PointA();
                    }

                    if (ToolBlock.Outputs["ModelX"].Value != null)
                        ModelPoint.X = (double)ToolBlock.Outputs["ModelX"].Value;
                    if (ToolBlock.Outputs["ModelY"].Value != null)
                        ModelPoint.Y = (double)ToolBlock.Outputs["ModelY"].Value;
                    if (ToolBlock.Outputs["ModelAngle"].Value != null)
                        ModelPoint.Angle = (double)ToolBlock.Outputs["ModelAngle"].Value;

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