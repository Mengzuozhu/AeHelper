using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace AeHelper.RightMenuCommand
{
    /// <summary>
    /// 删除图层
    /// </summary>
    public sealed class RemoveLayer : BaseCommand
    {
        private IHookHelper hookHelper;
        private readonly ILayer layer;

        /// <inheritdoc />
        public RemoveLayer(ILayer pLayer)
        {
            base.m_caption = "Remove layer";
            layer = pLayer;
        }

        /// <inheritdoc />
        public override void OnClick()
        {
            hookHelper.FocusMap.DeleteLayer(layer);
        }

        /// <inheritdoc />
        public override void OnCreate(object hook)
        {
            if (hook == null) return;
            hookHelper = new HookHelperClass {Hook = hook};
        }

    }

}
