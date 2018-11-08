using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace AeHelper.LayerProcess.FeatureProcess
{
    /// <summary>
    /// 复制数据集
    /// </summary>
    public class CopyDatasetClass
    {
        /// <summary>
        /// 数据集复制为矢量图层
        /// </summary>
        /// <param name="dataset">数据集</param>
        /// <param name="outFile">输出文件</param>
        /// <returns></returns>
        public static ILayer CopyDatasetAsFeatureLayer(IDataset dataset, string outFile)
        {
            //数据为空或文件已存在，则返回null
            if (dataset == null || File.Exists(outFile)) return null;
            string fileName = Path.GetFileNameWithoutExtension(outFile);
            IWorkspace workSpace = FeatureInfoClass.GetShapefileWorkspace(outFile);
            dataset.Copy(fileName, workSpace);
            return FeatureInfoClass.GetFeatureLayer(outFile);
        }

        /// <summary>
        /// 几何数据集复制为矢量图层
        /// </summary>
        /// <param name="geoDataset">几何数据集</param>
        /// <param name="outFile">输出文件</param>
        public static void CopyGeoDatasetAsFeatureLayer(IGeoDataset geoDataset, string outFile)
        {
            CopyDatasetAsFeatureLayer(geoDataset as IDataset, outFile);
        }

    }
}
