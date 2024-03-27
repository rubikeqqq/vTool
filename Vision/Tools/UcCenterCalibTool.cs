using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Frm;
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
        }

        private readonly CenterCalibTool _cTool;
        private readonly Station _station;
        private bool _init;

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            //自动加载9点标定结果

            _cTool.AddToolBlock();
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
            if (!_init)
            {
                Init();
                _init = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FrmCenterCalib frmCenterCalib = new FrmCenterCalib(_cTool);
            frmCenterCalib.ShowDialog();
        }

    }
}
