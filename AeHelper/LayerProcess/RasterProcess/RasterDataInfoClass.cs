using System;
using System.Collections.Generic;
using System.Linq;
using AeHelper.LayerProcess.AllLayerProcess;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using Path = System.IO.Path;

namespace AeHelper.LayerProcess.RasterProcess
{
    /// <summary>
    /// 栅格数据信息
    /// </summary>
    public class RasterDataInfoClass
    {
        /// <summary>
        /// 获取栅格图层
        /// </summary>
        /// <param name="filePath">图层文件路径</param>
        /// <returns></returns>
        public static IRasterLayer GetRasterLayer(string filePath)
        {
            IRasterDataset rasDataset = GetRasterDataset(filePath);
            IRasterLayer rasterLayer = new RasterLayerClass();
            rasterLayer.CreateFromDataset(rasDataset);
            return rasterLayer;
        }

        /// <summary>
        /// 通过数据集获取栅格图层
        /// </summary>
        /// <param name="dataset">数据集</param>
        /// <returns></returns>
        public static IRasterLayer GetRasterLayerByDataset(IDataset dataset)
        {
            IRasterWorkspaceEx pRasterWorkspace = (IRasterWorkspaceEx)dataset.Workspace;
            IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(dataset.Name);
            //影像金字塔判断与创建
            IRasterPyramid3 pRasPyrmid = pRasterDataset as IRasterPyramid3;
            if (pRasPyrmid != null)
            {
                if (!pRasPyrmid.Present)
                {
                    pRasPyrmid.Create(); //创建金字塔
                }
            }
            IRasterLayer pRasterLayer = new RasterLayerClass();
            pRasterLayer.CreateFromDataset(pRasterDataset);
            return pRasterLayer;
        }

        /// <summary>
        /// 根据图层名称和图层列表获取图层
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="rasterLayers">图层列表</param>
        /// <returns></returns>
        public static IRasterLayer GetRasterLayerByNameAndList(string layerName, List<IRasterLayer> rasterLayers)
        {
            return rasterLayers.FirstOrDefault(layer => layer.Name == layerName);
        }

        /// <summary>
        /// 获取栅格工作空间
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static IWorkspace GetRasterWorkspace(string filePath)
        {
            string outDir = Path.GetDirectoryName(filePath);
            IWorkspaceFactory workFactory = new RasterWorkspaceFactoryClass();
            return workFactory.OpenFromFile(outDir, 0);
        }

        /// <summary>
        /// 获取栅格数据集
        /// </summary>
        /// <param name="filePath">栅格文件路径</param>
        /// <returns></returns>
        public static IRasterDataset GetRasterDataset(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            IRasterWorkspace pRasterWorkspace = (IRasterWorkspace)GetRasterWorkspace(filePath);
            IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(fileName);
            return pRasterDataset;
        }

        /// <summary>
        /// 根据文件获取几何数据集
        /// </summary>
        /// <param name="inFile">输入文件</param>
        /// <returns></returns>
        public static IGeoDataset GetGeoDatasetByFile(string inFile)
        {
            IRasterLayer rasterLayer = GetRasterLayer(inFile);
            return rasterLayer as IGeoDataset;
        }

        /// <summary>
        /// 根据栅格和字段获取几何数据集
        /// </summary>
        /// <param name="raster">栅格</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public static IGeoDataset GetGeoDatasetByRaster(IRaster raster, string field)
        {
            IRasterDescriptor rasDescriptor = new RasterDescriptorClass();
            rasDescriptor.Create(raster, null, field);
            IGeoDataset geoDataset = (IGeoDataset)rasDescriptor;
            return geoDataset;
        }

        /// <summary>
        /// 获取栅格
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static IRaster GetRaster(string filePath)
        {
            IRasterLayer rasterLayer = GetRasterLayer(filePath);
            return rasterLayer.Raster;
        }

        /// <summary>
        /// 获取像元大小最小(分辨率最高)的栅格
        /// </summary>
        /// <param name="inRasters">输入栅格集合</param>
        /// <returns></returns>
        public static IRaster GetMinCellSizeRaster(IList<IRaster> inRasters)
        {
            if (inRasters.Count == 0) return null;
            IRasterProps rasterProps = (IRasterProps)inRasters[0];
            rasterProps.SpatialReference = SpatialReferenceClass.GetRasterProjectedReference(inRasters[0]);  //转换为投影坐标系
            double minSize = (rasterProps.MeanCellSize().X + rasterProps.MeanCellSize().Y) / 2.0; //栅格的平均像元大小
            int minIndex = 0;
            for (int i = 1; i < inRasters.Count; i++)
            {
                rasterProps = (IRasterProps)inRasters[i];
                rasterProps.SpatialReference = SpatialReferenceClass.GetRasterProjectedReference(inRasters[i]);
                double cellSize = (rasterProps.MeanCellSize().X + rasterProps.MeanCellSize().Y) / 2.0;
                if (cellSize < minSize)
                {
                    minIndex = i;
                }
            }
            return inRasters[minIndex];
        }

        /// <summary>
        /// 获取像元面积（单位：平方米，保留5位小数）
        /// </summary>
        /// <param name="raster">栅格</param>
        /// <returns></returns>
        public static double GetCellArea(IRaster raster)
        {
            IRasterProps rasterProps = (IRasterProps)raster;
            rasterProps.SpatialReference = SpatialReferenceClass.GetRasterProjectedReference(raster);
            double sizeX = rasterProps.MeanCellSize().X;
            double sizeY = rasterProps.MeanCellSize().Y;
            return Math.Round(sizeX * sizeY, 5);
        }

        /// <summary>
        /// 是否存在金字塔
        /// </summary>
        /// <param name="rasterFilePath">栅格文件路径</param>
        /// <returns></returns>
        public static bool IsExistRasterPyramid(string rasterFilePath)
        {
            IRasterDataset pRasterDataset = GetRasterDataset(rasterFilePath);
            //影像金字塔判断与创建
            IRasterPyramid3 pRasPyrmid = pRasterDataset as IRasterPyramid3;
            if (pRasPyrmid == null) return false;
            return pRasPyrmid.Present;
        }

        /// <summary>
        /// 创建金字塔
        /// </summary>
        /// <param name="rasterFilePath">栅格文件路径</param>
        public static void CreatePyramid(string rasterFilePath)
        {
            IRasterDataset pRasterDataset = GetRasterDataset(rasterFilePath);
            //影像金字塔判断与创建
            IRasterPyramid3 pRasPyramid = pRasterDataset as IRasterPyramid3;
            if (pRasPyramid == null) return;
            if (!pRasPyramid.Present)
            {
                pRasPyramid.Create(); //创建金字塔
            }
        }

    }
}
