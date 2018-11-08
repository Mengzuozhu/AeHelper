using System.Collections;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using LayerProcess.AttributeTable;

namespace LayerProcess.Field
{
    /// <summary>
    /// 图层字段信息
    /// </summary>
    public class LayerFieldInfoClass
    {
        /// <summary>
        /// 根据中文获取字段类型
        /// </summary>
        /// <param name="fieldType">字段类型名</param>
        /// <returns></returns>
        public static esriFieldType GetFieldTypeByChinese(string fieldType)
        {
            esriFieldType outType = esriFieldType.esriFieldTypeString;
            switch (fieldType)
            {
                case "长整型":
                    outType = esriFieldType.esriFieldTypeInteger;
                    break;
                case "短整型":
                    outType = esriFieldType.esriFieldTypeSmallInteger;
                    break;
                case "浮点型":
                    outType = esriFieldType.esriFieldTypeSingle;
                    break;
                case "双精度":
                    outType = esriFieldType.esriFieldTypeDouble;
                    break;
                case "文本型":
                    outType = esriFieldType.esriFieldTypeString;
                    break;
                case "日期型":
                    outType = esriFieldType.esriFieldTypeDate;
                    break;
            }
            return outType;
        }

        /// <summary>
        /// 根据英文获取字段类型
        /// </summary>
        /// <param name="fieldType">字段类型名</param>
        /// <returns></returns>
        public static esriFieldType GetFieldTypeByEnglish(string fieldType)
        {
            //默认字符串类型
            esriFieldType fType = esriFieldType.esriFieldTypeString;
            switch (fieldType)
            {
                case "SmallInteger":
                    fType = esriFieldType.esriFieldTypeSmallInteger;
                    break;
                case "Integer":
                    fType = esriFieldType.esriFieldTypeInteger;
                    break;
                case "Single":
                    fType = esriFieldType.esriFieldTypeSingle;
                    break;
                case "Double":
                    fType = esriFieldType.esriFieldTypeDouble;
                    break;
                case "String":
                    fType = esriFieldType.esriFieldTypeString;
                    break;
                case "Date":
                    fType = esriFieldType.esriFieldTypeDate;
                    break;
            }
            return fType;
        }

        /// <summary>
        /// 根据名称获取字段
        /// </summary>
        /// <param name="iTable"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IField GetFieldByName(ITable iTable, string fieldName)
        {
            if (iTable == null) return null;
            IFields fields = iTable.Fields;
            int fieldIndex = fields.FindField(fieldName);
            //不存在该字段，则返回null
            return fieldIndex == -1 ? null : fields.Field[fieldIndex];
        }

        /// <summary>
        /// 根据名称获取字段
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns></returns>
        public static IField GetFieldByName(ILayer layer, string fieldName)
        {
            return GetFieldByName(AttributeTableClass.GetITableByLayer(layer), fieldName);
        }

        /// <summary>
        /// 获取字段索引
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static int GetFieldIndex(ILayer layer, string fieldName)
        {
            ITable iTable = AttributeTableClass.GetITableByLayer(layer);
            if (iTable == null) return -1;
            return iTable.Fields.FindField(fieldName);
        }

        /// <summary>
        /// 获取图层字段的唯一值
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>唯一值</returns>
        public static List<string> GetLayerFieldUniqueValue(ILayer layer, string fieldName)
        {
            IDataStatistics dataStatistics = GetDataStatistics(layer, fieldName);
            if (dataStatistics == null) return null;
            //枚举唯一值
            IEnumerator pEnumVar = dataStatistics.UniqueValues;
            List<string> value = new List<string>();
            pEnumVar.Reset(); //将游标重置到第一个成员前面
            while (pEnumVar.MoveNext()) //将游标的内部位置向前移动
            {
                if (pEnumVar.Current != null)
                {
                    value.Add(pEnumVar.Current.ToString()); //获取当前的项（只读属性）
                }
            }
            return value;
        }

        /// <summary>
        /// 获取图层表格数据统计对象
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static IDataStatistics GetDataStatistics(ILayer layer, string fieldName)
        {
            ITable iTable = AttributeTableClass.GetITableByLayer(layer);
            if (iTable == null) return null;
            IQueryFilter2 queryFilter = new QueryFilterClass();
            queryFilter.SubFields = fieldName;
            //获得游标 
            ICursor pCursor = iTable.Search(queryFilter, false);
            IDataStatistics dataStatistics = new DataStatisticsClass();
            dataStatistics.Field = fieldName; //字段
            dataStatistics.Cursor = pCursor;
            return dataStatistics;
        }

        /// <summary>
        /// 判断图层是否拥有某个字段
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="fieldName">字段名</param>
        /// <returns></returns>
        public static bool IsLayerHaveField(ILayer layer, string fieldName)
        {
            return GetFieldIndex(layer, fieldName) > -1;
        }

        /// <summary>
        /// 判断空间表格是否存在指定类型的字段
        /// </summary>
        /// <param name="iTable">空间表格</param>
        /// <param name="fieldType">指定类型</param>
        /// <returns></returns>
        public static bool IsTableHasFieldType(ITable iTable, esriFieldType fieldType)
        {
            IFields fields = iTable.Fields;
            //判断是否存在字段类型为fieldType
            for (int i = 0; i < fields.FieldCount; i++)
            {
                IField ofield = fields.Field[i];
                if (ofield.Type != fieldType) continue;
                return true;
            }
            return false;
        }

    }
}
