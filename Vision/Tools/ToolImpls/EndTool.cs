using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Vision.Core;
using Vision.Projects;
using Vision.Stations;

namespace Vision.Tools.ToolImpls
{
    [ToolName("结束信号", 1)]
    [GroupInfo(name: "通讯工具", index: 4)]
    [Description("相机处理结束信号")]
    public class EndTool : ToolBase
    {
        /// <summary>
        /// 结束信号地址
        /// </summary>
        public string EndAddress { get; set; }

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
            RunTime = TimeSpan.Zero;
            if (!Enable) return;
            if (ProjectManager.Instance.Plc.IsOpened)
            {
                Stopwatch sw = Stopwatch.StartNew();
                if (!string.IsNullOrEmpty(EndAddress))
                {
                    ProjectManager.Instance.Plc.WriteShort(EndAddress, 1);
                }
                sw.Stop();
                RunTime = sw.Elapsed;
            }
            else
            {
                LogUI.AddLog("plc未连接！");
            }
        }

        public override void RunDebug()
        {
            RunTime = TimeSpan.Zero;
            //调试时不运行
        }

        #region ISerializable
        public override void LoadFromStream(SerializationInfo info,string toolName)
        {
            base.LoadFromStream(info,toolName);
            string endAddress = $"{toolName}.endAddress";

            EndAddress = info.GetString(endAddress);
        }

        public override void SaveToStream(SerializationInfo info,string toolName)
        {
            base.SaveToStream(info,toolName);

            string endAddress = $"{toolName}.endAddress";

            info.AddValue(endAddress,EndAddress);
        }
        #endregion
    }
}
