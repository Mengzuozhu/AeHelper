using System.IO;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;

namespace AeHelper.LayerProcess.RasterProcess
{
    /// <summary>
    /// 栅格保存器
    /// </summary>
    public class RasterSaver
    {
        /// <summary>
        /// 栅格保存为数据集（并创建金字塔）
        /// </summary>
        /// <param name="raster">栅格</param>
        /// <param name="outFile">输出文件</param>
        public static void SaveRasterAsDataset(IRaster raster, string outFile)
        {
            //文件已存在，则返回
            if (File.Exists(outFile) || raster == null) return;
            IRasterBandCollection pRasBandCol = raster as IRasterBandCollection;
            if (pRasBandCol == null) return;
            string fileName = Path.GetFileName(outFile);
            IWorkspace workspace = RasterDataInfoClass.GetRasterWorkspace(outFile);
            //保存波段集合为数据集
            IDataset dataset = pRasBandCol.SaveAs(fileName, workspace, "IMAGINE Image");
            ITemporaryDataset pRsGeo = dataset as ITemporaryDataset;
            if (pRsGeo != null && pRsGeo.IsTemporary())
            {
                pRsGeo.MakePermanent();
            }
            RasterDataInfoClass.CreatePyramid(outFile);  //创建金字塔
        }

        /// <summary>
        /// 几何数据集保存为数据集 
        /// </summary>
        /// <param name="geoDataset">几何数据集</param>
        /// <param name="outFile">输出文件</param>
        public static void GeoDatasetSaveAsDataset(IGeoDataset geoDataset, string outFile)
        {
            if (geoDataset == null) return;

            SaveRasterAsDataset((IRaster)geoDataset, outFile);
        }

        /// <summary>
        /// 保存栅格为输出文件（同时释放栅格对象）
        /// </summary>
        /// <param name="raster">栅格</param>
        /// <param name="outFile">输出文件</param>
        public static void SaveRasterByISaveAs(IRaster raster, string outFile)
        {
            if (raster == null) return;
            //保存结果，要用ISaveAs来保存
            ISaveAs saveAs = raster as ISaveAs;
            if (saveAs != null) saveAs.SaveAs(outFile, null, "IMAGINE Image");
            //释放栅格对象
            System.Runtime.InteropServices.Marshal.ReleaseComObject(raster);
        }

    }
}
