using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace AeHelper.RightMenuCommand
{
    /// <summary>
    /// 放大至整个图层
    /// </summary>
    public sealed class ZoomToLayer : BaseCommand
    {
        private IMapControl3 mapControl;
        private readonly ILayer selectLayer;

        /// <summary>
        /// 放大至整个图层
        /// </summary>
        /// <param name="layer"></param>
        public ZoomToLayer(ILayer layer)
        {
            base.m_caption = "Zoom To Layer";
            selectLayer = layer;
        }

        /// <inheritdoc />
        public override void OnClick()
        {
            if (selectLayer == null) return;
            mapControl.Extent = selectLayer.AreaOfInterest;
        }

        /// <inheritdoc />
        public override void OnCreate(object hook)
        {
            mapControl = hook as IMapControl3;
        }

        /// <inheritdoc />
        public override bool Enabled
        {
            get
            {
                //不是地图视图，则变灰色不可用
                return mapControl != null;
            }
        }
    }

}
