namespace Vision.Tools
{
    partial class UcKkRobotTool
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cogCalibNPointToNPointEditV21 = new Cognex.VisionPro.CalibFix.CogCalibNPointToNPointEditV2();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogCalibNPointToNPointEditV21)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cogCalibNPointToNPointEditV21);
            this.tabPage1.Location = new System.Drawing.Point(4, 32);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(632, 506);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "标定";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cogCalibNPointToNPointEditV21
            // 
            this.cogCalibNPointToNPointEditV21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogCalibNPointToNPointEditV21.Location = new System.Drawing.Point(2, 2);
            this.cogCalibNPointToNPointEditV21.Margin = new System.Windows.Forms.Padding(2);
            this.cogCalibNPointToNPointEditV21.MinimumSize = new System.Drawing.Size(367, 0);
            this.cogCalibNPointToNPointEditV21.Name = "cogCalibNPointToNPointEditV21";
            this.cogCalibNPointToNPointEditV21.Size = new System.Drawing.Size(628, 502);
            this.cogCalibNPointToNPointEditV21.SuspendElectricRuns = false;
            this.cogCalibNPointToNPointEditV21.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(640, 542);
            this.tabControl1.TabIndex = 0;
            // 
            // UcKkRobotTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "UcKkRobotTool";
            this.Size = new System.Drawing.Size(640, 542);
            this.Load += new System.EventHandler(this.UcKRTool_Load);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogCalibNPointToNPointEditV21)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage1;
        private Cognex.VisionPro.CalibFix.CogCalibNPointToNPointEditV2 cogCalibNPointToNPointEditV21;
        private System.Windows.Forms.TabControl tabControl1;
    }
}
