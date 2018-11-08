using System.Drawing;
using ESRI.ArcGIS.Display;

namespace AeDisplay.GeometryDisplay
{
    /// <summary>
    /// ArcEngine颜色
    /// </summary>
    public class AeColorClass
    {
        /// <summary>
        /// 获取RGB颜色
        /// </summary>
        /// <param name="intR">红</param>
        /// <param name="intG">绿</param>
        /// <param name="intB">蓝</param>
        /// <returns></returns>
        public static IRgbColor GetRgbColor(int intR, int intG, int intB)
        {
            if (intR < 0 || intR > 255 || intG < 0 || intG > 255 || intB < 0 || intB > 255)
            {
                return null;
            }
            IRgbColor pRgbColor = new RgbColorClass();
            pRgbColor.Red = intR;
            pRgbColor.Green = intG;
            pRgbColor.Blue = intB;
            return pRgbColor;
        }

        /// <summary>
        /// AE中的颜色转换为VS中的颜色
        /// </summary>
        /// <param name="aeColor">AE中的颜色</param>
        /// <returns></returns>
        public static Color AeColorToVsColor(IColor aeColor)
        {
            //Color转换为IColor : pColor.RGB = color.B * 65536 + color.G * 256 + color.R;
            int red = aeColor.RGB & 0xff;
            int green = (aeColor.RGB & 0xff00) / 0x100;
            int blue = (aeColor.RGB & 0xff0000) / 0x10000;
            return Color.FromArgb(red, green, blue);
        }

        /// <summary>
        /// VS中的颜色转换为AE中的颜色
        /// </summary>
        /// <param name="vsColor"></param>
        /// <returns></returns>
        public static IColor VsColorToAeColor(Color vsColor)
        {
            //Color转换为IColor : pColor.RGB = color.B * 65536 + color.G * 256 + color.R;
            IColor aeColor = new RgbColorClass();
            aeColor.RGB = vsColor.B * 65536 + vsColor.G * 256 + vsColor.R;
            return aeColor;
        }

    }
}
