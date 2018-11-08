using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;

namespace AeHelper.AeDisplay.Mapping
{
    /// <summary>
    /// 布局视图模板
    /// </summary>
    public class PageLayoutTemplate
    {
        /// <summary>
        /// 使用模板替换当前布局视图
        /// </summary>
        /// <param name="pCurPageLayout">页面布局</param>
        /// <param name="templatePath">模板路径</param>
        public static void ReplacePageLayoutByTemplate(IPageLayout pCurPageLayout, string templatePath)
        {
            IActiveView pActiveView = pCurPageLayout as IActiveView;
            if (pActiveView == null) return;
            IMap pMap = pActiveView.FocusMap;
            //读取模板
            IPageLayout pTempPageLayout = GetTempPageLayout(templatePath);
            //设置地图页属性
            SetPageProperty(pCurPageLayout.Page, pTempPageLayout.Page);

            IGraphicsContainer curGraph = pCurPageLayout as IGraphicsContainer;
            if (curGraph == null) return;
            IMapFrame oldMapFrame = curGraph.FindFrame(pMap) as IMapFrame;
            //删除当前layout中除了mapframe外的所有element
            DeleteElement(curGraph);
            IGraphicsContainer tempGraph = pTempPageLayout as IGraphicsContainer;
            if (tempGraph == null) return;
            //遍历模板中的PageLayout所有元素，替换当前PageLayout的所有元素
            IArray pArray = ReplaceOldIntoTempElement(tempGraph, oldMapFrame);
            for (int i = pArray.Count; i > 0; i--)
            {
                curGraph.AddElement(pArray.Element[i - 1] as IElement, 0);
            }
            pActiveView.Refresh();
        }

        /// <summary>
        /// 读取模板
        /// </summary>
        /// <param name="templatePath">目标文件</param>
        /// <returns></returns>
        private static IPageLayout GetTempPageLayout(string templatePath)
        {
            //读取模板
            IMapDocument pTempMapDocument = new MapDocumentClass();
            pTempMapDocument.Open(templatePath);
            IPageLayout pTempPageLayout = pTempMapDocument.PageLayout;
            return pTempPageLayout;
        }

        /// <summary>
        /// 设置地图页属性
        /// </summary>
        /// <param name="pCurPage">当前页</param>
        /// <param name="pTempPage">模板页</param>
        private static void SetPageProperty(IPage pCurPage, IPage pTempPage)
        {
            //替换单位及地图方向
            pCurPage.Orientation = pTempPage.Orientation;
            pCurPage.Units = pTempPage.Units;
            //替换页面尺寸
            Double dWidth, dHeight;
            pTempPage.QuerySize(out dWidth, out dHeight);
            pCurPage.PutCustomSize(dWidth, dHeight);
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

        /// <summary>
        /// 替换原地图容器中的元素为模板的元素
        /// </summary>
        /// <param name="tempGraph">模板</param>
        /// <param name="oldMapFrame">原地图容器</param>
        /// <returns></returns>
        private static IArray ReplaceOldIntoTempElement(IGraphicsContainer tempGraph, IMapFrame oldMapFrame)
        {
            tempGraph.Reset();
            IElement tempElement = tempGraph.Next();
            IArray pArray = new ArrayClass();
            while (tempElement != null)
            {
                if (tempElement is IMapFrame)
                {
                    //改变布局边框几何形状
                    IElement pMapFrameElement = oldMapFrame as IElement;
                    if (pMapFrameElement != null) pMapFrameElement.Geometry = tempElement.Geometry;
                }
                else
                {
                    IMapSurroundFrame pTempMapSurroundFrame = tempElement as IMapSurroundFrame;
                    if (pTempMapSurroundFrame != null)
                    {
                        //设置地图框架
                        pTempMapSurroundFrame.MapFrame = oldMapFrame;
                    }
                    //获取图例指北针等元素
                    pArray.Add(tempElement);
                }
                tempElement = tempGraph.Next();
            }
            return pArray;
        }

    }
}
