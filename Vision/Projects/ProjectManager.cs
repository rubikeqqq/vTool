using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Vision.Core;
using Vision.Frm;
using Vision.Stations;
using Vision.Tools.Interfaces;
using Vision.Tools.ToolImpls;

namespace Vision.Projects
{
    public class ProjectManager
    {
        private static ProjectManager _instance;
        private Project _project;
        private static object _lock = new object();
        private ProjectManager()
        {
            if (!Directory.Exists(ProjectDir))
            {
                Directory.CreateDirectory(ProjectDir);
            }
            try
            {
                OpenProject();
                LoadConfig();
            }
            catch (Exception ex)
            {
                ex.MsgBox();
            }
        }

        /// <summary>
        /// 项目保存前置事件
        /// </summary>
        public event EventHandler BeforeSaveProjectEvent;

        /// <summary>
        /// 显示界面全屏/分屏
        /// </summary>
        public event EventHandler<StationShowChangedEventArgs> UcStationChangedEvent;

        /// <summary>
        /// TreeView改变事件
        /// </summary>
        public event EventHandler<TreeEventArgs> TreeChangedEvent;

        public static ProjectManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ProjectManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 项目已经加载
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Project数据
        /// </summary>
        public Project Project
        {
            get => _project;
            set => _project = value;
        }

        /// <summary>
        /// 项目文件路径
        /// </summary>
        public static string ProjectPath => Path.Combine(Application.StartupPath, "Project", "proj.vpr");

        /// <summary>
        /// 项目文件夹
        /// </summary>
        public static string ProjectDir => Path.Combine(Application.StartupPath, "Project");

        /// <summary>
        /// 配置文件
        /// </summary>
        public static string ConfigPath => Path.Combine(ProjectDir, "config.ini");

        #region 【项目加载保存】

        /// <summary>
        /// 保存project 
        /// </summary>
        /// <exception cref="Exception">如果保存出现错误 返回exception</exception>
        public bool SaveProject()
        {
            if (!IsLoaded) return false;
            try
            {
                //项目保存前置事
                OnBeforeSaveProject();
            }
            catch (Exception ex)
            {
                LogNet.Log("项目保存失败！\r\n " + ex.Message);
                return false;
            }

            try
            {
                var res = SerializerHelper.SerializeToBinary(_project, ProjectPath);
                LogNet.Log($"项目保存成功！");
                if (!res)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.Message.MsgBox();
                return false;
            }
        }

        /// <summary>
        /// 打开项目
        /// </summary>
        public void OpenProject()
        {
            if (IsLoaded)
            {
                return;
            }

            if (!File.Exists(ProjectPath))
            {
                _project = new Project();
                IsLoaded = true;
                LogNet.Log("项目创建成功！");
                return;
            }
            try
            {
                Project data = SerializerHelper.DeSerializeFromBinary<Project>(ProjectPath);
                if (data != null)
                {
                    _project = data;
                    //加载vpp
                    foreach (var station in _project.StationList)
                    {
                        foreach (var tool in station.ToolList)
                        {
                            if (tool is IRegisterStation rTool)
                            {
                                rTool.RegisterStation(station);
                            }
                            if (tool is IVpp iTool)
                            {
                                iTool.LoadVpp();
                            }
                        }
                        station.RegisterViewDisplay();
                        station.ShowDisplayChangedEvent += Station_ShowDisplayChangedEvent;
                    }
                    IsLoaded = true;
                    LogNet.Log("项目载入成功！");
                    LogUI.AddLog("项目载入成功！");
                }
            }
            catch
            {
                IsLoaded = false;
                var msg = "视觉项目载入失败!";
                LogNet.Log(msg);
                LogUI.AddLog(msg);
                throw;
            }
        }

        /// <summary>
        /// 关闭项目
        /// </summary>
        public void CloseProject()
        {
            if (!IsLoaded) return;
            foreach (var station in _project.StationList)
            {
                station.ShowDisplayChangedEvent -= Station_ShowDisplayChangedEvent;
            }
            _project.Close();
            IsLoaded = false;
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        public bool LoadConfig()
        {
            return Config.LoadConfig(ConfigPath);
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <returns></returns>
        public bool SaveConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                File.Create(ConfigPath).Close();
            }
            return Config.SaveConfig(ConfigPath);
        }

        #endregion

        #region 【工位工具】
        /// <summary>
        /// 添加工位
        /// </summary>
        /// <returns></returns>
        public bool AddStation()
        {
            if (!IsLoaded) return false;
            if (_project == null) return false;

            if (!_project.AddStation())
            {
                return false;
            }
            try
            {
                //因为不知道station的名称 但是可以确定是最后一个添加的
                _project[_project.StationList.Count - 1].ShowDisplayChangedEvent += Station_ShowDisplayChangedEvent;
                UpdateTreeNode();
                SaveProject();
                return true;
            }
            catch (Exception ex)
            {
                LogUI.AddToolLog(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// 删除工位
        /// </summary>
        /// <param name="station"></param>
        public void DeleteStation(Station station)
        {
            if (!IsLoaded) return;
            if (_project == null) return;

            if (MessageBox.Show("是否确定删除此工位，此过程可能导致程序无法正常运行", "重要提示",
                   MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            if (_project.DeleteStation(station))
            {
                UpdateTreeNode();
                SaveProject();
            }
        }

        /// <summary>
        /// 工位重命名
        /// </summary>
        /// <param name="station"></param>
        public void RenameStation(Station station)
        {
            if (!IsLoaded) return;
            if (_project == null) return;
            FrmRename frm = new FrmRename();
            frm.OldName = station.StationName;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                var newName = frm.NewName;
                if (_project.RenameStation(station, newName))
                {
                    UpdateTreeNode();
                    SaveProject();
                }
            }
        }

        /// <summary>
        /// 工位上移
        /// </summary>
        /// <param name="station"></param>
        public void UpStation(Station station)
        {
            if (!IsLoaded) return;
            var index = _project.StationList.FindIndex(x => x.StationName == station.StationName);
            if (index <= 0) return;
            _project.StationList.RemoveAt(index);
            _project.StationList.Insert(index - 1, station);
            UpdateTreeNode();
            SaveProject();
        }

        /// <summary>
        /// 工位下移
        /// </summary>
        /// <param name="station"></param>
        public void DownStation(Station station)
        {
            if (!IsLoaded) return;
            var index = _project.StationList.FindIndex(x => x.StationName == station.StationName);
            if (index >= _project.StationList.Count - 1) return;
            _project.StationList.RemoveAt(index);
            _project.StationList.Insert(index + 1, station);
            UpdateTreeNode();
            SaveProject();
        }

        /// <summary>
        /// 添加工具 
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        public void AddTool(Station station, ToolBase tool)
        {
            if (!IsLoaded) return;
            station.AddTool(tool);
            UpdateTreeNode();
            SaveProject();
        }

        /// <summary>
        /// 删除工具
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        /// <returns></returns>
        public void DeleteTool(Station station, ToolBase tool)
        {
            if (!IsLoaded) return;
            station.DeleteTool(tool);
            UpdateTreeNode();
            SaveProject();
        }

        /// <summary>
        /// 删除所有工具
        /// </summary>
        /// <param name="station"></param>
        public void DeleteAllTool(Station station)
        {
            if (!IsLoaded) return;
            station.RemoveAllTool();
            UpdateTreeNode();
            SaveProject();
        }

        /// <summary>
        /// 工具重命名
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        public void RenameTool(Station station, ToolBase tool)
        {
            if (!IsLoaded) return;
            FrmRename frm = new FrmRename();
            frm.OldName = tool.ToolName;
            if (frm.ShowDialog() == DialogResult.OK)
            {
                var newName = frm.NewName;
                station.RenameTool(tool, newName);
                UpdateTreeNode();
                SaveProject();
            }
        }

        /// <summary>
        /// 工具上移
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        public void UpTool(Station station, ToolBase tool)
        {
            if (!IsLoaded) return;
            var index = station.ToolList.FindIndex(x => x.ToolName == tool.ToolName);
            if (index <= 0) return;
            station.ToolList.RemoveAt(index);
            station.ToolList.Insert(index - 1, tool);
            UpdateTreeNode();
            SaveProject();
        }

        /// <summary>
        /// 工具下移
        /// </summary>
        /// <param name="station"></param>
        /// <param name="tool"></param>
        public void DownTool(Station station, ToolBase tool)
        {
            if (!IsLoaded) return;
            var index = station.ToolList.FindIndex(x => x.ToolName == tool.ToolName);
            if (index >= station.ToolList.Count - 1) return;
            station.ToolList.RemoveAt(index);
            station.ToolList.Insert(index + 1, tool);
            UpdateTreeNode();
            SaveProject();
        }

        #endregion

        #region 【事件触发器】
        /// <summary>
        /// 保存project前置事件
        /// </summary>
        private void OnBeforeSaveProject()
        {
            BeforeSaveProjectEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 显示改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Station_ShowDisplayChangedEvent(object sender, StationShowChangedEventArgs e)
        {
            UcStationChangedEvent?.Invoke(this, e);
        }

        /// <summary>
        /// Treeview改变触发器
        /// </summary>
        /// <param name="e"></param>
        private void OnTreeChanged(TreeEventArgs e)
        {
            TreeChangedEvent?.Invoke(this, e);
        }

        #endregion

        #region TreeView
        public void UpdateTreeNode()
        {
            TreeNode node = new TreeNode() { Text = "项目信息" };
            node.ImageIndex = 0;
            node.SelectedImageIndex = 0;
            node.Name = "项目信息";
            foreach (var station in Project.StationList)
            {
                TreeNode tnStation = new TreeNode(station.StationName);
                tnStation.Name = "组" + station.StationName;
                tnStation.ImageIndex = 1;
                tnStation.SelectedImageIndex = 1;
                foreach (var tool in station.ToolList)
                {
                    if (tool is null)
                    {
                        continue;
                    }
                    TreeNode tnTool = new TreeNode(tool.ToolName);
                    tnTool.Name = "子工具=" + tnStation.Text + ">" + tool.ToolName;
                    tnTool.ImageIndex = 2;
                    tnTool.SelectedImageIndex = 2;
                    tnStation.Nodes.Add(tnTool);
                }
                node.Nodes.Add(tnStation);
            }
            OnTreeChanged(new TreeEventArgs(node));
        }

        public List<string> GetToolNameList(string toolPath)
        {
            int index1 = toolPath.IndexOf("=");
            int index2 = toolPath.IndexOf(">");
            int length = toolPath.Length;
            string stationName = toolPath.Substring(index1 + 1, index2 - index1 - 1);
            string toolName = toolPath.Substring(index2 + 1, length - index2 - 1);
            List<string> list = new List<string>() { stationName, toolName };
            return list;
        }

        public StationToolData GetStationAndTool(string toolPath)
        {
            var nameList = GetToolNameList(toolPath);
            var station = _project[nameList[0]];
            var tool = station[nameList[1]];
            return new StationToolData(station, tool);
        }

        #endregion
    }
}