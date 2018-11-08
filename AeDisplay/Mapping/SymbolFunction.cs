using System;
using System.Drawing;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;

namespace AeDisplay.Mapping
{
    /// <summary>
    /// ArcEngine系统符号样式
    /// </summary>
    public class SymbolFunction
    {
        /// <summary>
        /// 加载系统符号样式
        /// </summary>
        /// <param name="symbologyCtrl">符号显示控件</param>
        /// <param name="symbologyStyle">符号样式</param>
        public static void LoadSymbolStyle(AxSymbologyControl symbologyCtrl, esriSymbologyStyleClass symbologyStyle)
        {
            symbologyCtrl.Clear();
            string appPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;  //目录结尾包含反斜杠\
            string styleFilePath = appPath + @"StyleSymbol\ESRI.ServerStyle";
            //载入系统符号库
            symbologyCtrl.LoadStyleFile(styleFilePath);
            symbologyCtrl.StyleClass = symbologyStyle;
            ISymbologyStyleClass pSymStyleClass = symbologyCtrl.GetStyleClass(symbologyCtrl.StyleClass);
            pSymStyleClass.GetItem(0);
            pSymStyleClass.UnselectItem();
        }

        /// <summary>
        /// 在图片框中预览符号样式
        /// </summary>
        /// <param name="symbologyCtrl">符号控件</param>
        /// <param name="styleGalleryItem">符号样式</param>
        /// <param name="pictureBox">图片框</param>
        public static void PreviewImage(AxSymbologyControl symbologyCtrl, IStyleGalleryItem styleGalleryItem,
            PictureBox pictureBox)
        {
            //获取符号样式类
            ISymbologyStyleClass symbologyStyleClass = symbologyCtrl.GetStyleClass(symbologyCtrl.StyleClass);
            //获取符号图片
            stdole.IPictureDisp picture = symbologyStyleClass.PreviewItem(styleGalleryItem, pictureBox.Width,
                pictureBox.Height);
            Image image = Image.FromHbitmap(new IntPtr(picture.Handle));
            pictureBox.Image = image;
        }

    }
}
