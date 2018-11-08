using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using LayerProcess.RasterProcess;

namespace AeRightMenuCommand
{
    /// <summary>
    /// 创建属性表
    /// </summary>
    public sealed class BuildAttributeTable : BaseCommand
    {
        ILayer layer;

        /// <summary>
        /// 创建属性表
        /// </summary>
        /// <param name="pLayer"></param>
        public BuildAttributeTable(ILayer pLayer)
        {
            base.m_caption = "Build attribute table";
            layer = pLayer;
        }

        public override void OnClick()
        {
            Task.Factory.StartNew(() =>
            {
                MessageBox.Show(RasterTableFunction.BuildRasterAttributeTable(layer) == null ? "Fail" : "Success");
            });
        }

        public override void OnCreate(object hook)
        {


        }

        //判断右键菜单项是否灰色不可用
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
