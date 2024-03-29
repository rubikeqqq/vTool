using System;
using System.IO;

namespace Vision.Core
{
    /// <summary>
    /// 图像配置类
    /// </summary>
    public class ImageConfig
    {
        public ImageConfig()
        {
        }

        /// <summary>
        /// 保存图像NG的文件夹
        /// </summary>
        public string SaveImageDir { get; set; } = "D:\\Images";

        /// <summary>
        /// 是否保存NG图像
        /// </summary>
        public bool IsSaveNGImage { get; set; } = false;

        /// <summary>
        /// 是否保存OK图像
        /// </summary>
        public bool IsSaveOKImage { get; set; } = false;

        /// <summary>
        /// 按天删除
        /// </summary>
        public int DeleteDayTime { get; set; } = 365;

        /// <summary>
        /// 是否按天删除
        /// </summary>
        public bool IsDeleteByTime { get; set; } = false;

        /// <summary>
        /// 按大小删除
        /// </summary>
        public int DeleteSize { get; set; } = 10240;

        /// <summary>
        /// 是否按大小删除
        /// </summary>
        public bool IsDeleteBySize { get; set; } = false;

        public bool Load(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(ImageConfig);


            SaveImageDir = IniHelper.ReadString(section, nameof(SaveImageDir), default, path);
            IsSaveNGImage = IniHelper.ReadBoolean(section, nameof(IsSaveNGImage), default, path);
            IsSaveOKImage = IniHelper.ReadBoolean(section, nameof(IsSaveOKImage), default, path);
            DeleteDayTime = IniHelper.ReadInteger(section, nameof(DeleteDayTime), default, path);
            IsDeleteByTime = IniHelper.ReadBoolean(section, nameof(IsDeleteByTime), default, path);
            DeleteDayTime = IniHelper.ReadInteger(section, nameof(DeleteDayTime), default, path);
            IsDeleteBySize = IniHelper.ReadBoolean(section, nameof(IsDeleteBySize), default, path);
            return true;
        }

        public bool Save(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(ImageConfig);


            IniHelper.WriteString(section, nameof(SaveImageDir), SaveImageDir, path);
            IniHelper.WriteBoolean(section, nameof(IsSaveNGImage), IsSaveNGImage, path);
            IniHelper.WriteBoolean(section, nameof(IsSaveOKImage), IsSaveOKImage, path);
            IniHelper.WriteInteger(section, nameof(DeleteDayTime), DeleteDayTime, path);
            IniHelper.WriteBoolean(section, nameof(IsDeleteByTime), IsDeleteByTime, path);
            IniHelper.WriteInteger(section, nameof(DeleteDayTime), DeleteDayTime, path);
            IniHelper.WriteBoolean(section, nameof(IsDeleteBySize), IsDeleteBySize, path);
            return true;
        }
    }

    /// <summary>
    /// 系统配置类
    /// </summary>
    public class SystemConfig
    {
        /// <summary>
        /// 是否开机运行
        /// </summary>
        public bool AutoRun { get; set; }

        /// <summary>
        /// 心跳地址
        /// </summary>
        public string HeartAddress { get; set; } = "D5000";

        /// <summary>
        /// 联机地址
        /// </summary>
        public string OnlineAddress { get; set; } = "D5001";

        public SystemConfig()
        {
        }

        public bool Load(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(SystemConfig);


            AutoRun = IniHelper.ReadBoolean(section, nameof(AutoRun), default, path);
            HeartAddress = IniHelper.ReadString(section, nameof(HeartAddress), default, path);
            OnlineAddress = IniHelper.ReadString(section, nameof(OnlineAddress), default, path);
            return true;
        }

        public bool Save(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(SystemConfig);


            IniHelper.WriteBoolean(section, nameof(AutoRun), AutoRun, path);
            IniHelper.WriteString(section, nameof(HeartAddress), HeartAddress, path);
            IniHelper.WriteString(section, nameof(OnlineAddress), OnlineAddress, path);
            return true;
        }
    }

    /// <summary>
    /// 数据配置类
    /// </summary>
    public class CalibConfig
    {
        /// <summary>
        /// 旋转标定时机械手点位
        /// </summary>
        public PointD CenterCalibRobotPoint { get; set; } = new PointD();

        /// <summary>
        /// 机械手的示教位
        /// </summary>
        public PointA RobotOriginPosition { get; set; } = new PointA();

        /// <summary>
        /// 模板的示教点位
        /// </summary>
        public PointA ModelOriginPoint { get; set; } = new PointA();

        /// <summary>
        /// 旋转中心点
        /// </summary>
        public PointD CenterPoint { get; set; } = new PointD();

        public bool Load(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(CalibConfig);


            CenterCalibRobotPoint = IniHelper.ReadPointD(section, nameof(CenterCalibRobotPoint), default, path);
            CenterPoint = IniHelper.ReadPointD(section, nameof(CenterPoint), default, path);
            RobotOriginPosition = IniHelper.ReadPointA(section, nameof(RobotOriginPosition), default, path);
            ModelOriginPoint = IniHelper.ReadPointA(section, nameof(ModelOriginPoint), default, path);


            return true;
        }

        public bool Save(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(CalibConfig);


            IniHelper.WritePointD(section, nameof(CenterCalibRobotPoint), CenterCalibRobotPoint, path);
            IniHelper.WritePointD(section, nameof(CenterPoint), CenterPoint, path);
            IniHelper.WritePointA(section, nameof(RobotOriginPosition), RobotOriginPosition, path);
            IniHelper.WritePointA(section, nameof(ModelOriginPoint), ModelOriginPoint, path);
            return true;
        }
    }

    /// <summary>
    /// kk配置类
    /// </summary>
    public class KKConfig
    {
        /// <summary>
        /// plc中存储 kk x的地址
        /// </summary>
        public string AddressX { get; set; }

        /// <summary>
        /// plc中存储 kk y的地址
        /// </summary>
        public string AddressY { get; set; }

        /// <summary>
        /// kk初始保存的坐标
        /// </summary>
        public PointD KKOriginPosition { get; set; } = new PointD();

        public bool Load(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(KKConfig);


            KKOriginPosition = IniHelper.ReadPointD(section, nameof(KKOriginPosition), default, path);
            AddressX = IniHelper.ReadString(section, nameof(AddressX), default, path);
            AddressY = IniHelper.ReadString(section, nameof(AddressY), default, path);
            return true;
        }

        public bool Save(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(KKConfig);


            IniHelper.WritePointD(section, nameof(KKOriginPosition), KKOriginPosition, path);
            IniHelper.WriteString(section, nameof(AddressX), AddressX, path);
            IniHelper.WriteString(section, nameof(AddressY), AddressY, path);
            return true;
        }
    }

    /// <summary>
    /// PLC配置类
    /// </summary>
    public class PLCConfig
    {
        public string IP { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "60000";

        public bool Load(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(PLCConfig);


            IP = IniHelper.ReadString(section, nameof(IP), default, path);
            Port = IniHelper.ReadString(section, nameof(Port), default, path);
            return true;
        }

        public bool Save(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(PLCConfig);


            IniHelper.WriteString(section, nameof(IP), IP, path);
            IniHelper.WriteString(section, nameof(Port), Port, path);
            return true;
        }
    }

    /// <summary>
    /// 补偿配置类
    /// </summary>
    public class OffsetConfig
    {
        /// <summary>
        /// x补偿
        /// </summary>
        public double OffsetX { get; set; }

        /// <summary>
        /// y补偿
        /// </summary>
        public double OffsetY { get; set; }

        public bool Load(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(OffsetConfig);


            OffsetX = Convert.ToDouble(IniHelper.ReadString(section, nameof(OffsetX), default, path));
            OffsetY = Convert.ToDouble(IniHelper.ReadString(section, nameof(OffsetY), default, path));
            return true;
        }

        public bool Save(string path)
        {
            if (!File.Exists(path)) return false;
            string section = nameof(OffsetConfig);


            IniHelper.WriteString(section, nameof(OffsetX), OffsetX.ToString(), path);
            IniHelper.WriteString(section, nameof(OffsetY), OffsetY.ToString(), path);
            return true;
        }
    }

    public class Config
    {
        public static ImageConfig ImageConfig { get; set; } = new ImageConfig();

        public static CalibConfig CalibConfig { get; set; } = new CalibConfig();

        public static KKConfig KKConfig { get; set; } = new KKConfig();

        public static SystemConfig SystemConfig { get; set; } = new SystemConfig();

        public static OffsetConfig OffsetConfig { get; set; } = new OffsetConfig();

        public static PLCConfig PLCConfig { get; set; } = new PLCConfig();

        public static bool LoadConfig(string path)
        {
            var b1 = ImageConfig.Load(path);
            var b2 = CalibConfig.Load(path);
            var b3 = KKConfig.Load(path);
            var b4 = SystemConfig.Load(path);
            var b5 = OffsetConfig.Load(path);
            var b6 = PLCConfig.Load(path);
            return b1 & b2 & b3 & b4 & b5 & b6;
        }

        public static bool SaveConfig(string path)
        {
            var b1 = ImageConfig.Save(path);
            var b2 = CalibConfig.Save(path);
            var b3 = KKConfig.Save(path);
            var b4 = SystemConfig.Save(path);
            var b5 = OffsetConfig.Save(path);
            var b6 = PLCConfig.Save(path);
            return b1 & b2 & b3 & b4 & b5 & b6;
        }
    }
}
