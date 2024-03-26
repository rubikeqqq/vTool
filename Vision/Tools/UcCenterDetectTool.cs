using System;
using System.ComponentModel;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcCenterDetectTool : UserControl
    {
        public UcCenterDetectTool(Station station, CenterDetectTool tool)
        {
            InitializeComponent();
            _station = station;
            _tool = tool;
            ProjectManager.Instance.BeforeSaveProjectEvent += Instance_BeforeSaveProjectEvent;
        }

        private readonly CenterDetectTool _tool;
        private readonly Station _station;
        private bool _init;

        /// <summary>
        /// 控件关闭
        /// </summary>
        private void Close()
        {
            ProjectManager.Instance.BeforeSaveProjectEvent -= Instance_BeforeSaveProjectEvent;
        }

        private void InitUI()
        {
            if (_tool.ModelOriginPoint == null)
            {
                _tool.ModelOriginPoint = new PointA();
            }
            numModelX.Value = (decimal)_tool.ModelOriginPoint.X;
            numModelY.Value = (decimal)_tool.ModelOriginPoint.Y;
            numModelA.Value = (decimal)_tool.ModelOriginPoint.Angle;
        }

        /// <summary>
        /// 图像源
        /// </summary>
        public void GetImageIn()
        {
            if (_station != null)
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(_station.GetImageInToolNames(_tool));
                if (_tool.ImageInName != null)
                {
                    comboBox1.SelectedItem = _tool.ImageInName;
                }
            }
        }

        /// <summary>
        /// 图像源切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!_init) return;
            if (comboBox1.SelectedIndex != -1)
            {
                string imageToolName = comboBox1.Text;
                _tool.ImageInName = imageToolName;
            }
            else
            {
                _tool.ImageInName = null;
            }
        }

        private void UcDetectTool_Load(object sender, System.EventArgs e)
        {
            GetImageIn();
            InitUI();
            _init = true;
        }

        /// <summary>
        /// 在保存之前强制刷新输入图像源事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Instance_BeforeSaveProjectEvent(object sender, EventArgs e)
        {
            comboBox1_SelectedIndexChanged(this, null);
        }

        private void cogToolBlockEditV21_Load(object sender, System.EventArgs e)
        {
            if (_tool.ToolBlock != null)
            {
                cogToolBlockEditV21.Subject = _tool.ToolBlock;
            }
        }

        private void numModelX_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _tool.ModelOriginPoint.X = (double)numModelX.Value;
        }

        private void numModelY_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _tool.ModelOriginPoint.Y = (double)numModelY.Value;
        }

        private void numModelA_ValueChanged(object sender, EventArgs e)
        {
            if (_init)
                _tool.ModelOriginPoint.Angle = (double)numModelA.Value;
        }

    }
}
