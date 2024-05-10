using System.Windows.Forms;

using Vision.Core;
using Vision.Stations;

namespace Vision.Frm
{
    public partial class FormStationSet : Form
    {
        public FormStationSet(Station station)
        {
            InitializeComponent();
            _station = station;
            _config = station.DataConfig;
            _offset = _config.OffsetConfig;
            _calibConfig = _config.CalibConfig;
            _kkConfig = _config.KKConfig;

            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private Station _station;
        private StationDataConfig _config;
        private OffsetConfig _offset;
        private CalibConfig _calibConfig;
        private KKConfig _kkConfig;
        private bool _init;

        private void FormStationSet_Load(object sender, System.EventArgs e)
        {
            //补偿
            numOffsetX.Value = (decimal)_offset.OffsetX;
            numOffsetY.Value = (decimal)_offset.OffsetY;

            //标定 机械手点位
            numRX.Value = (decimal)_calibConfig.CenterCalibRobotPoint.X;
            numRY.Value = (decimal)_calibConfig.CenterCalibRobotPoint.Y;
            //机械手示教位
            numRobotX.Value = (decimal)_calibConfig.RobotOriginPosition.X;
            numRobotY.Value = (decimal)_calibConfig.RobotOriginPosition.Y;
            numRobotA.Value = (decimal)_calibConfig.RobotOriginPosition.Angle;

            // kk
            //kk初始坐标
            numInitX.Value = (decimal)_kkConfig.KKOriginPosition.X;
            numInitY.Value = (decimal)_kkConfig.KKOriginPosition.Y;

            //plc地址初始化
            tbPLCX.Text = _kkConfig.AddressX;
            tbPLCY.Text = _kkConfig.AddressY;
            _init = true;
        }

        private void numOffsetX_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _offset.OffsetX = (double)numOffsetX.Value;
        }

        private void numOffsetY_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _offset.OffsetY = (double)numOffsetY.Value;
        }

        private void numRX_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _calibConfig.CenterCalibRobotPoint.X = (double)numRX.Value;
        }

        private void numRY_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _calibConfig.CenterCalibRobotPoint.Y = (double)numRY.Value;
        }

        private void numRobotX_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _calibConfig.RobotOriginPosition.X = (double)numRobotX.Value;
        }

        private void numRobotY_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _calibConfig.RobotOriginPosition.Y = (double)numRobotY.Value;
        }

        private void numRobotA_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _calibConfig.RobotOriginPosition.Angle = (double)numRobotA.Value;
        }

        private void numInitX_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _kkConfig.KKOriginPosition.X = (double)numInitX.Value;
        }

        private void numInitY_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _kkConfig.KKOriginPosition.Y = (double)numInitY.Value;
        }

        private void tbPLCX_TextChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _kkConfig.AddressX = tbPLCX.Text;
        }

        private void tbPLCY_TextChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _kkConfig.AddressY = tbPLCY.Text;
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            _station.SaveData();
            this.Close();
        }
    }
}
