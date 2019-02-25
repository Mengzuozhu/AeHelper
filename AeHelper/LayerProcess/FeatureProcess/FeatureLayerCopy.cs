using System.Collections.Generic;
using System.Data;
using AeHelper.LayerProcess.AllLayerProcess;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using VSTypeFunction.DataTableHelper;

namespace AeHelper.LayerProcess.FeatureProcess
{
    /// <summary>
    /// 复制要素图层
    /// </summary>
    public class FeatureLayerCopy
    {
        //新图层
        private readonly IFeatureLayer newLayer;

        /// <summary>
        /// 复制要素图层
        /// </summary>
        /// <param name="inVectorFile"></param>
        /// <param name="outFile"></param>
        public FeatureLayerCopy(string inVectorFile, string outFile)
        {
            newLayer = CopyLayer(inVectorFile, outFile);
        }

        /// <summary>
        /// 获取复制图层
        /// </summary>
        /// <returns></returns>
        public IFeatureLayer CopyLayer()
        {
            return newLayer;
        }

        /// <summary>
        /// 复制图层
        /// </summary>
        /// <param name="inFile">输入文件</param>
        /// <param name="outFile">输出文件</param>
        /// <returns></returns>
        private static IFeatureLayer CopyLayer(string inFile, string outFile)
        {
            IFeatureClass featureClass = FeatureInfoClass.GetFeatureClass(inFile);
            IDataset dataset = featureClass as IDataset;
            if (dataset == null || !dataset.CanCopy())
            {
                return null;
            }
            DatasetHelper.CopyDatasetAsFeatureLayer(dataset, outFile);
            return FeatureInfoClass.GetFeatureLayer(outFile);
        }

        /// <summary>
        /// 根据新属性表删除要素
        /// </summary>
        /// <param name="newAttributeTable">新属性表</param>
        /// <returns></returns>
        public IFeatureLayer DeleteFeaturesByTable(DataTable newAttributeTable)
        {
            DeleteFeatures(newAttributeTable);
            return newLayer;
        }

        /// <summary>
        /// 根据查询语句删除要素
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public IFeatureLayer DeleteFeaturesByQuery(string whereClause)
        {
            IQueryFilter2 queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = whereClause;
            ITable pTable = newLayer as ITable;
            if (pTable != null)
            {
                pTable.DeleteSearchedRows(queryFilter);
            }
            return newLayer;
        }

        /// <summary>
        /// 删除要素和添加字段
        /// </summary>
        /// <param name="newAttributeTable"></param>
        /// <param name="fieldNames"></param>
        public void AddFields(DataTable newAttributeTable, List<string> fieldNames)
        {
            AddNewFiled(newAttributeTable, fieldNames);
        }

        /// <summary>
        /// 添加新字段
        /// </summary>
        /// <param name="newAttributeTable"></param>
        /// <param name="fieldNames"></param>
        private void AddNewFiled(DataTable newAttributeTable, List<string> fieldNames)
        {
            NewFieldCreateClass createNewField = new NewFieldCreateClass(newLayer, newAttributeTable);
            createNewField.AddNewFields(fieldNames, esriFieldType.esriFieldTypeString);
        }

        /// <summary>
        /// 删除要素
        /// </summary>
        /// <param name="newAttributeTable">新属性表</param>
        private void DeleteFeatures(DataTable newAttributeTable)
        {
            //FID值
            Dictionary<string, int> fidValue = DataTableInfoClass.GetColumnValueDictionary(newAttributeTable, "FID");
            IFeatureCursor featureCursor = newLayer.FeatureClass.Search(null, false);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                //FID不在新属性表中，则删除
                if (!fidValue.ContainsKey(feature.Value[0].ToString()))
                {
                    feature.Delete();
                }
                feature = featureCursor.NextFeature(); //游标移动
            }
        }
    }
}
