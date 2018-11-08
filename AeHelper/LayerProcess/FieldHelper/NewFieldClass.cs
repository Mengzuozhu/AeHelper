using AeHelper.LayerProcess.AttributeTable;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace AeHelper.LayerProcess.FieldHelper
{
    /// <summary>
    /// 新字段
    /// </summary>
    public class NewFieldClass
    {
        private readonly IFieldEdit fieldEdit;

        /// <summary>
        /// 新字段
        /// </summary>
        public NewFieldClass()
        {
            fieldEdit = GetFieldEdit();
        }

        private static IFieldEdit GetFieldEdit()
        {
            IField newField = new FieldClass();
            return (IFieldEdit)newField;
        }

        private void SetFieldEdit(string fieldName, esriFieldType fieldType, bool isNullable = true,
            bool isRequired = false, bool isEditable = true)
        {
            SetName(fieldName);
            SetAliasName(fieldName);
            SetType(fieldType);
            SetIsNullable(isNullable);
            SetIsRequired(isRequired);
            SetIsEditable(isEditable);
        }

        /// <summary>
        /// 添加新字段
        /// </summary>
        /// <param name="pTable">空间表格</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldType">字段类型</param>
        /// <param name="isNullable"></param>
        /// <param name="isRequired"></param>
        /// <param name="isEditable"></param>
        /// <returns></returns>
        public bool AddNewField(ITable pTable, string fieldName, esriFieldType fieldType, bool isNullable = true,
            bool isRequired = false, bool isEditable = true)
        {
            if (pTable == null || pTable.Fields.FindField(fieldName) != -1) return false;
            SetFieldEdit(fieldName, fieldType, isNullable, isRequired, isEditable);
            pTable.AddField(fieldEdit);
            return true;
        }

        /// <summary>
        /// 添加新字段
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldType">字段类型</param>
        /// <param name="isNullable"></param>
        /// <param name="isRequired"></param>
        /// <param name="isEditable"></param>
        /// <returns></returns>
        public bool AddNewField(ILayer layer, string fieldName, esriFieldType fieldType, bool isNullable = true,
            bool isRequired = false, bool isEditable = true)
        {
            ITable iTable = AttributeTableClass.GetITableByLayer(layer);
            return AddNewField(iTable, fieldName, fieldType, isNullable, isRequired, isEditable);
        }

        /// <summary>
        /// 获取新字段
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="fieldType">字段类型</param>
        /// <returns></returns>
        public static IFieldEdit GetNewFieldEdit(string fieldName, esriFieldType fieldType)
        {
            IFieldEdit fieldEdit = GetFieldEdit();
            //字段名称和别名
            fieldEdit.Name_2 = fieldName;
            fieldEdit.AliasName_2 = fieldName;
            //字段类型
            fieldEdit.Type_2 = fieldType;
            fieldEdit.Editable_2 = true;  //可编辑
            fieldEdit.IsNullable_2 = true;
            return fieldEdit;
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="pTable"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public static int AddAndGetNewFieldIndex(ITable pTable, string fieldName, esriFieldType fieldType)
        {
            int oldFieldIndex = pTable.Fields.FindField(fieldName);
            if (oldFieldIndex != -1)
            {
                return oldFieldIndex;
            }
            IFieldEdit newField = GetNewFieldEdit(fieldName, fieldType);
            pTable.AddField(newField);
            return pTable.Fields.FindField(fieldName);
        }

        private void SetName(string fieldName)
        {
            fieldEdit.Name_2 = fieldName;
        }
        private void SetType(esriFieldType fieldType)
        {
            //字段类型
            fieldEdit.Type_2 = fieldType;
        }

        /// <summary>
        /// 设置别名
        /// </summary>
        /// <param name="aliasName"></param>
        public void SetAliasName(string aliasName)
        {
            fieldEdit.AliasName_2 = aliasName;
        }

        /// <summary>
        /// 设置精度
        /// </summary>
        /// <param name="precision"></param>
        public void SetPrecision(int precision)
        {
            fieldEdit.Precision_2 = precision;
        }

        /// <summary>
        /// 设置刻度
        /// </summary>
        /// <param name="scale"></param>
        public void SetScale(int scale)
        {
            fieldEdit.Scale_2 = scale;
        }

        /// <summary>
        /// 设置长度
        /// </summary>
        /// <param name="length"></param>
        public void SetLength(int length)
        {
            fieldEdit.Length_2 = length;
        }

        /// <summary>
        /// 设置是否为空
        /// </summary>
        /// <param name="isNullable"></param>
        public void SetIsNullable(bool isNullable)
        {
            //字段是否可空
            fieldEdit.IsNullable_2 = isNullable;
        }

        /// <summary>
        /// 设置是否必需
        /// </summary>
        /// <param name="isRequired"></param>
        public void SetIsRequired(bool isRequired)
        {
            fieldEdit.Required_2 = isRequired;
        }

        /// <summary>
        /// 设置是否可编辑
        /// </summary>
        /// <param name="isEditable"></param>
        public void SetIsEditable(bool isEditable)
        {
            fieldEdit.Editable_2 = isEditable;
        }

    }
}
