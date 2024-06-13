using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Vision.Core;
using Vision.Frm;
using Vision.Stations;
using Vision.Tools;

namespace Vision.Projects
{
    [ToolboxItem(false)]
    public partial class UcProject : UserControl
    {
        private UcControlBase _baseUI;
        private int cnt = 0; // 记录鼠标（左键）点击次数
        private Station _copyStation;

        public UcProject()
        {
            InitializeComponent();
            _baseUI = new UcControlBase();
        }

        /// <summary>
        /// treeview 单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvProject_MouseClick(object sender, MouseEventArgs e)
        {
            if (!ProjectManager.Instance.IsLoaded)
            {
                return;
            }
            Point ClickPoint = new Point(e.X, e.Y);
            TreeNode CurrentNode = this.tvProject.GetNodeAt(ClickPoint);
            if (MouseButtons.Right == e.Button)
            {
                CurrentNode.ContextMenuStrip = null;
                if (CurrentNode == null)
                {
                    tvProject.ContextMenuStrip = cmsPasteStation;
                }
                if (CurrentNode.Parent == null)
                {
                    //判断是顶级节点
                    CurrentNode.ContextMenuStrip = cmsProject;
                }
                else
                {
                    if (CurrentNode.Name.Contains("组"))
                    {
                        CurrentNode.ContextMenuStrip = cmsStation;
                    }
                    else if (CurrentNode.Name.Contains("子工具"))
                    {
                        CurrentNode.ContextMenuStrip = cmsTool;
                    }
                }
                this.tvProject.SelectedNode = CurrentNode;
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (CurrentNode != null)
                {
                    if (CurrentNode.Name.Contains("子工具"))
                    {
                        tvProject.SelectedNode = CurrentNode;
                        //获取被点击的工具
                        ShowTool(CurrentNode.Name);
                    }
                    else if (CurrentNode.Name.Contains("组"))
                    {
                        this.tvProject.SelectedNode = CurrentNode;
                        //工位被双击
                        ShowStation(CurrentNode.Text);
                    }
                }
            }
        }

        /// <summary>
        /// 显示station运行界面
        /// </summary>
        /// <param name="stationName"></param>
        private void ShowStation(string stationName)
        {
            _baseUI.AddStationUI(ProjectManager.Instance.Project[stationName]);
            _baseUI.Dock = DockStyle.Fill;
            panelMain.Controls.Clear();
            panelMain.Controls.Add(_baseUI);
            _baseUI.Show();
        }

        /// <summary>
        /// 显示工具界面
        /// </summary>
        /// <param name="path"></param>
        private void ShowTool(string path)
        {
            var data = ProjectManager.Instance.GetStationAndTool(path);
            _baseUI.AddToolUI(data.Station, data.Tool);
            _baseUI.ToolEnableEvent += ToolEnableEvent;
            _baseUI.Dock = DockStyle.Fill;
            panelMain.Controls.Clear();
            panelMain.Controls.Add(_baseUI);
            _baseUI.Show();
        }

        /// <summary>
        /// 工具开启/关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolEnableEvent(object sender, bool e)
        {
            var toolNode = tvProject.SelectedNode;
            toolNode.ForeColor = e ? Color.Black : Color.LightGray;
        }

        /// <summary>
        /// 清除Control显示
        /// </summary>
        private void ShowNull()
        {
            panelMain.Controls.Clear();
        }

        private void UcProject_Load(object sender, System.EventArgs e)
        {
            ProjectManager.Instance.TreeChangedEvent += Instance_TreeChangedEvent;
            ProjectManager.Instance.UpdateTreeNode();
            InitTreeState(this.tvProject.Nodes);
        }

        /// <summary>
        /// 更新显示TreeView
        /// </summary>
        /// <param name="nodes"></param>
        private void InitTreeState(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                InitTreeState(node.Nodes);
            }
        }

        /// <summary>
        /// TreeView变更事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Instance_TreeChangedEvent(object sender, TreeEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action(() =>
                    {
                        Instance_TreeChangedEvent(sender, e);
                    })
                );
                return;
            }
            if (e.Node != null)
            {
                tvProject.Nodes.Clear();
                tvProject.Nodes.Add(e.Node);
                //tvProject.ExpandAll();
            }
        }

        /// <summary>
        /// root节点点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsProject_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem;
            if (item == null)
                return;
            if (item.Text == "新增工位")
            {
                Cursor = Cursors.WaitCursor;
                ProjectManager.Instance.AddStation();
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 工具点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsTool_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var selectNode = tvProject.SelectedNode.Name;
            var data = ProjectManager.Instance.GetStationAndTool(selectNode);
            var station = data.Station;
            var tool = data.Tool;

            var item = e.ClickedItem;
            if (item == null)
                return;
            switch (item.Text)
            {
                case "删除工具":
                    ProjectManager.Instance.DeleteTool(station, tool);
                    ShowNull();
                    break;

                case "重命名":
                    ProjectManager.Instance.RenameTool(station, tool);
                    break;

                case "上移":
                    ProjectManager.Instance.UpTool(station, tool);
                    break;

                case "下移":
                    ProjectManager.Instance.DownTool(station, tool);
                    break;
            }
        }

        /// <summary>
        /// 工位点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsStation_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var selectNode = tvProject.SelectedNode;
            var station = ProjectManager.Instance.Project[selectNode.Text];

            var item = e.ClickedItem;
            if (item == null)
                return;
            switch (item.Text)
            {
                case "新建工具":
                    FrmToolBox frm = new FrmToolBox();
                    frm.SelectedStation = ProjectManager.Instance.Project[
                        tvProject.SelectedNode.Text
                    ];
                    frm.ShowDialog();
                    break;

                case "工位重命名":
                    ProjectManager.Instance.RenameStation(station);
                    break;

                case "工位上移":
                    ProjectManager.Instance.UpStation(station);
                    break;

                case "工位下移":
                    ProjectManager.Instance.DownStation(station);
                    break;

                case "删除全部工具":
                    ProjectManager.Instance.DeleteAllTool(station);
                    ShowNull();
                    break;

                case "删除工位":
                    ProjectManager.Instance.DeleteStation(station);
                    ShowNull();
                    break;

                case "参数设置":
                    FormStationSet form = new FormStationSet(station);
                    form.ShowDialog();
                    break;

                case "复制工位":
                    _copyStation = ProjectManager.Instance.CopyStation(station);
                    break;
            }
        }

        /// <summary>
        /// 工位粘贴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsPasteStation_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var item = e.ClickedItem;
            if (item == null)
                return;
            switch (item.Text)
            {
                case "粘贴工位":
                    ProjectManager.Instance.PasteStation(_copyStation);
                    break;
            }
        }

        private void Close()
        {
            ProjectManager.Instance.TreeChangedEvent -= Instance_TreeChangedEvent;
            _baseUI.ToolEnableEvent -= ToolEnableEvent;
        }

        #region treeview双击不折叠
        private void tvProject_MouseDown(object sender, MouseEventArgs e)
        {
            // 统计左键点击次数
            cnt = e.Clicks;
            if (e.Button == MouseButtons.Right)
            {
                if (_copyStation != null)
                    tvProject.ContextMenuStrip = cmsPasteStation;
            }
        }

        private void tvProject_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = cnt > 1;
        }

        private void tvProject_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = cnt > 1;
        }
        #endregion
    }
}
