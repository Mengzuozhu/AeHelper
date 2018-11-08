using System;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LayerProcess.AllLayerProcess;

namespace LayerProcess
{
    /// <summary>
    /// 空间参考
    /// </summary>
    public class SpatialReferenceClass
    {
        /// <summary>
        /// 将经纬度点转换为平面坐标
        /// </summary>
        /// <param name="spatialReference">平面坐标参考系</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns>平面坐标点</returns>
        public static IPoint GetProjectCoord(ISpatialReference spatialReference, double x, double y)
        {
            return SetSpatialReference(GetGeographicCoordinateSystem(), spatialReference, x, y);
        }

        /// <summary>
        /// 与对照坐标不同，则将经纬度点转换为平面坐标
        /// </summary>
        /// <param name="spatialReference">平面坐标参考系</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="compare">对照坐标</param>
        /// <returns>平面坐标点</returns>
        public static IPoint GetPointCompareProject(ISpatialReference spatialReference, double x, double y,
            double compare)
        {
            IGeometry geo = GetPoint(x, y);
            //如果参考系与对照参考系相同，就不需要转换，直接返回
            if ((x > 360 && compare > 360) || (x < 360 && compare < 360))
            {
                return geo as IPoint;
            }
            return SetSpatialReference(GetGeographicCoordinateSystem(), spatialReference, x, y);
        }

        /// <summary>
        /// 将平面坐标转换为经纬度
        /// </summary>
        /// <param name="spatialReference">空间参考</param>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <returns></returns>
        public static IPoint GetGeoCoord(ISpatialReference spatialReference, double x, double y)
        {
            return SetSpatialReference(spatialReference, GetGeographicCoordinateSystem(), x, y);
        }

        /// <summary>
        /// 设置空间参考
        /// </summary>
        /// <param name="spatialReference"></param>
        /// <param name="projectSpatialReference"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static IPoint SetSpatialReference(ISpatialReference spatialReference,
            ISpatialReference projectSpatialReference, double x, double y)
        {
            IGeometry geo = GetPoint(x, y);
            geo.SpatialReference = spatialReference;
            geo.Project(projectSpatialReference);
            return geo as IPoint;
        }

        /// <summary>
        /// 获取点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static IGeometry GetPoint(double x, double y)
        {
            IPoint point = new PointClass();
            point.PutCoords(x, y);
            return point;
        }

        /// <summary>
        /// 获取地理坐标系
        /// </summary>
        /// <returns></returns>
        private static ISpatialReference GetGeographicCoordinateSystem()
        {
            ISpatialReferenceFactory pfactory = new SpatialReferenceEnvironmentClass();
            return pfactory.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_Beijing1954);
        }

        /// <summary>
        /// 根据图层范围获取WGS_UTM_Zone 投影坐标系
        /// </summary>
        /// <param name="filePath">图层文件路径</param>
        /// <returns></returns>
        public static ISpatialReference GetProjectedReference(string filePath)
        {
            ILayer layer = LayerInfoClass.GetLayerByFile(filePath);
            if (layer == null) return null;
            return IsProjectedSpatialReference(layer.AreaOfInterest)
                ? null
                : ComputeProjectedSpatialReference(layer.AreaOfInterest);
        }

        /// <summary>
        /// 获取WGS_UTM_Zone 投影坐标系
        /// </summary>
        /// <param name="raster">输入栅格</param>
        /// <returns></returns>
        public static ISpatialReference GetRasterProjectedReference(IRaster raster)
        {
            IRasterProps rasterProps = (IRasterProps)raster;
            return ComputeProjectedSpatialReference(rasterProps.Extent);
        }

        /// <summary>
        /// 计算空间参考
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        private static ISpatialReference ComputeProjectedSpatialReference(IEnvelope envelope)
        {
            if (IsProjectedSpatialReference(envelope))  //判断是否是投影坐标系，若是，则直接返回原坐标系
            {
                return envelope.SpatialReference;
            }
            double xMax = Math.Round(envelope.XMax, 3);
            double xMin = Math.Round(envelope.XMin, 3);
            double mean = (xMax + xMin) / 2;
            //esriSRProjCS_WGS1984UTM_1N 对应 pcsType:32601 中国UTM投影带计算公式：(经度+180)/6
            //因为浮点转为整型会省略小数点，所以加上32601而不是32600
            const int basedType = 32601;
            int zone = basedType + (int)(mean + 180) / 6;
            ISpatialReferenceFactory pfactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference proReference = pfactory.CreateProjectedCoordinateSystem(zone);
            return proReference;
        }

        /// <summary>
        /// 是投影坐标
        /// </summary>
        /// <param name="envelope"></param>
        /// <returns></returns>
        private static bool IsProjectedSpatialReference(IEnvelope envelope)
        {
            double xMax = Math.Round(envelope.XMax, 3);
            return xMax > 360 || xMax < -360;
        }
    }
}
