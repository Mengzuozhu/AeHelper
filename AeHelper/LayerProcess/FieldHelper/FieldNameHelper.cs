using System.Collections.Generic;
using AeHelper.LayerProcess.AttributeTable;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace AeHelper.LayerProcess.FieldHelper
{
    /// <summary>
    /// 字段名称
    /// </summary>
    public class FieldNameHelper
    {
        /// <summary>
        /// 获取指定类型的字段名称
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="fieldTypes">指定类型的字段</param>
        /// <returns></returns>
        private static List<string> GetFieldNameInFieldTypes(ILayer layer,
            Dictionary<esriFieldType, int> fieldTypes)
        {
            ITable iTable = AttributeTableClass.GetITableByLayer(layer);
            if (iTable == null) return null;
            return GetFieldNameInFieldTypes(iTable.Fields, fieldTypes);
        }

        /// <summary>
        /// 获取不在指定类型中的字段
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="fieldTypes">指定类型的字段</param>
        /// <returns></returns>
        private static List<string> GetFieldNameNotInFieldTypes(ILayer layer,
            Dictionary<esriFieldType, int> fieldTypes)
        {
            ITable iTable = AttributeTableClass.GetITableByLayer(layer);
            if (iTable == null) return null;
            return GetFieldNameNotInFieldTypes(iTable.Fields, fieldTypes);
        }

        /// <summary>
        /// 获取指定类型的字段名称
        /// </summary>
        /// <param name="fields">字段集合</param>
        /// <param name="fieldTypes">指定类型的字段</param>
        /// <returns></returns>
        private static List<string> GetFieldNameInFieldTypes(IFields fields,
            Dictionary<esriFieldType, int> fieldTypes)
        {
            List<string> fieldNames = new List<string>();
            for (int i = 0; i < fields.FieldCount; i++)
            {
                IField curField = fields.Field[i];
                //是指定类型，则添加
                if (fieldTypes.ContainsKey(curField.Type))
                {
                    fieldNames.Add(curField.Name);
                }
            }
            return fieldNames;
        }

        /// <summary>
        /// 获取指定类型的字段名称
        /// </summary>
        /// <param name="fields">字段集合</param>
        /// <param name="fieldTypes">指定类型的字段</param>
        /// <returns></returns>
        private static List<string> GetFieldNameNotInFieldTypes(IFields fields,
            Dictionary<esriFieldType, int> fieldTypes)
        {
            List<string> fieldNames = new List<string>();
            for (int i = 0; i < fields.FieldCount; i++)
            {
                IField curField = fields.Field[i];
                //不是指定类型，则添加
                if (!fieldTypes.ContainsKey(curField.Type))
                {
                    fieldNames.Add(curField.Name);
                }
            }
            return fieldNames;
        }

        /// <summary>
        /// 获取图层所有字段名称（除了几何字段）
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public static List<string> GetAllFieldNames(ILayer layer)
        {
            //字段类型为几何，则不添加
            return GetFieldNameNotInFieldTypes(layer, GetGeometryFieldType());
        }

        /// <summary>
        /// 根据要素集合所有字段名称（除了几何字段）
        /// </summary>
        /// <param name="featureClass">要素集合</param>
        /// <returns></returns>
        public static List<string> GetAllFieldNames(IFeatureClass featureClass)
        {
            //字段类型为几何，则不添加
            return featureClass == null
                ? null
                : GetFieldNameNotInFieldTypes(featureClass.Fields, GetGeometryFieldType());
        }

        /// <summary>
        /// 获取几何类型字段
        /// </summary>
        /// <returns></returns>
        private static Dictionary<esriFieldType, int> GetGeometryFieldType()
        {
            return new Dictionary<esriFieldType, int>
            {
                {esriFieldType.esriFieldTypeGeometry, 0}
            };
        }

        /// <summary>
        /// 获取图层整型或字符串字段名称
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public static List<string> GetIntegerOrStringFieldNames(ILayer layer)
        {
            //字段可为整型或字符串型字段。
            Dictionary<esriFieldType, int> fieldTypes = GetOidAndIntegerFieldType();
            fieldTypes.Add(esriFieldType.esriFieldTypeString, 0);
            return GetFieldNameInFieldTypes(layer, fieldTypes);
        }

        /// <summary>
        /// 获取图层整型或浮点字段名称
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public static List<string> GetIntegerOrFloatFieldNames(ILayer layer)
        {
            //字段可为浮点型或整型字段。
            Dictionary<esriFieldType, int> fieldTypes = GetOidIntegerAndFloatFieldType();
            return GetFieldNameInFieldTypes(layer, fieldTypes);
        }

        /// <summary>
        /// 获取图层整型、字符串或浮点字段名称
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public static List<string> GetIntegerOrStringOrFloatFieldNames(ILayer layer)
        {
            //字段可为字符串、浮点型或整型字段。
            Dictionary<esriFieldType, int> fieldTypes = GetOidIntegerAndFloatFieldType();
            fieldTypes.Add(esriFieldType.esriFieldTypeString, 0);
            return GetFieldNameInFieldTypes(layer, fieldTypes);
        }

        /// <summary>
        /// 获取OID和整型字段类型
        /// </summary>
        /// <returns></returns>
        private static Dictionary<esriFieldType, int> GetOidAndIntegerFieldType()
        {
            return new Dictionary<esriFieldType, int>
            {
                {esriFieldType.esriFieldTypeOID, 0},
                {esriFieldType.esriFieldTypeInteger, 0},
                {esriFieldType.esriFieldTypeSmallInteger, 0},
            };
        }

        /// <summary>
        /// 获取OID、整型和浮点字段类型
        /// </summary>
        /// <returns></returns>
        private static Dictionary<esriFieldType, int> GetOidIntegerAndFloatFieldType()
        {
            Dictionary<esriFieldType, int> fieldTypes = GetOidAndIntegerFieldType();
            fieldTypes.Add(esriFieldType.esriFieldTypeSingle, 0);
            fieldTypes.Add(esriFieldType.esriFieldTypeDouble, 0);
            return fieldTypes;
        }

        /// <summary>
        /// 获取图层日期字段名称
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public static List<string> GetDateFieldNames(ILayer layer)
        {
            //字段类型为日期
            Dictionary<esriFieldType, int> fieldTypes = new Dictionary<esriFieldType, int>
            {
                {esriFieldType.esriFieldTypeDate, 0}
            };
            return GetFieldNameInFieldTypes(layer, fieldTypes);
        }

        /// <summary>
        /// 获取可编辑字段
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public static List<string> GetEditableFieldNames(ILayer layer)
        {
            ITable iTable = AttributeTableClass.GetITableByLayer(layer);
            if (iTable == null) return null;
            IFields fields = iTable.Fields;
            List<string> outField = new List<string>();
            for (int i = 0; i < fields.FieldCount; i++)
            {
                IField curField = fields.Field[i];
                //字段不可编辑或为几何类型，则跳过
                if (!IsFieldEditable(curField)) continue;
                outField.Add(curField.Name);
            }
            return outField;
        }

        /// <summary>
        /// 获取不可编辑字段
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns></returns>
        public static List<string> GetReadonlyFieldNames(ILayer layer)
        {
            ITable iTable = AttributeTableClass.GetITableByLayer(layer);
            if (iTable == null) return null;
            IFields fields = iTable.Fields;
            List<string> outField = new List<string>();
            for (int i = 0; i < fields.FieldCount; i++)
            {
                IField curField = fields.Field[i];
                //字段可编辑，则跳过
                if (IsFieldEditable(curField)) continue;
                outField.Add(curField.Name);
            }
            return outField;
        }

        /// <summary>
        /// 字段可编辑
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public static bool IsFieldEditable(IField field)
        {
            //字段可编辑，且不是几何类型
            return field.Editable && field.Type != esriFieldType.esriFieldTypeGeometry;
        }
    }
}
