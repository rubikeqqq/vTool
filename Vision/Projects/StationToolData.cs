using Vision.Stations;
using Vision.Tools.ToolImpls;

namespace Vision.Projects
{
    /// <summary>
    /// 工位工具数据
    /// </summary>
    public class StationToolData
    {
        public StationToolData(Station station, ToolBase tool)
        {
            Station = station;
            Tool = tool;
        }

        /// <summary>
        /// 工位数据
        /// </summary>
        public Station Station { get; set; }

        /// <summary>
        /// 工具数据
        /// </summary>
        public ToolBase Tool { get; set; }
    }
}
