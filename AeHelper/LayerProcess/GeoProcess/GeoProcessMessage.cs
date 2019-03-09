using System;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;

namespace AeHelper.LayerProcess.GeoProcess
{
    class GeoProcessMessage
    {
        /// <summary>
        /// 执行GP，并返回处理结果
        /// </summary>
        /// <param name="gProcessor">GP对象</param>
        /// <param name="process">GP工具</param>
        /// <param name="trackCancel">取消对象</param>
        /// <returns>处理结果</returns>
        public static IGeoProcessorResult ExecuteWithResult(Geoprocessor gProcessor, IGPProcess process,
            ITrackCancel trackCancel)
        {
            IGeoProcessorResult geoResult = null;
            gProcessor.OverwriteOutput = true; //是否覆盖
            try
            {
                geoResult = (IGeoProcessorResult) gProcessor.Execute(process, null);
                ReturnMessages(gProcessor);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
                ReturnMessages(gProcessor); //当调试正确后注释本行
            }

            return geoResult;
        }

        /// <summary>
        /// GP处理结果信息
        /// </summary>
        /// <param name="gp">GP对象</param>
        private static void ReturnMessages(Geoprocessor gp)
        {
            string ms = "";
            if (gp.MessageCount > 0)
            {
                for (int i = 0; i <= gp.MessageCount - 1; i++)
                {
                    ms += "$" + gp.GetMessage(i) + "\n\n";
                }
            }

            MessageBox.Show(ms);
        }
    }
}