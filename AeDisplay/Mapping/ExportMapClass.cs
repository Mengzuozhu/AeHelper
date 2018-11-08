using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using ExternalProgram.ExternalDialog;

namespace AeDisplay.Mapping
{
    /// <summary>
    /// 地图导出
    /// </summary>
    public class ExportMapClass
    {
        /// <summary>
        /// 地图导出
        /// </summary>
        /// <param name="pLayoutControl">布局控件</param>
        /// <param name="resolution">图片分辨率</param>
        public static void ExportMapToImage(AxPageLayoutControl pLayoutControl, short resolution)
        {
            IExport pExport = GetExport();
            if (pExport == null) return;
            IActiveView activeView = pLayoutControl.ActiveView;
            //获取屏幕分辨率的值
            double screenDispalyResolution = activeView.ScreenDisplay.DisplayTransformation.Resolution;
            //获取布局页面宽度和高度
            IEnvelope pEnvelope = GetPageSize(pLayoutControl);
            //布局坐标转为屏幕坐标
            tagRECT deviceRect = new tagRECT();
            activeView.ScreenDisplay.DisplayTransformation.TransformRect(pEnvelope, ref deviceRect, 9);
            //设置输出范围坐标
            deviceRect = SetDeviceRectProperty(deviceRect, resolution, screenDispalyResolution);
            //设置输出图片的范围
            SetExportPixelBounds(pExport, deviceRect);
            ITrackCancel pCancle = new CancelTrackerClass();  //可用ESC键取消操作
            activeView.Output(pExport.StartExporting(), resolution, ref deviceRect, pEnvelope, pCancle);
            Application.DoEvents();
            pExport.FinishExporting();
            MessageBox.Show("导出完成");
        }

        /// <summary>
        /// 设置输出范围坐标
        /// </summary>
        /// <param name="deviceRect">输出范围</param>
        /// <param name="resolution">输出分辨率</param>
        /// <param name="screenDispalyResolution">屏幕分辨率</param>
        /// <returns></returns>
        private static tagRECT SetDeviceRectProperty(tagRECT deviceRect, short resolution, double screenDispalyResolution)
        {
            int width = deviceRect.right - deviceRect.left;
            int higth = deviceRect.bottom - deviceRect.top;
            deviceRect.left = 0;
            deviceRect.top = 0;
            deviceRect.right = width * resolution / (int)screenDispalyResolution;
            deviceRect.bottom = higth * resolution / (int)screenDispalyResolution;
            return deviceRect;
        }

        /// <summary>
        /// 设置输出图片的范围
        /// </summary>
        /// <param name="pExport">输出</param>
        /// <param name="deviceRect">输出范围</param>
        private static void SetExportPixelBounds(IExport pExport, tagRECT deviceRect)
        {
            IEnvelope pDeviceEnvelope = new EnvelopeClass();
            pDeviceEnvelope.PutCoords(deviceRect.left, deviceRect.bottom, deviceRect.right, deviceRect.top);
            pExport.PixelBounds = pDeviceEnvelope; // 输出图片的范围
        }

        /// <summary>
        /// 设置导出路径，获取导出工具
        /// </summary>
        /// <returns></returns>
        public static IExport GetExport()
        {
            string filter =
             "JPGE 文件(*.jpeg)|*.jpeg|BMP 文件(*.bmp)|*.bmp|GIF 文件(*.gif)|*.gif|TIF 文件(*.tif)|*.tif|PNG 文件(*.png)|*.png|PDF 文件(*.pdf)|*.pdf";
            string outPath = SaveDialogClass.GetOutPathByDialog(filter);
            if (string.IsNullOrEmpty(outPath)) return null;
            IExport pExport;
            string sType = System.IO.Path.GetExtension(outPath);
            switch (sType)
            {
                case ".jpeg":
                    pExport = new ExportJPEGClass();
                    break;
                case ".bmp":
                    pExport = new ExportBMPClass();
                    break;
                case ".gif":
                    pExport = new ExportGIFClass();
                    break;
                case ".tif":
                    pExport = new ExportTIFFClass();
                    break;
                case ".png":
                    pExport = new ExportPNGClass();
                    break;
                case ".pdf":
                    pExport = new ExportPDFClass();
                    break;
                default:
                    MessageBox.Show("没有输出格式，默认到JPEG格式");
                    pExport = new ExportJPEGClass();
                    break;
            }
            pExport.ExportFileName = outPath;
            return pExport;
        }

        /// <summary>
        /// 获取布局页面宽度和高度
        /// </summary>
        /// <param name="pLayoutControl">布局控件</param>
        /// <returns></returns>
        public static IEnvelope GetPageSize(AxPageLayoutControl pLayoutControl)
        {
            IPageLayout pTempPageLayout = pLayoutControl.PageLayout;
            IPage pTempPage = pTempPageLayout.Page;
            //获取布局页面宽度和高度
            Double dWidth, dHeight;
            pTempPage.QuerySize(out dWidth, out dHeight);
            IEnvelope pEnvelope = new EnvelopeClass();
            pEnvelope.PutCoords(0, dHeight, dWidth, 0);
            return pEnvelope;
        }

    }
}
