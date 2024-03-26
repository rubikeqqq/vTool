using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Core;
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

            if (_cTool.CenterCalibRobotPoint == null)
            {
                _cTool.CenterCalibRobotPoint = new PointD();
            }
            if (_cTool.CenterPoint == null)
            {
                _cTool.CenterPoint = new PointD();
            }

            if (_cTool.RobotOriginPosition == null)
            {
                _cTool.RobotOriginPosition = new PointA();
            }

            //点位
            numCX.Value = (decimal)_cTool.CenterPoint.X;
            numCY.Value = (decimal)_cTool.CenterPoint.Y;
            numRX.Value = (decimal)_cTool.CenterCalibRobotPoint.X;
            numRY.Value = (decimal)_cTool.CenterCalibRobotPoint.Y;

            numRobotX.Value = (decimal)_cTool.RobotOriginPosition.X;
            numRobotY.Value = (decimal)_cTool.RobotOriginPosition.Y;
            numRobotA.Value = (decimal)_cTool.RobotOriginPosition.Angle;
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

        private void numCX_ValueChanged(object sender, EventArgs e)
        {
            _cTool.CenterPoint.X = (double)numCX.Value;
        }

        private void numCY_ValueChanged(object sender, EventArgs e)
        {
            _cTool.CenterPoint.Y = (double)numCY.Value;
        }

        private void numRX_ValueChanged(object sender, EventArgs e)
        {
            _cTool.CenterCalibRobotPoint.X = (double)numRX.Value;
        }

        private void numRY_ValueChanged(object sender, EventArgs e)
        {
            _cTool.CenterCalibRobotPoint.Y = (double)numRY.Value;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FrmCenterCalib frmCenterCalib = new FrmCenterCalib(_cTool);
            frmCenterCalib.ShowDialog();
        }

        private void numRobotX_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _cTool.RobotOriginPosition.X = (double)numRobotX.Value;
        }

        private void numRobotY_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _cTool.RobotOriginPosition.Y = (double)numRobotY.Value;
        }

        private void numRobotA_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _cTool.RobotOriginPosition.Angle = (double)numRobotA.Value;
        }
    }
}
