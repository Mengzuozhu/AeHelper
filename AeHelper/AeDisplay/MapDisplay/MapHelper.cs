using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace AeHelper.AeDisplay.MapDisplay
{
    /// <summary>
    /// 地图辅助类
    /// </summary>
    public class MapHelper
    {
        /// <summary>
        /// 根据查询表达式，选中高亮要素
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="whereClause">查询表达式</param>
        public static void HighLightFeature(IFeatureLayer featureLayer, string whereClause)
        {
            IQueryFilter query = new QueryFilterClass();
            query.WhereClause = whereClause;
            IFeatureSelection featSelection = featureLayer as IFeatureSelection;
            if (featSelection != null)
            {
                featSelection.SelectFeatures(query, esriSelectionResultEnum.esriSelectionResultNew, false);
            }
        }

        /// <summary>
        /// 改变激活视图的显示范围为要素范围
        /// </summary>
        /// <param name="feature">要素</param>
        /// <param name="activeView">激活视图</param>
        public static void ChangeViewIntoFeatureExtent(IFeature feature, IActiveView activeView)
        {
            //定义新的IEnvelope接口对象获取该要素的空间范围
            IEnvelope outEnvelope = new EnvelopeClass();
            //通过IGeometry接口的QueryEnvelope方法获取要素的空间范围
            feature.Shape.QueryEnvelope(outEnvelope);
            //将主窗体地图的当前可视范围定义为要素的空间范围，并刷新地图
            if (activeView == null) return;
            activeView.Extent = outEnvelope;
            activeView.Refresh();
        }

        /// <summary>
        /// 取消图层要素高亮显示
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="activeView">激活视图</param>
        public static void ClearFeatureSelection(ILayer layer, IActiveView activeView)
        {
            if (activeView == null) return;
            IFeatureLayer featureLayer = layer as IFeatureLayer;
            if (featureLayer == null) return;
            IFeatureSelection featureSelection = featureLayer as IFeatureSelection;
            //清除高亮
            if (featureSelection != null)
            {
                featureSelection.Clear();
            }

            PartialRefresh(activeView, featureLayer.AreaOfInterest.Envelope);
        }

        /// <summary>
        /// 清除地图所有图层数据
        /// </summary>
        /// <param name="mapControl">地图控件</param>
        public static void ClearAllMapData(IMapControl3 mapControl)
        {
            if (mapControl.Map == null || mapControl.Map.LayerCount <= 0) return;
            //新建mainMapControl中Map
            IMap dataMap = new MapClass();
            dataMap.Name = "Map";
            mapControl.DocumentFilename = string.Empty;
            mapControl.Map = dataMap;
        }

        /// <summary>
        /// 清除布局所有要素，除了框架
        /// </summary>
        /// <param name="pageLayoutControl">布局控件</param>
        public static void ClearAllLayoutData(AxPageLayoutControl pageLayoutControl)
        {
            if (pageLayoutControl == null) return;
            IGraphicsContainer curGraph = pageLayoutControl.ActiveView as IGraphicsContainer;
            if (curGraph == null) return;
            curGraph.Reset();
            IElement pElement = curGraph.Next();
            while (pElement != null)
            {
                //不是MapFrame框架对象，则删除
                if (!(pElement is IMapFrame))
                {
                    curGraph.DeleteElement(pElement);
                    //必须重置，不然会叠加之前的元素
                    curGraph.Reset();
                }

                pElement = curGraph.Next();
            }

            pageLayoutControl.Refresh();
        }

        /// <summary>
        /// 取消地图要素高亮显示
        /// </summary>
        /// <param name="mapControl">地图控件</param>
        public static void ClearMapSelection(IMapControl3 mapControl)
        {
            mapControl.Map.ClearSelection(); //清除高亮选择
            IActiveView activeView = mapControl.ActiveView;
            PartialRefresh(activeView, activeView.Extent);
        }

        /// <summary>
        /// 刷新图层部分区域
        /// </summary>
        /// <param name="activeView"></param>
        /// <param name="envelope"></param>
        public static void PartialRefresh(IActiveView activeView, IEnvelope envelope)
        {
            if (activeView != null)
            {
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, envelope);
            }
        }
    }
}