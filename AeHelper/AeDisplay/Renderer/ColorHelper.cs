using System;
using System.Collections.Generic;
using System.Drawing;

namespace AeHelper.AeDisplay.Renderer
{
    /// <summary>
    /// 颜色辅助类
    /// </summary>
    class ColorHelper
    {
        /// <summary>
        /// 获取颜色组
        /// </summary>
        /// <returns></returns>
        public static List<Color> GetDefaultColors(int colorCount)
        {
            List<Color> colors = new List<Color>()
            {
                Color.Red, Color.Aqua, Color.SpringGreen, Color.DarkOrange, Color.Yellow, Color.Fuchsia,
                Color.Blue, Color.Green, Color.MediumPurple, Color.SkyBlue, Color.Brown, Color.Gold, Color.Black,
            };
            if (colorCount <= colors.Count)
            {
                return colors;
            }

            AddRandomColorToList(colorCount, colors);
            return colors;
        }

        /// <summary>
        /// 添加随机颜色
        /// </summary>
        /// <param name="colorCount"></param>
        /// <param name="colors"></param>
        private static void AddRandomColorToList(int colorCount, ICollection<Color> colors)
        {
            const int seed = 11;
            Random random = new Random(seed);
            for (int i = colors.Count; i <= colorCount; i++)
            {
                int r = random.Next(255);
                int g = random.Next(255);
                int b = random.Next(255);
                colors.Add(Color.FromArgb(r, g, b));
            }
        }
    }
}