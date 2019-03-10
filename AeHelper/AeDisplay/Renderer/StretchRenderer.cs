using System;
using System.Data;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;

namespace AeHelper.AeDisplay.Renderer
{
    /// <summary>
    /// 拉伸渲染
    /// </summary>
    public class StretchRenderer 
    {
        /// <summary>
        /// 添加拉伸方法
        /// </summary>
        /// <param name="comboBox">拉伸方法下拉框</param>
        public static void AddStretchMethod(ComboBox comboBox)
        {
            //初始化拉伸类型
            comboBox.Items.Clear();
            comboBox.Items.Add("None");
            comboBox.Items.Add("Custom");
            comboBox.Items.Add("StandardDeviations");
            comboBox.Items.Add("HistogramEqualize");
            comboBox.Items.Add("MinimumMaximum");
            comboBox.Items.Add("HistogramSpecification");
            comboBox.Items.Add("PercentClip");
            comboBox.SelectedIndex = 2;  //默认StandardDeviations
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        /// <summary>
        /// 拉伸渲染
        /// </summary>
        /// <param name="rasterLayer">渲染图层</param>
        /// <param name="stretType">拉伸类型</param>
        /// <param name="pStyleGalleryItem">颜色带样式</param>
        public static void RasterStretchRenderer(IRasterLayer rasterLayer, esriRasterStretchTypesEnum stretType,
            IStyleGalleryItem pStyleGalleryItem)
        {
            if (pStyleGalleryItem == null)
            {
                MessageBox.Show(@"Color ramp cannot be empty");
                return;
            }
            //创建拉伸渲染
            IRasterStretchColorRampRenderer stretchRenderer = new RasterStretchColorRampRendererClass();
            IRasterRenderer rasterRenderer = (IRasterRenderer)stretchRenderer;
            //设置拉伸渲染参数属性
            IRaster raster = rasterLayer.Raster;
            rasterRenderer.Raster = raster;
            rasterRenderer.Update();
            stretchRenderer.BandIndex = 0;
            stretchRenderer.ColorRamp = RenderHelper.GetColorRamp(pStyleGalleryItem, 255);
            //设置拉伸类型
            IRasterStretch stretchType = (IRasterStretch)rasterRenderer;
            stretchType.StretchType = stretType;
            rasterLayer.Renderer = rasterRenderer;
        }

        /// <summary>
        /// 获取拉伸标注
        /// </summary>
        /// <param name="rasterLayer">图层</param>
        /// <returns></returns>
        public static string[] GetStretchLabels(IRasterLayer rasterLayer)
        {
            IRasterStretchColorRampRenderer rasterStretchColorRampRenderer = new RasterStretchColorRampRendererClass();
            IRasterRenderer rasterRenderer = rasterStretchColorRampRenderer as IRasterRenderer;
            rasterRenderer.Raster = rasterLayer.Raster;
            rasterRenderer.Update();
            string[] labels = new string[2];
            labels[0] = rasterStretchColorRampRenderer.LabelHigh;
            labels[1] = rasterStretchColorRampRenderer.LabelLow;
            return labels;
        }

        /// <summary>
        /// 添加数据到数据表格视图
        /// </summary>
        /// <param name="labels">标注</param>
        /// <param name="dgView">数据表格视图</param>
        public static void AddDataToDataView(string[] labels, DataGridView dgView)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Value");
            table.Columns.Add("Label");
            foreach (string lab in labels)
            {
                DataRow dr = table.NewRow();
                string label = lab;
                //获取":"的位置
                int index = label.IndexOf(":", StringComparison.Ordinal);
                dr["Value"] = label.Substring(index + 2);  //去掉:和1个空格
                dr["Label"] = label;
                table.Rows.Add(dr);
            }
            dgView.DataSource = null;
            dgView.DataSource = table;
            //不可编辑
            dgView.ReadOnly = true;
            dgView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        /// <summary>
        /// 获取拉伸类型
        /// </summary>
        /// <param name="stretchType">类型名</param>
        /// <returns></returns>
        public static esriRasterStretchTypesEnum GetStretchType(string stretchType)
        {
            //默认标准差拉伸类型
            esriRasterStretchTypesEnum schType = esriRasterStretchTypesEnum.esriRasterStretch_StandardDeviations;
            switch (stretchType)
            {
                case "None":
                    schType = esriRasterStretchTypesEnum.esriRasterStretch_NONE;
                    break;
                case "Custom":
                    schType = esriRasterStretchTypesEnum.esriRasterStretch_Custom;
                    break;
                case "StandardDeviations":
                    schType = esriRasterStretchTypesEnum.esriRasterStretch_StandardDeviations;
                    break;
                case "HistogramEqualize":
                    schType = esriRasterStretchTypesEnum.esriRasterStretch_HistogramEqualize;
                    break;
                case "MinimumMaximum":
                    schType = esriRasterStretchTypesEnum.esriRasterStretch_MinimumMaximum;
                    break;
                case "HistogramSpecification":
                    schType = esriRasterStretchTypesEnum.esriRasterStretch_HistogramSpecification;
                    break;
                case "PercentClip":
                    schType = esriRasterStretchTypesEnum.esriRasterStretch_PercentMinimumMaximum;
                    break;
            }
            return schType;
        }

    }
}
