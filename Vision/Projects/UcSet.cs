using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Core;

namespace Vision.Projects
{
    [ToolboxItem(false)]
    public partial class UcSet : UserControl
    {
        public UcSet()
        {
            InitializeComponent();
        }


        private bool _init;
        private ImageConfig _imageImageConfig;
        private SystemConfig _systemConfig;
        private OffsetConfig _offset;
        private CalibConfig _calibConfig;
        private KKConfig _kkConfig;

        private void ControlInit()
        {
            try
            {
                if (_init)
                    return;
                //初始化读取参数
                _imageImageConfig =Config.ImageConfig;
                _systemConfig = Config.SystemConfig;
                _offset = Config.OffsetConfig;
                _calibConfig = Config.CalibConfig;
                _kkConfig = Config.KKConfig;

                //图像配置
                cbNG.Checked = _imageImageConfig.IsSaveNGImage;
                cbOK.Checked = _imageImageConfig.IsSaveOKImage;
                cbTime.Checked = _imageImageConfig.IsDeleteByTime;
                cbSize.Checked = _imageImageConfig.IsDeleteBySize;
                numSize.Value = _imageImageConfig.DeleteSize;
                numTime.Value = _imageImageConfig.DeleteDayTime;

                //系统配置
                cbAutoRun.Checked = _systemConfig.AutoRun;

                tbHeart.Text = _systemConfig.HeartAddress;
                tbOnline.Text = _systemConfig.OnlineAddress;

                //补偿
                numOffsetX.Value = (decimal)_offset.OffsetX;
                numOffsetY.Value = (decimal)_offset.OffsetY;
                //保存图像路径
                tbPath.Text = _imageImageConfig.SaveImageDir;

                //旋转中心
                numCX.Value = (decimal)_calibConfig.CenterPoint.X;
                numCY.Value = (decimal)_calibConfig.CenterPoint.Y;
                //标定 机械手点位
                numRX.Value = (decimal)_calibConfig.CenterCalibRobotPoint.X;
                numRY.Value = (decimal)_calibConfig.CenterCalibRobotPoint.Y;
                //机械手示教位
                numRobotX.Value = (decimal)_calibConfig.RobotOriginPosition.X;
                numRobotY.Value = (decimal)_calibConfig.RobotOriginPosition.Y;
                numRobotA.Value = (decimal)_calibConfig.RobotOriginPosition.Angle;
                //模板点位
                numModelX.Value = (decimal)_calibConfig.ModelOriginPoint.X;
                numModelY.Value = (decimal)_calibConfig.ModelOriginPoint.Y;
                numModelA.Value = (decimal)_calibConfig.ModelOriginPoint.Angle;

                // kk
                //kk初始坐标
                numInitX.Value = (decimal)_kkConfig.KKOriginPosition.X;
                numInitY.Value = (decimal)_kkConfig.KKOriginPosition.Y;

                //plc地址初始化
                tbPLCX.Text = _kkConfig.AddressX;
                tbPLCY.Text = _kkConfig.AddressY;

                _init = true;
            }
            catch (Exception ex)
            {
                LogNet.Log(ex.ToString());
            }

        }

        private void UcSet_Load(object sender, System.EventArgs e)
        {
            ControlInit();
        }

        private void numTime_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageImageConfig.DeleteDayTime = (int)numTime.Value;
        }

        private void numSize_ValueChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageImageConfig.DeleteSize = (int)numSize.Value;
        }

        private void cbTime_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageImageConfig.IsDeleteByTime = cbTime.Checked;
        }

        private void cbSize_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageImageConfig.IsDeleteBySize = cbSize.Checked;
        }

        private void cbNG_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageImageConfig.IsSaveNGImage = cbNG.Checked;
        }

        private void cbOK_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _imageImageConfig.IsSaveOKImage = cbOK.Checked;
        }

        private void cbAutoRun_CheckedChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _systemConfig.AutoRun = cbAutoRun.Checked;
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            ProjectManager.Instance.SaveConfig();
        }

        private void tbHeart_TextChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _systemConfig.HeartAddress = tbHeart.Text;
        }

        private void tbOnline_TextChanged(object sender, System.EventArgs e)
        {
            if (_init)
                _systemConfig.OnlineAddress = tbOnline.Text;
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

        private void btnPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var path = fbd.SelectedPath + "Images";
                tbPath.Text = path;
                _imageImageConfig.SaveImageDir = path;
            }
        }

        private void numCX_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _calibConfig.CenterPoint.X = (double)numCX.Value;
        }

        private void numCY_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _calibConfig.CenterPoint.Y = (double)numCY.Value;
        }

        private void numRX_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _calibConfig.CenterCalibRobotPoint.X = (double)numRX.Value;
        }

        private void numRY_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _calibConfig.CenterCalibRobotPoint.Y = (double)numRY.Value;
        }

        private void numRobotX_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _calibConfig.RobotOriginPosition.X = (double)numRobotX.Value;
        }

        private void numRobotY_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _calibConfig.RobotOriginPosition.Y = (double)numRobotY.Value;
        }

        private void numRobotA_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _calibConfig.RobotOriginPosition.Angle = (double)numRobotA.Value;
        }

        private void numModelX_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _calibConfig.ModelOriginPoint.X = (double)numModelX.Value;
        }

        private void numModelY_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _calibConfig.ModelOriginPoint.Y = (double)numModelY.Value;
        }

        private void numModelA_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _calibConfig.ModelOriginPoint.Angle = (double)numModelA.Value;
        }

        private void numInitX_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _kkConfig.KKOriginPosition.X = (double)numInitX.Value;
        }

        private void numInitY_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _kkConfig.KKOriginPosition.Y = (double)numInitY.Value;
        }

        private void tbPLCX_TextChanged(object sender, EventArgs e)
        {
            if (_init)
                _kkConfig.AddressX = tbPLCX.Text;
        }

        private void tbPLCY_TextChanged(object sender, EventArgs e)
        {
            if (_init)
                _kkConfig.AddressY = tbPLCY.Text;
        }
    }
}
