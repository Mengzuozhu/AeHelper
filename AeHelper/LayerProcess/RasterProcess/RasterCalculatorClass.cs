using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SpatialAnalyst;

namespace AeHelper.LayerProcess.RasterProcess
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
        /// 获取名称和栅格
        /// </summary>
        /// <param name="inRasters"></param>
        /// <param name="rasterName"></param>
        /// <returns></returns>
        public static Dictionary<string, IRaster> GetNameAndRasters(IList<IRaster> inRasters, IList<string> rasterName)
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
        /// 获取栅格计算的栅格结果
        /// </summary>
        /// <param name="nameAndRasters"></param>
        /// <param name="referencedRaster"></param>
        /// <param name="expression">计算表达式</param>
        /// <param name="cellSize"></param>
        public static IRaster GetRasterCalculateResult(Dictionary<string, IRaster> nameAndRasters,
            IRaster referencedRaster, string expression, double cellSize)
        {
            IMapAlgebraOp mapAlgebraOp = GetAlgebraOp(referencedRaster, cellSize);
            return RasterCalculate(mapAlgebraOp, nameAndRasters, expression);
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

            IRaster outRaster;
            try
            {
                //进行栅格计算
                outRaster = (IRaster)mapAlgebraOp.Execute(expression);
            }
            finally
            {
                try
                {
                    //解绑
                    foreach (string key in nameAndRasters.Keys)
                    {
                        mapAlgebraOp.UnbindRaster(key);
                    }
                }
                catch
                {
                    //解绑异常，则忽略
                }
            }

            return outRaster;
        }

        /// <summary>
        /// 栅格计算
        /// </summary>
        /// <param name="nameAndRasters"></param>
        /// <param name="referencedRaster"></param>
        /// <param name="expression">计算表达式</param>
        /// <param name="outFile"></param>
        /// <param name="cellSize"></param>
        public static void RasterCalculate(Dictionary<string, IRaster> nameAndRasters, IRaster referencedRaster,
            string expression, double cellSize, string outFile)
        {
            IMapAlgebraOp mapAlgebraOp = GetAlgebraOp(referencedRaster, cellSize);
            IRaster outRaster = RasterCalculate(mapAlgebraOp, nameAndRasters, expression);

            if (outRaster == null) return;
            RasterSaver.SaveRasterAsDataset(outRaster, outFile);
        }

        /// <summary>
        /// 获取分辨率最高的栅格计算操作类
        /// </summary>
        /// <param name="referencedRaster"></param>
        /// <param name="cellSize"></param>
        /// <returns></returns>
        private static IMapAlgebraOp GetAlgebraOp(IRaster referencedRaster, double cellSize)
        {
            //实例化栅格运算工具
            IMapAlgebraOp mapAlgebraOp = new RasterMapAlgebraOp() as IMapAlgebraOp;
            //以分辨率最高的栅格为参考对象
            RasterAnalysisEnvironmentClass.SetAnalysisEnvironment(mapAlgebraOp, referencedRaster, cellSize);
            return mapAlgebraOp;
        }
    }
}