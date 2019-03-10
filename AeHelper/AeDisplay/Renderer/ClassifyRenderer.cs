using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AeHelper.LayerProcess.RasterProcess;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace AeHelper.AeDisplay.Renderer
{
    /// <summary>
    /// 分级渲染
    /// </summary>
    public class ClassifyRenderer 
    {
        private readonly IRasterLayer rasterLayer;

        /// <summary>
        /// 分级渲染
        /// </summary>
        /// <param name="rasterLayer"></param>
        public ClassifyRenderer(IRasterLayer rasterLayer)
        {
            this.rasterLayer = rasterLayer;
        }

        /// <summary>
        /// 设置分级数
        /// </summary>
        /// <param name="comboBox">分级数下拉框</param>
        public static void SetClassCount(ComboBox comboBox)
        {
            //初始化分级总数下拉框
            comboBox.Items.Clear();
            for (int i = 1; i < 21; i++)
            {
                comboBox.Items.Add(i);
            }
            comboBox.SelectedIndex = 4;  //初始化为5
        }

        /// <summary>
        /// 添加分级算法
        /// </summary>
        public static void AddClassMethod(ComboBox comboBox)
        {
            //初始化分级算法
            comboBox.Items.Clear();
            comboBox.Items.Add("Manual");
            comboBox.Items.Add("Equal Interval");
            comboBox.Items.Add("Natural Breaks");
            comboBox.Items.Add("Quantile");
            comboBox.Items.Add("Geometrical Interval");
            comboBox.SelectedIndex = 2;  //默认自然分级
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// 获取分级数
        /// </summary>
        /// <param name="classField">字段</param>
        /// <param name="classCount">类别总数</param>
        /// <param name="classMethod">分级算法</param>
        /// <returns></returns>
        public IList<double> GetRasterBreaks(string classField, int classCount, string classMethod)
        {
            IRasterClassifyColorRampRenderer pRClassRend = GetRasterClassifyRenderer(classField, classCount, classMethod);
            IList<double> classBreaks = new List<double>();
            //count + 1，因为分级数比间隔数（类别总数）多一个
            for (int i = 0; i < classCount + 1; i++)
            {
                //取6位小数
                classBreaks.Add(Math.Round(pRClassRend.Break[i], 2));
            }
            return classBreaks;
        }

        /// <summary>
        /// 获取数据表格中的分级数
        /// </summary>
        /// <param name="table">数据表格</param>
        /// <returns></returns>
        public List<double> GetViewClassBreaks(DataTable table)
        {
            if (table.Rows.Count < 1) return null;
            List<double> classBreaks = new List<double>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                string[] sArray = Regex.Split(table.Rows[i]["Range"].ToString(), " - ", RegexOptions.IgnoreCase);
                if (sArray.Length != 2)
                {
                    MessageBox.Show(@"The input range is incorrect.");
                    return null;
                }
                classBreaks.AddRange(sArray.Select(double.Parse));
            }
            return classBreaks;
        }

        /// <summary>
        /// 添加数据到表格视图
        /// </summary>
        /// <param name="classes">分级数</param>
        /// <param name="dgView">数据表格视图</param>
        public void AddDataToGridView(IList<double> classes, DataGridView dgView)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Class");
            table.Columns.Add("Range");
            //从第二个分级数开始
            for (int i = 1; i < classes.Count; i++)
            {
                DataRow dr = table.NewRow();
                dr["Class"] = i;
                dr["Range"] = classes[i - 1] + " - " + classes[i];
                table.Rows.Add(dr);
            }
            dgView.DataSource = null;
            dgView.DataSource = table;
        }

        /// <summary>
        /// 获取字段的唯一值个数
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public int GetUniqueCountOfField(string fieldName)
        {
            int uniqueCount;
            //有属性表
            uniqueCount = RasterTableFunction.IsRasterLayerHaveTable(rasterLayer.Raster)
                ? GetUniqueCountByTable(rasterLayer, fieldName) : GetFloatValueUniqueCount(rasterLayer);
            return uniqueCount;
        }

        /// <summary>
        /// 获取浮点值字段的唯一值个数（无属性表）
        /// </summary>
        /// <param name="rasterLayer">图层</param>
        /// <returns></returns>
        private static int GetFloatValueUniqueCount(IRasterLayer rasterLayer)
        {
            IRasterCalcUniqueValues calcUniqueValues = new RasterCalcUniqueValuesClass();
            IUniqueValues uniqueValues = new UniqueValuesClass();
            //从栅格的第1个波段添加到唯一值
            calcUniqueValues.AddFromRaster(rasterLayer.Raster, 0, uniqueValues);
            return uniqueValues.Count;
        }

        /// <summary>
        /// 获取有属性表的字段唯一值个数
        /// </summary>
        /// <param name="rasterLayer">图层</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        private static int GetUniqueCountByTable(IRasterLayer rasterLayer, string fieldName)
        {
            ITable pRTable = (ITable)rasterLayer;
            //获得栅格游标 
            ICursor pCursor = pRTable.Search(null, false);
            IDataStatistics pData = new DataStatisticsClass();
            pData.Field = fieldName; //字段
            pData.Cursor = pCursor;
            //枚举唯一值
            IEnumerator pEnumVar = pData.UniqueValues;
            //获取唯一值
            return pData.UniqueValueCount;
        }

        /// <summary>
        /// 获取分级渲染设置
        /// </summary>
        /// <param name="classField">字段</param>
        /// <param name="classCount">类别总数</param>
        /// <param name="classMethod">分级算法</param>
        /// <returns></returns>
        public IRasterClassifyColorRampRenderer GetRasterClassifyRenderer(string classField, int classCount,
            string classMethod)
        {
            //先计算统计值，不然无法获取分类数据
            CalculateRasterStatistics(rasterLayer);
            IRasterClassifyColorRampRenderer pRClassRend = new RasterClassifyColorRampRendererClass();
            //分级属性接口
            IRasterClassifyUIProperties classProperties = pRClassRend as IRasterClassifyUIProperties;
            //设置分级算法
            SetClassMethod(classProperties, classMethod);
            //字段
            pRClassRend.ClassField = classField;
            //类别总数
            pRClassRend.ClassCount = classCount;
            IRasterRenderer rasterRend = (IRasterRenderer)pRClassRend;
            rasterRend.Raster = rasterLayer.Raster;
            rasterRend.Update();
            return pRClassRend;
        }

        /// <summary>
        /// 计算栅格统计值
        /// </summary>
        /// <param name="pRasterLayer"></param>
        private static void CalculateRasterStatistics(IRasterLayer pRasterLayer)
        {
            IRasterBandCollection rasBandCollection = pRasterLayer.Raster as IRasterBandCollection;
            if (rasBandCollection == null) return;
            IRasterBand rasBand = rasBandCollection.Item(0);
            bool hasStats;
            rasBand.HasStatistics(out hasStats);
            if (hasStats) return;
            rasBand.ComputeStatsAndHist();
        }

        /// <summary>
        /// 设置分级算法
        /// </summary>
        /// <param name="classProperties">栅格分级渲染属性</param>
        /// <param name="classMethod">分级算法</param>
        private static void SetClassMethod(IRasterClassifyUIProperties classProperties, string classMethod)
        {
            //分级方法初始化
            IClassifyGEN pClassifyGen;
            //判断分级方法，以获取分段点
            switch (classMethod)
            {
                case "Equal Interval":
                    pClassifyGen = new EqualIntervalClass();
                    break;
                case "Natural Breaks":
                    pClassifyGen = new NaturalBreaksClass();
                    break;
                case "Quantile":
                    pClassifyGen = new QuantileClass();
                    break;
                case "Geometrical Interval":
                    pClassifyGen = new GeometricalIntervalClass();
                    break;
                default:
                    pClassifyGen = new NaturalBreaksClass();
                    break;
            }
            //获取分级算法UID
            classProperties.ClassificationMethod = pClassifyGen.ClassID;
        }

        /// <summary>
        /// 获取手动分级渲染设置
        /// </summary>
        /// <param name="classBreaks">分级数</param>
        /// <param name="classField">分级字段</param>
        /// <returns></returns>
        public IRasterClassifyColorRampRenderer GetManualRasterClassifyRenderer(List<double> classBreaks,
            string classField)
        {
            IRasterClassifyColorRampRenderer pRClassRend = new RasterClassifyColorRampRendererClass();
            //字段
            pRClassRend.ClassField = classField;
            //类别总数
            pRClassRend.ClassCount = classBreaks.Count / 2;
            //设置第一个分级数
            pRClassRend.Break[0] = classBreaks[0];
            //每行第二个数值为分级数
            for (int i = 0; i < classBreaks.Count / 2; i++)
            {
                pRClassRend.Break[i + 1] = classBreaks[2 * i + 1];
            }
            return pRClassRend;
        }

        /// <summary>
        /// 栅格分级渲染（注意选中字段唯一值个数不能小于设置的唯一值个数）
        /// </summary>
        /// <param name="rasterClassRenderer">分级渲染设置</param>
        /// <param name="pStyleGalleryItem">颜色带样式</param>
        public void ClassRenderer(IRasterClassifyColorRampRenderer rasterClassRenderer,
            IStyleGalleryItem pStyleGalleryItem)
        {
            if (pStyleGalleryItem == null)
            {
                MessageBox.Show(@"Color ramp cannot be empty");
                return;
            }
            IRasterRenderer rasterRend = (IRasterRenderer)rasterClassRenderer;
            rasterRend.Raster = rasterLayer.Raster;
            rasterRend.Update();
            IEnumColors enumColors = RenderHelper.GetColorRamp(pStyleGalleryItem, rasterClassRenderer.ClassCount).Colors;
            enumColors.Reset();
            IFillSymbol fillSymbol = new SimpleFillSymbolClass();
            for (int i = 0; i < rasterClassRenderer.ClassCount; i++)
            {
                fillSymbol.Color = enumColors.Next();
                rasterClassRenderer.set_Symbol(i, (ISymbol)fillSymbol);
                string label = Math.Round(rasterClassRenderer.Break[i], 2) + "-"
                 + Math.Round(rasterClassRenderer.Break[i + 1], 2);
                rasterClassRenderer.set_Label(i, label);
            }
            rasterLayer.Renderer = rasterRend;
        }

    }
}
