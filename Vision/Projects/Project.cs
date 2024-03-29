using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vision.Core;
using Vision.Stations;

namespace Vision.Projects
{
    [Serializable]
    public class Project
    {
        private List<Station> _stations;    //工位集合

        public Project()
        {
            
        }

        /// <summary>
        /// 工位列表
        /// </summary>
        public List<Station> StationList
        {
            get => _stations ?? (_stations = new List<Station>());
            private set => _stations = value;
        }

        public Station this[string name]
        {
            get
            {
                return StationList.FirstOrDefault(s => s.StationName == name);
            }
        }

        public Station this[int index] => StationList[index];

        /// <summary>
        /// 添加工位
        /// </summary>
        /// <returns></returns>
        public bool AddStation()
        {
            if(StationList.Count >= 8)
            {
                LogUI.AddToolLog("工位已经达到上限！");
                return false;
            }
            var defaultName = GenDefaultStationName();
            var station = new Station()
            {
                StationName = defaultName,
            };
            station.RegisterViewDisplay();

            if (!StationExsit(station))
            {
                var path = Path.Combine(ProjectManager.ProjectDir, station.StationName);
                //添加文件夹
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                StationList.Add(station);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 重命名工位
        /// </summary>
        /// <param name="station"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool RenameStation(Station station, string newName)
        {
            if (station == null)
                return false;

            if (StationExsit(station))
            {
                var oldPath = Path.Combine(ProjectManager.ProjectDir, station.StationName);
                station.StationName = newName;
                var newPath = Path.Combine(ProjectManager.ProjectDir, newName);
                Local.RenameDirectory(oldPath, newPath);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除工位
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public bool DeleteStation(Station station)
        {
            if (station == null) return false;
            if (StationExsit(station))
            {
                var path = Path.Combine(ProjectManager.ProjectDir, station.StationName);
                //删除文件夹
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                StationList.Remove(station);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 生成默认工位名称
        /// </summary>
        /// <returns></returns>
        public string GenDefaultStationName()
        {
            int m = 1;
            string name = "工位" + m.ToString();
            while (StationExsit(name))
            {
                m++;
                name = "工位" + m.ToString();
            }
            return "工位" + m.ToString();
        }

        /// <summary>
        /// 工位是否存在
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public bool StationExsit(Station station)
        {
            if (StationList == null) { return false; }
            foreach (Station s in StationList)
            {
                if (s.StationName == station.StationName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 工位是否存在（根据名称）
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool StationExsit(string name)
        {
            if (StationList == null) { return false; }
            var station = StationList.Find(s => s.StationName == name);
            if (station != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 关闭项目
        /// </summary>
        public void Close()
        {
            if (StationList != null && StationList.Count > 0)
            {
                foreach (Station station in StationList)
                {
                    station.Close();
                }
                StationList.Clear();
            }
        }

    }
}