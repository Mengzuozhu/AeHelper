using System.Collections.Generic;
using AeHelper.LayerProcess.FeatureProcess;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SpatialAnalystTools;

namespace AeHelper.LayerProcess.GeoProcess
{
    /// <summary>
    /// 几何处理工具
    /// </summary>
    public class GeoTools
    {
        /// <summary>
        /// 擦除图层
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="eraseFile"></param>
        /// <param name="outFile"></param>
        public static void Erase(string inFile, string eraseFile, string outFile)
        {
            Erase pErase = new Erase
            {
                in_features = inFile,
                erase_features = eraseFile,
                out_feature_class = outFile
            };
            GeoprocessorHelper.GpExecute(pErase);
        }

        /// <summary>
        /// 融合
        /// </summary>
        public static void Merge(List<string> inFiles, string outFile)
        {
            Merge pIntersect = new Merge
            {
                inputs = GeoprocessorHelper.GetGpInFiles(inFiles),
                output = outFile
            };
            GeoprocessorHelper.GpExecute(pIntersect);
        }

        /// <summary>
        /// 栅格掩膜提取
        /// </summary>
        /// <param name="inRasterFile"></param>
        /// <param name="inBorderFile"></param>
        /// <param name="outFile"></param>
        public static void RasterExtractByMask(string inRasterFile, string inBorderFile, string outFile)
        {
            ExtractByMask pExtractByMask = new ExtractByMask
            {
                in_raster = inRasterFile,
                in_mask_data = inBorderFile,
                out_raster = outFile
            };
            GeoprocessorHelper.GpExecute(pExtractByMask);
        }

        /// <summary>
        /// 裁剪
        /// </summary>
        /// <param name="inVectorFile"></param>
        /// <param name="borderFile"></param>
        /// <param name="outFile"></param>
        public static void Clip(string inVectorFile, string borderFile, string outFile)
        {
            ESRI.ArcGIS.AnalysisTools.Clip pClip = new ESRI.ArcGIS.AnalysisTools.Clip
            {
                in_features = inVectorFile,
                clip_features = borderFile,
                out_feature_class = outFile
            };
            GeoprocessorHelper.GpExecute(pClip);
        }

        /// <summary>
        /// 相交
        /// </summary>
        /// <param name="inFiles"></param>
        /// <param name="outFile"></param>
        public static void Intersect(List<string> inFiles, string outFile)
        {
            Intersect intersect = new Intersect
            {
                in_features = GeoprocessorHelper.GetGpInFiles(inFiles),
                out_feature_class = outFile
            };
            GeoprocessorHelper.GpExecute(intersect);
        }

        /// <summary>
        /// 栅格重采样
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outFile"></param>
        /// <param name="newCellSize"></param>
        /// <param name="resampleMethod"></param>
        public static void RasterResample(string inFile, string outFile, double newCellSize, string resampleMethod)
        {
            Resample resample = new Resample()
            {
                in_raster = inFile,
                out_raster = outFile,
                cell_size = newCellSize,
                resampling_type = resampleMethod,
            };
            GeoprocessorHelper.GpExecute(resample);
        }

        /// <summary>
        /// 转换图层坐标系
        /// </summary>
        /// <param name="inFilePath">图层</param>
        /// <param name="spatialReference">坐标系</param>
        /// <param name="outFile">输出文件</param>
        public static void ChangeLayerCoordinateSystem(string inFilePath, ISpatialReference spatialReference,
            string outFile)
        {
            if (spatialReference == null)
            {
                return;
            }

            if (FeatureInfoClass.IsFeatureFile(inFilePath))
            {
                ChangeFeatureCoordinateSystem(inFilePath, spatialReference, outFile);
            }
            else
            {
                ChangeRasterCoordinateSystem(inFilePath, spatialReference, outFile);
            }
        }

        /// <summary>
        /// 转换矢量图层坐标系
        /// </summary>
        /// <param name="inFilePath">矢量图层</param>
        /// <param name="spatialReference">坐标系</param>
        /// <param name="outFile">输出文件</param>
        public static void ChangeFeatureCoordinateSystem(string inFilePath, ISpatialReference spatialReference,
            string outFile)
        {
            Project featureProject = new Project
            {
                in_dataset = inFilePath,
                out_coor_system = spatialReference, //坐标系
                out_dataset = outFile
            };
            GeoprocessorHelper.GpExecute(featureProject);
        }

        /// <summary>
        /// 转换栅格图层坐标系
        /// </summary>
        /// <param name="inFilePath">矢量图层</param>
        /// <param name="spatialReference">坐标系</param>
        /// <param name="outFile">输出文件</param>
        public static void ChangeRasterCoordinateSystem(string inFilePath, ISpatialReference spatialReference,
            string outFile)
        {
            ProjectRaster projectRaster = new ProjectRaster
            {
                in_raster = inFilePath,
                out_coor_system = spatialReference,
                out_raster = outFile
            };
            GeoprocessorHelper.GpExecute(projectRaster);
        }
    }
}