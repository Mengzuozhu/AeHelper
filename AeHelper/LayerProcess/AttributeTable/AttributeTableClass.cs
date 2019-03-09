using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using AeHelper.LayerProcess.AllLayerProcess;
using AeHelper.LayerProcess.FeatureProcess;
using AeHelper.LayerProcess.RasterProcess;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace AeHelper.LayerProcess.AttributeTable
{
    /// <summary>
    /// 属性表
    /// </summary>
    public class AttributeTableClass
    {
        /// <summary>
        /// 将GeoDatabase字段类型转换成.Net相应的数据类型
        /// </summary>
        /// <param name="fieldType">字段类型</param>
        /// <returns></returns>
        private static string ConvertFieldType(esriFieldType fieldType)
        {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return "System.String";
                case esriFieldType.esriFieldTypeDate:
                    return "System.DateTime";
                case esriFieldType.esriFieldTypeDouble:
                    return "System.Double";
                case esriFieldType.esriFieldTypeGeometry:
                    return "System.String";
                case esriFieldType.esriFieldTypeGlobalID:
                    return "System.String";
                case esriFieldType.esriFieldTypeGUID:
                    return "System.String";
                case esriFieldType.esriFieldTypeSmallInteger:
                    return "System.Int16";
                case esriFieldType.esriFieldTypeInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeOID:
                    return "System.String";
                case esriFieldType.esriFieldTypeRaster:
                    return "System.String";
                case esriFieldType.esriFieldTypeSingle:
                    return "System.Single";
                case esriFieldType.esriFieldTypeString:
                    return "System.String";
                case esriFieldType.esriFieldTypeXML:
                    return "System.String";
                default:
                    return "System.String";
            }
        }

        /// <summary>
        /// 根据字段属性设置数据表的列属性
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="table">数据表</param>
        /// <returns>数据表</returns>
        public static DataTable AddTableColumnsByField(IField field, DataTable table)
        {
            //新建一个DataColumn并设置其属性
            DataColumn pDataColumn = new DataColumn(field.Name);
            //字段值是否允许为空
            pDataColumn.AllowDBNull = field.IsNullable;
            //字段别名
            pDataColumn.Caption = field.AliasName;
            //字段数据类型
            pDataColumn.DataType = Type.GetType(ConvertFieldType(field.Type));
            //字段默认值
            pDataColumn.DefaultValue = field.DefaultValue;
            //当字段为String类型是设置字段长度
            if (field.VarType == 8)
            {
                pDataColumn.MaxLength = field.Length;
            }
            //字段添加到表中
            table.Columns.Add(pDataColumn);
            return table;
        }

        /// <summary>
        /// 获取图层的属性表
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="queryFilter2">查询条件</param>
        /// <param name="rowCount">显示的行数</param>
        /// <returns>属性表</returns>
        public static DataTable GetAttributeTable(ILayer layer, IQueryFilter2 queryFilter2 = null,
            int rowCount = 0)
        {
            return GetAttributeTableByITable(GetITableByLayer(layer), FeatureInfoClass.GetShapeType(layer), queryFilter2,
                rowCount);
        }

        /// <summary>
        /// 获取图层指定字段名称的属性表
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="queryFilter2">查询条件</param>
        /// <param name="rowCount">显示的行数</param>
        /// <param name="inFieldName">指定字段名称</param>
        /// <returns>属性表</returns>
        public static DataTable GetAttributeTable(ILayer layer, HashSet<string> inFieldName,
            IQueryFilter2 queryFilter2 = null, int rowCount = 0)
        {
            return GetAttributeTableByITable(GetITableByLayer(layer), inFieldName, FeatureInfoClass.GetShapeType(layer),
                queryFilter2, rowCount);
        }

        /// <summary>
        /// 获取图层指定字段名称的属性表
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="queryFilter2">查询条件</param>
        /// <param name="rowCount">显示的行数</param>
        /// <param name="inFieldName">指定字段名称</param>
        /// <returns>属性表</returns>
        public static DataTable GetAttributeTable(ILayer layer, Dictionary<string,int> inFieldName,
            IQueryFilter2 queryFilter2 = null, int rowCount = 0)
        {
            HashSet<string> fieldNames = new HashSet<string>();
            foreach (var key in inFieldName.Keys)
            {
                fieldNames.Add(key);
            }
            return GetAttributeTableByITable(GetITableByLayer(layer), fieldNames, FeatureInfoClass.GetShapeType(layer),
                queryFilter2, rowCount);
        }

        /// <summary>
        /// 获取空间表格的属性表
        /// </summary>
        /// <param name="iTable">空间表格</param>
        /// <param name="shapeType">图层几何类型</param>
        /// <param name="queryFilter2">查询条件</param>
        /// <param name="rowCount">显示的行数</param>
        /// <returns>属性表</returns>
        public static DataTable GetAttributeTableByITable(ITable iTable, string shapeType = "",
            IQueryFilter2 queryFilter2 = null, int rowCount = 0)
        {
            if (iTable == null) return null;
            HashSet<string> selectFieldName = new HashSet<string>();
            IFields fields = iTable.Fields;
            for (int i = 0; i < fields.FieldCount; i++)
            {
                IField field = fields.Field[i];
                selectFieldName.Add(field.Name);
            }
            return GetAttributeTableByITable(iTable, selectFieldName, shapeType, queryFilter2, rowCount);
        }

        /// <summary>
        /// 获取空间表格指定字段名称的属性表
        /// </summary>
        /// <param name="iTable">空间表格</param>
        /// <param name="queryFilter2">查询条件</param>
        /// <param name="shapeType">图层几何类型</param>
        /// <param name="rowCount">显示的行数</param>
        /// <param name="selectFieldName">指定字段名称</param>
        /// <returns>属性表</returns>
        public static DataTable GetAttributeTableByITable(ITable iTable, HashSet<string> selectFieldName,
             string shapeType = "", IQueryFilter2 queryFilter2 = null, int rowCount = 0)
        {
            if (iTable == null) return null;
            ICursor pCursor = iTable.Search(queryFilter2, false);
            IRow pRow = pCursor.NextRow();
            if (pRow == null) return null;
            IFields fields = pRow.Fields;
            DataTable dataTable = new DataTable();
            for (int i = 0; i < fields.FieldCount; i++)
            {
                //字段名称在指定列表
                if (IsFieldContainInList(fields.Field[i], selectFieldName))
                {
                    dataTable = AddTableColumnsByField(fields.Field[i], dataTable);
                }
            }
            while (pRow != null)
            {
                //新建DataTable的行对象
                DataRow pDataRow = dataTable.NewRow();
                SetDataRow(pRow, pDataRow, selectFieldName, shapeType);
                //添加DataRow到DataTable
                dataTable.Rows.Add(pDataRow);
                pRow = pCursor.NextRow();
                //设置行数
                if (rowCount != 0 && dataTable.Rows.Count == rowCount) break;
            }
            return dataTable;
        }

        /// <summary>
        /// 获取属性表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAttributeTable(ComboBox cboInVector, List<string> mapFeatureFilePath)
        {
            string vectorFile = LayerInfoClass.GetFilePathFromList(cboInVector.Text, mapFeatureFilePath);
            if (string.IsNullOrEmpty(vectorFile)) return null;
            IFeatureLayer featureLayer = FeatureInfoClass.GetFeatureLayer(vectorFile);
            return GetAttributeTable(featureLayer);
        }

        /// <summary>
        /// 根据图层获取空间表格
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns>空间表格</returns>
        public static ITable GetITableByLayer(ILayer layer)
        {
            if (layer == null) return null;
            ITable iTable = null;
            if (layer is IFeatureLayer)
            {
                iTable = (ITable)layer;
            }
            else
            {
                IRasterLayer rasLayer = layer as IRasterLayer;
                //判断是否存在属性表
                if (rasLayer != null && RasterTableFunction.IsRasterLayerHaveTable(rasLayer.Raster))
                {
                    iTable = (ITable)layer;
                }
            }
            return iTable;
        }

        /// <summary>
        /// 设置属性表的行数据
        /// </summary>
        /// <param name="pRow">图层行</param>
        /// <param name="dataRow">表格行</param>
        /// <param name="selectFieldName">选中字段</param>
        /// <param name="shapeType">图层几何类型</param>
        private static void SetDataRow(IRow pRow, DataRow dataRow, HashSet<string> selectFieldName,
            string shapeType)
        {
            for (int i = 0; i < pRow.Fields.FieldCount; i++)
            {
                IField field = pRow.Fields.Field[i];
                //字段名称不在指定列表
                if (!IsFieldContainInList(field, selectFieldName)) continue;
                //根据图层类型设置字段值
                switch (field.Type)
                {
                    case esriFieldType.esriFieldTypeGeometry:
                        dataRow[field.Name] = shapeType;
                        break;
                    case esriFieldType.esriFieldTypeBlob:
                        dataRow[field.Name] = "Element";
                        break;
                    default:
                        dataRow[field.Name] = pRow.Value[i];
                        break;
                }
            }
        }

        /// <summary>
        /// 字段名称是否在指定的字段名称列表中
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="selectFieldName">指定的字段名称列表</param>
        /// <returns></returns>
        private static bool IsFieldContainInList(IField field, HashSet<string> selectFieldName)
        {
            return selectFieldName != null && selectFieldName.Contains(field.Name);
        }
    }
}
