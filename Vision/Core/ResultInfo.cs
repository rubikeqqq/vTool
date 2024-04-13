using System;
using System.Runtime.Serialization;

namespace Vision.Core
{
    /// <summary>
    /// 结果类
    /// </summary>
    public class ResultInfo
    {
        public string Source { get; set; }

        public ResultType Type { get; set; }

        public string Address { get; set; }

        public object Value { get; set; }

        public ResultInfo()
        {

        }

        public ResultInfo(string source,ResultType type,string address)
        {
            Type = type;
            Source = source;
            Address = address;
        }

        public void LoadFromStream(SerializationInfo info,string resName)
        {
            string source = $"{resName}.Source";
            string type = $"{resName}.Type";
            string address = $"{resName}.Address";

            Source = info.GetString(source);
            Type = (ResultType)Enum.Parse(typeof(ResultType),info.GetString(type));
            Address = info.GetString(address);
        }

        public void SaveToStream(SerializationInfo info,string resName)
        {
            string source = $"{resName}.Source";
            string type = $"{resName}.Type";
            string address = $"{resName}.Address";

            info.AddValue(source,Source);
            info.AddValue(type,Type.ToString());
            info.AddValue(address,Address);
        }
    }
}
