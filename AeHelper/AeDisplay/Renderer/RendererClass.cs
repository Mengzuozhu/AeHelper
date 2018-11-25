using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AeHelper.LayerProcess.FieldHelper;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

namespace AeHelper.AeDisplay.Renderer
{
    /// <summary>
    /// 渲染
    /// </summary>
    public class RendererClass
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
            IColorRamp colorRamp = (IColorRamp)pStyleGalleryItem.Item;
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
    }


}
