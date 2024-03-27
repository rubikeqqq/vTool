using System;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;

namespace Vision.Tools.ToolImpls
{
    /// <summary>
    /// 工具基类 所有的工具类都要继承此接口
    /// </summary>
    [Serializable]
    public abstract class ToolBase
    {
        private bool _enable = true;

        /// <summary>
        /// 外部显示的名称
        /// </summary>
        public string ToolName { get; set; }

        /// <summary>
        /// 工具是否启用
        /// </summary>
        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable == value) return;
                _enable = value;
                string msg = value ? $"[{ToolName}] 启用" : $"[{ToolName}] 禁用";
                LogUI.AddToolLog(msg);
            }
        }

        /// <summary>
        /// 运行工具
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// 调试运行工具
        /// </summary>
        public abstract void RunDebug();
        
        /// <summary>
        /// 保存工具
        /// </summary>
        public virtual void Save()
        {
            LogUI.AddLog(ProjectManager.Instance.SaveProject() ? "保存成功！" : "保存失败！");
        }

        /// <summary>
        /// 关闭工具
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// 获取工具的界面
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public abstract UserControl GetToolControl(Station station);
    }

    /// <summary>
    /// 工具运行异常类
    /// </summary>
    public class ToolException : Exception
    {
        /// <summary>
        /// 图像为空
        /// </summary>
        public bool ImageInNull { get; set; }

        /// <summary>
        /// 工具运行失败
        /// </summary>
        public bool RunError { get; set; }

        /// <summary>
        /// 工具名称
        /// </summary>
        public string ToolName { get; set; }

        public ToolException(string message) : base(message) 
        {
        }
    }

}