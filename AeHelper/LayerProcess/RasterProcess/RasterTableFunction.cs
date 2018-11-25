using System.Data;
using AeHelper.LayerProcess.AttributeTable;
using AeHelper.LayerProcess.FieldHelper;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;

namespace AeHelper.LayerProcess.RasterProcess
{
    /// <summary>
    /// 栅格属性表函数
    /// </summary>
    public class RasterTableFunction
    {
        /// <summary>
        /// 获取栅格数据集的属性表
        /// </summary>
        /// <param name="geoDataset">输入几何数据集</param>
        /// <returns></returns>
        public static DataTable GetRasterTableByGeoDataset(IGeoDataset geoDataset)
        {
            IRaster raster = (IRaster)geoDataset;
            ITable iTable = GetTableByRaster(raster);
            return AttributeTableClass.GetAttributeTableByITable(iTable);
        }

        /// <summary>
        /// 获取栅格图层的属性表
        /// </summary>
        /// <param name="layer">栅格图层</param>
        /// <param name="queryFilter2">过滤条件</param>
        /// <returns></returns>
        public static DataTable GetRasterTableByLayer(ILayer layer, IQueryFilter2 queryFilter2 = null)
        {
            ITable iTable = BuildRasterAttributeTable(layer);
            //ITable iTable = (ITable)layer; //若已存在属性表，直接用该行代码
            return AttributeTableClass.GetAttributeTableByITable(iTable);
        }

        /// <summary>
        /// 构建栅格图层属性表
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns>属性表ITable</returns>
        public static ITable BuildRasterAttributeTable(ILayer layer)
        {
            if (!(layer is IRasterLayer)) return null;
            IRasterLayer rasterLayer = (IRasterLayer)layer;
            IRaster pRaster = rasterLayer.Raster;
            ITable rTable = GetTableByRaster(pRaster);
            if (rTable != null) return rTable; //直接获取属性表
            //建立新属性表
            BuildRasterAttributeTable(rasterLayer);
            //更新属性表
            return GetTableByRaster(pRaster);    //重新获取属性表
        }

        /// <summary>
        /// 建立栅格图层属性表
        /// </summary>
        /// <param name="rasterDataset">栅格数据集</param>
        public static void BuildRasterAttributeTable(IRasterDataset rasterDataset)
        {
            //建立默认属性表
            IRasterDatasetEdit2 rasterDatasetEdit = (IRasterDatasetEdit2)rasterDataset;
            if (rasterDatasetEdit == null) return;
            rasterDatasetEdit.BuildAttributeTable();
        }

        /// <summary>
        /// 根据栅格获取空间表格
        /// </summary>
        /// <param name="raster">栅格</param>
        /// <returns></returns>
        public static ITable GetTableByRaster(IRaster raster)
        {
            //判断栅格像元值是否是浮点型
            return IsFloatPixel(raster) ? null : GetRasterBandAttributeTable(raster);
        }

        /// <summary>  
        /// 删除栅格图层字段（字符串字段）  
        /// </summary>  
        /// <param name="rasterLayer">栅格图层</param>  
        /// <returns></returns>  
        public static IRasterLayer DeleteRasterStringField(IRasterLayer rasterLayer)
        {
            if (!IsRasterLayerHaveTable(rasterLayer.Raster)) return rasterLayer;
            ITable oldTable = (ITable)rasterLayer;
            //判断是否存在字符串字段
            bool hasString = LayerFieldInfo.IsTableHasFieldType(oldTable, esriFieldType.esriFieldTypeString);
            //不需要删除，则返回原始图层
            if (!hasString) return rasterLayer;
            ILayer copyLayer = CopyRasterDatasetClass.CopyAsTempRasterLayer(rasterLayer.FilePath);
            if (copyLayer == null) return null;
            //删除字符串类型字段
            ILayer layer = DeleteFieldClass.DeleteFieldType(copyLayer, esriFieldType.esriFieldTypeString);
            return (IRasterLayer)layer;
        }

        /// <summary>
        /// 判断栅格图层是否拥有属性表
        /// </summary>
        /// <param name="raster">栅格</param>
        /// <returns>是否拥有属性表</returns>
        public static bool IsRasterLayerHaveTable(IRaster raster)
        {
            if (IsFloatPixel(raster))
            {
                return false;
            }
            return GetRasterBandAttributeTable(raster) != null;
        }

        /// <summary>
        /// 栅格像元值是浮点型
        /// </summary>
        /// <param name="raster"></param>
        /// <returns></returns>
        private static bool IsFloatPixel(IRaster raster)
        {
            //判断栅格像元值是否是浮点型
            IRasterProps pProp = raster as IRasterProps;
            if (pProp == null)
            {
                return true;
            }
            return pProp.PixelType == rstPixelType.PT_FLOAT || pProp.PixelType == rstPixelType.PT_DOUBLE;
        }

        /// <summary>
        /// 获取栅格波段中的属性表
        /// </summary>
        /// <param name="raster"></param>
        /// <returns></returns>
        private static ITable GetRasterBandAttributeTable(IRaster raster)
        {
            IRasterBandCollection pRasterbandCollection = (IRasterBandCollection)raster;
            IRasterBand rasterBand = pRasterbandCollection.Item(0);
            return rasterBand.AttributeTable;
        }

        /// <summary>
        /// 建立默认属性表
        /// </summary>
        /// <param name="rasterLayer"></param>
        private static void BuildRasterAttributeTable(IRasterLayer rasterLayer)
        {
            IRasterDataset rasterDataset = RasterDataInfoClass.GetRasterDataset(rasterLayer.FilePath);
            BuildRasterAttributeTable(rasterDataset);
        }

    }
}
