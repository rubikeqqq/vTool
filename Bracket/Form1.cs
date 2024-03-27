using PlcComm;
using System;
using System.Windows.Forms;
using Vision;

namespace Bracket
{
    public partial class Form1 : Form
    {
        UcMain _main;
        private Melsoft_PLC_TCP2 _plc = new Melsoft_PLC_TCP2();
        public Form1()
        {
            InitializeComponent();
            //_plc.sClient("127.0.0.1", "6000");
            //_plc.Connect();
            //var f = _plc.IsConnected;
            _main = new UcMain(null);
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
