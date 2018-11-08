using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace LayerProcess.FeatureProcess
{
    /// <summary>
    /// 合并要素
    /// </summary>
    public class UnionFeatureClass
    {
        /// <summary>
        /// 将矢量图层中所有要素的几何体进行合并操作得到一个新几何体。
        /// </summary>
        /// <param name="featureLayer">输入矢量图层</param>
        /// <returns>合并后的新几何体</returns>
        public static IGeometry UnionFeatureToOneGeometry(IFeatureLayer featureLayer)
        {
            //定义IGeometry接口的对象，存储每一步拓扑操作后得到的几何体
            IGeometry geometry = null;
            //使用null作为查询过滤器得到图层中所有要素的游标
            IFeatureCursor featureCursor = featureLayer.Search(null, false);
            //获取IFeature接口的游标中的第一个元素
            IFeature feature = featureCursor.NextFeature();
            //当游标不为空时
            while (feature != null)
            {
                //如果几何体不为空
                if (geometry != null)
                {
                    //进行接口转换，使用当前几何体的ITopologicalOperator接口进行拓扑操作
                    ITopologicalOperator topologicalOperator = geometry as ITopologicalOperator;
                    //执行拓扑合并操作，将当前要素的几何体与已有几何体进行Union，返回新的合并后的几何体
                    if (topologicalOperator != null) geometry = topologicalOperator.Union(feature.Shape);
                }
                else
                {
                    geometry = feature.Shape;
                }
                //移动游标到下一个要素
                feature = featureCursor.NextFeature();
            }
            return geometry;
        }

    }
}
