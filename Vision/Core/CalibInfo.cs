using System;
using System.Collections.Generic;
using System.IO;

namespace Vision.Core
{
    /// <summary>
    /// 标定的具体数据类
    /// </summary>
    public class CenterData
    {
        public double ImageX { get; set; }

        public double ImageY { get; set; }

        public double RobotX { get; set; }

        public double RobotY { get; set; }
    }

    /// <summary>
    /// 标定数据类
    /// </summary>
    public class CenterDataList
    {
        public List<CenterData> CenterList { get; set; } = new List<CenterData>();

        public void Add(CenterData data)
        {
            CenterList.Add(data);
        }

        public void Clear()
        {
            CenterList.Clear();
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
            if (!File.Exists(path))
                return false;
            string section = nameof(CalibConfig);

            try
            {
                CenterCalibRobotPoint = IniHelper.ReadPointD(
                    section,
                    nameof(CenterCalibRobotPoint),
                    default,
                    path
                );
                CenterPoint = IniHelper.ReadPointD(section, nameof(CenterPoint), default, path);
                RobotOriginPosition = IniHelper.ReadPointA(
                    section,
                    nameof(RobotOriginPosition),
                    default,
                    path
                );
                ModelOriginPoint = IniHelper.ReadPointA(
                    section,
                    nameof(ModelOriginPoint),
                    default,
                    path
                );

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Save(string path)
        {
            if (!File.Exists(path))
                return false;
            string section = nameof(CalibConfig);

            IniHelper.WritePointD(
                section,
                nameof(CenterCalibRobotPoint),
                CenterCalibRobotPoint,
                path
            );
            IniHelper.WritePointD(section, nameof(CenterPoint), CenterPoint, path);
            IniHelper.WritePointA(section, nameof(RobotOriginPosition), RobotOriginPosition, path);
            IniHelper.WritePointA(section, nameof(ModelOriginPoint), ModelOriginPoint, path);
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
            if (!File.Exists(path))
                return false;
            string section = nameof(OffsetConfig);

            try
            {
                OffsetX = Convert.ToDouble(
                    IniHelper.ReadString(section, nameof(OffsetX), default, path)
                );
                OffsetY = Convert.ToDouble(
                    IniHelper.ReadString(section, nameof(OffsetY), default, path)
                );
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Save(string path)
        {
            if (!File.Exists(path))
                return false;
            string section = nameof(OffsetConfig);

            IniHelper.WriteString(section, nameof(OffsetX), OffsetX.ToString(), path);
            IniHelper.WriteString(section, nameof(OffsetY), OffsetY.ToString(), path);
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
            if (!File.Exists(path))
                return false;
            string section = nameof(KKConfig);

            try
            {
                KKOriginPosition = IniHelper.ReadPointD(
                    section,
                    nameof(KKOriginPosition),
                    default,
                    path
                );
                AddressX = IniHelper.ReadString(section, nameof(AddressX), default, path);
                AddressY = IniHelper.ReadString(section, nameof(AddressY), default, path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Save(string path)
        {
            if (!File.Exists(path))
                return false;
            string section = nameof(KKConfig);

            IniHelper.WritePointD(section, nameof(KKOriginPosition), KKOriginPosition, path);
            IniHelper.WriteString(section, nameof(AddressX), AddressX, path);
            IniHelper.WriteString(section, nameof(AddressY), AddressY, path);
            return true;
        }
    }

    public class StationDataConfig
    {
        public CalibConfig CalibConfig { get; set; } = new CalibConfig();

        public OffsetConfig OffsetConfig { get; set; } = new OffsetConfig();

        public KKConfig KKConfig { get; set; } = new KKConfig();

        public bool LoadConfig(string path)
        {
            var b1 = CalibConfig.Load(path);
            var b2 = OffsetConfig.Load(path);
            var b3 = KKConfig.Load(path);
            return b1 & b2 & b3;
        }

        public bool SaveConfig(string path)
        {
            var b1 = CalibConfig.Save(path);
            var b2 = OffsetConfig.Save(path);
            var b3 = KKConfig.Save(path);
            return b1 & b2 & b3;
        }
    }
}
