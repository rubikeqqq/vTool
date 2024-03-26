﻿using System;
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

        protected ToolBase()
        {
        }

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

        public abstract void Run();

        public virtual void Save()
        {
            LogUI.AddLog(ProjectManager.Instance.SaveProject() ? "保存成功！" : "保存失败！");
        }

        public virtual void Close()
        {
        }

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