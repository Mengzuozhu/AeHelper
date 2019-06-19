using AeHelper.AeDisplay.GeometryDisplay;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;
using stdole;

namespace AeHelper.AeDisplay.Mapping
{
    /// <summary>
    /// 图例
    /// </summary>
    public class MyLegendClass
    {
        /// <summary>
        /// 添加默认图例
        /// </summary>
        /// <param name="layoutControl">布局视图</param>
        /// <param name="pEnv">矩形框</param>
        public static void AddDefaultLegend(AxPageLayoutControl layoutControl, IEnvelope pEnv)
        {
            IGraphicsContainer pGraphicsContainer = layoutControl.PageLayout as IGraphicsContainer;
            if (pGraphicsContainer == null) return;
            IMap pMap = layoutControl.ActiveView.FocusMap;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            if (pMapFrame == null) return;
            UID pID = new UID {Value = "esriCarto.Legend"};
            IMapSurroundFrame
                pMapSurroundFrame = pMapFrame.CreateSurroundFrame(pID, null); //根据唯一标示符，创建与之对应MapSurroundFrame
            //如果已经存在图例，删除已经存在的图例
            MappingHelper.DeleteElementByName(layoutControl, "Legend");
            //添加图例
            IElement pElement = pMapSurroundFrame as IElement;
            if (pElement == null) return;
            pElement.Geometry = pEnv;
            IMapSurround pMapSurround = pMapSurroundFrame.MapSurround;
            ILegend2 pLegend = pMapSurround as ILegend2;
            if (pLegend == null) return;
            //设置图例属性
            SetLegendProperty(pLegend, pMap);
            //添加元素
            MappingHelper.AddElementWithName(pGraphicsContainer, pElement, "Legend");
        }

        /// <summary>
        /// 添加图例
        /// </summary>
        /// <param name="layoutControl">布局视图</param>
        /// <param name="pEnv">矩形框</param>
        /// <param name="pLegend">图例</param>
        public static void AddLegend(AxPageLayoutControl layoutControl, IEnvelope pEnv, ILegend2 pLegend)
        {
            if (pLegend == null) return;
            IGraphicsContainer pGraphicsContainer = layoutControl.PageLayout as IGraphicsContainer;
            if (pGraphicsContainer == null) return;
            IMap pMap = layoutControl.ActiveView.FocusMap;
            IMapFrame pMapFrame = pGraphicsContainer.FindFrame(pMap) as IMapFrame;
            if (pMapFrame == null) return;
            IMapSurroundFrame pMapSurroundFrame = new MapSurroundFrameClass();
            pMapSurroundFrame.MapFrame = pMapFrame;
            //如果已经存在图例，删除已经存在的图例
            MappingHelper.DeleteElementByName(layoutControl, "Legend");
            pMapSurroundFrame.MapSurround = pLegend as IMapSurround;
            //添加图例
            IElement pElement = pMapSurroundFrame as IElement;
            pElement.Geometry = pEnv;
            //添加元素
            MappingHelper.AddElementWithName(pGraphicsContainer, pElement, "Legend");
        }

        /// <summary>
        /// 设置图例属性
        /// </summary>
        /// <param name="pLegend">图例</param>
        /// <param name="pMap">地图</param>
        private static void SetLegendProperty(ILegend2 pLegend, IMap pMap)
        {
            pLegend.ClearItems();
            pLegend.Title = "Legend";
            pLegend.AutoVisibility = true;
            ILegendFormat lFormat = pLegend.Format;
            lFormat.DefaultPatchHeight = 20;
            lFormat.DefaultPatchWidth = 30;
            lFormat.TitleSymbol = GetTextSymbol();
            for (int i = 0; i < pMap.LayerCount; i++)
            {
                ILegendItem pLegendItem = new HorizontalLegendItemClass();
                pLegendItem.Layer = pMap.Layer[i]; //获取添加图例关联图层             
                pLegendItem.Columns = 2;
                ILegendClassFormat iClassFormat = pLegendItem.LegendClassFormat;
                iClassFormat.LabelSymbol = GetTextSymbol();
                //iClassFormat.DescriptionSymbol = GetTextSymbol();
                pLegendItem.HeadingSymbol = GetTextSymbol();
                pLegendItem.ShowDescriptions = false;
                pLegendItem.ShowHeading = true;
                pLegendItem.ShowLabels = true;
                pLegendItem.ShowLayerName = false;
                pLegend.AddItem(pLegendItem); //添加图例内容
            }
        }

        /// <summary>
        /// 获取图例背景
        /// </summary>
        /// <returns></returns>
        private static ISymbolBackground GetSymbolBackground()
        {
            //设置MapSurroundFrame背景
            ISymbolBackground pSymbolBackground = new SymbolBackgroundClass();
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ILineSymbol pLineSymbol = new SimpleLineSymbolClass();
            pLineSymbol.Color = AeColor.GetRgbColor(0, 0, 0);
            pFillSymbol.Color = AeColor.GetRgbColor(240, 240, 240);
            pFillSymbol.Outline = pLineSymbol;
            pSymbolBackground.FillSymbol = pFillSymbol;
            return pSymbolBackground;
        }

        /// <summary>
        /// 获取文本符号
        /// </summary>
        /// <returns></returns>
        private static ITextSymbol GetTextSymbol(decimal fontSize = 24)
        {
            //设置文本格式
            ITextSymbol pTextSymbol = new TextSymbolClass();
            //pTextSymbol.Text = "测试";
            pTextSymbol.Font = (IFontDisp) MappingHelper.GetFont(fontSize: fontSize, isBold: true);
            pTextSymbol.Angle = 0;
            pTextSymbol.RightToLeft = false; //文本由左向右排列
            pTextSymbol.VerticalAlignment = esriTextVerticalAlignment.esriTVABaseline; //垂直方向基线对齐
            pTextSymbol.HorizontalAlignment = esriTextHorizontalAlignment.esriTHACenter; //文本水平居中
            return pTextSymbol;
        }
    }
}