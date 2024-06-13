using System;
using System.Threading;
using HslCommunication;
using Vision.Core;

namespace Vision.Hardware
{
    public class MXPlc : IPlc
    {
        #region Fields
        private HslCommunication.Profinet.Melsec.MelsecMcNet mPlcMC;
        private static MXPlc mInstance;

        private Mutex mAccessMutex;

        private bool mIsOpened;
        private string mIPAddr;
        private int mPort;
        private int mDelayTime;
        private bool mConnectThreadFlag;
        private Thread mConnectThread;
        private string mHeartBeatAddress = string.Empty; //心跳数据地址
        #endregion

        #region Properties
        public bool IsOpened
        {
            get { return mIsOpened; }
            set
            {
                if (mIsOpened != value)
                {
                    mIsOpened = value;
                    ConnectedStateChanged?.Invoke(this, mIsOpened ? "plc连接成功！" : "plc断开连接！");
                }
            }
        }

        public string PLCIPAddress
        {
            get { return mIPAddr; }
            set { mIPAddr = value; }
        }

        public int PLCPort
        {
            get { return mPort; }
            set { mPort = value; }
        }

        public int DelayTime
        {
            set { mDelayTime = value; }
        }

        public string HeartBeatAddress
        {
            set => mHeartBeatAddress = value;
            get => mHeartBeatAddress;
        }

        public event EventHandler<string> ConnectedStateChanged;
        #endregion

        #region Implements

        public static MXPlc GetInstance()
        {
            if (mInstance == null)
            {
                mInstance = new MXPlc();
            }
            return mInstance;
        }

        private MXPlc()
        {
            mPlcMC = null;
            IsOpened = false;
            mDelayTime = 15;
            mAccessMutex = new Mutex();

            //开启PLC线重连
            mConnectThread = new Thread(new ThreadStart(this.ConnectThread))
            {
                IsBackground = true
            };
            mConnectThread.Start();
            mConnectThreadFlag = true;
        }

        public bool OpenPLC()
        {
            OperateResult opres;
            mPlcMC = new HslCommunication.Profinet.Melsec.MelsecMcNet(mIPAddr, mPort);
            opres = mPlcMC.ConnectServer();
            if (opres.IsSuccess)
            {
                IsOpened = true;
                return true;
            }
            else
            {
                IsOpened = false;
                return false;
            }
        }

        public void ClosePLC()
        {
            try
            {
                mPlcMC.ConnectClose();
            }
            catch
            {
                LogNet.Log("PLC关闭失败!");
            }
        }

        ~MXPlc()
        {
            mConnectThreadFlag = false;
        }

        public bool WriteBool(string DeviceName, bool Value)
        {
            OperateResult res;
            bool ErrFlag = true;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                res = mPlcMC.Write(DeviceName, Value);
                if (res.IsSuccess)
                    ErrFlag = false;

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool ReadBool(string DeviceName, out bool Value)
        {
            OperateResult<bool> res = null;
            Value = false;
            bool ErrFlag = true;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                res = mPlcMC.ReadBool(DeviceName);
                if (res.IsSuccess)
                {
                    Value = res.Content;
                    ErrFlag = false;
                }

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool WriteBoolArray(string[] DeviceName, int Size, bool[] Value)
        {
            OperateResult res;
            bool ErrFlag = true;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                res = mPlcMC.Write(DeviceName[0], Value);
                if (res.IsSuccess)
                    ErrFlag = false;

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool ReadBoolArray(string[] DeviceName, int Size, out bool[] Value)
        {
            OperateResult<bool[]> res = null;
            bool ErrFlag = true;
            Value = null;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                Value = new bool[Size];
                res = mPlcMC.ReadBool(DeviceName[0], (ushort)Size);
                if (res.IsSuccess)
                {
                    Value = res.Content;
                    ErrFlag = false;
                }

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool WriteShort(string DeviceName, short Value)
        {
            OperateResult res;
            bool ErrFlag = true;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                res = mPlcMC.Write(DeviceName, Value);
                if (res.IsSuccess)
                    ErrFlag = false;

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool ReadShort(string DeviceName, out short Value)
        {
            OperateResult<short> read = null;
            bool ErrFlag = true;
            Value = 0;
            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                read = mPlcMC.ReadInt16(DeviceName);
                if (read.IsSuccess)
                {
                    Value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool WriteShrotArray(string[] DeviceName, int Size, short[] Value)
        {
            OperateResult res;
            bool ErrFlag = true;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                res = mPlcMC.Write(DeviceName[0], Value);
                if (res.IsSuccess)
                    ErrFlag = false;

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool ReadShortArray(string[] DeviceName, int Size, out short[] Value)
        {
            OperateResult<short[]> read = null;
            bool ErrFlag = true;
            Value = null;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                Value = new short[Size];
                read = mPlcMC.ReadInt16(DeviceName[0], (ushort)Size);
                if (read.IsSuccess)
                {
                    Value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool WriteInt(string DeviceName, int Value)
        {
            OperateResult res;
            bool ErrFlag = true;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                res = mPlcMC.Write(DeviceName, Value);
                if (res.IsSuccess)
                    ErrFlag = false;

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool ReadInt(string DeviceName, out int Value)
        {
            OperateResult<int> read = null;
            bool ErrFlag = true;

            Value = 0;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                read = mPlcMC.ReadInt32(DeviceName);
                if (read.IsSuccess)
                {
                    Value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool WriteIntArray(string[] DeviceName, int Size, int[] Value)
        {
            OperateResult res;
            bool ErrFlag = true;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                res = mPlcMC.Write(DeviceName[0], Value);
                if (res.IsSuccess)
                    ErrFlag = false;

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool ReadIntArray(string[] DeviceName, int Size, out int[] Value)
        {
            OperateResult<int[]> read = null;
            bool ErrFlag = true;
            Value = null;
            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                Value = new int[Size];
                read = mPlcMC.ReadInt32(DeviceName[0], (ushort)Size);

                if (read.IsSuccess)
                {
                    Value = read.Content;
                    ErrFlag = false;
                }

                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        public bool WriteDouble(string DeviceName, double Value, int pointNum = 3)
        {
            int temp = 0;
            switch (pointNum)
            {
                case 0:
                    temp = (int)Value;
                    break;
                case 1:
                    temp = (int)(Value * 10);
                    break;
                case 2:
                    temp = (int)(Value * 100);
                    break;
                case 3:
                    temp = (int)(Value * 1000);
                    break;
            }
            return WriteInt(DeviceName, temp);
        }

        public bool ReadDouble(string DeviceName, out double Value, int pointNum = 3)
        {
            bool res = ReadInt(DeviceName, out int val);
            double temp = 0;

            switch (pointNum)
            {
                case 0:
                    temp = val;
                    break;
                case 1:
                    temp = val / 10.0;
                    break;
                case 2:
                    temp = val / 100.0;
                    break;
                case 3:
                    temp = val / 1000.0;
                    break;
                case 4:
                    temp = val / 10000.0;
                    break;
            }
            Value = temp;
            return res;
        }

        public bool WriteString(string address, string result)
        {
            OperateResult res;
            bool ErrFlag = true;

            if (!mIsOpened)
                return false;

            try
            {
                mAccessMutex.WaitOne();
                res = mPlcMC.Write(address, result);

                if (res.IsSuccess)
                    ErrFlag = false;
                Thread.Sleep(mDelayTime);
                mAccessMutex.ReleaseMutex();

                if (!ErrFlag)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                mAccessMutex.ReleaseMutex();
                LogNet.Log("Write PLC data exception " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 断线重连线程
        /// </summary>
        private void ConnectThread()
        {
            //如果没有设置心跳 直接返回
            if (string.IsNullOrEmpty(mHeartBeatAddress))
            {
                return;
            }
            while (mConnectThreadFlag)
            {
                //循环次数
                int LoopCount = 0;
                int Index = 0;
                //连接PLC
                OpenPLC();
                //已经连接上
                while (IsOpened && mConnectThreadFlag)
                {
                    Thread.Sleep(200);
                    if (LoopCount < 5)
                    {
                        if (Index != 1)
                        {
                            IsOpened = WriteShort(mHeartBeatAddress, 0);
                            Index = 1;
                        }
                    }
                    else
                    {
                        if (Index != 0)
                        {
                            IsOpened = WriteShort(mHeartBeatAddress, 0);
                            Index = 0;
                        }
                    }
                    LoopCount++;
                    LoopCount = LoopCount % 20;
                }
                Thread.Sleep(1000);
            }
        }

        #endregion
    }
}
