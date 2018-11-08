using System.Collections.Generic;
using System.Data;
using System.IO;
using AeHelper.LayerProcess.FieldHelper;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using VSTypeFunction.DataTableClass;

namespace AeHelper.LayerProcess.FeatureProcess
{
    /// <summary>
    /// 创建新的要素图层
    /// </summary>
    public class CreateNewFeatureLayerClass
    {
        private readonly DataTable newAttributeTable; //新图层的属性表格
        private readonly IFeatureLayer oldFeatureLayer; //被查询的旧图层

        /// <summary>
        /// 创建新的要素图层
        /// </summary>
        /// <param name="inVectorFile"></param>
        /// <param name="newLayerTable"></param>
        public CreateNewFeatureLayerClass(string inVectorFile, DataTable newLayerTable)
        {
            oldFeatureLayer = FeatureInfoClass.GetFeatureLayer(inVectorFile);
            newAttributeTable = newLayerTable;
        }

        /// <summary>
        /// 选择要素，创建新图层
        /// </summary>
        /// <param name="whereClause">选择条件</param>
        /// <param name="saveFile">保存路径</param>
        /// <returns></returns>
        public IFeatureLayer SelectAndCreateNewLayer(string whereClause, string saveFile)
        {
            //选择要素
            SelectFeature(whereClause);
            //创建新图层
            return CreateNewLayer(saveFile);
        }

        /// <summary>
        /// 添加新字段
        /// </summary>
        /// <param name="newLayer">新图层</param>
        /// <param name="columnName"></param>
        public void AddNewFiled(IFeatureLayer newLayer, string columnName)
        {
            CreateNewFieldClass createNewField = new CreateNewFieldClass(newLayer, newAttributeTable);
            createNewField.AddNewFiled(columnName, esriFieldType.esriFieldTypeString);
        }

        /// <summary>
        /// 添加多个新字段
        /// </summary>
        /// <param name="newLayer">新图层</param>
        /// <param name="fieldNames">字段列表</param>
        public void AddNewFields(IFeatureLayer newLayer, List<string> fieldNames)
        {
            CreateNewFieldClass createNewField = new CreateNewFieldClass(newLayer, newAttributeTable);
            createNewField.AddNewFields(fieldNames, esriFieldType.esriFieldTypeString);
        }

        /// <summary>
        /// 选择要素
        /// </summary>
        /// <param name="whereClause">选择条件</param>
        private void SelectFeature(string whereClause)
        {
            IQueryFilter2 queryFilter = new QueryFilterClass(); //查询条件
            if (!string.IsNullOrEmpty(whereClause))
            {
                queryFilter.WhereClause = whereClause;
            }
            IFeatureSelection featureSelection = (IFeatureSelection)oldFeatureLayer;
            var esriSrEnum = esriSelectionResultEnum.esriSelectionResultNew;
            //根据查询添加进行选择
            featureSelection.SelectFeatures(queryFilter, esriSrEnum, false);
        }

        /// <summary>
        /// 创建新图层
        /// </summary>
        /// <param name="saveFile">保存路径</param>
        /// <returns></returns>
        private IFeatureLayer CreateNewLayer(string saveFile)
        {
            string saveName = Path.GetFileNameWithoutExtension(saveFile);
            IFeatureLayerDefinition featureLayerDefinition = oldFeatureLayer as IFeatureLayerDefinition;
            if (featureLayerDefinition == null) return null;
            //从要素图层的选择集中创建新要素图层
            IFeatureLayer selectionLayer = featureLayerDefinition.CreateSelectionLayer(saveName, true, null, null);
            //打开要素工作空间
            IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)FeatureInfoClass.GetShapefileWorkspace(saveFile);
            if (pFeatureWorkspace == null || selectionLayer == null) return null;
            IFeatureLayer copyFeatureLayer = CopyOldFeatureLayerWithSelectFiled(pFeatureWorkspace, selectionLayer, saveName);
            return copyFeatureLayer;
        }

        /// <summary>
        /// 复制旧要素图层（只包含选中字段）
        /// </summary>
        /// <param name="featureWorkspace">要素工作空间</param>
        /// <param name="selectionLayer">被复制要素图层</param>
        /// <param name="newLayerName">新图层名称</param>
        /// <returns></returns>
        private IFeatureLayer CopyOldFeatureLayerWithSelectFiled(IFeatureWorkspace featureWorkspace,
            IFeatureLayer selectionLayer, string newLayerName)
        {
            Dictionary<string, int> fidValue = DataTableInfoClass.GetColumnValueDictionary(newAttributeTable, "FID");  //FID值列表
            List<string> columnName = DataTableInfoClass.GetAllColumnName(newAttributeTable);  //表格所有列名
            IWorkspaceEdit workspaceEdit = featureWorkspace as IWorkspaceEdit;
            if (workspaceEdit == null) return null;
            //开始编辑
            StartEdit(workspaceEdit);
            try
            {
                //以旧图层为基础创建新图层要素集合
                IFeatureClass newFeatureClass = CreateNewFeatureClass(featureWorkspace, selectionLayer, newLayerName);
                //复制旧要素值到新要素集合
                CopyFeatureValueToNewFeature(selectionLayer, newFeatureClass, fidValue, columnName);
                //停止并保存编辑
                EndAndSaveEdit(workspaceEdit);

                //删除非选中字段
                DeleteFeatureClassFields(newFeatureClass, columnName);
                return FeatureInfoClass.GetFeatureLayer(newFeatureClass);
            }
            finally
            {
                //撤销编辑
                UndoEdit(workspaceEdit);
            }
        }

        /// <summary>
        /// 以旧图层为基础创建新图层要素集合
        /// </summary>
        /// <param name="featureWorkspace">要素工作空间</param>
        /// <param name="selectionLayer">旧图层</param>
        /// <param name="newLayerName">新图层名称</param>
        /// <returns></returns>
        private static IFeatureClass CreateNewFeatureClass(IFeatureWorkspace featureWorkspace,
            IFeatureLayer selectionLayer, string newLayerName)
        {
            IFeatureClass oldFeatureClass = selectionLayer.FeatureClass;
            //以旧图层为基础创建新图层要素集合
            IFeatureClass newFeatureClass = featureWorkspace.CreateFeatureClass(newLayerName, oldFeatureClass.Fields,
                oldFeatureClass.CLSID, oldFeatureClass.EXTCLSID, oldFeatureClass.FeatureType,
                oldFeatureClass.ShapeFieldName, "");
            return newFeatureClass;
        }

        /// <summary>
        /// 复制旧要素值到新要素集合
        /// </summary>
        /// <param name="selectionLayer">旧图层</param>
        /// <param name="newFeatureClass">新要素集合</param>
        /// <param name="fidValue">FID值</param>
        /// <param name="selectFieldName">选中字段名称</param>
        private static void CopyFeatureValueToNewFeature(IFeatureLayer selectionLayer, IFeatureClass newFeatureClass,
            Dictionary<string, int> fidValue, List<string> selectFieldName)
        {
            IFeatureCursor featureCursor = selectionLayer.FeatureClass.Search(null, false);
            IFeature oldFeature = featureCursor.NextFeature();
            while (oldFeature != null)
            {
                //判断当前要素FID是否在FID集合中，在的话，则赋值给新图层
                if (fidValue.ContainsKey(oldFeature.Value[0].ToString()))
                {
                    IFeature newFeature = newFeatureClass.CreateFeature();
                    IFields fields = newFeatureClass.Fields;
                    for (int i = 0; i < fields.FieldCount; i++)
                    {
                        IField field = fields.Field[i];
                        //字段是选中且可编辑的
                        if (selectFieldName.Contains(field.Name) && field.Editable)
                        {
                            newFeature.set_Value(i, oldFeature.Value[i]); //字段赋值
                        }
                    }
                    newFeature.Store(); //保存新要素
                }
                oldFeature = featureCursor.NextFeature(); //游标移动
            }
        }

        /// <summary>
        /// 使用游标插入要素
        /// </summary>
        /// <param name="selectionLayer">旧图层</param>
        /// <param name="newFeatureClass">新要素集合</param>
        /// <param name="fidValue">FID值</param>
        /// <param name="selectFieldName">选中字段名称</param>
        private static void InsertFeaturesUsingCursor(IFeatureLayer selectionLayer, IFeatureClass newFeatureClass,
            Dictionary<string, int> fidValue, List<string> selectFieldName)
        {
            IFeatureCursor featureCursor = selectionLayer.FeatureClass.Search(null, false);
            IFeature oldFeature = featureCursor.NextFeature();
            IFeatureCursor insertCursor = newFeatureClass.Insert(true);
            while (oldFeature != null)
            {
                //判断当前要素FID是否在FID集合中，在的话，则赋值给新图层
                if (fidValue.ContainsKey(oldFeature.Value[0].ToString()))
                {
                    // Create a feature buffer.
                    IFeatureBuffer featureBuffer = newFeatureClass.CreateFeatureBuffer();
                    for (int i = 0; i < featureBuffer.Fields.FieldCount; i++)
                    {
                        IField field = featureBuffer.Fields.Field[i];
                        //字段是选中且可编辑的
                        if (selectFieldName.Contains(field.Name) && field.Editable)
                        {
                            featureBuffer.set_Value(i, oldFeature.Value[i]); //字段赋值
                        }
                    }
                    insertCursor.InsertFeature(featureBuffer);
                    // Flush the buffer to the geodatabase.
                    insertCursor.Flush();
                }
                oldFeature = featureCursor.NextFeature(); //游标移动
            }
        }

        /// <summary>
        /// 删除要素集合非选中字段
        /// </summary>
        /// <param name="featureClass">要素集合</param>
        /// <param name="selectFieldName">选中字段名称</param>
        private static void DeleteFeatureClassFields(IFeatureClass featureClass, List<string> selectFieldName)
        {
            int fieldCount = featureClass.Fields.FieldCount; //字段总数
            //字段数等于选中字段数，直接返回
            if (selectFieldName.Count == fieldCount) return;
            //删除不在表格中的字段
            //从大往小进行遍历，因为字段总数会变化
            for (int i = fieldCount - 1; i > 0; i--) //保留FID和Shape字段，所以不删除索引为0,1的字段
            {
                IField field = featureClass.Fields.Field[i]; //当前字段
                //是选中或只读字段，则跳过
                if (IsSelectOrReadonlyField(selectFieldName, field)) continue;
                featureClass.DeleteField(field);
            }
        }

        /// <summary>
        /// 是选中或只读字段
        /// </summary>
        /// <param name="selectFieldName">选中字段列表</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        private static bool IsSelectOrReadonlyField(List<string> selectFieldName, IField field)
        {
            //是选中或只读字段
            return selectFieldName.Contains(field.Name) || !FieldNameClass.IsFieldEditable(field);
        }

        /// <summary>
        /// 开始编辑
        /// </summary>
        /// <param name="workspaceEdit">编辑</param>
        private static void StartEdit(IWorkspaceEdit workspaceEdit)
        {
            //开始编辑
            workspaceEdit.StartEditing(true);
            workspaceEdit.StartEditOperation();
        }

        /// <summary>
        /// 停止并保存编辑结果
        /// </summary>
        /// <param name="workspaceEdit">编辑</param>
        private static void EndAndSaveEdit(IWorkspaceEdit workspaceEdit)
        {
            workspaceEdit.StopEditOperation(); //停止编辑操作
            workspaceEdit.StopEditing(true); //保存编辑结果
        }

        /// <summary>
        /// 撤销编辑
        /// </summary>
        /// <param name="workspaceEdit">编辑</param>
        private static void UndoEdit(IWorkspaceEdit workspaceEdit)
        {
            if (!workspaceEdit.IsBeingEdited()) return;
            //撤销编辑
            workspaceEdit.UndoEditOperation();
            workspaceEdit.StopEditing(false);
        }

    }

}
