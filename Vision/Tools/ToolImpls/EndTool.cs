using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Hardware;
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
            if (!Enable) return;
            if (MXPlc.GetInstance().IsOpened)
            {
                if (!string.IsNullOrEmpty(EndAddress))
                {
                    MXPlc.GetInstance().WriteShort(EndAddress, 1);
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
