using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace AeHelper.LayerProcess.FeatureProcess
{
    /// <summary>
    /// 要素面积
    /// </summary>
    public class FeatureAreaClass
    {
        /// <summary>
        /// 计算要素FID和面积
        /// </summary>
        /// <param name="vectorFile"></param>
        /// <returns></returns>
        public static Dictionary<string, double> ComputeFeatureFidAndArea(string vectorFile)
        {
            IFeatureLayer featureLayer = FeatureInfoClass.GetFeatureLayer(vectorFile);
            IQueryFilter pFilter = new QueryFilterClass();
            pFilter.SubFields = "Shape, FID";  //设置过滤字段，提高效率
            IFeatureCursor featureCursor = featureLayer.Search(pFilter, false);
            return ComputeAreaByCursor(featureCursor);
        }

        /// <summary>
        /// 计算FID和面积
        /// </summary>
        /// <param name="featureCursor">要素游标</param>
        /// <returns></returns>
        private static Dictionary<string, double> ComputeAreaByCursor(IFeatureCursor featureCursor)
        {
            Dictionary<string, double> fidAndArea = new Dictionary<string, double>();
            IFeature pFeature = featureCursor.NextFeature();
            while (pFeature != null)
            {
                string fid = pFeature.Value[0].ToString();
                IArea pArea = pFeature.Shape as IArea;
                if (pArea != null)
                {
                    double area = Math.Round(pArea.Area, 5);
                    fidAndArea.Add(fid, area);
                }
                pFeature = featureCursor.NextFeature();
            }
            Marshal.ReleaseComObject(featureCursor);
            return fidAndArea;
        }

    }
}
