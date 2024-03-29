using System;
using System.Windows.Forms;
using Vision;

namespace Bracket
{
    public partial class Form1 : Form
    {
        UcMain _main;
        public Form1()
        {
            InitializeComponent();
            _main = new UcMain();
            this.WindowState = FormWindowState.Maximized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _main.Dock = DockStyle.Fill;
            panel1.Controls.Add(_main);
            _main.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _main.Close();
        }
    }
}
