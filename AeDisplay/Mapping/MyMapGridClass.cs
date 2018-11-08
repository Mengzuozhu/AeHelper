using AeDisplay.GeometryDisplay;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using stdole;

namespace AeDisplay.Mapping
{
    /// <summary>
    /// 地图格网
    /// </summary>
    public class MyMapGridClass
    {
        /// <summary>
        /// 获取地图格网
        /// </summary>
        /// <param name="layoutControl">布局视图</param>
        /// <returns></returns>
        public static IMapGrids GetMapGrids(AxPageLayoutControl layoutControl)
        {
            IGraphicsContainer graphicsContainer = layoutControl.PageLayout as IGraphicsContainer;
            if (graphicsContainer == null) return null;
            IMap pMap = layoutControl.ActiveView.FocusMap;
            IFrameElement frameElement = graphicsContainer.FindFrame(pMap);
            IMapFrame mapFrame = frameElement as IMapFrame;
            IMapGrids pMapGrids = mapFrame as IMapGrids;
            return pMapGrids;
        }

        /// <summary>
        /// 获取经纬网，并设置属性
        /// </summary>
        /// <returns></returns>
        public static IGraticule GetGraticuleAndSetProperty()
        {
            IGraticule pGraticule = new GraticuleClass();
            pGraticule.Name = "Map Grid";
            //设置网格线的符号样式
            ICartographicLineSymbol pLineSymbol = GetLineSymbol();
            pGraticule.LineSymbol = pLineSymbol;
            //设置网格的边框样式           
            pGraticule.Border = (IMapGridBorder)GetMapGridBorder();
            //设置网格的主刻度的样式和可见性
            pGraticule.TickLength = 15;
            pGraticule.TickMarkSymbol = null;
            pGraticule.TickLineSymbol = pLineSymbol;
            //设置网格的次级刻度的样式和可见性
            pGraticule.SubTickCount = 5;
            pGraticule.SubTickLength = 10;
            pGraticule.SubTickLineSymbol = pLineSymbol;
            pGraticule.SetSubTickVisibility(true, true, true, true);
            //设置网格的标签的样式和可见性
            pGraticule.LabelFormat.Font = MappingClass.GetFont("Arial") as IFontDisp;
            pGraticule.Visible = true;
            return pGraticule;
        }

        /// <summary>
        /// 获取网格线的符号样式
        /// </summary>
        /// <returns></returns>
        public static ICartographicLineSymbol GetLineSymbol()
        {
            //设置网格线的符号样式
            ICartographicLineSymbol pLineSymbol = new CartographicLineSymbolClass();
            pLineSymbol.Cap = esriLineCapStyle.esriLCSButt;
            pLineSymbol.Width = 1;
            pLineSymbol.Color = AeColorClass.GetRgbColor(0, 0, 0);  //黑色
            return pLineSymbol;
        }

        /// <summary>
        /// 获取网格的边框样式 
        /// </summary>
        /// <returns></returns>
        public static ISimpleMapGridBorder GetMapGridBorder()
        {
            //设置网格的边框样式           
            ISimpleMapGridBorder simpleMapGridBorder = new SimpleMapGridBorderClass();
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
            simpleLineSymbol.Width = 2;
            simpleMapGridBorder.LineSymbol = simpleLineSymbol;
            return simpleMapGridBorder;
        }

        /// <summary>
        /// 获取数字格式
        /// </summary>
        /// <returns></returns>
        public static INumericFormat GetNumericFormat()
        {
            INumericFormat pNumericFormat = new NumericFormatClass();
            pNumericFormat.AlignmentOption = esriNumericAlignmentEnum.esriAlignLeft;
            pNumericFormat.RoundingOption = esriRoundingOptionEnum.esriRoundNumberOfDecimals;
            pNumericFormat.RoundingValue = 0;
            pNumericFormat.ZeroPad = true;
            return pNumericFormat;
        }

        /// <summary>
        /// 设置测量格网的单位和间隔
        /// </summary>
        /// <param name="pMeasuredGrid">测量格网</param>
        /// <param name="units">单位</param>
        /// <param name="xInter">X间隔</param>
        /// <param name="yInter">Y间隔</param>
        public static void SetMeasuredGrid(IMeasuredGrid pMeasuredGrid, esriUnits units, double xInter, double yInter)
        {
            pMeasuredGrid.Units = units;  //单位
            pMeasuredGrid.FixedOrigin = false;
            pMeasuredGrid.XIntervalSize = xInter;  //间隔
            pMeasuredGrid.YIntervalSize = yInter;  //间隔
        }

        /// <summary>
        /// 删除已存在格网
        /// </summary>
        /// <param name="pActiveView"></param>
        public static void DeleteExistMapGrid(IActiveView pActiveView)
        {
            IGraphicsContainer graphicsContainer = pActiveView as IGraphicsContainer;
            if (graphicsContainer == null) return;
            IMap pMap = pActiveView.FocusMap;
            IFrameElement frameElement = graphicsContainer.FindFrame(pMap);
            IMapFrame mapFrame = frameElement as IMapFrame;
            IMapGrids mapGrids = mapFrame as IMapGrids;
            if (mapGrids == null) return;
            if (mapGrids.MapGridCount > 0)
            {
                IMapGrid pMapGrid = mapGrids.MapGrid[0];
                mapGrids.DeleteMapGrid(pMapGrid);
            }
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
        }

    }
}
