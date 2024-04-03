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

    public class Config
    {
        public static ImageConfig ImageConfig { get; set; } = new ImageConfig();

        public static SystemConfig SystemConfig { get; set; } = new SystemConfig();

        public static PLCConfig PLCConfig { get; set; } = new PLCConfig();

        public static bool LoadConfig(string path)
        {
            var b1 = ImageConfig.Load(path);
            var b2 = SystemConfig.Load(path);
            var b3 = PLCConfig.Load(path);
            return b1 & b2 & b3 ;
        }

        public static bool SaveConfig(string path)
        {
            var b1 = ImageConfig.Save(path);
            var b2 = SystemConfig.Save(path);
            var b3 = PLCConfig.Save(path);
            return b1 & b2 & b3 ;
        }
    }
}
