using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace AeDisplay.GeometryDisplay
{
    /// <summary>
    /// 绘制几何形状
    /// </summary>
    public class DrawGeometryClass
    {
        /// <summary>
        /// 绘制标记点元素
        /// </summary>
        /// <param name="point">标记点</param>
        /// <param name="map">地图</param>
        public static void DrawPoint(IGeometry point, IMap map)
        {
            IGraphicsContainer pGraphicsContainer = DeleteAllElements(map);
            if (pGraphicsContainer == null) return;
            //标记元素
            IMarkerElement markElement = new MarkerElement() as IMarkerElement;
            //设置形状
            IElement element = markElement as IElement;
            if (element == null) return;
            element.Geometry = point;
            //设置标记符号
            markElement.Symbol = GetSimpleMarkerSymbol();

            pGraphicsContainer.AddElement(element, 0);
            PartialRefresh(pGraphicsContainer);
        }

        /// <summary>
        /// 获取简单标记符号
        /// </summary>
        /// <returns></returns>
        private static ISimpleMarkerSymbol GetSimpleMarkerSymbol()
        {
            ISimpleMarkerSymbol simpleMark = new SimpleMarkerSymbol();
            simpleMark.Size = 3;
            simpleMark.Color = AeColorClass.GetRgbColor(255, 0, 0);
            simpleMark.Style = esriSimpleMarkerStyle.esriSMSDiamond;
            return simpleMark;
        }

        /// <summary>
        /// 部分刷新
        /// </summary>
        /// <param name="pGraphicsContainer"></param>
        private static void PartialRefresh(IGraphicsContainer pGraphicsContainer)
        {
            IActiveView pActiveView = pGraphicsContainer as IActiveView;
            if (pActiveView != null) pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        /// <summary>
        /// 绘制红色矩形框
        /// </summary>
        /// <param name="envelope">矩形框</param>
        /// <param name="map">地图</param>
        public static void DrawRectangle(IGeometry envelope, IMap map)
        {
            //在绘制前，清除之前绘制的矩形框
            IGraphicsContainer pGraphicsContainer = DeleteAllElements(map);
            if (pGraphicsContainer == null) return;
            //得到当前视图范围
            IRectangleElement pRectangleElement = new RectangleElementClass();
            IElement pElement = (IElement)pRectangleElement;
            pElement.Geometry = envelope;
            //向地图中添加矩形框
            IFillShapeElement pFillShapeElement = pElement as IFillShapeElement;
            if (pFillShapeElement == null) return;
            pFillShapeElement.Symbol = GetFillSymbol();
            pGraphicsContainer.AddElement((IElement)pFillShapeElement, 0);
            PartialRefresh(pGraphicsContainer);
        }

        /// <summary>
        /// 获取填充符号（实质为中间透明，边框带颜色）
        /// </summary>
        /// <returns></returns>
        private static IFillSymbol GetFillSymbol()
        {
            //设置填充符号（实质为中间透明，边框带颜色）
            IRgbColor pColor = AeColorClass.GetRgbColor(255, 0, 0);
            pColor.Transparency = 255;
            ILineSymbol pOutLine = new SimpleLineSymbolClass();
            pOutLine.Width = 1;
            pOutLine.Color = pColor;

            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pColor = new RgbColorClass();
            pColor.Transparency = 0;
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutLine;
            return pFillSymbol;
        }

        /// <summary>
        /// 删除所有元素
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static IGraphicsContainer DeleteAllElements(IMap map)
        {
            //在绘制前，清除之前绘制的图形
            IGraphicsContainer pGraphicsContainer = map as IGraphicsContainer;
            if (pGraphicsContainer == null) return null;
            pGraphicsContainer.DeleteAllElements();
            return pGraphicsContainer;
        }
    }
}
