using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace AeHelper.LayerProcess.FieldHelper
{
    /// <summary>
    /// 删除字段
    /// </summary>
    public class DeleteFieldClass
    {
        /// <summary>
        /// 删除字段
        /// </summary>
        /// <param name="iTable">空间表格</param>
        /// <param name="fieldName">字段名称</param>
        public static void DeleteField(ITable iTable, string fieldName)
        {
            IField field = LayerFieldInfoClass.GetFieldByName(iTable, fieldName);
            if (field == null) return;
            iTable.DeleteField(field);
        }

        /// <summary>
        /// 删除指定类型的字段
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="fieldType">指定字段类型</param>
        /// <returns>图层</returns>
        public static ILayer DeleteFieldType(ILayer layer, esriFieldType fieldType)
        {
            ITable pTable = (ITable)layer;
            IFields pfields = pTable.Fields;
            //删除指定类型的字段
            for (int i = 0; i < pfields.FieldCount; i++)
            {
                IField pfield = pfields.Field[i];
                if (pfield.Type == fieldType)
                {
                    pTable.DeleteField(pfield);
                }
            }
            return layer;
        }

        /// <summary>
        /// 删除不被选中的字段
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="selectedFieldName">选中的字段列表</param>
        public static ILayer DeleteNoSelectedField(ILayer layer, List<string> selectedFieldName)
        {
            ITable pTable = (ITable)layer;
            IFields fields = pTable.Fields;
            for (int i = fields.FieldCount - 1; i > 0; i--)  //遍历删除字段
            {
                IField field = fields.Field[i];
                //不是要删除的字段
                if (IsNotDeleteField(selectedFieldName, field)) continue;
                //删除字段
                pTable.DeleteField(field);
            }
            return layer;
        }

        /// <summary>
        /// 不是要删除的字段
        /// </summary>
        /// <param name="selectedFieldName">选中的字段名称</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        private static bool IsNotDeleteField(List<string> selectedFieldName, IField field)
        {
            return selectedFieldName.Contains(field.Name) || !field.Editable
                   || field.Type == esriFieldType.esriFieldTypeGeometry;
        }

    }
}
