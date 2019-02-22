using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Forms;
using AeHelper.LayerProcess.RasterProcess;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using VSTypeFunction.TypeFunction;

namespace AeHelper.AeDisplay.Renderer
{
    /// <summary>
    /// 唯一值渲染
    /// </summary>
    public class MyUniqueValueRenderer : RendererClass
    {
        IRasterLayer rasterLayer;

        /// <summary>
        /// 唯一值渲染
        /// </summary>
        /// <param name="rasterLayer"></param>
        public MyUniqueValueRenderer(IRasterLayer rasterLayer)
        {
            this.rasterLayer = rasterLayer;
        }

        /// <summary>
        /// 唯一值渲染（注意字段类型要一致）
        /// </summary>
        /// <param name="uniqueValue">唯一值列表</param>
        /// <param name="uniqueFiled">字段</param>
        /// <param name="pStyleGalleryItem">颜色带样式</param>
        public void UniqueRenderer<T>(List<T> uniqueValue, string uniqueFiled, IStyleGalleryItem pStyleGalleryItem)
        {
            if (pStyleGalleryItem == null)
            {
                MessageBox.Show(@"Color ramp cannot be empty");
                return;
            }
            IRasterUniqueValueRenderer uniqueValueRenderer = new RasterUniqueValueRendererClass();
            IRasterRenderer pRasterRenderer = uniqueValueRenderer as IRasterRenderer;
            pRasterRenderer.Raster = rasterLayer.Raster;
            pRasterRenderer.Update();
            int uniqueCount = uniqueValue.Count;
            if (uniqueCount == 0)
            {
                MessageBox.Show(@"The count of unique value cannot be zero");
                return;
            }
            //设置唯一值渲染属性
            SetUniqueRendererProperty(uniqueValueRenderer, uniqueFiled, uniqueCount);
            IEnumColors enumColors = GetEnumColors(pStyleGalleryItem, uniqueCount);
            enumColors.Reset();
            //添加唯一值和设置标注符号
            AddValueAndSetLabelSymbol(uniqueValueRenderer, uniqueValue, enumColors);
            rasterLayer.Renderer = pRasterRenderer;
        }

        /// <summary>
        /// 浮点类型图层的唯一值渲染
        /// </summary>
        /// <param name="pStyleGalleryItem">颜色带样式</param>
        public void FloatRasterUniqueRenderer(IStyleGalleryItem pStyleGalleryItem)
        {
            if (pStyleGalleryItem == null)
            {
                MessageBox.Show(@"Color ramp cannot be empty");
                return;
            }
            IRasterUniqueValueRenderer uniqueValueRenderer = new RasterUniqueValueRendererClass();
            IRasterRenderer pRasterRenderer = uniqueValueRenderer as IRasterRenderer;
            pRasterRenderer.Raster = rasterLayer.Raster;
            pRasterRenderer.Update();
            //必须设置栅格渲染的唯一值，不然只能渲染0值
            SetRasterUniqueValue(uniqueValueRenderer);
            //获取排好序的唯一值
            List<object> uniqueValue = GetFloatSortedUniqueValue();
            int uniqueCount = uniqueValue.Count;
            if (uniqueCount == 0)
            {
                MessageBox.Show(@"The count of unique value cannot be zero");
                return ;
            }
            uniqueValueRenderer.set_ClassCount(0, uniqueCount);
            IEnumColors enumColors = GetEnumColors(pStyleGalleryItem, uniqueCount);
            enumColors.Reset();
            //添加唯一值和设置标注符号
            AddValueAndSetLabelSymbol(uniqueValueRenderer, uniqueValue, enumColors);
            rasterLayer.Renderer = pRasterRenderer;
        }

        /// <summary>
        /// 设置栅格渲染的唯一值
        /// </summary>
        /// <param name="uniqueValueRenderer">唯一值渲染</param>
        private void SetRasterUniqueValue(IRasterUniqueValueRenderer uniqueValueRenderer)
        {
            //必须计算获取唯一值，添加到图层，不然只能渲染0值
            IUniqueValues uniqueValues = GetFloatValueAndCount(rasterLayer);
            IRasterRendererUniqueValues renderUniqueValues = uniqueValueRenderer as IRasterRendererUniqueValues;
            if (renderUniqueValues != null) renderUniqueValues.UniqueValues = uniqueValues;
        }

        /// <summary>
        /// 添加唯一值和设置标注符号
        /// </summary>
        /// <param name="uniqueValueRenderer">唯一值渲染</param>
        /// <param name="uniqueValue">唯一值</param>
        /// <param name="enumColors">颜色带</param>
        private static void AddValueAndSetLabelSymbol<T>(IRasterUniqueValueRenderer uniqueValueRenderer, List<T> uniqueValue,
            IEnumColors enumColors)
        {
            IFillSymbol fillSymbol = new SimpleFillSymbolClass();
            for (int i = 0; i < uniqueValue.Count; i++)
            {
                object currentValue = uniqueValue[i];
                uniqueValueRenderer.AddValue(0, i, currentValue);
                fillSymbol.Color = enumColors.Next();
                string valueLabel = currentValue.ToString();
                if (StringHelper.IsNumeric(valueLabel))
                {
                    //获取四位小数的字符串
                     valueLabel = GetRound(currentValue);
                }
                uniqueValueRenderer.set_Label(0, i, valueLabel);
                uniqueValueRenderer.set_Symbol(0, i, (ISymbol)fillSymbol);
            }
        }

        /// <summary>
        /// 设置唯一值渲染属性
        /// </summary>
        /// <param name="uniqueValueRenderer">唯一值渲染</param>
        /// <param name="uniqueFiled">渲染字段</param>
        /// <param name="uniqueCount">唯一值个数</param>
        private static void SetUniqueRendererProperty(IRasterUniqueValueRenderer uniqueValueRenderer,
            string uniqueFiled, int uniqueCount)
        {
            //设置渲染字段
            uniqueValueRenderer.Field = uniqueFiled;
            uniqueValueRenderer.HeadingCount = 1;
            uniqueValueRenderer.set_Heading(0, uniqueFiled);  //设置标题
            uniqueValueRenderer.set_ClassCount(0, uniqueCount);
        }

        /// <summary>
        /// 获取四位小数的字符串
        /// </summary>
        /// <param name="uniqueValue">唯一值</param>
        /// <returns></returns>
        private static string GetRound(object uniqueValue)
        {
            return Math.Round(Convert.ToDouble(uniqueValue), 4).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 添加数据到表格视图
        /// </summary>
        /// <param name="valAndCount">值和数目</param>
        /// <param name="dgView">数据表格视图</param>
        public static void AddDataToGridView(UniqueValueAndCount valAndCount, DataGridView dgView)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Value");
            table.Columns.Add("Count");
            for (int i = 0; i < valAndCount.UniqueValue.Count; i++)
            {
                DataRow dr = table.NewRow();
                dr["Value"] = valAndCount.UniqueValue[i];
                dr["Count"] = valAndCount.UniqueCount[i];
                table.Rows.Add(dr);
            }
            dgView.DataSource = null;
            dgView.DataSource = table;
            dgView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        /// <summary>
        /// 获取唯一值和相应数目
        /// </summary>
        /// <param name="fieldName">字段</param>
        /// <returns></returns>
        public UniqueValueAndCount GetUniqueValueAndCount(string fieldName)
        {
            //判断是否存在属性表
            IUniqueValues uniqueValues = IsFloatRasterLayer()
                ? GetFloatValueAndCount(rasterLayer)
                : GetTableValueAndCount(rasterLayer, fieldName);
            //获取唯一值和相应数目
            UniqueValueAndCount uniqueValueCount = new UniqueValueAndCount();
            for (int i = 0; i < uniqueValues.Count; i++)
            {
                uniqueValueCount.AddValue(uniqueValues.UniqueValue[i]);
                uniqueValueCount.AddCount(uniqueValues.UniqueCount[i]);
            }
            uniqueValueCount.UniqueValue.Sort();
            return uniqueValueCount;
        }

        /// <summary>
        /// 获取浮点类型栅格图层的唯一值（排好序的）
        /// </summary>
        /// <returns></returns>
        private List<object> GetFloatSortedUniqueValue()
        {
            IUniqueValues uniqueValues = GetFloatValueAndCount(rasterLayer);
            List<object> uniqueValue = new List<object>();
            for (int i = 0; i < uniqueValues.Count; i++)
            {
                uniqueValue.Add(uniqueValues.UniqueValue[i]);
            }
            uniqueValue.Sort();
            return uniqueValue;
        }

        /// <summary>
        /// 获取浮点值和数目（无属性表）
        /// </summary>
        /// <param name="rasterLayer">图层</param>
        /// <returns></returns>
        public static IUniqueValues GetFloatValueAndCount(IRasterLayer rasterLayer)
        {
            IRasterCalcUniqueValues calcUniqueValues = new RasterCalcUniqueValuesClass();
            IUniqueValues uniqueValues = new UniqueValuesClass();
            //从栅格的第1个波段添加到唯一值
            calcUniqueValues.AddFromRaster(rasterLayer.Raster, 0, uniqueValues);
            return uniqueValues;
        }

        /// <summary>
        /// 获取有属性表的值和数目
        /// </summary>
        /// <param name="rasterLayer"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IUniqueValues GetTableValueAndCount(IRasterLayer rasterLayer, string fieldName)
        {
            ITable pRTable = (ITable)rasterLayer;
            //获得表格游标 
            ICursor pCursor = pRTable.Search(null, false);
            int fieldIndex = pCursor.FindField(fieldName);
            int countIndex = pCursor.FindField("Count");
            IRasterCalcUniqueValues calcUniqueValues = new RasterCalcUniqueValuesClass();
            IUniqueValues uniqueValues = new UniqueValuesClass();
            //添加表格数据到唯一值中
            calcUniqueValues.AddFromTable(pCursor, fieldIndex, countIndex, uniqueValues);
            return uniqueValues;
        }

        /// <summary>
        /// 获取表格视图的数据（字符串类型）
        /// </summary>
        /// <param name="table">表格</param>
        /// <returns></returns>
        public static List<string> GetViewData(DataTable table)
        {
            if (table.Rows.Count < 1) return null;
            List<string> uValue = new List<string>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                uValue.Add(table.Rows[i][0].ToString());
            }
            return uValue;
        }

        /// <summary>
        /// 判断是否是字符串类型字段
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public bool IsStringField(string fieldName)
        {
            //没有属性表则为浮点型，不是字符串类型
            if (!RasterTableFunction.IsRasterLayerHaveTable(rasterLayer.Raster))
            {
                return false;
            }
            ITable iTable = (ITable)rasterLayer;
            IFields fields = iTable.Fields;
            IField field = fields.Field[fields.FindField(fieldName)];
            return field.Type == esriFieldType.esriFieldTypeString;
        }

        /// <summary>
        /// 是否是浮点数据栅格图层
        /// </summary>
        /// <returns></returns>
        public bool IsFloatRasterLayer()
        {
            return !RasterTableFunction.IsRasterLayerHaveTable(rasterLayer.Raster);
        }

    }
    /// <summary>
    /// 唯一值和相应数目
    /// </summary>
    public class UniqueValueAndCount
    {
        /// <summary>
        /// 唯一值数目
        /// </summary>
        public List<int> UniqueCount { get; set; }

        /// <summary>
        /// 唯一值
        /// </summary>
        public List<object> UniqueValue { get; set; }


        /// <summary>
        /// 唯一值和相应数目
        /// </summary>
        public UniqueValueAndCount()
        {
            UniqueValue = new List<object>();
            UniqueCount = new List<int>();
        }

        /// <summary>
        /// 添加值
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(object value)
        {
            UniqueValue.Add(value);
        }

        /// <summary>
        /// 添加元素个数
        /// </summary>
        /// <param name="count"></param>
        public void AddCount(int count)
        {
            UniqueCount.Add(count);
        }

        /// <summary>
        /// 获取元素个数
        /// </summary>
        /// <returns></returns>
        public int GetItemsCount()
        {
            return UniqueValue == null ? 0 : UniqueValue.Count;
        }
    }

}
