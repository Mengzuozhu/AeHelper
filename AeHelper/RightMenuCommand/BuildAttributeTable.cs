using System.Threading.Tasks;
using System.Windows.Forms;
using AeHelper.LayerProcess.RasterProcess;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;

namespace AeHelper.RightMenuCommand
{
    /// <summary>
    /// 创建属性表
    /// </summary>
    public sealed class BuildAttributeTable : BaseCommand
    {
        readonly ILayer layer;

        /// <summary>
        /// 创建属性表
        /// </summary>
        /// <param name="pLayer"></param>
        public BuildAttributeTable(ILayer pLayer)
        {
            base.m_caption = "Build Attribute Table";
            layer = pLayer;
        }

        /// <summary>
        /// 点击
        /// </summary>
        public override void OnClick()
        {
            Task.Factory.StartNew(() =>
            {
                MessageBox.Show(RasterTableFunction.BuildRasterAttributeTable(layer) == null ? "Fail" : "Success");
            });
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="hook"></param>
        public override void OnCreate(object hook)
        {


        }

        /// <summary>
        /// 是否可用
        /// </summary>
        public override bool Enabled
        {
            get
            {
                IRasterLayer rasterLayer = layer as IRasterLayer;
                return rasterLayer != null && RasterTableFunction.IsRasterLayerHaveTable(rasterLayer.Raster);
            }

        }

    }
}
