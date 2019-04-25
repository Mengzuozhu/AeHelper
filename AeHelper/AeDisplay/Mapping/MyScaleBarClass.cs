using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace AeHelper.AeDisplay.Mapping
{
    /// <summary>
    /// 比例尺
    /// </summary>
    public class MyScaleBarClass
    {
        /// <summary>
        /// 添加默认比例尺
        /// </summary>
        /// <param name="layoutControl">布局视图</param>
        /// <param name="pEnv">矩形框</param>
        /// <param name="pStyleGalleryItem">比例尺样式</param>
        public static void AddDefaultScaleBar(AxPageLayoutControl layoutControl, IEnvelope pEnv, IStyleGalleryItem pStyleGalleryItem)
        {
            if (pStyleGalleryItem == null) return;
            IGraphicsContainer pGraphicsContainer = layoutControl.PageLayout as IGraphicsContainer;
            if (pGraphicsContainer == null) return;
            IMap pMap = layoutControl.ActiveView.FocusMap;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
            pMapSurroundFrame.MapFrame = pMapFrame;
            //删除已经存在的比例尺
            MappingHelper.DeleteElementByName(layoutControl, "Scale Line");
            IScaleBar2 scaleBar2 = pStyleGalleryItem.Item as IScaleBar2;
            if (scaleBar2 != null)
            {
                scaleBar2.BarHeight = pEnv.Height * 20;
                scaleBar2.Divisions = 1;
                scaleBar2.Subdivisions = 2;
                pMapSurroundFrame.MapSurround = scaleBar2 as IMapSurround;
            }
            IElement pElement = (IElement)pMapSurroundFrame;
            pElement.Geometry = pEnv;
            //添加元素
            MappingHelper.AddElementWithName(pGraphicsContainer, pElement, "Scale Line");
        }

        /// <summary>
        /// 添加比例尺
        /// </summary>
        /// <param name="layoutControl">布局视图</param>
        /// <param name="pEnv">矩形框</param>
        /// <param name="scaleBar">比例尺</param>
        public static void AddScaleBar(AxPageLayoutControl layoutControl, IEnvelope pEnv, IScaleBar2 scaleBar)
        {
            if (scaleBar == null) return;
            IGraphicsContainer pGraphicsContainer = layoutControl.PageLayout as IGraphicsContainer;
            if (pGraphicsContainer == null) return;
            IMap pMap = layoutControl.ActiveView.FocusMap;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
            pMapSurroundFrame.MapFrame = pMapFrame;
            scaleBar.BarHeight = pEnv.Height * 10;
            //删除已经存在的比例尺
            MappingHelper.DeleteElementByName(layoutControl, "Scale Line");
            pMapSurroundFrame.MapSurround = scaleBar as IMapSurround;
            IElement pElement = (IElement)pMapSurroundFrame;
            pElement.Geometry = pEnv;
            //添加元素
            MappingHelper.AddElementWithName(pGraphicsContainer, pElement, "Scale Line");
        }
    }
}
