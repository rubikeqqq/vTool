using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [ToolName("触发信号", 0)]
    [GroupInfo(name: "通讯工具", index: 4)]
    [Description("PLC触发相机拍照信号")]
    public class TriggerTool : ToolBase
    {
        /// <summary>
        /// 触发信号地址
        /// </summary>
        public string TriggerAddress { get; set; }

        [field: NonSerialized]
        public UserControl UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            return UI ?? (UI = new UcTriggerTool(this));
        }

        public override void Run()
        {
            var plc = ProjectManager.Instance.Project.MxPlc;
            if (plc != null && plc.IsConnected)
            {
                if (!string.IsNullOrEmpty(TriggerAddress))
                {
                    while (true)
                    {
                        var flag = plc.ReadShort(TriggerAddress);
                        if (flag == 1)
                        {
                            //LogUI.AddLog($"收到触发信号{TriggerAddress}！");
                            //复位
                            plc.WriteShort(TriggerAddress, 0);
                            // LogUI.AddLog($"复位触发信号{TriggerAddress}！");
                            break;
                        }
                        Thread.Sleep(10);
                    }
                }
            }
            else
            {
                throw new ToolException("plc未连接！")
                {
                    ImageInNull = true
                };
            }
        }

        public override void RunDebug()
        {
            //调试模式时不运行
        }
    }
}
