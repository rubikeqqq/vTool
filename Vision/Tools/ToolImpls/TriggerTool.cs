using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;

namespace Vision.Tools.ToolImpls
{
    [ToolName("触发信号",0)]
    [GroupInfo(name: "通讯工具",index: 4)]
    [Description("PLC触发相机拍照信号")]
    public class TriggerTool:ToolBase
    {
        /// <summary>
        /// 触发信号地址
        /// </summary>
        public string TriggerAddress { get; set; }

        public UserControl UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            return UI ?? (UI = new UcTriggerTool(this));
        }

        public override void Run()
        {
            RunTime = TimeSpan.Zero;
            if(!Enable) return;
            var plc = ProjectManager.Instance.Plc;
            if(plc.IsOpened)
            {
                if(!string.IsNullOrEmpty(TriggerAddress))
                {
                    while(true)
                    {
                        plc.ReadShort(TriggerAddress,out short flag);
                        if(flag == 1)
                        {
                            //复位
                            plc.WriteShort(TriggerAddress,0);
                            break;
                        }
                        Thread.Sleep(10);
                    }
                }
            }
            else
            {
                throw new Exception("plc未连接！");
            }
        }

        public override void RunDebug()
        {
            //调试模式时不运行
            RunTime = TimeSpan.Zero;
        }

        public override void LoadFromStream(SerializationInfo info,string toolName)
        {
            base.LoadFromStream(info,toolName);
            string triggerAddress = $"{toolName}.triggerAddress";

            TriggerAddress = info.GetString(triggerAddress);
        }

        public override void SaveToStream(SerializationInfo info,string toolName)
        {
            base.SaveToStream(info,toolName);

            string triggerAddress = $"{toolName}.triggerAddress";

            info.AddValue(triggerAddress,TriggerAddress);
        }
    }
}
