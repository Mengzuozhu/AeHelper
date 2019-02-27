using System.IO;
using AeHelper.LayerProcess.FeatureProcess;
using AeHelper.LayerProcess.RasterProcess;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ExternalProgram.FileAndDirectory;

namespace AeHelper.LayerProcess.AllLayerProcess
{
    /// <summary>
    /// 数据集复制
    /// </summary>
    public class DatasetHelper
    {
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="inFile"></param>
        public static IDataset GetDataset(string inFile)
        {
            return FeatureInfoClass.IsFeatureFile(inFile)
                ? FeatureInfoClass.GetDataset(inFile)
                : RasterDataInfoClass.GetDataset(inFile);
        }

        /// <summary>
        /// 数据集复制为图层
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outFile">输出文件</param>
        /// <returns></returns>
        public static ILayer CopyAsLayer(string inFile, string outFile)
        {
            IDataset dataset = GetDataset(inFile);
            //数据不能复制
            if (IsCannotCopy(dataset, outFile)) return null;

            //从数据集中复制图层
            ILayer layer = FeatureInfoClass.IsFeatureFile(inFile)
                ? CopyDatasetAsFeatureLayer(dataset, outFile)
                : CopyDatasetAsRasterLayer(dataset, outFile);
            return layer;
        }

        /// <summary>
        /// 数据集复制为临时图层
        /// </summary>
        /// <param name="inFile"></param>
        /// <returns></returns>
        public static ILayer CopyAsTempLayer(string inFile)
        {
            bool isFeatureFile = FeatureInfoClass.IsFeatureFile(inFile);
            string outFile = isFeatureFile
                ? TempFile.CreateNewTempFile(temExtension: "shp")
                : TempFile.CreateNewTempFile();
            return CopyAsLayer(inFile, outFile);
        }

        /// <summary>
        /// 数据集复制为矢量图层
        /// </summary>
        /// <param name="dataset">数据集</param>
        /// <param name="outFile">输出文件</param>
        /// <returns></returns>
        public static ILayer CopyDatasetAsFeatureLayer(IDataset dataset, string outFile)
        {
            //数据为空或文件已存在，则返回null
            if (IsCannotCopy(dataset, outFile)) return null;

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

        /// <summary>
        /// 复制数据集为栅格图层
        /// </summary>
        /// <param name="dataset">数据集</param>
        /// <param name="outFile">输出路径</param>
        /// <returns></returns>
        public static ILayer CopyDatasetAsRasterLayer(IDataset dataset, string outFile)
        {
            //不能复制
            if (IsCannotCopy(dataset, outFile)) return null;

            string fileName = Path.GetFileName(outFile);
            IWorkspace workspace = RasterDataInfoClass.GetRasterWorkspace(outFile);
            IDataset rasterDataset = dataset.Copy(fileName, workspace);
            IRasterLayer copyRasterLayer = new RasterLayerClass();
            copyRasterLayer.CreateFromDataset((IRasterDataset) rasterDataset);
            return copyRasterLayer;
        }

        /// <summary>
        /// 不能复制
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="outFile"></param>
        /// <returns></returns>
        public static bool IsCannotCopy(IDataset dataset, string outFile)
        {
            return IsDatasetCannotCopy(dataset) || File.Exists(outFile);
        }

        /// <summary>
        /// 数据集不能复制
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns></returns>
        private static bool IsDatasetCannotCopy(IDataset dataset)
        {
            return dataset == null || !dataset.CanCopy();
        }
    }
}