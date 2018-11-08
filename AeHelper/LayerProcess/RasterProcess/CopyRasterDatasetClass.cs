using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ExternalProgram.FileAndDirectory;

namespace AeHelper.LayerProcess.RasterProcess
{
    /// <summary>
    /// 复制栅格数据集
    /// </summary>
    public class CopyRasterDatasetClass
    {
        /// <summary>
        /// 复制数据集为栅格图层
        /// </summary>
        /// <param name="dataset">数据集</param>
        /// <param name="outFile">输出路径</param>
        /// <returns></returns>
        public static ILayer CopyDatasetAsRasterLayer(IDataset dataset, string outFile)
        {
            //不能复制
            if (CannotCopy(dataset, outFile)) return null;
            string fileName = Path.GetFileName(outFile);
            IWorkspace workspace = RasterDataInfoClass.GetRasterWorkspace(outFile);
            IDataset rasterDataset = dataset.Copy(fileName, workspace);
            IRasterLayer copyRasterLayer = new RasterLayerClass();
            copyRasterLayer.CreateFromDataset((IRasterDataset)rasterDataset);
            return copyRasterLayer;
        }

        /// <summary>
        /// 不能复制
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="outFile"></param>
        /// <returns></returns>
        private static bool CannotCopy(IDataset dataset, string outFile)
        {
            return dataset == null || !dataset.CanCopy() || File.Exists(outFile);
        }

        /// <summary>
        /// 复制获取临时栅格数据集
        /// </summary>
        /// <param name="rasterLayerFile">栅格图层文件</param>
        /// <returns></returns>
        public static IRasterDataset CopyAsTempRasterDataset(string rasterLayerFile)
        {
            //复制原始栅格为临时栅格
            string tempFile = CreateNewTempFile();
            CopyRasterLayer(rasterLayerFile, tempFile);
            return RasterDataInfoClass.GetRasterDataset(tempFile);
        }

        /// <summary>
        /// 复制获取临时栅格数据集
        /// </summary>
        /// <param name="rasterLayerFile">栅格图层文件</param>
        /// <returns></returns>
        public static ILayer CopyAsTempRasterLayer(string rasterLayerFile)
        {
            //复制原始栅格为临时栅格
            return CopyRasterLayer(rasterLayerFile, CreateNewTempFile());
        }

        /// <summary>
        /// 创建临时栅格输出文件
        /// </summary>
        /// <returns></returns>
        private static string CreateNewTempFile()
        {
            return TempFile.CreateNewTempFile();
        }

        /// <summary>
        /// 复制栅格图层
        /// </summary>
        /// <param name="inFile">输入路径</param>
        /// <param name="outFile">输出路径</param>
        public static ILayer CopyRasterLayer(string inFile, string outFile)
        {
            IRasterDataset rasterDataset = RasterDataInfoClass.GetRasterDataset(inFile);
            return CopyDatasetAsRasterLayer(rasterDataset as IDataset, outFile);
        }

    }
}
