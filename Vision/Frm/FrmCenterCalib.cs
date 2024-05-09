using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.ID;
using Cognex.VisionPro.ImageFile;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Vision.Core;
using Vision.Projects;
using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Frm
{
    public partial class FrmCenterCalib : Form
    {
        public FrmCenterCalib(Station station, CenterCalibTool tool)
        {
            InitializeComponent();
            _station = station;
            _path = Path.Combine(ProjectManager.ProjectDir, station.StationName, "CenterCalib.xml"); 
            _centerTool = tool;
            LoadData();
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
        }

        private ICogImage _image;
        private int _index = 0;

        private Station _station;
        private readonly CenterCalibTool _centerTool;
        private CogCalibNPointToNPointTool _nPointTool;
        private CogAcqFifoTool _acqTool;
        private CogFitCircleTool _fitCircleTool;
        private CogIDTool _idTool;

        private CogIDResult _idResult;
        private CenterDataList _centerListData = new CenterDataList();
        private readonly string _path;

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
        /// 计算标定
        /// </summary>
        private void Calibration()
        {
            try
            {
                //==============================先9点标定================================
                _nPointTool.InputImage = _image;
                _nPointTool.Calibration.Calibrate();
                if (_nPointTool.Calibration.Calibrated)
                {
                    Log("9点标定成功");
                }
                else
                {
                    Log("9点标定失败");
                    return;
                }

                Thread.Sleep(100);

                //============================旋转标定===================================
                //使用9点标定的输出图像作为拟合圆的输出图像
                _fitCircleTool.InputImage = _image;
                //保证大于3个点 拟合圆
                if (_fitCircleTool.RunParams.NumPoints >= 3)
                {
                    _fitCircleTool.Run();

                    if (_fitCircleTool.Result.GetCircle() == null)
                    {
                        throw new Exception("旋转标定失败！");
                    }

                    var x = _fitCircleTool.Result.GetX(0);
                    var y = _fitCircleTool.Result.GetY(0);

                    //将图像坐标转换成实际坐标
                    var cogtranform2DLinear = _nPointTool.Calibration.GetComputedUncalibratedFromCalibratedTransform();
                    cogtranform2DLinear.InvertBase().MapPoint(x, y, out var currentRx, out var currentRy);

                    _station.DataConfig.CalibConfig.CenterPoint = new PointD(Math.Round(currentRx,3), Math.Round(currentRy,3));
                    Log("旋转标定成功");
                }
                Log("标定完成！");
            }
            catch (Exception ex)
            {
                _station.DataConfig.CalibConfig.CenterPoint = null;
                Log(ex.Message);
                ex.MsgBox();
            }
        }

        /// <summary>
        /// 拍照一次 运行
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
                //((CogImageFileTool)_centerTool.ToolBlock.Tools["CogImageFileTool1"]).Run();
                //_image = ((CogImageFileTool)_centerTool.ToolBlock.Tools["CogImageFileTool1"]).OutputImage;
                //return true;
            }
            catch (Exception ex)
            {
                ex.MsgBox();
                return false;
            }
        }

        /// <summary>
        /// 初始化窗体
        /// </summary>
        private void InitControl()
        {
            if (!CheckTools())
            {
                this.Close();
                return;
            }
            try
            {
                //没数据
                if (_centerListData.CenterList.Count == 0)
                {
                    return;
                }
                //有数据
                for (int i = 0; i < _centerListData.CenterList.Count; i++)
                {
                    dgv.Rows.Add(i + 1, _centerListData.CenterList[i].ImageX, _centerListData.CenterList[i].ImageY,
                        _centerListData.CenterList[i].RobotX, _centerListData.CenterList[i].RobotY);
                    _index++;
                }
            }
            catch (Exception ex)
            {
                ex.Message.MsgBox();
            }
        }

        /// <summary>
        /// 获取工具
        /// </summary>
        /// <returns></returns>
        private bool CheckTools()
        {
            try
            {
                if (_centerTool != null)
                {
                    _acqTool = _centerTool.ToolBlock.Tools["CogAcqFifoTool1"] as CogAcqFifoTool;
                    _fitCircleTool = _centerTool.ToolBlock.Tools["CogFitCircleTool1"] as CogFitCircleTool;
                    _nPointTool = _centerTool.ToolBlock.Tools["CogCalibNPointToNPointTool1"] as CogCalibNPointToNPointTool;
                    _idTool = _centerTool.ToolBlock.Tools["CogIDTool1"] as CogIDTool;

                    return true;
                }
                return false;
            }
            catch
            {
                "旋转标定的工具不齐全，请检查！".MsgBox();
                return false;
            }
        }

        /// <summary>
        /// 设置datagridview
        /// </summary>
        private void InitDgv()
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
            if (dgv.Rows.Count < 14)
            {
                Log("标定的点位不足14个");
                return false;
            }
            try
            {
                //=============================传入9点标定的数据===============================
                //清除原有点位
                int nNumber = _nPointTool.Calibration.NumPoints;
                for (int i = 0; i < nNumber; i++)
                {
                    _nPointTool.Calibration.DeletePointPair(0);
                }
                _centerListData.Clear();
                //添加新的点位
                for (int i = 0; i < 9; i++)
                {
                    var c1 = dgv.Rows[i].Cells[1].Value;
                    var c2 = dgv.Rows[i].Cells[2].Value;
                    var c3 = dgv.Rows[i].Cells[3].Value;
                    var c4 = dgv.Rows[i].Cells[4].Value;


                    if (c1 == null || c2 == null || c3 == null || c4 == null)
                    {
                        Log("9点标定的点位不正确，请检查");
                        return false;
                    }

                    double imageX = double.Parse(c1.ToString().Trim());
                    double imageY = double.Parse(c2.ToString().Trim());
                    double rX = double.Parse(c3.ToString().Trim());
                    double rY = double.Parse(c4.ToString().Trim());

                    _nPointTool.Calibration.AddPointPair(imageX, imageY, rX, rY);
                    _centerListData.Add(new CenterData() { ImageX = imageX, ImageY = imageY, RobotX = rX, RobotY = rY });
                }

                //=============================传入旋转标定的数据===============================
                //清除原有点位
                int pNumber = _fitCircleTool.RunParams.NumPoints;
                for (int i = 0; i < pNumber; i++)
                {
                    _fitCircleTool.RunParams.DeletePoint(0);
                }
                //添加新的点位
                for (int i = 9; i < 14; i++)
                {
                    var cell1 = dgv.Rows[i].Cells[1].Value;
                    var cell2 = dgv.Rows[i].Cells[2].Value;

                    if (cell1 == null || cell2 == null)
                    {
                        Log("旋转标定点位不正确，请检查！");
                        return false;
                    }

                    double x1 = Convert.ToDouble(cell1.ToString().Trim());
                    double y1 = Convert.ToDouble(cell2.ToString().Trim());

                    _fitCircleTool.RunParams.AddPoint(x1, y1);
                    _centerListData.Add(new CenterData() { ImageX = x1, ImageY = y1 });
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
        private void AddData(double x, double y)
        {
            dgv.Rows.Add(_index + 1, x, y);
        }

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="record"></param>
        private void ShowRecord(ICogGraphic g)
        {
            //先清除图像
            cogRecordDisplay1.StaticGraphics.Clear();
            cogRecordDisplay1.InteractiveGraphics.Clear();
            cogRecordDisplay1.Image = null;

            if (g != null)
            {
                cogRecordDisplay1.Image = _image;
                cogRecordDisplay1.StaticGraphics.Add(g, "");
                cogRecordDisplay1.Fit(true);
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

                cogRecordDisplay1.Fit(true);
            }
        }

        /// <summary>
        /// 按钮背景色改变
        /// </summary>
        private void ActiveButtonEnable()
        {
            btnStopLive.Enabled = cogRecordDisplay1.LiveDisplayRunning;
            btnStartLive.Enabled = !cogRecordDisplay1.LiveDisplayRunning;
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCenterCalib_Load(object sender, System.EventArgs e)
        {
            ActiveButtonEnable();
            InitDgv();
            InitControl();
        }

        /// <summary>
        /// 运行一次 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun_Click(object sender, EventArgs e)
        {
            bool r = Grab();
            try
            {
                if (_index >= 14)
                    return;
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
                                "未设置标准idTool".MsgBox();
                                return;
                            }

                            //查找设置的标准id
                            foreach (CogIDResult res in _idTool.Results)
                            {
                                if (res.DecodedData.DecodedString == _idResult.DecodedData.DecodedString)
                                {
                                    var x = Math.Round(res.CenterX, 3);
                                    var y = Math.Round(res.CenterY, 3);
                                    AddData(x, y);
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
        /// 清除拟合圆参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            dgv.Rows.Clear();
            _centerListData.Clear();
            _index = 0;
        }

        /// <summary>
        /// 开始采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartLive_Click(object sender, EventArgs e)
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
                    Log("相机未设置");
                }
            }

            ActiveButtonEnable();
        }

        /// <summary>
        /// 停止采集
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
                    Log("相机未设置");
            }

            ActiveButtonEnable();
        }

        /// <summary>
        /// 标定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalc_Click(object sender, EventArgs e)
        {
            if (SetPoints())
            {
                Grab();
                Calibration();
            }
        }

        /// <summary>
        /// 保存标定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveCalib_Click(object sender, EventArgs e)
        {
            _centerTool?.SaveVpp();
            SaveData();
            _station.SaveData();
        }

        private void FrmCenterCalib_FormClosing(object sender, FormClosingEventArgs e)
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
                _centerListData = SerializerHelper.DeSerializeFromXml<CenterDataList>(_path);
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
            SerializerHelper.SerializeToXml(_centerListData, _path);
            Log("标定数据保存成功");
        }
    }

}
