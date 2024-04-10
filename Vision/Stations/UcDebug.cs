using Cognex.VisionPro;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Tools.ToolImpls;

namespace Vision.Stations
{
    [ToolboxItem(false)]
    public partial class UcDebug : UserControl
    {
        private Station _station;

        private CogAcqFifoTool _acqTool;

        private bool _init;

        private bool _living;

        public UcDebug()
        {
            InitializeComponent();
        }

        public void ChangeStation(Station station)
        {
            if(station == _station)
            {
                //combox还是要刷新一下 以防toolblock中更新了record图像
                UpdateComBox();
                return;
            }
            if(_station != null)
            {
                //关闭之前的相机
                if(_living)
                {
                    StopLive();
                    btnLive.Text = "连续采集";
                }
                _station.StationRanEvent -= Station_StationRan;
                _station.StationEnableEvent -= Station_StationEnableEvent;
            }
            _station = station;
            _station.RegisterDebugShow(this);
            _station.StationRanEvent += Station_StationRan;
            _station.StationEnableEvent += Station_StationEnableEvent;
            //更新相机
            UpdateCamera();
            //更新combox
            UpdateComBox();
            //更新Station Enable状态
            UpdateStationEnableStatus();
            //清除之前显示
            ClearDisplay();
        }

        /// <summary>
        /// 开启采集
        /// </summary>
        public void StartLive()
        {
            if(_acqTool == null || cogRecordDisplay1.LiveDisplayRunning) return;
            if(_acqTool.Operator != null)
            {
                cogRecordDisplay1.AutoFit = true;
                cogRecordDisplay1.StaticGraphics.Clear();
                cogRecordDisplay1.InteractiveGraphics.Clear();
                cogRecordDisplay1.StartLiveDisplay(_acqTool.Operator);
                _living = true;
            }
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        public void StopLive()
        {
            if(_acqTool == null || !cogRecordDisplay1.LiveDisplayRunning) return;
            if(_acqTool.Operator != null)
            {
                cogRecordDisplay1.StaticGraphics.Clear();
                cogRecordDisplay1.InteractiveGraphics.Clear();
                cogRecordDisplay1.StopLiveDisplay();
                _living = false;
            }
        }

        /// <summary>
        /// 更新相机
        /// </summary>
        private void UpdateCamera()
        {
            if(_station == null) return;
            if(_acqTool == null)
            {
                foreach(ToolBase tool in _station.ToolList)
                {
                    if(tool is ImageAcqTool aTool)
                    {
                        _acqTool = aTool.AcqFifoTool;
                    }
                }
            }
        }

        private void Close()
        {
            if(_station != null)
            {
                _station.StationRanEvent -= Station_StationRan;
                _station.StationEnableEvent -= Station_StationEnableEvent;
            }
        }

        /// <summary>
        /// Station debug运行事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Station_StationRan(object sender,ShowDebugWindowEventArgs e)
        {
            if(_station == null) return;
            //清楚之前的显示
            ClearDisplay();
            SetResultGraphicOnRecordDisplay(e.Image);
            SetTitle(e.Result ? "运行成功" : $"{e.ErrorMsg}", e.Result ? Color.Green : Color.Red);
            SetTime(e.Time);
        }

        /// <summary>
        /// 启用/关闭按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Station_StationEnableEvent(object sender,bool e)
        {
            rbtnEnable.Checked = e;
            rbtnDisable.Checked = !e;
        }

        private void SetResultGraphicOnRecordDisplay(object image)
        {
            if (InvokeRequired)
            {
                cogRecordDisplay1.Invoke(new Action<object>(SetResultGraphicOnRecordDisplay),
                    image);
                return;
            }
            try
            {
                if (cogRecordDisplay1 == null) return;
                //判断是ICogImage 还是 IRecordImage
                if (image is ICogImage image1)
                {
                    cogRecordDisplay1.Image = image1;
                    cogRecordDisplay1.AutoFit = true;
                }
                else if (image is ICogRecord image2)
                {
                    //如果没有设置输出的图像 则显示原图
                    cogRecordDisplay1.Record = image2;
                    cogRecordDisplay1.AutoFit = true;
                }
            }
            catch
            {
                LogUI.AddToolLog("图像显示失败！");
            }
        }

        /// <summary>
        /// 显示文字
        /// </summary>
        /// <param name="label"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="alignment"></param>
        /// <param name="selectedNameSpace"></param>
        private void GraphicCreateLabel(string label,double x,double y,int size,
            CogColorConstants color,CogGraphicLabelAlignmentConstants alignment,string selectedNameSpace)
        {
            if(InvokeRequired)
            {
                cogRecordDisplay1.Invoke(new Action<string,double,double,int,CogColorConstants,CogGraphicLabelAlignmentConstants,string>(GraphicCreateLabel),
                    label,x,y,size,color,alignment,selectedNameSpace);
                return;
            }
            var myLabel = new CogGraphicLabel();
            var font = new Font("微软雅黑",size,FontStyle.Bold);
            myLabel.GraphicDOFEnable = CogGraphicLabelDOFConstants.None;
            myLabel.Interactive = false;
            myLabel.Font = font;
            myLabel.Alignment = alignment;
            myLabel.Color = color;
            myLabel.SetXYText(x,y,label);
            myLabel.SelectedSpaceName = selectedNameSpace;

            cogRecordDisplay1.StaticGraphics.Add(myLabel,"");
        }

        /// <summary>
        /// 显示文字简易版
        /// </summary>
        /// <param name="ok"></param>
        private void GraphicCreateLabel(bool ok)
        {
            if(InvokeRequired)
            {
                cogRecordDisplay1.Invoke(new Action<bool>(GraphicCreateLabel),ok);
                return;
            }
            double x = 20;
            double y = 20;

            var size = cogRecordDisplay1.Width / 30;

            var myLabel = new CogGraphicLabel();
            var font = new Font("微软雅黑",size,FontStyle.Bold);
            myLabel.GraphicDOFEnable = CogGraphicLabelDOFConstants.None;
            myLabel.Interactive = false;
            myLabel.Font = font;
            myLabel.Alignment = CogGraphicLabelAlignmentConstants.TopLeft;
            myLabel.Color = ok ? CogColorConstants.Green : CogColorConstants.Red;
            myLabel.SetXYText(x,y,ok ? "OK" : "NG");
            myLabel.SelectedSpaceName = "@";

            cogRecordDisplay1.StaticGraphics.Add(myLabel,"");
        }


        /// <summary>
        /// 清除显示界面
        /// </summary>
        public void ClearDisplay()
        {
            if(InvokeRequired)
            {
                Invoke(new Action(ClearDisplay));
                return;
            }

            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.Image = null;
        }

        /// <summary>
        /// 显示标题提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="color"></param>
        public void SetTitle(string title,Color color)
        {
            if(InvokeRequired)
            {
                Invoke(new Action<string,Color>(SetTitle),title,color);
                return;
            }

            labelStatu.Text = title;
            labelStatu.ForeColor = color;
        }

        /// <summary>
        /// 显示时间
        /// </summary>
        /// <param name="time"></param>
        public void SetTime(TimeSpan time)
        {
            if(InvokeRequired)
            {
                Invoke(new Action<TimeSpan>(SetTime),time);
                return;
            }

            labelTime.Text = $@"运行时间:{time.TotalMilliseconds:f2} ms";
        }

        private void UcStationShow_Load(object sender,System.EventArgs e)
        {
            //UpdateComBox();
            _init = true;
        }

        /// <summary>
        /// 更新Combox
        /// </summary>
        private void UpdateComBox()
        {
            var names = _station.GetLastRunRecordName();
            comboBox1.Items.Clear();
            foreach(var name in names)
            {
                comboBox1.Items.Add(name);
            }
            if(names.Contains(_station.LastRecordName))
            {
                comboBox1.SelectedItem = _station.LastRecordName;
            }
            else
            {
                comboBox1.SelectedIndex = -1;
            }
        }

        private void UpdateStationEnableStatus()
        {
            rbtnEnable.Checked = _station.Enable;
            rbtnDisable.Checked = !_station.Enable;
        }

        /// <summary>
        /// combox变换事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender,EventArgs e)
        {
            if(_init)
            {
                if(comboBox1.SelectedIndex == -1) return;
                _station.LastRecordName = comboBox1.Text;
                ProjectManager.Instance.SaveProject();
            }
        }

        /// <summary>
        /// 运行按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender,EventArgs e)
        {
            _station.DebugRun();
        }

        /// <summary>
        /// 采集按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLive_Click(object sender,EventArgs e)
        {
            if(btnLive.Text == "连续采集" && !_living)
            {
                StartLive();
                if(_living)
                    btnLive.Text = "停止采集";
            }
            else if(btnLive.Text == "停止采集" && _living)
            {
                StopLive();
                if(!_living)
                    btnLive.Text = "连续采集";
            }
        }

        private void rbtnEnable_CheckedChanged(object sender,EventArgs e)
        {
            if(_init)
            {
                _station.Enable = rbtnEnable.Checked;
                ProjectManager.Instance.SaveProject();
            }
        }

        private void rbtnDisable_CheckedChanged(object sender,EventArgs e)
        {
            if(_init)
            {
                _station.Enable = !rbtnDisable.Checked;
                ProjectManager.Instance.SaveProject();
            }
        }
    }
}
