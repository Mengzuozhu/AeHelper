using System.Collections.Generic;
using System.Linq;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Path = System.IO.Path;

namespace AeHelper.LayerProcess.FeatureProcess
{
    /// <summary>
    /// 矢量信息
    /// </summary>
    public class FeatureInfoClass
    {
        /// <summary>
        /// 获取要素集合
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>要素集合</returns>
        public static IFeatureClass GetFeatureClass(string filePath)
        {
            IWorkspace pWorkspace = GetShapefileWorkspace(filePath);
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)pWorkspace;
            if (featureWorkspace == null) return null;
            string fileName = Path.GetFileName(filePath);
            IFeatureClass pFeatureClass = featureWorkspace.OpenFeatureClass(fileName);
            return pFeatureClass;
        }

        /// <summary>
        /// 根据文件路径获取要素图层
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>要素图层</returns>
        public static IFeatureLayer GetFeatureLayer(string filePath)
        {
            IFeatureClass featureClass = GetFeatureClass(filePath);
            return GetFeatureLayer(featureClass);
        }

        /// <summary>
        /// 根据要素集合获取要素图层
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static IFeatureLayer GetFeatureLayer(IFeatureClass featureClass)
        {
            return new FeatureLayer
            {
                FeatureClass = featureClass,
                Name = featureClass.AliasName
            };
        }

        /// <summary>
        /// 通过数据集获取矢量图层
        /// </summary>
        /// <param name="dataset">数据集</param>
        /// <returns></returns>
        public static IFeatureLayer GetFeatureLayerByDataset(IDataset dataset)
        {
            IWorkspace workspace = dataset.Workspace;
            IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)workspace;
            IFeatureClass featureClass = pFeatureWorkspace.OpenFeatureClass(dataset.Name);
            return GetFeatureLayer(featureClass);
        }

        /// <summary>
        /// 根据图层名从图层集合中获取图层
        /// </summary>
        /// <param name="layerName">图层名</param>
        /// <param name="featureLayers">图层集合</param>
        /// <returns></returns>
        public static IFeatureLayer GetFeatureLayerByName(string layerName, List<IFeatureLayer> featureLayers)
        {
            return featureLayers.FirstOrDefault(layer => layer.Name == layerName);
        }

        /// <summary>
        /// 获取矢量工作空间
        /// </summary>
        /// <param name="filePath">矢量文件路径</param>
        /// <returns></returns>
        public static IWorkspace GetShapefileWorkspace(string filePath)
        {
            string fileDir = Path.GetDirectoryName(filePath);
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass();
            return workspaceFactory.OpenFromFile(fileDir, 0);
        }

        /// <summary>
        /// 获得图层的Shape类型
        /// </summary>
        /// <param name="pLayer">图层</param>
        /// <returns>Shape类型</returns>
        public static string GetShapeType(ILayer pLayer)
        {
            //判断矢量图层是否为空
            IFeatureLayer featureLayer = pLayer as IFeatureLayer;
            if (featureLayer == null) return "未识别";
            switch (featureLayer.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    return "Point";
                case esriGeometryType.esriGeometryPolyline:
                    return "Polyline";
                case esriGeometryType.esriGeometryPolygon:
                    return "Polygon";
                case esriGeometryType.esriGeometryMultipoint:
                    return "Multipoint";
                case esriGeometryType.esriGeometryNull:
                    return "Null";
                case esriGeometryType.esriGeometryLine:
                    return "Line";
                case esriGeometryType.esriGeometryCircularArc:
                    return "CircularArc";
                case esriGeometryType.esriGeometryEllipticArc:
                    return "EllipticArc";
                case esriGeometryType.esriGeometryBezier3Curve:
                    return "Bezier3Curve";
                case esriGeometryType.esriGeometryPath:
                    return "Path";
                case esriGeometryType.esriGeometryRing:
                    return "Ring";
                case esriGeometryType.esriGeometryEnvelope:
                    return "Envelope";
                case esriGeometryType.esriGeometryAny:
                    return "Any";
                case esriGeometryType.esriGeometryBag:
                    return "Bag";
                case esriGeometryType.esriGeometryMultiPatch:
                    return "MultiPatch";
                case esriGeometryType.esriGeometryTriangleStrip:
                    return "TriangleStrip";
                case esriGeometryType.esriGeometryTriangleFan:
                    return "TriangleFan";
                case esriGeometryType.esriGeometryRay:
                    return "Ray";
                case esriGeometryType.esriGeometrySphere:
                    return "Sphere";
                case esriGeometryType.esriGeometryTriangles:
                    return "Triangles";
                default:
                    return "未识别";
            }
        }

        /// <summary>
        /// 获取矢量图层文件路径
        /// </summary>
        /// <param name="inFeatureLayer">输入矢量图层</param>
        /// <returns>文件路径</returns>
        public static string GetFeatureFilePath(IFeatureLayer inFeatureLayer)
        {
            string directory = null;
            string fileName = null;
            IDataLayer2 pDataLayer = (IDataLayer2)inFeatureLayer;
            IDatasetName datasetName = pDataLayer.DataSourceName as IDatasetName;
            if (datasetName != null)
            {
                IDatasetName pDatasetName = datasetName;
                IWorkspaceName pWorkspaceName = pDatasetName.WorkspaceName;
                directory = pWorkspaceName.PathName;  //文件目录
                fileName = inFeatureLayer.Name;  //文件名
            }
            if (fileName == null || directory == null) return null;
            return directory + @"\" + fileName + ".shp";
        }

        /// <summary>
        /// 是否是要素图层路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsFeatureFile(string filePath)
        {
            return Path.GetExtension(filePath) == ".shp";
        }

        /// <summary>
        /// 不是点图层
        /// </summary>
        /// <param name="feaLatLayer">图层</param>
        /// <returns></returns>
        public static bool IsNotPointLayer(IFeatureLayer feaLatLayer)
        {
            return feaLatLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPoint
                   && feaLatLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryMultipoint;
        }

    }
}
