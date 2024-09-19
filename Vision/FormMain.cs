using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;

namespace Vision
{
    [ToolboxItem(false)]
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            _ucWindow = new UcWindowShow(ProjectManager.Instance.Project);
            _ucProject = new UcProject();
            _ucSet = new UcSet();

            if (Config.SystemConfig.AutoRun)
            {
                Run();
            }
            ProjectManager.Instance.UcStationChangedEvent += Instance_UcStationChangedEvent;
            this.WindowState = FormWindowState.Maximized;
        }

        private UcWindowShow _ucWindow;
        private UcProject _ucProject;
        private UcSet _ucSet;
        private bool _cycle; //检测循环
        private bool _logCycle = true; //log循环flag

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (!ProjectManager.Instance.IsLoaded)
            {
                return;
            }
            AddControl(_ucWindow);
        }

        /// <summary>
        /// 显示log
        /// </summary>
        private void ShowLog()
        {
            Task.Run(async () =>
            {
                while (_logCycle)
                {
                    var s = LogUI.GetLog();
                    listBox1.BeginInvoke(
                        new Action(() =>
                        {
                            if (s != null)
                            {
                                if (listBox1.Items.Count > 200)
                                {
                                    listBox1.Items.RemoveAt(0);
                                }
                                listBox1.Items.Add(s);
                                listBox1.TopIndex = listBox1.Items.Count - 1;
                            }
                        })
                    );
                    await Task.Delay(100);
                }
            });
        }

        /// <summary>
        /// 添加控件
        /// </summary>
        /// <param name="control"></param>
        private void AddControl(UserControl control)
        {
            panel1.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panel1.Controls.Add(control);
        }

        /// <summary>
        /// 开始运行
        /// </summary>
        private void Run()
        {
            foreach (Station station in ProjectManager.Instance.Project.StationList)
            {
                station.Start();
                _cycle = true;
            }
            AddControl(_ucWindow);
            _ucWindow.ShowUnit();
        }

        /// <summary>
        /// 停止运行
        /// </summary>
        private void Stop()
        {
            foreach (Station station in ProjectManager.Instance.Project.StationList)
            {
                station.Stop();
                _cycle = false;
            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem;
            switch (item.Text)
            {
                case "视觉调试":
                    if (_cycle)
                    {
                        Stop();
                    }
                    AddControl(_ucProject);
                    break;
                case "设置界面":
                    if (_cycle)
                    {
                        Stop();
                    }
                    AddControl(_ucSet);
                    break;
                case "运行界面":
                    Run();
                    break;
                case "停止运行":
                    Stop();
                    break;
            }
        }

        private void Instance_UcStationChangedEvent(object sender, StationShowChangedEventArgs e)
        {
            if (e.ShowOne)
                _ucWindow.ShowUnit(e.StationName);
            else
                _ucWindow.ShowUnit();
        }

        private void FrmMain_Load(object sender, System.EventArgs e)
        {
            Init();
            ShowLog();
        }

        private void UcMain_FormClosing( object sender , FormClosingEventArgs e )
        {
            _logCycle = false;
            if( _cycle )
            {
                Stop( );
            }
            if( ProjectManager.Instance.IsLoaded )
            {
                ProjectManager.Instance.CloseProject( );
            }
            if( ProjectManager.Instance.Plc.IsOpened )
            {
                ProjectManager.Instance.Plc.ClosePLC( );
            }
            ProjectManager.Instance.UcStationChangedEvent -= Instance_UcStationChangedEvent;
        }
    }
}
