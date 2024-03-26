using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.Core;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Tools
{
    [ToolboxItem(false)]
    public partial class UcControlBase : UserControl
    {
        public UcControlBase()
        {
            InitializeComponent();
            _ucTool = new UcToolBase();
            _ucDebug = new UcDebug();
        }

        private UcToolBase _ucTool;

        private UcDebug _ucDebug;

        public ToolBase Tool { get;set; }

        public Station Station { get; set; }

        public event EventHandler<bool> ToolEnableEvent;

        public void AddStationUI(Station station)
        {
            Station = station;
            _ucDebug.ChangeStation(station);
            _ucDebug.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(_ucDebug);
            _ucDebug.Show();
            label1.Text = station.StationName;
        }

        public void AddToolUI(Station station, ToolBase tool)
        {
            Station = station;
            Tool = tool;
            _ucTool.ToolEnableChangedEvent += _ucTool_ToolEnableChangedEvent;
            _ucTool.ChangeTool(station, tool);
            _ucTool.Dock = DockStyle.Fill;
            panel1.Controls.Clear();
            panel1.Controls.Add(_ucTool);
            _ucTool.Show();
            label1.Text = $"{station.StationName} {tool.ToolName}";
        }

        private void _ucTool_ToolEnableChangedEvent(object sender, bool e)
        {
            ToolEnableEvent?.Invoke(this, e);
        }

        private void UcControlBase_Load(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var s = LogUI.GetToolLog();
                    listBox1.BeginInvoke(new Action(() =>
                    {
                        if (s != null)
                        {
                            if (listBox1.Items.Count > 100)
                            {
                                listBox1.Items.Remove(0);
                            }
                            listBox1.Items.Add(s);
                            listBox1.TopIndex = listBox1.Items.Count - 1;
                        }

                    }));
                    await Task.Delay(100);
                }
            });
        }
    }
}
