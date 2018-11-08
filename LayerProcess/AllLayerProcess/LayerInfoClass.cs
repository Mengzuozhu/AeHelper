using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ESRI.ArcGIS.Carto;
using LayerProcess.FeatureProcess;
using LayerProcess.RasterProcess;

namespace LayerProcess.AllLayerProcess
{
    /// <summary>
    /// 图层信息
    /// </summary>
    public class LayerInfoClass
    {
        /// <summary>
        /// 根据图层名和文件路径集合，获取文件路径
        /// </summary>
        /// <param name="selectFile">图层名或文件路径</param>
        /// <param name="filePaths">文件路径集合</param>
        /// <returns></returns>
        public static string GetFilePathFromList(string selectFile, List<string> filePaths)
        {
            if (String.IsNullOrEmpty(selectFile))
            {
                return null;
            }
            //判断下拉框当前内容是文件名还是文件路径
            return selectFile.Contains(@"\")
                ? selectFile
                : filePaths.FirstOrDefault(filePath => selectFile == Path.GetFileName(filePath));
        }

        /// <summary>
        /// 根据图层名获取矢量或栅格文件路径
        /// </summary>
        /// <param name="selectFile">图层名</param>
        /// <param name="featureFile">矢量文件集合</param>
        /// <param name="rasterFile">栅格文件集合</param>
        /// <returns>文件路径</returns>
        public static string GetFeatureOrRasterPath(string selectFile, List<string> featureFile = null,
            List<string> rasterFile = null)
        {
            if (String.IsNullOrEmpty(selectFile))
            {
                return null;
            }
            //判断下拉框当前内容是文件名还是文件路径
            if (selectFile.Contains(@"\"))
            {
                return selectFile;
            }
            //矢量图层
            if (featureFile != null)
            {
                foreach (string file in featureFile.Where(file => selectFile == Path.GetFileName(file)))
                {
                    return file;
                }
            }
            if (rasterFile != null)
            {
                return rasterFile.FirstOrDefault(file => selectFile == Path.GetFileName(file));
            }
            return null;
        }

        /// <summary>
        /// 根据文件获取图层
        /// </summary>
        /// <param name="filePath">文件</param>
        /// <returns></returns>
        public static ILayer GetLayerByFile(string filePath)
        {
            if (Path.GetExtension(filePath) == ".shp")
            {
                return FeatureInfoClass.GetFeatureLayer(filePath);
            }
            return RasterDataInfoClass.GetRasterLayer(filePath);
        }

        /// <summary>
        /// 在地图中根据图层名称获得图层。
        /// </summary>
        /// <param name="map">地图</param>
        /// <param name="layerName">图层名称</param>
        /// <returns>ILayer接口的图层对象</returns>
        public static ILayer GetLayerByName(IMap map, string layerName)
        {
            //对地图中的图层进行遍历
            for (int i = 0; i < map.LayerCount; i++)
            {
                //如果该图层为图层组类型，则分别对所包含的每个图层进行操作
                if (map.Layer[i] is GroupLayer)
                {
                    //使用ICompositeLayer接口进行遍历操作
                    ICompositeLayer compositeLayer = map.Layer[i] as ICompositeLayer;
                    if (compositeLayer == null)
                    {
                        return null;
                    }
                    for (int j = 0; j < compositeLayer.Count; j++)
                    {
                        //如果图层名称为所要查询的图层名称，则返回IFeatureLayer接口的矢量图层对象
                        if (compositeLayer.Layer[j].Name == layerName)
                            return compositeLayer.Layer[j];
                    }
                }
                //如果图层不是图层组类型，则直接进行判断
                else
                {
                    if (map.Layer[i].Name == layerName)
                        return map.Layer[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 根据图层集合获得图层。
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="featureLayers">矢量图层</param>
        /// <param name="rasterLayers">栅格图层</param>
        /// <returns></returns>
        public static ILayer GetLayerFormLayerList(string layerName, List<IFeatureLayer> featureLayers = null,
            List<IRasterLayer> rasterLayers = null)
        {
            if (featureLayers != null)
            {
                //矢量图层
                foreach (IFeatureLayer feaLayer in featureLayers.Where(feaLayer => layerName == feaLayer.Name))
                {
                    return feaLayer;
                }
            }
            return rasterLayers == null ? null : rasterLayers.FirstOrDefault(rasLayer => layerName == rasLayer.Name);
            //单波段栅格图层
        }
    }
}
