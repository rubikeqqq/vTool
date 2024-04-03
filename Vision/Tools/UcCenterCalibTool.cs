using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Frm;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcCenterCalibTool : UserControl
    {
        public UcCenterCalibTool(Station station, CenterCalibTool tool)
        {
            InitializeComponent();
            _station = station;
            _cTool = tool;
            GetImageIn();
        }

        private readonly CenterCalibTool _cTool;
        private readonly Station _station;
        private bool _init;

        public void GetImageIn()
        {
            if (_station != null)
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(_station.GetImageInToolNames(_cTool));
                if (_cTool.ImageInName != null)
                {
                    comboBox1.SelectedItem = _cTool.ImageInName.ToString();
                }
            }
        }

        /// <summary>
        /// vpp绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cogToolBlockEditV21_Load(object sender, System.EventArgs e)
        {
            if (_cTool.ToolBlock != null)
            {
                cogToolBlockEditV21.Subject = _cTool.ToolBlock;
            }
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UcCenterCalibTool_Load(object sender, System.EventArgs e)
        {
            _init = true;
        }

        private void btnCalib_Click(object sender, EventArgs e)
        {
            FrmCenterCalib frmCenterCalib = new FrmCenterCalib(_station, _cTool);
            frmCenterCalib.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_init) return;
            if (comboBox1.SelectedIndex != -1)
            {
                string imageToolName = comboBox1.Text;
                _cTool.ImageInName = imageToolName;
            }
            else
            {
                _cTool.ImageInName = null;
            }
            ProjectManager.Instance.SaveProject();
        }
    }
}
