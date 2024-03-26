using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.Core;
using Vision.Properties;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcToolBase : UserControl
    {
        public UcToolBase()
        {
            InitializeComponent();
        }

        private ToolBase _baseTool;
        private Station _station;

        public event EventHandler<bool> ToolEnableChangedEvent;

        public void ChangeTool(Station station, ToolBase tool)
        {
            _station = station;
            _baseTool = tool;
            ChangeToolUI(station,tool);
        }

        /// <summary>
        /// 切换工具UI
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        private void ChangeToolUI(Station station, ToolBase tool)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Station, ToolBase>(ChangeToolUI), station, tool);
                return;
            }
            panelMain.Controls.Clear();
            var ui = tool.GetToolControl(station);
            ui.Dock = DockStyle.Fill;
            panelMain.Controls.Add(ui);
            UpdateToolStatu(tool);
            OnToolEnableChangedEvent(this, tool.Enable);
        }

        /// <summary>
        /// 更新工具界面显示（启用/禁用）
        /// </summary>
        /// <param name="tool"></param>
        private void UpdateToolStatu(ToolBase tool)
        {
            tsTool.Image = tool.Enable ? Resources.Enable : Resources.Disable;
        }

        /// <summary>
        /// 工具启用/关闭事件触发器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="b"></param>
        private void OnToolEnableChangedEvent(object sender, bool b)
        {
            ToolEnableChangedEvent?.Invoke(sender, b);
        }

        /// <summary>
        /// 功能按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var toolStrip = e.ClickedItem;
            if (_baseTool == null)
            {
                LogUI.AddToolLog("未选择工具");
                return;
            }
            try
            {
                switch (toolStrip.Text)
                {
                    case "运行工具":
                        _station.DebugRun();
                        break;
                    case "工具状态":
                        _baseTool.Enable = !_baseTool.Enable;
                        UpdateToolStatu(_baseTool);
                        OnToolEnableChangedEvent(this, _baseTool.Enable);
                        break;
                    case "保存工具":
                        _baseTool.Save();
                        break;
                }
            }
            catch (ToolException ex)
            {
                LogUI.AddToolLog(ex.Message);
            }
        }

        /// <summary>
        /// 界面Load事件 添加一个实时log显示线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UcToolBase_Load(object sender, EventArgs e)
        {
            
        }

    }
}
