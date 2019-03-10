using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AeHelper.AeDisplay.GeometryDisplay;
using AeHelper.LayerProcess.FieldHelper;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;

namespace AeHelper.AeDisplay.Renderer
{
    /// <summary>
    /// 渲染
    /// </summary>
    public class RenderHelper
    {
        /// <summary>
        /// 获取颜色带枚举
        /// </summary>
        /// <param name="pStyleGalleryItem">颜色带样式</param>
        /// <param name="size">颜色个数</param>
        /// <returns></returns>
        public static IEnumColors GetEnumColors(IStyleGalleryItem pStyleGalleryItem, int size)
        {
            if (size == 0) return null;
            return GetColorRamp(pStyleGalleryItem, size).Colors;
        }

        /// <summary>
        /// 获取颜色带
        /// </summary>
        /// <param name="pStyleGalleryItem">颜色带样式</param>
        /// <param name="size">颜色个数</param>
        /// <returns></returns>
        public static IColorRamp GetColorRamp(IStyleGalleryItem pStyleGalleryItem, int size)
        {
            if (size == 0) return null;
            IColorRamp colorRamp = (IColorRamp) pStyleGalleryItem.Item;
            //颜色个数至少为2个
            colorRamp.Size = size == 1 ? 2 : size;
            bool createColorRamp;
            colorRamp.CreateRamp(out createColorRamp);
            return createColorRamp ? colorRamp : null;
        }

        /// <summary>
        /// 添加所有数值字段名称到下拉框（除了 Count、OID）
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="cboData">下拉框</param>
        public static void AddNumberFieldsToComboBox(IRasterLayer layer, ComboBox cboData)
        {
            List<string> fields = FieldNameHelper.GetIntegerOrFloatFieldNames(layer);
            AddFieldsToComboBox(fields, cboData);
        }

        /// <summary>
        /// 添加所有数值和字符串字段名称到下拉框（除了 Count、OID）
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="cboData">下拉框</param>
        public static void AddNumberAndStringFieldsToComboBox(IRasterLayer layer, ComboBox cboData)
        {
            List<string> fields = FieldNameHelper.GetIntegerOrStringOrFloatFieldNames(layer);
            AddFieldsToComboBox(fields, cboData);
        }

        /// <summary>
        /// 添加字段名称到下拉框（除了 Count、OID）
        /// </summary>
        /// <param name="fields">字段名称</param>
        /// <param name="cboData">下拉框</param>
        private static void AddFieldsToComboBox(List<string> fields, ComboBox cboData)
        {
            cboData.Items.Clear();
            if (fields.Count == 0)
            {
                cboData.Items.Add("Value");
            }
            else
            {
                foreach (
                    var name in
                    fields.Where(name => name.ToLowerInvariant() != "count" && name.ToLowerInvariant() != "oid"))
                {
                    cboData.Items.Add(name);
                }
            }

            if (cboData.Items.Count < 1) return;
            cboData.SelectedIndex = 0;
        }

        /// <summary>
        /// 获取渐变色带
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IAlgorithmicColorRamp GetDefaultGradualColor(int size)
        {
            IAlgorithmicColorRamp pColorRamp = new AlgorithmicColorRampClass();
            pColorRamp.FromColor = AeColor.GetRgbColor(0, 255, 0);
            pColorRamp.ToColor = AeColor.GetRgbColor(0, 0, 255);
            if (size <= 1)
            {
                size = 2;
            }

            pColorRamp.Size = size; //渐变颜色带范围
            bool ok;
            pColorRamp.CreateRamp(out ok);
            return pColorRamp;
        }

        /// <summary>
        /// 获取颜色符号  
        /// </summary>
        /// <param name="geometryType"></param>
        /// <param name="pColor"></param>
        /// <returns></returns>
        public static ISymbol GetColorSymbol(esriGeometryType geometryType, IColor pColor)
        {
            switch (geometryType)
            {
                case esriGeometryType.esriGeometryLine:
                case esriGeometryType.esriGeometryPolyline:
                    ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();
                    pLineSymbol.Color = pColor;
                    pLineSymbol.Width = 2;
                    pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                    return (ISymbol) pLineSymbol;
                case esriGeometryType.esriGeometryPoint:
                    ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
                    pMarkerSymbol.Color = pColor;
                    pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                    return (ISymbol) pMarkerSymbol;
                case esriGeometryType.esriGeometryPolygon:
                    ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                    pFillSymbol.Color = pColor;
                    pFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
                    return (ISymbol) pFillSymbol;
            }

            return null;
        }

        /// <summary>
        /// 获取枚举色带
        /// </summary>
        /// <param name="styleGalleryItem"></param>
        /// <param name="fieldCount"></param>
        /// <returns></returns>
        public static List<IColor> GetColors(int fieldCount, IStyleGalleryItem styleGalleryItem = null)
        {
            //获取默认颜色带
            return styleGalleryItem == null
                ? GetDefaultColors(fieldCount)
                : EnumColorsToList(GetEnumColors(styleGalleryItem, fieldCount));
        }

        /// <summary>
        /// 枚举颜色变为列表
        /// </summary>
        /// <param name="pEnumColor"></param>
        /// <returns></returns>
        private static List<IColor> EnumColorsToList(IEnumColors pEnumColor)
        {
            pEnumColor.Reset();
            List<IColor> aeColors = new List<IColor>();
            IColor color = pEnumColor.Next();
            while (color != null)
            {
                aeColors.Add(color);
                color = pEnumColor.Next();
            }

            return aeColors;
        }

        /// <summary>
        /// 获取默认颜色
        /// </summary>
        /// <param name="colorCount"></param>
        /// <returns></returns>
        private static List<IColor> GetDefaultColors(int colorCount)
        {
            var colors = ColorHelper.GetDefaultColors(colorCount);
            List<IColor> aeColors = new List<IColor>();
            foreach (Color color in colors)
            {
                aeColors.Add(AeColor.VsColorToAeColor(color));
            }

            return aeColors;
        }
    }
}