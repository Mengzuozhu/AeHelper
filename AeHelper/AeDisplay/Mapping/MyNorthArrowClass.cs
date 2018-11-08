using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace AeHelper.AeDisplay.Mapping
{
    /// <summary>
    /// 指北针
    /// </summary>
    public class MyNorthArrowClass
    {
        /// <summary>
        /// 添加指北针
        /// </summary>
        /// <param name="layoutControl">布局视图</param>
        /// <param name="pEnv">矩形框</param>
        /// <param name="pStyleGalleryItem">指北针样式</param>
        public static void AddNorthArrow(AxPageLayoutControl layoutControl, IEnvelope pEnv, IStyleGalleryItem pStyleGalleryItem)
        {
            if (pStyleGalleryItem == null) return;
            IGraphicsContainer pGraphicsContainer = layoutControl.PageLayout as IGraphicsContainer;
            if (pGraphicsContainer == null) return;
            IMap pMap = layoutControl.ActiveView.FocusMap;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
            pMapSurroundFrame.MapFrame = pMapFrame;
            //删除已经存在的指北针
            MappingClass.DeleteElementByName(layoutControl, "North Arrow");
            INorthArrow pNorthArrow = pStyleGalleryItem.Item as INorthArrow;
            if (pNorthArrow != null)
            {
                pNorthArrow.Size = pEnv.Width * 50;
                pMapSurroundFrame.MapSurround = pNorthArrow;//根据用户的选取，获取相应的MapSurround            
            }
            IElement pElement = (IElement)pMapSurroundFrame;
            pElement.Geometry = pEnv;
            //添加元素
            MappingClass.AddElementWithName(pGraphicsContainer, pElement, "North Arrow");
        }

        /// <summary>
        /// 删除地图容器中除了地图框架外的所有元素
        /// </summary>
        /// <param name="curGraph">地图容器</param>
        private static void DeleteElement(IGraphicsContainer curGraph)
        {
            curGraph.Reset();
            IElement pElement = curGraph.Next();
            while (pElement != null)
            {
                //不是MapFrame对象，则删除
                if (!(pElement is IMapFrame))
                {
                    curGraph.DeleteElement(pElement);
                    //必须重置，不然会叠加之前的元素
                    curGraph.Reset();
                }
                pElement = curGraph.Next();
            }
        }

    }
}
