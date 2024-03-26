namespace Vision.Tools
{
    partial class UcCenterCalibTool
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
            if(disposing && (components != null))
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cogToolBlockEditV21 = new Cognex.VisionPro.ToolBlock.CogToolBlockEditV2();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numRY = new System.Windows.Forms.NumericUpDown();
            this.numRX = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numCY = new System.Windows.Forms.NumericUpDown();
            this.numCX = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numRobotA = new System.Windows.Forms.NumericUpDown();
            this.numRobotY = new System.Windows.Forms.NumericUpDown();
            this.numRobotX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRX)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCX)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotX)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.ItemSize = new System.Drawing.Size(95, 35);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(940, 648);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 39);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Size = new System.Drawing.Size(932, 605);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "标定";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 75.86207F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(926, 601);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.tableLayoutPanel2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 2);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(920, 597);
            this.panel2.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.cogToolBlockEditV21, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(916, 593);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // cogToolBlockEditV21
            // 
            this.cogToolBlockEditV21.AllowDrop = true;
            this.cogToolBlockEditV21.ContextMenuCustomizer = null;
            this.cogToolBlockEditV21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogToolBlockEditV21.Location = new System.Drawing.Point(4, 5);
            this.cogToolBlockEditV21.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cogToolBlockEditV21.MinimumSize = new System.Drawing.Size(672, 0);
            this.cogToolBlockEditV21.Name = "cogToolBlockEditV21";
            this.cogToolBlockEditV21.ShowNodeToolTips = true;
            this.cogToolBlockEditV21.Size = new System.Drawing.Size(908, 464);
            this.cogToolBlockEditV21.SuspendElectricRuns = false;
            this.cogToolBlockEditV21.TabIndex = 0;
            this.cogToolBlockEditV21.Load += new System.EventHandler(this.cogToolBlockEditV21_Load);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 477);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(910, 113);
            this.panel1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(325, 13);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(261, 86);
            this.button2.TabIndex = 3;
            this.button2.Text = "打开标定";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Controls.Add(this.groupBox2);
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 39);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage3.Size = new System.Drawing.Size(932, 605);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "点位设置";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numRY);
            this.groupBox2.Controls.Add(this.numRX);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(3, 124);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(926, 122);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "旋转标定时机械手点位";
            // 
            // numRY
            // 
            this.numRY.DecimalPlaces = 3;
            this.numRY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numRY.Location = new System.Drawing.Point(427, 40);
            this.numRY.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numRY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRY.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numRY.Name = "numRY";
            this.numRY.Size = new System.Drawing.Size(139, 31);
            this.numRY.TabIndex = 23;
            this.numRY.ValueChanged += new System.EventHandler(this.numRY_ValueChanged);
            // 
            // numRX
            // 
            this.numRX.DecimalPlaces = 3;
            this.numRX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numRX.Location = new System.Drawing.Point(133, 38);
            this.numRX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numRX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRX.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numRX.Name = "numRX";
            this.numRX.Size = new System.Drawing.Size(139, 31);
            this.numRX.TabIndex = 22;
            this.numRX.ValueChanged += new System.EventHandler(this.numRX_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 24);
            this.label4.TabIndex = 16;
            this.label4.Text = "x";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(352, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 24);
            this.label7.TabIndex = 17;
            this.label7.Text = "y";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numCY);
            this.groupBox3.Controls.Add(this.numCX);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(3, 2);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Size = new System.Drawing.Size(926, 122);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "旋转中心点位";
            // 
            // numCY
            // 
            this.numCY.DecimalPlaces = 3;
            this.numCY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numCY.Location = new System.Drawing.Point(427, 40);
            this.numCY.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numCY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numCY.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numCY.Name = "numCY";
            this.numCY.Size = new System.Drawing.Size(139, 31);
            this.numCY.TabIndex = 23;
            this.numCY.ValueChanged += new System.EventHandler(this.numCY_ValueChanged);
            // 
            // numCX
            // 
            this.numCX.DecimalPlaces = 3;
            this.numCX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numCX.Location = new System.Drawing.Point(133, 38);
            this.numCX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numCX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numCX.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numCX.Name = "numCX";
            this.numCX.Size = new System.Drawing.Size(139, 31);
            this.numCX.TabIndex = 22;
            this.numCX.ValueChanged += new System.EventHandler(this.numCX_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(59, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 24);
            this.label5.TabIndex = 16;
            this.label5.Text = "x";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(352, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 24);
            this.label6.TabIndex = 17;
            this.label6.Text = "y";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numRobotA);
            this.groupBox1.Controls.Add(this.numRobotY);
            this.groupBox1.Controls.Add(this.numRobotX);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(3, 246);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(926, 161);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "机械手示教点位";
            // 
            // numRobotA
            // 
            this.numRobotA.DecimalPlaces = 3;
            this.numRobotA.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numRobotA.Location = new System.Drawing.Point(444, 67);
            this.numRobotA.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRobotA.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numRobotA.Name = "numRobotA";
            this.numRobotA.Size = new System.Drawing.Size(135, 31);
            this.numRobotA.TabIndex = 24;
            this.numRobotA.ValueChanged += new System.EventHandler(this.numRobotA_ValueChanged);
            // 
            // numRobotY
            // 
            this.numRobotY.DecimalPlaces = 3;
            this.numRobotY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numRobotY.Location = new System.Drawing.Point(134, 99);
            this.numRobotY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRobotY.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numRobotY.Name = "numRobotY";
            this.numRobotY.Size = new System.Drawing.Size(138, 31);
            this.numRobotY.TabIndex = 23;
            this.numRobotY.ValueChanged += new System.EventHandler(this.numRobotY_ValueChanged);
            // 
            // numRobotX
            // 
            this.numRobotX.DecimalPlaces = 3;
            this.numRobotX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.numRobotX.Location = new System.Drawing.Point(134, 38);
            this.numRobotX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRobotX.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numRobotX.Name = "numRobotX";
            this.numRobotX.Size = new System.Drawing.Size(138, 31);
            this.numRobotX.TabIndex = 22;
            this.numRobotX.ValueChanged += new System.EventHandler(this.numRobotX_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(372, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 24);
            this.label3.TabIndex = 20;
            this.label3.Text = "angle";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 24);
            this.label1.TabIndex = 16;
            this.label1.Text = "x";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 24);
            this.label2.TabIndex = 17;
            this.label2.Text = "y";
            // 
            // UcCenterCalibTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "UcCenterCalibTool";
            this.Size = new System.Drawing.Size(940, 648);
            this.Load += new System.EventHandler(this.UcCenterCalibTool_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogToolBlockEditV21)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRX)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCX)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRobotX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numCY;
        private System.Windows.Forms.NumericUpDown numCX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numRY;
        private System.Windows.Forms.NumericUpDown numRX;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private Cognex.VisionPro.ToolBlock.CogToolBlockEditV2 cogToolBlockEditV21;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown numRobotA;
        private System.Windows.Forms.NumericUpDown numRobotY;
        private System.Windows.Forms.NumericUpDown numRobotX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
