using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using LayerProcess.AttributeTable;

namespace LayerProcess.Field
{
    /// <summary>
    /// 修改字段
    /// </summary>
    public class ChangeFieldClass
    {
        private readonly ITable fieldTable;

        /// <summary>
        /// 修改字段
        /// </summary>
        /// <param name="layer"></param>
        public ChangeFieldClass(ILayer layer)
        {
            fieldTable = AttributeTableClass.GetITableByLayer(layer);
        }

        /// <summary>
        /// 字段重命名
        /// </summary>
        /// <param name="oldFieldIndex">字段位置</param>
        /// <param name="newName">新名称</param>
        public bool RenameField(int oldFieldIndex, string newName)
        {
            IField oldField = fieldTable.Fields.Field[oldFieldIndex];
            ChangeNameAndType(oldField, newName, oldField.Type, true);
            return true;
        }

        /// <summary>
        /// 获取字段，并改变名称
        /// </summary>
        /// <param name="oldField">旧字段</param>
        /// <param name="fieldName">新字段名称</param>
        /// <returns></returns>
        public static IFieldEdit GetAndChangeFieldName(IField oldField, string fieldName)
        {
            //定义新字段
            IField newField = new FieldClass();
            //字段编辑
            IFieldEdit newFieldEdit = (IFieldEdit)newField;
            //设置新字段属性
            newFieldEdit.Name_2 = fieldName;
            newFieldEdit.AliasName_2 = fieldName;
            newFieldEdit.Length_2 = oldField.Length;
            newFieldEdit.Type_2 = oldField.Type;
            return newFieldEdit;
        }

        /// <summary>
        /// 修改名称和类型
        /// </summary>
        /// <param name="oldField"></param>
        /// <param name="newName"></param>
        /// <param name="fieldType"></param>
        /// <param name="isDeleteOldField"></param>
        /// <returns></returns>
        public bool ChangeNameAndType(IField oldField, string newName, esriFieldType fieldType,
            bool isDeleteOldField = false)
        {
            if (IsEditFail(oldField)) return false;
            CopyNewField(oldField, newName, fieldType);
            CopyOldValue(oldField, newName);
            DeleteOldField(oldField, isDeleteOldField);
            return true;
        }

        private bool IsEditFail(IField oldField)
        {
            if (fieldTable != null || IsFieldEditable(oldField)) return false;
            MessageBox.Show(@"This field cannot be edited");
            return true;
        }

        private static bool IsFieldEditable(IField field)
        {
            return field.Editable && field.Type != esriFieldType.esriFieldTypeGeometry;
        }

        private void CopyNewField(IField oldField, string newName, esriFieldType fieldType)
        {
            NewFieldClass newFieldProp = new NewFieldClass();
            newFieldProp.SetLength(oldField.Length);
            newFieldProp.AddNewField(fieldTable, newName, fieldType, oldField.IsNullable,
               oldField.Required, oldField.Editable);
        }

        private void CopyOldValue(IField oldField, string newName)
        {
            int newIndex = fieldTable.Fields.FindField(newName);
            int oldFiledIndex = fieldTable.Fields.FindField(oldField.Name);
            IQueryFilter pFilter = new QueryFilterClass();
            pFilter.SubFields = newName + ", " + oldField.Name;
            ICursor pCursor = fieldTable.Update(pFilter, false);
            IRow pRow = pCursor.NextRow();
            for (int i = 0; i < fieldTable.RowCount(null); i++)
            {
                pRow.set_Value(newIndex, pRow.Value[oldFiledIndex]);
                pCursor.UpdateRow(pRow);
                pRow = pCursor.NextRow();
            }
        }

        private void DeleteOldField(IField oldField, bool isDeleteOldField)
        {
            if (isDeleteOldField)
            {
                fieldTable.DeleteField(oldField); //删除原始字段
            }
        }
    }
}
