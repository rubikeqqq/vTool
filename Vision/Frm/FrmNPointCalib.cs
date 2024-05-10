using System;
using System.IO;
using System.Windows.Forms;

using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.ID;

using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Frm
{
    public partial class FrmNPointCalib : Form
    {
        public FrmNPointCalib(Station station, NPointCalibTool nTool)
        {
            InitializeComponent();
            _path = Path.Combine(ProjectManager.ProjectDir, station.StationName, "Calib.xml");
            LoadData();
            _nTool = nTool;
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
        }

        private readonly NPointCalibTool _nTool;
        private ICogImage _image;
        private int _index = 0;

        private CogAcqFifoTool _acqTool;
        private CogIDTool _idTool;
        private CogCalibNPointToNPointTool _nPointTool;
        private CogIDResult _idResult;

        private CenterDataList _centerDataList = new CenterDataList();
        private readonly string _path;

        /// <summary>
        /// 采集按钮颜色改变
        /// </summary>
        private void ActiveButtonEnable()
        {
            btnStopLive.Enabled = cogRecordDisplay1.LiveDisplayRunning;
            btnStartLive.Enabled = !cogRecordDisplay1.LiveDisplayRunning;
        }

        /// <summary>
        /// 运行标定
        /// </summary>
        private void Calibration()
        {
            try
            {
                _nPointTool.InputImage = _image;
                _nPointTool.Calibration.Calibrate();
                Log("标定完成！");
            }
            catch (Exception ex)
            {
                ex.Message.MsgBox();
            }
        }

        /// <summary>
        /// 采集图像
        /// </summary>
        /// <returns></returns>
        private bool Grab()
        {
            if (cogRecordDisplay1.LiveDisplayRunning)
            {
                Log("请先停止连续相机取图");
                return false;
            }

            try
            {
                if (_acqTool != null)
                {
                    _acqTool.Run();
                    if (_acqTool.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        _image = _acqTool.OutputImage;
                        return true;
                    }
                    else
                    {
                        Log("取图失败");
                        return false;
                    }
                }
                return false;
                //((CogImageFileTool)_nTool.ToolBlock.Tools["CogImageFileTool1"]).Run();
                //_image = ((CogImageFileTool)_nTool.ToolBlock.Tools["CogImageFileTool1"]).OutputImage;
                //return true;
            }
            catch (Exception ex)
            {
                ex.MsgBox();
                return false;
            }
        }

        /// <summary>
        /// 检查工具是否齐全
        /// </summary>
        /// <returns></returns>
        private bool CheckTools()
        {
            if (_nTool != null)
            {
                var tb = _nTool.ToolBlock;
                if (tb != null)
                {
                    try
                    {
                        _acqTool = tb.Tools["CogAcqFifoTool1"] as CogAcqFifoTool;
                        _idTool = tb.Tools["CogIDTool1"] as CogIDTool;
                        _nPointTool = tb.Tools["CogCalibNPointToNPointTool1"] as CogCalibNPointToNPointTool;
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// 如果已经标定 将标定的点位显示到表格中
        /// </summary>
        private void InitControl()
        {
            if (CheckTools())
            {
                try
                {
                    //有标定数据
                    if (_centerDataList.CenterList.Count == 0)
                    {
                        return;
                    }
                    for (int i = 0; i < _centerDataList.CenterList.Count; i++)
                    {
                        dgv.Rows.Add(i + 1, _centerDataList.CenterList[i].ImageX, _centerDataList.CenterList[i].ImageY,
                       _centerDataList.CenterList[i].RobotX, _centerDataList.CenterList[i].RobotY);
                        _index++;
                    }

                }
                catch (Exception ex)
                {
                    ex.Message.MsgBox();
                }
            }
            else
            {
                "标定工具不全，请检查！".MsgBox();
            }
        }

        /// <summary>
        /// 设置表格显示
        /// </summary>
        private void SetDgv()
        {
            for (int i = 0; i < 3; i++)
            {
                dgv.Columns[i].ReadOnly = true;
            }
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// 传入点位
        /// </summary>
        private bool SetPoints()
        {
            if (dgv.Rows.Count != 9)
            {
                Log("标定的点位不足9个");
                return false;
            }
            try
            {
                //清除原有点位
                int pNumber = _nPointTool.Calibration.NumPoints;
                for (int i = 0; i < pNumber; i++)
                {
                    _nPointTool.Calibration.DeletePointPair(0);
                }
                _centerDataList.Clear();

                //添加新的点位
                for (int i = 0; i < 9; i++)
                {
                    var cell1 = dgv.Rows[i].Cells[1].Value;
                    var cell2 = dgv.Rows[i].Cells[2].Value;
                    var cell3 = dgv.Rows[i].Cells[3].Value;
                    var cell4 = dgv.Rows[i].Cells[4].Value;

                    if (cell1 == null || cell2 == null || cell3 == null || cell4 == null)
                    {
                        Log("点位不全，请检查！");
                        return false;
                    }

                    double x1 = Convert.ToDouble(cell1.ToString().Trim());
                    double y1 = Convert.ToDouble(cell2.ToString().Trim());
                    double x2 = Convert.ToDouble(cell3.ToString().Trim());
                    double y2 = Convert.ToDouble(cell4.ToString().Trim());


                    _nPointTool.Calibration.AddPointPair(x1, y1, x2, y2);
                    _centerDataList.Add(new CenterData() { ImageX = x1, ImageY = y1, RobotX = x2, RobotY = y2 });
                }
                return true;
            }
            catch (Exception ex)
            {
                Log($"点位设置失败，请检查\r\n{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// dgv添加数据
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void AddDgv(double x, double y)
        {
            dgv.Rows.Add(_index + 1, x, y);
        }

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="graphic"></param>
        private void ShowRecord(ICogGraphic graphic)
        {
            //先清除图像
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.Image = null;

            if (graphic != null)
            {
                cogRecordDisplay1.Image = _image;
                cogRecordDisplay1.StaticGraphics.Add(graphic, "");
                cogRecordDisplay1.AutoFit = true;
            }
            else
            {
                cogRecordDisplay1.Image = _image;

                CogGraphicLabel label = new CogGraphicLabel();
                label.Font = new System.Drawing.Font("宋体", 20);
                label.Color = CogColorConstants.Red;
                label.SetXYText(100, 100, "二维码未找到！");
                label.Alignment = CogGraphicLabelAlignmentConstants.TopLeft;
                cogRecordDisplay1.StaticGraphics.Add(label, "");

                cogRecordDisplay1.AutoFit = true;
            }
        }

        /// <summary>
        /// 采集按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartLive_Click(object sender, System.EventArgs e)
        {
            if (_acqTool != null && !cogRecordDisplay1.LiveDisplayRunning)
            {
                if (_acqTool.Operator != null)
                {
                    cogRecordDisplay1.StaticGraphics.Clear();
                    cogRecordDisplay1.InteractiveGraphics.Clear();
                    cogRecordDisplay1.StartLiveDisplay(_acqTool.Operator);
                    cogRecordDisplay1.AutoFit = true;
                    cogRecordDisplay1.Fit(true);
                }
                else
                {
                    "相机未设置".MsgBox();
                }
            }

            ActiveButtonEnable();
        }

        /// <summary>
        /// 停止采集按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStopLive_Click(object sender, EventArgs e)
        {
            if (_acqTool != null && cogRecordDisplay1.LiveDisplayRunning)
            {
                if (_acqTool.Operator != null)
                    cogRecordDisplay1.StopLiveDisplay();
                else
                    "相机未设置".MsgBox();
            }

            ActiveButtonEnable();
        }

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmNCalib_Load(object sender, EventArgs e)
        {
            ActiveButtonEnable();
            SetDgv();
            InitControl();
        }

        /// <summary>
        /// 显示log
        /// </summary>
        /// <param name="log"></param>
        private void Log(string log)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke(new Action<string>(Log));
                return;
            }
            listBox1.Items.Add(DateTime.Now.ToString("T") + "   " + log);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }

        /// <summary>
        /// 保存标定数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveCalib_Click(object sender, EventArgs e)
        {
            _nTool?.SaveVpp();
            SaveData();
        }

        /// <summary>
        /// 运行查找特征点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender, EventArgs e)
        {
            bool r = Grab();
            try
            {
                if (_index >= 9)
                {
                    Log("已经有9个点");
                    return;
                }

                if (r)
                {
                    _idTool.InputImage = _image;

                    _idTool.Run();
                    if (_idTool.RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        if (_idTool.Results.Count > 0)
                        {
                            //第一次设置查找的ID
                            if (_index == 0)
                            {
                                GetCalcID(_idTool);
                            }
                            if (_idResult == null)
                            {
                                Log("未设置标准idTool");
                                return;
                            }

                            //查找设置的标准id
                            foreach (CogIDResult res in _idTool.Results)
                            {
                                if (res.DecodedData.DecodedString == _idResult.DecodedData.DecodedString)
                                {
                                    var x = Math.Round(res.CenterX, 3);
                                    var y = Math.Round(res.CenterY, 3);
                                    AddDgv(x, y);
                                    _index++;

                                    //显示
                                    var g = res.BoundsPolygon;
                                    g.Color = CogColorConstants.Green;
                                    g.LineWidthInScreenPixels = 2;
                                    ShowRecord(g);

                                    //log显示数据
                                    Log($"第{_index}个点：x={x} y={y}");
                                    return;
                                }
                            }

                            //所有的都不匹配
                            ShowRecord(null);
                            Log("二维码未找到！");
                        }
                        else
                        {
                            ShowRecord(null);
                            Log("二维码未找到！");
                            return;
                        }
                    }
                    else
                    {
                        ShowRecord(null);
                        Log("二维码未找到！");
                    }
                }
            }
            catch (Exception ex)
            {
                ex.MsgBox();
            }
        }

        /// <summary>
        /// 查找中心的二维码工具
        /// </summary>
        /// <param name="idTool"></param>
        private void GetCalcID(CogIDTool idTool)
        {
            ICogImage image = idTool.InputImage;
            double cY = image.Height / 2;
            double cX = image.Width / 2;

            double min = 9999;


            int index = 0;
            //解析码的数据

            for (int i = 0; i < idTool.Results.Count; i++)
            {
                double temp = Math.Pow((idTool.Results[i].CenterX - cX), 2) + Math.Pow((idTool.Results[i].CenterY - cY), 2);
                double dis = Math.Sqrt(temp);
                if (dis < min)
                {
                    min = dis;
                    index = i;
                }

            }

            _idResult = idTool.Results[index];
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            dgv.Rows.Clear();
            _centerDataList.Clear();
            _index = 0;
        }

        /// <summary>
        /// 标定计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalc_Click(object sender, EventArgs e)
        {
            if (SetPoints())
            {
                Calibration();
            }
        }

        private void FrmNPointCalib_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (_acqTool != null && cogRecordDisplay1.LiveDisplayRunning)
            //{
            //    if (_acqTool.Operator != null)
            //    {
            //        cogRecordDisplay1.StopLiveDisplay();
            //        if (_acqTool.Operator.FrameGrabber != null)
            //        {
            //            _acqTool.Operator.FrameGrabber.Disconnect(true);
            //            _acqTool.Dispose();
            //            _acqTool = null;
            //        }
            //    }

            //}
        }

        /// <summary>
        /// 加载标定数据
        /// </summary>
        private void LoadData()
        {
            if (File.Exists(_path))
            {
                _centerDataList = SerializerHelper.DeSerializeFromXml<CenterDataList>(_path);
            }
        }

        /// <summary>
        /// 保存标定数据
        /// </summary>
        private void SaveData()
        {
            if (!File.Exists(_path))
            {
                File.Create(_path).Close();
            }
            if (SerializerHelper.SerializeToXml(_centerDataList, _path))
            {
                Log("标定数据保存成功！");
            }
        }
    }
}
