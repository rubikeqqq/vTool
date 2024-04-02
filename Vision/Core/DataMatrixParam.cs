using Cognex.VisionPro;
using Cognex.VisionPro.ID;
using System;

namespace Vision.Core
{
    /// <summary>
    /// 自动标定时二维码的信息
    /// </summary>
    public class DataMatrixParam
    {
        public int X {  get; set; }

        public int Y { get; set; }

        CogIDResult GetMidMatrix(CogIDTool idTool)
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

            return idTool.Results[index];

        }
    }
}
