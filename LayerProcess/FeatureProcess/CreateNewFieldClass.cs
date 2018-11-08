using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using LayerProcess.AttributeTable;
using LayerProcess.Field;

namespace LayerProcess.FeatureProcess
{
    /// <summary>
    /// 创建新的字段
    /// </summary>
    public class CreateNewFieldClass
    {
        private readonly IFeatureLayer newFieldLayer;
        private readonly DataTable newFieldTable;

        /// <summary>
        /// 创建新的字段
        /// </summary>
        /// <param name="inFeatureLayer"></param>
        /// <param name="newFieldTable"></param>
        public CreateNewFieldClass(IFeatureLayer inFeatureLayer, DataTable newFieldTable)
        {
            newFieldLayer = inFeatureLayer;
            this.newFieldTable = newFieldTable;
        }

        /// <summary>
        /// 创建新的字段
        /// </summary>
        /// <param name="vectorFile"></param>
        /// <param name="newFieldTable"></param>
        public CreateNewFieldClass(string vectorFile, DataTable newFieldTable)
        {
            newFieldLayer = FeatureInfoClass.GetFeatureLayer(vectorFile);
            this.newFieldTable = newFieldTable;
        }

        /// <summary>
        /// 添加新字段
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldType">字段类型</param>
        public void AddNewFiled(string fieldName, esriFieldType fieldType)
        {
            ITable iTable = AttributeTableClass.GetITableByLayer(newFieldLayer);
            //先删除
            DeleteFieldClass.DeleteField(iTable, fieldName);
            //再添加
            iTable.AddField(NewFieldClass.GetNewFieldEdit(fieldName, fieldType));
            AddTableColumn(fieldName, fieldName);
        }

        /// <summary>
        /// 添加多个新字段
        /// </summary>
        /// <param name="fieldNames"></param>
        /// <param name="fieldType">字段类型</param>
        public void AddNewFields(List<string> fieldNames, esriFieldType fieldType)
        {
            ITable iTable = AttributeTableClass.GetITableByLayer(newFieldLayer);
            foreach (string fieldName in fieldNames)
            {
                //先删除
                DeleteFieldClass.DeleteField(iTable, fieldName);
                //再添加
                iTable.AddField(NewFieldClass.GetNewFieldEdit(fieldName, fieldType));
            }
            AddTableColumns(fieldNames);
        }

        /// <summary>
        /// 添加表格的数据到图层
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="columnName">列名称</param>
        private void AddTableColumnByDictionary(string fieldName, string columnName)
        {
            if (!newFieldTable.Columns.Contains(columnName))
            {
                MessageBox.Show(@"The column is not in the table！");
                return;
            }
            ITable pTable = AttributeTableClass.GetITableByLayer(newFieldLayer);
            if (pTable == null) return;
            int addFieldIndex = pTable.Fields.FindField(fieldName);
            if (addFieldIndex == -1) return;
            IQueryFilter pFilter = new QueryFilterClass();
            pFilter.SubFields = fieldName;  //设置过滤字段，提高效率
            ICursor pCursor = pTable.Update(pFilter, false);
            UpdateRow(pCursor, addFieldIndex, columnName);
        }

        private void UpdateRow(ICursor pCursor, int addFieldIndex, string columnName)
        {
            Dictionary<string, object> fidAndRowValue = GetFidAndRowValue(columnName);
            IRow pRow = pCursor.NextRow();
            while (pRow != null)
            {
                string fid = pRow.Value[0].ToString();
                if (fidAndRowValue.ContainsKey(fid))
                {
                    object rowValue = fidAndRowValue[fid];
                    //非空
                    if (rowValue != DBNull.Value)
                    {
                        pRow.set_Value(addFieldIndex, rowValue);
                        pCursor.UpdateRow(pRow);
                    }
                }
                pRow = pCursor.NextRow();
            }
        }

        /// <summary>
        /// 添加表格的单列数据到图层
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="columnName">列名称</param>
        private void AddTableColumn(string fieldName, string columnName)
        {
            if (!newFieldTable.Columns.Contains(columnName))
            {
                MessageBox.Show(@"The column is not in the table！");
                return;
            }
            ITable pTable = AttributeTableClass.GetITableByLayer(newFieldLayer);
            if (pTable == null) return;
            int addFieldIndex = pTable.Fields.FindField(fieldName);
            if (addFieldIndex == -1) return;
            IQueryFilter pFilter = new QueryFilterClass();
            pFilter.SubFields = fieldName;  //设置过滤字段，提高效率
            ICursor pCursor = pTable.Update(pFilter, false);
            IRow pRow = pCursor.NextRow();
            int rowCount = pTable.RowCount(null);
            for (int i = 0; i < rowCount; i++)
            {
                object rowValue = newFieldTable.Rows[i][columnName];
                //非空
                if (rowValue != DBNull.Value)
                {
                    pRow.set_Value(addFieldIndex, rowValue);
                    pCursor.UpdateRow(pRow);
                }
                pRow = pCursor.NextRow();
            }
        }

        /// <summary>
        /// 添加表格的多列数据到图层
        /// </summary>
        /// <param name="fieldNames"></param>
        private void AddTableColumns(List<string> fieldNames)
        {
            ITable pTable = AttributeTableClass.GetITableByLayer(newFieldLayer);
            if (pTable == null) return;
            Dictionary<int, string> indexAndNames = GetFieldIndexAndName(pTable, fieldNames);
            if (indexAndNames.Count == 0) return;
            IQueryFilter pFilter = GetQueryFilter(indexAndNames.Values.ToList());
            ICursor pCursor = pTable.Update(pFilter, false);
            IRow pRow = pCursor.NextRow();
            int rowCount = pTable.RowCount(null);
            for (int i = 0; i < rowCount; i++)
            {
                foreach (var indexAndName in indexAndNames)
                {
                    object rowValue = newFieldTable.Rows[i][indexAndName.Value];
                    //非空
                    if (rowValue == DBNull.Value) continue;
                    pRow.set_Value(indexAndName.Key, rowValue);
                    pCursor.UpdateRow(pRow);
                }
                pRow = pCursor.NextRow();
            }
        }

        /// <summary>
        /// 获取字段索引和名称
        /// </summary>
        /// <param name="pTable"></param>
        /// <param name="fieldNames"></param>
        /// <returns></returns>
        private static Dictionary<int, string> GetFieldIndexAndName(ITable pTable, List<string> fieldNames)
        {
            Dictionary<int, string> indexAndName = new Dictionary<int, string>();
            foreach (string fieldName in fieldNames)
            {
                int addFieldIndex = pTable.Fields.FindField(fieldName);
                if (addFieldIndex == -1) continue;
                indexAndName.Add(addFieldIndex, fieldName);
            }
            return indexAndName;
        }

        /// <summary>
        /// 获取过滤字段对象
        /// </summary>
        /// <param name="subFields">过滤字段</param>
        /// <returns></returns>
        private static IQueryFilter GetQueryFilter(List<string> subFields)
        {
            IQueryFilter queryFilter = new QueryFilterClass();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(subFields[0]);
            for (int i = 1; i < subFields.Count; i++)
            {
                stringBuilder.Append(", " + subFields[i]);
            }
            queryFilter.SubFields = stringBuilder.ToString();  //设置过滤字段，提高效率
            return queryFilter;
        }

        private Dictionary<string, object> GetFidAndRowValue(string columnName)
        {
            Dictionary<string, object> fidAndRowValue = new Dictionary<string, object>();
            for (int i = 0; i < newFieldTable.Rows.Count; i++)
            {
                string fid = newFieldTable.Rows[i][0].ToString();
                object rowValue = newFieldTable.Rows[i][columnName];
                fidAndRowValue.Add(fid, rowValue);
            }
            return fidAndRowValue;
        }
    }
}
