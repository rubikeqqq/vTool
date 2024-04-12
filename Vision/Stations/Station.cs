using Cognex.VisionPro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Vision.Core;
using Vision.Projects;
using Vision.Tools.Interfaces;
using Vision.Tools.ToolImpls;

namespace Vision.Stations
{
    /// <summary>
    /// 工位类
    /// 每一个工位对应一个工位类
    /// </summary>
    [Serializable]
    public class Station
    {
        private string _name;

        private bool _enabled = true;

        [NonSerialized]
        private ManualResetEvent _manualResetEvent;

        [NonSerialized]
        private Thread _cycleThread;

        [NonSerialized]
        private bool _threadFlag;

        [NonSerialized]
        private bool _cycle;

        [field: NonSerialized]
        public object ShowImage {  get; set; }

        /// <summary>
        /// 工位名称
        /// </summary>
        public string StationName
        {
            get => _name;
            set
            {
                if(_name == value) return;
                _name = value;
                DisplayView?.SetTitle(value);
            }
        }

        /// <summary>
        /// 工位是否启用
        /// </summary>
        public bool Enable
        {
            get { return _enabled; }
            set
            {
                if(_enabled == value) return;
                _enabled = value;
                StationEnableEvent?.Invoke(this,value);
                ProjectManager.Instance.UpdateTreeNode();
                LogUI.AddToolLog(value? $"[{StationName}] 工位启用": $"[{StationName}] 工位关闭");
            }
        }

        /// <summary>
        /// 工位显示record名称
        /// </summary>
        public string LastRecordName { get; set; }

        /// <summary>
        /// 工具集合
        /// </summary>
        public List<ToolBase> ToolList { get; set; } = new List<ToolBase>();

        [field: NonSerialized]
        public event EventHandler<ShowDebugWindowEventArgs> StationDebugShowEvent;

        [field: NonSerialized]
        public event EventHandler<bool> StationEnableEvent;

        [field: NonSerialized]
        public event EventHandler<StationShowChangedEventArgs> StationDisplayChangedEvent;

        [field: NonSerialized]
        public CogDisplayView DisplayView { get; set; }

        [field: NonSerialized]
        public StationDataConfig DataConfig { get; set; }

        public ToolBase this[string name]
        {
            get
            {
                if(this.ToolList != null)
                {
                    return this.ToolList.FirstOrDefault(tool => tool.ToolName == name);
                }
                else
                {
                    return null;
                }
            }
        }

        public Station()
        {
            DataConfig = new StationDataConfig();
        }

        public void Init()
        {
            DataConfig = new StationDataConfig();
        }

        #region 运行相关
        /// <summary>
        /// 运行单次
        /// </summary>
        public void Run()
        {
            if(ToolList.Count > 0)
            {
                ShowImage = null;
                var result = true;
                string errMsg = string.Empty;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach(ToolBase tool in ToolList)
                {
                    try
                    {
                        tool.Run();
                    }
                    catch(Exception e)
                    {
                        result = false;

                        //一个工具NG后还是要发送NG的结果的
                        ToolList.Find(x => x is ResultTool)?.Run();
                        ToolList.Find(x => x is EndTool)?.Run();
                        //打印工具失败
                        LogUI.AddLog($"[{StationName}] => {e.Message}");

                        break;
                    }
                }
                stopwatch.Stop();
                var time = stopwatch.Elapsed;
                //显示
                ShowWindow(new ShowWindowEventArgs(result,time,ShowImage));
                //存图
                SaveImage(result);
            }
        }

        /// <summary>
        /// 调试运行
        /// </summary>
        public void DebugRun()
        {
            if(ToolList.Count > 0)
            {
                ShowImage = null;
                var result = true;
                string errMsg = string.Empty;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                foreach(ToolBase tool in ToolList)
                {
                    try
                    {
                        Stopwatch stopwatch1 = new Stopwatch();
                        stopwatch1.Start();
                        tool.RunDebug();
                        stopwatch1.Stop();
                        var t = stopwatch1.Elapsed;
                        LogUI.AddToolLog(tool.ToolName + "=> " + t.TotalMilliseconds.ToString("f2") + "ms");
                    }
                    catch(Exception ex)
                    {
                        result = false;
                        LogUI.AddToolLog($"[{StationName}] => {ex.Message}");
                        break;
                    }
                }
                stopwatch.Stop();
                var time = stopwatch.Elapsed;
                ShowDebugWindow(new ShowDebugWindowEventArgs(result,time,ShowImage,errMsg));
            }
        }

        /// <summary>
        /// 如果未开启线程 则开启线程
        /// 如果已经开启 则将线程继续
        /// </summary>
        public void Start()
        {
            if(!_cycle)
            {
                if(_manualResetEvent == null)
                {
                    _manualResetEvent = new ManualResetEvent(true);
                }

                _cycleThread = new Thread(RunCycle)
                {
                    IsBackground = true
                };
                _threadFlag = true;
                _cycleThread.Start();
            }
            else
            {
                _manualResetEvent?.Set();
            }
        }

        /// <summary>
        /// 停止循环运行
        /// </summary>
        public void Stop()
        {
            _manualResetEvent?.Reset();
        }

        /// <summary>
        /// 循环运行
        /// </summary>
        private void RunCycle()
        {
            _cycle = true;
            while(_threadFlag)
            {
                if(Enable)
                {
                    _manualResetEvent?.WaitOne();
                    Run();
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 窗口显示
        /// </summary>
        /// <param name="args"></param>
        private void ShowWindow(ShowWindowEventArgs args)
        {
            //运行视图模式下显示
            if (DisplayView != null)
            {
                //先清除之前的显示
                DisplayView.ClearDisplay();

                DisplayView.SetResultGraphicOnRecordDisplay(args.Image);
                DisplayView.SetTime(args.Time);
                DisplayView.GraphicCreateLabel(args.Result);
            }
        }

        /// <summary>
        /// 调试模式显示
        /// </summary>
        /// <param name="args"></param>
        private void ShowDebugWindow(ShowDebugWindowEventArgs args)
        {
            //debug模式下
            OnStationRan(args);
        }

        /// <summary>
        /// 保存图像
        /// </summary>
        /// <exception cref="System.Exception">当图像保存的路径未设置时 抛出路径未设置异常</exception>
        private void SaveImage(bool result)
        {
            try
            {

                if(string.IsNullOrEmpty(Config.ImageConfig.SaveImageDir))
                {
                    string err = "未设置图像的保存路径！";
                    LogNet.Log(err);
                    throw new System.Exception(err);
                }
                //按天进行保存 每张图像的名称就是当前的时间格式
                string fileName = DateTime.Now.ToString("HH_mm_ss_fff");

                //按配置进行OK,NG存储
                if(Config.ImageConfig.IsSaveOKImage && result)    //OK
                {
                    DisplayView?.SaveOriginImage(Config.ImageConfig.SaveImageDir + $"\\{StationName}\\OK\\",fileName);
                }
                if(Config.ImageConfig.IsSaveNGImage && !result)   //NG
                {
                    //保存原图
                    DisplayView?.SaveOriginImage(Config.ImageConfig.SaveImageDir + $"\\{StationName}\\NG\\",fileName);
                    //保存截屏
                    DisplayView?.SaveScreenImage(Config.ImageConfig.SaveImageDir + $"\\{StationName}\\NG\\",fileName + "_");
                }
            }
            catch(Exception ex)
            {
                LogNet.Log(ex.Message);
            }
        }

        #endregion

        #region 工具相关
        /// <summary>
        /// 新建工具
        /// </summary>
        /// <param name="tool"></param>
        public void AddTool(ToolBase tool)
        {
            string name = GenDefaultToolName(tool);
            tool.ToolName = name;
            try
            {
                if(tool is IRegisterStation rTool)
                {
                    rTool.RegisterStation(this);
                }
                if(tool is IVpp iTool)
                {
                    iTool.CreateVpp();
                    iTool.SaveVpp();
                }
                LogUI.AddToolLog($"[{tool.ToolName}]新建成功");
            }
            catch(Exception ex)
            {
                LogUI.AddToolLog($"工具新建失败  " + ex.Message);
            }
            ToolList.Add(tool);
        }

        /// <summary>
        /// 删除工具
        /// </summary>
        /// <param name="tool"></param>
        public void DeleteTool(ToolBase tool)
        {
            //先判断存在
            if(this[tool.ToolName] != null)
            {
                if(tool is IVpp iTool)
                {
                    iTool.RemoveVpp();
                }
                ToolList.Remove(tool);
                LogUI.AddToolLog($"[{tool.ToolName}]移除成功");
            }
        }

        /// <summary>
        /// 工具重命名
        /// </summary>
        /// <param name="tool"></param>
        /// <param name="newName"></param>
        /// <exception cref="Exception"></exception>
        public void RenameTool(ToolBase tool,string newName)
        {
            if(tool == null) return;

            //判断新的名称工具是否已经存在
            if(this[newName] != null)
            {
                LogUI.AddToolLog("新的名称已经存在！");
            }

            string oldName = tool.ToolName;
            //判断旧的工具是否存在
            if(this[tool.ToolName] == null) return;
            foreach(var t in ToolList)
            {
                if(t.ToolName == tool.ToolName)
                {
                    t.ToolName = newName;
                    if(t is IVpp)
                    {
                        var oldPath = Path.Combine(ProjectManager.ProjectDir,StationName,$"{oldName}.vpp");
                        var newPath = Path.Combine(ProjectManager.ProjectDir,StationName,$"{newName}.vpp");
                        Local.MoveFile(oldPath,newPath);
                    }
                }
            }
            LogUI.AddToolLog($"[{oldName}]重命名成功");
        }

        /// <summary>
        /// 删除所有工具
        /// </summary>
        public void RemoveAllTool()
        {
            if(ToolList != null)
            {
                foreach(var t in ToolList)
                {
                    if(t is IVpp)
                    {
                        File.Delete(Path.Combine(ProjectManager.ProjectDir,StationName,$"{t.ToolName}.vpp"));
                    }
                }
                ToolList.Clear();
                LogUI.AddToolLog($"[{StationName}]所有工具删除成功");
            }
        }

        /// <summary>
        /// 获取toolblock下所有的record名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetLastRunRecordName()
        {
            List<string> lists = new List<string>();

            //遍历所有CenterDetectTool
            foreach(var tool in ToolList)
            {
                if(tool is CenterDetectTool dTool)
                {
                    var lastRecord = dTool.ToolBlock.CreateLastRunRecord();
                    if(lastRecord != null)
                    {
                        foreach(ICogRecord r in lastRecord.SubRecords)
                        {
                            lists.Add(r.RecordKey);
                        }
                    }
                }
            }
            //遍历所有CenterDetectTool
            foreach(var tool in ToolList)
            {
                if(tool is DetectTool dTool)
                {
                    var lastRecord = dTool.ToolBlock.CreateLastRunRecord();
                    if(lastRecord != null)
                    {
                        foreach(ICogRecord r in lastRecord.SubRecords)
                        {
                            lists.Add(r.RecordKey);
                        }
                    }
                }
            }

            return lists;
        }

        /// <summary>
        /// 获取所有CenterDetectTool的Tool名称
        /// </summary>
        /// <returns></returns>
        private List<string> GetCenterDetectToolName()
        {
            List<string> inputs = new List<string>();
            foreach(var tool in ToolList)
            {
                if(tool is CenterDetectTool)
                {
                    //只添加之前的工具
                    if(tool.ToolName == StationName)
                        break;
                    inputs.Add(tool.ToolName);
                }
            }
            return inputs;
        }

        /// <summary>
        /// 获取第Index个的CenterDetectTool的Tool名称
        /// </summary>
        /// <returns></returns>
        public ToolBase GetCenterDetectTool(int index)
        {
            return this[GetCenterDetectToolName()[index]];
        }

        /// <summary>
        /// 获取所有继承KK机器人接口的工具名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetKkRobotCalibToolNames()
        {
            List<string> inputs = new List<string>();
            foreach(var tool in ToolList)
            {
                if(tool is KkRobotCalibTool)
                {
                    //只添加之前的工具
                    if(tool.ToolName == StationName)
                        break;
                    inputs.Add(tool.ToolName);
                }
            }
            return inputs;
        }

        /// <summary>
        /// 获取继承KK机器人接口的第index个的工具名称
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ToolBase GetRobotCalibTool(int index)
        {
            return this[GetKkRobotCalibToolNames()[index]];
        }

        /// <summary>
        /// 获取9点标定的工具
        /// </summary>
        /// <returns></returns>
        public NPointCalibTool GetNPointTool()
        {
            foreach(var tool in ToolList)
            {
                if(tool is NPointCalibTool nTool)
                {
                    return nTool;
                }
            }
            return null;
        }

        /// <summary>
        /// 注册station主窗体显示界面
        /// </summary>
        /// <returns></returns>
        public CogDisplayView RegisterViewDisplay()
        {
            if(DisplayView == null)
            {
                DisplayView = new CogDisplayView();
                DisplayView.SetTitle(StationName);
                DisplayView.ShowDisplay += DisplayView_ShowDisplayOne;
            }
            return DisplayView;
        }

        /// <summary>
        /// 获取所有图像工具名称
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public string[] GetImageInToolNames(ToolBase tool)
        {
            List<string> list = new List<string>();
            if(ToolList.Count > 0)
            {
                foreach(var toolBase in ToolList)
                {
                    if(toolBase == tool)
                    {
                        break;
                    }
                    if(toolBase is IImageOut)
                    {
                        if(toolBase.Enable)
                            list.Add(toolBase.ToolName);
                    }
                }
            }
            return list.ToArray();
        }

        public void Close()
        {
            foreach(var t in ToolList)
            {
                t.Close();
            }
            if(_threadFlag)
            {
                _threadFlag = false;
                _cycleThread.Abort();
                _cycleThread.Join();
            }
            if(DisplayView == null)
            {
                DisplayView.ShowDisplay -= DisplayView_ShowDisplayOne;
            }
        }

        /// <summary>
        /// 生成默认的工具名称
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        private string GenDefaultToolName(ToolBase tool)
        {
            int m = 1;

            var defaultName = tool.GetType().GetCustomAttribute<ToolNameAttribute>()?.Name;
            string name = defaultName + m.ToString();
            while(ToolExsit(name))
            {
                m++;
                name = defaultName + m.ToString();
            }
            return defaultName + m.ToString();
        }

        /// <summary>
        /// 检查工具是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool ToolExsit(string name)
        {
            return ToolList.Exists(x => x.ToolName == name);
        }

        /// <summary>
        /// 工位运行事件触发器
        /// </summary>
        /// <param name="args"></param>
        private void OnStationRan(ShowDebugWindowEventArgs args)
        {
            StationDebugShowEvent?.Invoke(this,args);
        }

        private void DisplayView_ShowDisplayOne(object sender,StationShowChangedEventArgs e)
        {
            e.StationName = StationName;
            StationDisplayChangedEvent?.Invoke(sender,e);
        }
        #endregion

        /// <summary>
        /// 加载数据
        /// </summary>
        public void LoadData()
        {
            string path = Path.Combine(ProjectManager.ProjectDir,StationName,"Data.ini");
            if(File.Exists(path))
            {
                if(!DataConfig.LoadConfig(path))
                {
                    ($"[{StationName}] 参数加载失败！").MsgBox();
                }
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveData()
        {
            string path = Path.Combine(ProjectManager.ProjectDir,StationName,"Data.ini");
            if(!File.Exists(path))
            {
                File.Create(path).Close();
            }
            DataConfig.SaveConfig(path);
            "数据保存成功！".MsgBox();
        }

        /// <summary>
        /// 深拷贝当前对象
        /// </summary>
        /// <returns></returns>
        public Station DeepClone()
        {
            return SerializerHelper.Clone(this);
        }
    }
}