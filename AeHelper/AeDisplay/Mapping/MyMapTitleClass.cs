using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;

namespace AeHelper.AeDisplay.Mapping
{
    /// <summary>
    /// 制图标题
    /// </summary>
    public class MyMapTitleClass
    {
        /// <summary>
        /// 添加制图标题
        /// </summary>
        /// <param name="layoutControl">布局视图</param>
        /// <param name="pEnv">矩形框</param>
        /// <param name="textElement">文本元素</param>
        public static void AddMapTitle(AxPageLayoutControl layoutControl, IEnvelope pEnv, ITextElement textElement)
        {
            if (textElement == null) return;
            IGraphicsContainer pGraphicsContainer = layoutControl.PageLayout as IGraphicsContainer;
            if (pGraphicsContainer == null) return;
            //如果存在标题，则删除标题
            MappingHelper.DeleteElementByName(layoutControl, "MapTitle");
            IElement pElement = (IElement)textElement;
            pElement.Geometry = pEnv;
            //添加元素
            MappingHelper.AddElementWithName(pGraphicsContainer, pElement, "MapTitle");
        }

    }
}
