﻿using System.ComponentModel;
using System.Windows.Forms;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcDetectTool : UserControl
    {
        public UcDetectTool(Station station, DetectTool tool)
        {
            InitializeComponent();
            _station = station;
            _tool = tool;
        }

        private readonly DetectTool _tool;
        private readonly Station _station;
        private bool _init;

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
            if (!_init)
                return;
            if (comboBox1.SelectedIndex != -1)
            {
                string imageToolName = comboBox1.Text;
                _tool.ImageInName = imageToolName;
            }
            else
            {
                _tool.ImageInName = null;
            }
            ProjectManager.Instance.SaveProject();
        }

        private void UcDetectTool_Load(object sender, System.EventArgs e)
        {
            GetImageIn();
            _init = true;
        }

        private void cogToolBlockEditV21_Load(object sender, System.EventArgs e)
        {
            if (_tool.ToolBlock != null)
            {
                cogToolBlockEditV21.Subject = _tool.ToolBlock;
            }
        }
    }
}
