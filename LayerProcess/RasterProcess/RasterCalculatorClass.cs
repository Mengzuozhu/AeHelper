using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SpatialAnalyst;

namespace LayerProcess.RasterProcess
{
    /// <summary>
    /// 栅格计算器
    /// </summary>
    public class RasterCalculatorClass
    {
        /// <summary>
        /// 获取栅格计算的栅格结果
        /// </summary>
        /// <param name="pMapAlgebraOp">计算工具</param>
        /// <param name="inRasters">输入栅格集合</param>
        /// <param name="rasterName">栅格名称集合</param>
        /// <param name="expression">计算表达式</param>
        public static IRaster GetRasterCalculateResult(IMapAlgebraOp pMapAlgebraOp, IList<IRaster> inRasters,
            IList<string> rasterName, string expression)
        {
            var nameAndRasters = GetNameAndRasters(inRasters, rasterName);
            return RasterCalculate(pMapAlgebraOp, nameAndRasters, expression);
        }

        /// <summary>
        /// 获取栅格计算的栅格结果
        /// </summary>
        /// <param name="nameAndRasters"></param>
        /// <param name="expression">计算表达式</param>
        public static IRaster GetRasterCalculateResult(Dictionary<string, IRaster> nameAndRasters, string expression)
        {
            IMapAlgebraOp mapAlgebraOp = GetAlgebraOpWithMaxResolution(nameAndRasters);
            return RasterCalculate(mapAlgebraOp, nameAndRasters, expression);

        }

        /// <summary>
        /// 获取名称和栅格
        /// </summary>
        /// <param name="inRasters"></param>
        /// <param name="rasterName"></param>
        /// <returns></returns>
        private static Dictionary<string, IRaster> GetNameAndRasters(IList<IRaster> inRasters, IList<string> rasterName)
        {
            Dictionary<string, IRaster> nameAndRasters = new Dictionary<string, IRaster>();
            //获取唯一值,避免解绑失败
            for (int i = 0; i < rasterName.Count; i++)
            {
                if (nameAndRasters.ContainsKey(rasterName[i])) continue;
                nameAndRasters.Add(rasterName[i], inRasters[i]);
            }

            return nameAndRasters;
        }

        /// <summary>
        /// 栅格计算
        /// </summary>
        /// <param name="mapAlgebraOp"></param>
        /// <param name="expression"></param>
        /// <param name="nameAndRasters"></param>
        /// <returns></returns>
        public static IRaster RasterCalculate(IMapAlgebraOp mapAlgebraOp, Dictionary<string, IRaster> nameAndRasters,
            string expression)
        {
            if (mapAlgebraOp == null) return null;
            //绑定
            foreach (var item in nameAndRasters)
            {
                mapAlgebraOp.BindRaster((IGeoDataset)item.Value, item.Key);
            }

            //进行栅格计算
            IRaster outRaster = (IRaster)mapAlgebraOp.Execute(expression);
            //解绑
            foreach (string key in nameAndRasters.Keys)
            {
                mapAlgebraOp.UnbindRaster(key);
            }

            return outRaster;
        }

        /// <summary>
        /// 栅格计算
        /// </summary>
        /// <param name="nameAndRasters"></param>
        /// <param name="expression">计算表达式</param>
        /// <param name="outFile"></param>
        public static void RasterCalculate(Dictionary<string, IRaster> nameAndRasters, string expression,
            string outFile)
        {
            IMapAlgebraOp mapAlgebraOp = GetAlgebraOpWithMaxResolution(nameAndRasters);
            IRaster outRaster = RasterCalculate(mapAlgebraOp, nameAndRasters, expression);

            if (outRaster == null) return;
            SaveAsRasterClass.RasterSaveAsDataset(outRaster, outFile);
        }

        /// <summary>
        /// 获取分辨率最高的栅格计算操作类
        /// </summary>
        /// <param name="nameAndRasters"></param>
        /// <returns></returns>
        private static IMapAlgebraOp GetAlgebraOpWithMaxResolution(Dictionary<string, IRaster> nameAndRasters)
        {
            List<IRaster> rasters = new List<IRaster>();
            foreach (IRaster raster in nameAndRasters.Values)
            {
                rasters.Add(raster);
            }

            //获取分辨率最高的栅格
            IRaster minRaster = RasterDataInfoClass.GetMinCellSizeRaster(rasters);
            //实例化栅格运算工具
            IMapAlgebraOp mapAlgebraOp = new RasterMapAlgebraOp() as IMapAlgebraOp;
            RasterAnalysisEnvironmentClass.SetAnalysisEnvironment(mapAlgebraOp, minRaster); //以分辨率最高的栅格为参考对象
            return mapAlgebraOp;
        }

    }
}
