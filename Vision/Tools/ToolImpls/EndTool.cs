using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [ToolName("结束信号", 1)]
    [GroupInfo(name: "通讯工具", index: 4)]
    [Description("相机处理结束信号")]
    public class EndTool : ToolBase
    {
        /// <summary>
        /// 结束信号地址
        /// </summary>
        public string EndAddress { get; set; }

        [field: NonSerialized]
        public UserControl UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            if (UI == null)
            {
                UI = new UcEndTool(this);
            }
            return UI;
        }

        public override void Run()
        {
            var plc = ProjectManager.Instance.ProjectData.MxPlc;
            if (plc != null && plc.IsConnected)
            {
                if (!string.IsNullOrEmpty(EndAddress))
                {
                    plc.WriteShort(EndAddress, 1);
                }
            }
            else
            {
                LogUI.AddLog("plc未连接！");
            }
        }

        public override void RunDebug()
        {
            //调试时不运行
        }
    }
}
