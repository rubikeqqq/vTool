using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Cognex.VisionPro;
using Vision.Core;
using Vision.Stations;
using Vision.Tools.Interfaces;

namespace Vision.Tools.ToolImpls
{
    [Serializable]
    [GroupInfo("图像工具", 0)]
    [ToolName("图像仿真", 1)]
    [Description("通过读取本地图像进行仿真测试")]
    public class ImageTool : ToolBase, IImageOut, IRegisterStation
    {
        /// <summary>
        /// 文件夹时使用的图像计数
        /// </summary>
        [NonSerialized]
        private int _imageIndex;

        [NonSerialized]
        private Station _station;

        /// <summary>
        /// 仿真的格式
        /// </summary>
        public EmulationType EmulationType { get; set; }

        /// <summary>
        /// 仿真的路径
        /// </summary>
        public string Path { get; set; }

        [field: NonSerialized]
        public ICogImage ImageOut { get; private set; }

        [field: NonSerialized]
        public UcImageTool UI { get; set; }

        [field: NonSerialized]
        /// <summary>
        /// 图像显示事件
        /// </summary>
        public event EventHandler<ICogImage> ImageShowEvent;

        public override UserControl GetToolControl(Station station)
        {
            if (UI == null)
            {
                UI = new UcImageTool(this);
            }
            return UI;
        }

        public override void Run()
        {
            RunTime = TimeSpan.Zero;
            if (!Enable)
                return;
            Stopwatch sw = Stopwatch.StartNew();
            ImageOut = GetImage();
            if (ImageOut == null)
            {
                LogNet.Log($"[{ToolName}] 输出图像失败，请检查设置！");
                throw new Exception($"[{ToolName}] 输出图像失败，请检查设置！");
            }
            sw.Stop();
            RunTime = sw.Elapsed;

            _station.ShowImage = ImageOut;
            OnImageShowEvent(ImageOut);
        }

        public override void RunDebug()
        {
            Run();
        }

        /// <summary>
        /// 图像显示触发事件
        /// </summary>
        /// <param name="image"></param>
        private void OnImageShowEvent(ICogImage image)
        {
            ImageShowEvent?.Invoke(this, image);
        }

        /// <summary>
        /// 读取图像
        /// </summary>
        /// <returns></returns>
        private CogImage8Grey GetImage()
        {
            CogImage8Grey image = null;
            //如果是文件夹
            if (EmulationType == EmulationType.Dir)
            {
                if (string.IsNullOrEmpty(Path))
                {
                    return null;
                }
                string dir = Path;
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                if (dirInfo.Exists)
                {
                    var fileInfo = dirInfo.GetFiles();

                    var imageList = fileInfo
                        .ToList()
                        .Where(x =>
                            x.Extension.ToLower() == ".bmp" /*|| x.Extension.ToLower() == ".jpg"*/
                        )
                        .ToList()
                        .Select(x => x.FullName)
                        .ToList();

                    //没有图像
                    if (imageList.Count == 0)
                    {
                        return null;
                    }

                    if (_imageIndex >= imageList.Count)
                    {
                        _imageIndex = 0;
                    }
                    var file = imageList[_imageIndex];
                    var bmp = new Bitmap(file);
                    image = new CogImage8Grey(bmp);
                    _imageIndex++;
                }
            }
            else //单个图像文件
            {
                string file = Path;
                var bmp = new Bitmap(file);
                image = new CogImage8Grey(bmp);
            }
            return image;
        }

        public void RegisterStation(Station station)
        {
            _station = station;
        }
    }

    /// <summary>
    /// 仿真的类型
    /// </summary>
    [Serializable]
    public enum EmulationType
    {
        Dir,
        File,
    }
}
