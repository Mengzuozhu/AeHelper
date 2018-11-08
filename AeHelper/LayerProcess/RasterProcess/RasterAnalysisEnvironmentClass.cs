using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ExternalProgram.FileAndDirectory;

namespace AeHelper.LayerProcess.RasterProcess
{
    /// <summary>
    /// 栅格分析环境
    /// </summary>
    public class RasterAnalysisEnvironmentClass
    {
        /// <summary>
        /// 设置栅格分析环境的输出空间为临时文件夹
        /// </summary>
        /// <param name="rasterAnalysisEnvironment">栅格分析环境</param>
        public static void SetRasterAnalysisOutWorkspace(IRasterAnalysisEnvironment rasterAnalysisEnvironment)
        {
            if (rasterAnalysisEnvironment == null) return;
            //获取临时目录                    
            string finalTemPath = TempFile.CreateNewTempDirectory();
            IWorkspace workspace = RasterDataInfoClass.GetRasterWorkspace(finalTemPath);
            //设置输出工作空间
            rasterAnalysisEnvironment.OutWorkspace = workspace;
        }

        /// <summary>
        /// 设置栅格分析环境
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="analysisOperation"></param>
        /// <param name="raster"></param>
        public static void SetAnalysisEnvironment<T>(T analysisOperation, IRaster raster)
        {
            IRasterAnalysisEnvironment pRasAnalysisEnvironment = (IRasterAnalysisEnvironment)analysisOperation;
            //设置输出空间
            SetRasterAnalysisOutWorkspace(pRasAnalysisEnvironment);
            //设置输出参考栅格
            IRasterProps rasterProps = (IRasterProps)raster;
            //栅格的平均像元大小
            object cellSize = GetOutCellSize(rasterProps);
            //装箱操作，必须设置，因为输入栅格可能空间参考等属性不同  
            //设置输出数据像元大小
            pRasAnalysisEnvironment.SetCellSize(esriRasterEnvSettingEnum.esriRasterEnvValue, ref cellSize);
            //设置栅格分析处理范围
            object objExtent = rasterProps.Extent;
            pRasAnalysisEnvironment.SetExtent(esriRasterEnvSettingEnum.esriRasterEnvValue, ref objExtent);
            //设置输出数据空间参考
            SetOutSpatialReference(raster, pRasAnalysisEnvironment);
        }

        /// <summary>
        /// 获取输出像元大小
        /// </summary>
        /// <param name="rasterProps"></param>
        /// <returns></returns>
        private static object GetOutCellSize(IRasterProps rasterProps)
        {
            return (rasterProps.MeanCellSize().X + rasterProps.MeanCellSize().Y) / 2.0; //栅格的平均像元大小
        }

        /// <summary>
        /// 设置输出数据空间参考
        /// </summary>
        /// <param name="raster"></param>
        /// <param name="pRasAnalysisEnvironment"></param>
        private static void SetOutSpatialReference(IRaster raster, IRasterAnalysisEnvironment pRasAnalysisEnvironment)
        {
            //设置输出数据空间参考
            IGeoDataset inGeoDataset = raster as IGeoDataset;
            if (inGeoDataset == null) return;
            pRasAnalysisEnvironment.OutSpatialReference = inGeoDataset.SpatialReference;
        }
    }
}
