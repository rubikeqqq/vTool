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
        private PLCConfig _plcConfig;

        private void ControlInit()
        {
            try
            {
                if (_init)
                    return;
                //初始化读取参数
                _imageImageConfig = Config.ImageConfig;
                _systemConfig = Config.SystemConfig;
                _plcConfig = Config.PLCConfig;

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

                tbIP.Text = _plcConfig.IP;
                tbPort.Text = _plcConfig.Port;

                //保存图像路径
                tbPath.Text = _imageImageConfig.SaveImageDir;

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

        private void tbIP_TextChanged(object sender, EventArgs e)
        {
            if (_init)
                _plcConfig.IP = tbIP.Text;
        }

        private void tbPort_TextChanged(object sender, EventArgs e)
        {
            if (_init)
                _plcConfig.Port = tbPort.Text;
        }
    }
}
