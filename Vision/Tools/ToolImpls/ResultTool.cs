using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{

    [GroupInfo("结果工具", 3)]
    [ToolName("结果编辑", 0)]
    [Description("编辑结果和其他控制系统进行交互")]
    public class ResultTool : ToolBase, IRegisterStation
    {
        private Station _station;

        /// <summary>
        /// 运行的结果数据
        /// </summary>
        public List<ResultInfo> ResultData { get; set; } = new List<ResultInfo>();

        public UcResult UI { get; set; }

        public override UserControl GetToolControl(Station station)
        {
            if (UI == null)
            {
                UI = new UcResult(station, this);
            }
            else
            {
                UI.InitUI();
            }
            return UI;
        }

        public void RegisterStation(Station station)
        {
            _station = station;
        }

        public override void Run() 
        {
            RunTime = TimeSpan.Zero;
            if (!Enable) return;
            Stopwatch sw = Stopwatch.StartNew();
            GetResult();
            SendData();
            sw.Stop();
            RunTime = sw.Elapsed;
        }

        public override void RunDebug()
        {
            //调试模式时不运行
            RunTime = TimeSpan.Zero;
        }

        /// <summary>
        /// 获取结果
        /// </summary>
        /// <exception cref="ToolException"></exception>
        private void GetResult()
        {
            if (ResultData.Count == 0)
            {
                LogNet.Log($"[{_station.StationName}]没有分配输出结果");
                return;
            }
            foreach (ResultInfo result in ResultData)
            {
                //先获取工具
                var toolName = result.Source.Split('.')[0];
                var tool = _station[toolName];

                //数据源
                var data = result.Source.Split('.')[1];

                //分为2种
                //1、detectTool
                //2、IPointOut
                switch (tool)
                {
                    case CenterDetectTool dTool:
                        //根据名称获取结果
                        result.Value = dTool.GetValue(data);
                        break;

                    case DetectTool dTool:
                        //根据名称获取结果
                        result.Value = dTool.GetValue(data);
                        break;

                    case CenterCalibTool pointTool:
                        //总共就3个结果
                        switch (data)
                        {
                            case "X":
                                result.Value = pointTool.PointOut.X;
                                break;
                            case "Y":
                                result.Value = pointTool.PointOut.Y;
                                break;
                            case "Angle":
                                result.Value = pointTool.PointOut.Angle;
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <exception cref="ToolException"></exception>
        /// <exception cref="Exception"></exception>
        private void SendData()
        {
            if (ResultData.Count <= 0)
            {
                return;
            }
            var plc = ProjectManager.Instance.Plc;
            if (!plc.IsOpened)
            {
                LogUI.AddLog("plc未连接！");
                return;
            }

            try
            {
                foreach (ResultInfo res in ResultData)
                {
                    switch (res.Type)
                    {
                        case ResultType.Bool:
                            {
                                plc.WriteBool(res.Address, Convert.ToBoolean(res.Value));
                            }
                            break;
                        case ResultType.Short:
                            {
                                plc.WriteShort(res.Address, Convert.ToInt16(res.Value));
                            }
                            break;
                        case ResultType.Int:
                            {
                                plc.WriteInt(res.Address, Convert.ToInt32(res.Value));
                            }
                            break;
                        case ResultType.Double:
                            {
                                plc.WriteDouble(res.Address, Convert.ToDouble(res.Value));
                            }
                            break;
                        case ResultType.String:
                            {
                                plc.WriteString(res.Address, res.Value.ToString());
                            }
                            break;
                    }
                    Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {
                LogNet.Log("发送PLC结果失败：" + ex.Message);
                LogUI.AddLog("发送PLC结果失败：" + ex.Message);
            }
        }

        public override void LoadFromStream(SerializationInfo info,string toolName)
        {
            base.LoadFromStream(info,toolName);

            int n = info.GetInt32($"{toolName}.Count");
            //添加结果类
            ResultData = new List<ResultInfo>();

            ResultInfo result = null;

            for (int i = 0;i < n;i++)
            {
                string r = $"{toolName}.Result.{i}";
                var typeName = info.GetString(r);
                result = (ResultInfo)Assembly.GetExecutingAssembly().CreateInstance(typeName);
                result.LoadFromStream(info,r);

                ResultData.Add(result);
            }
        }

        public override void SaveToStream(SerializationInfo info,string toolName)
        {
            base.SaveToStream(info,toolName);

           //添加结果类
           info.AddValue($"{toolName}.Count",ResultData.Count);

            int n = 0;
            foreach(var res in ResultData)
            {
                string r = $"{toolName}.Result.{n}";
                info.AddValue(r,res.GetType().FullName);
                res.SaveToStream(info,r);
                n++;
            }
        }
    }
}
