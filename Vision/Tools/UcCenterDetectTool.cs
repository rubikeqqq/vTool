﻿using System;
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
        }

        private readonly CenterDetectTool _tool;
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
            //显示模板点
            labelModel.Text =
                $"模板点:      X:{_station.DataConfig.CalibConfig.ModelOriginPoint.X}"
                + $"      Y:{_station.DataConfig.CalibConfig.ModelOriginPoint.Y}"
                + $"      Angle:{_station.DataConfig.CalibConfig.ModelOriginPoint.Angle}";
            _init = true;
        }

        private void cogToolBlockEditV21_Load(object sender, System.EventArgs e)
        {
            if (_tool.ToolBlock != null)
            {
                cogToolBlockEditV21.Subject = _tool.ToolBlock;
            }
        }

        private void btnModel_Click(object sender, EventArgs e)
        {
            var dialog = MessageBox.Show(
                "是否确定更新模板点？\r\n这将保存此工具的最后结果！",
                "重要提示",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (dialog == DialogResult.Yes)
            {
                try
                {
                    if (
                        _tool.ToolBlock.Outputs["X"].Value != null
                        && _tool.ToolBlock.Outputs["Y"].Value != null
                        && _tool.ToolBlock.Outputs["Angle"].Value != null
                    )
                    {
                        _station.DataConfig.CalibConfig.ModelOriginPoint = new PointA(
                            (double)_tool.ToolBlock.Outputs["X"].Value,
                            (double)_tool.ToolBlock.Outputs["Y"].Value,
                            (double)_tool.ToolBlock.Outputs["Angle"].Value
                        );

                        _station.SaveData();
                        _tool.SaveVpp();

                        //显示模板点
                        labelModel.Text =
                            $"模板点:      X:{_station.DataConfig.CalibConfig.ModelOriginPoint.X}"
                            + $"      Y:{_station.DataConfig.CalibConfig.ModelOriginPoint.Y}"
                            + $"      Angle:{_station.DataConfig.CalibConfig.ModelOriginPoint.Angle}";
                    }
                    else
                    {
                        "模板数据不存在！".MsgBox();
                    }
                }
                catch
                {
                    "模板点保存失败".MsgBox();
                }
            }
        }
    }
}
