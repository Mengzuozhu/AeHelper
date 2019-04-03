using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace AeHelper.RightMenuCommand
{
    /// <summary>
    /// 矢量图层属性
    /// </summary>
    public sealed class LayerPropertiesCmd : BaseCommand
    {
        private IHookHelper m_hookHelper;
        private IActiveView m_activeView;
        private readonly ILayer currentLayer;
        private readonly ITOCControl2 tocControl;

        /// <inheritdoc />
        public LayerPropertiesCmd(ITOCControl2 mTocControl, ILayer pLayer, string name = "图层属性(&P)")
        {
            base.m_caption = name;
            base.m_enabled = true;

            tocControl = mTocControl;
            currentLayer = pLayer;
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null) return;

            m_hookHelper = new HookHelperClass {Hook = hook};
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            if (currentLayer == null) return;
            m_activeView = m_hookHelper.ActiveView;
            if (currentLayer is IFeatureLayer)
            {
                SetupFeaturePropertySheet(currentLayer);
            }
            else
            {
                SetupRasterPropertySheet(currentLayer);
            }
        }

        //判断右键菜单项是否灰色不可用
        //public override bool Enabled
        //{
        //    get
        //    {
        //        //bool enabled = !(currentLayer is IRasterLayer);

        //        return true;

        //    }

        //}

        #endregion

        private void SetupFeaturePropertySheet(ILayer layer)
        {
            if (layer == null) return;
            ESRI.ArcGIS.Framework.IComPropertySheet pComPropSheet;
            pComPropSheet = new ESRI.ArcGIS.Framework.ComPropertySheet();
            pComPropSheet.Title = layer.Name + " - properties";

            ESRI.ArcGIS.esriSystem.UID pPPUID = new ESRI.ArcGIS.esriSystem.UIDClass();
            pComPropSheet.AddCategoryID(pPPUID);

            // General....
            ESRI.ArcGIS.Framework.IPropertyPage pGenPage = new ESRI.ArcGIS.CartoUI.GeneralLayerPropPageClass();
            pComPropSheet.AddPage(pGenPage);

            // Source
            ESRI.ArcGIS.Framework.IPropertyPage
                pSrcPage = new ESRI.ArcGIS.CartoUI.FeatureLayerSourcePropertyPageClass();
            pComPropSheet.AddPage(pSrcPage);

            // Selection...
            ESRI.ArcGIS.Framework.IPropertyPage pSelectPage =
                new ESRI.ArcGIS.CartoUI.FeatureLayerSelectionPropertyPageClass();
            pComPropSheet.AddPage(pSelectPage);

            // Display....
            ESRI.ArcGIS.Framework.IPropertyPage pDispPage =
                new ESRI.ArcGIS.CartoUI.FeatureLayerDisplayPropertyPageClass();
            pComPropSheet.AddPage(pDispPage);

            // Symbology....
            ESRI.ArcGIS.Framework.IPropertyPage pDrawPage = new ESRI.ArcGIS.CartoUI.LayerDrawingPropertyPageClass();
            pComPropSheet.AddPage(pDrawPage);

            // Fields... 
            ESRI.ArcGIS.Framework.IPropertyPage pFieldsPage = new ESRI.ArcGIS.CartoUI.LayerFieldsPropertyPageClass();
            pComPropSheet.AddPage(pFieldsPage);

            // Definition Query... 
            ESRI.ArcGIS.Framework.IPropertyPage pQueryPage =
                new ESRI.ArcGIS.CartoUI.LayerDefinitionQueryPropertyPageClass();
            pComPropSheet.AddPage(pQueryPage);

            // Labels....
            ESRI.ArcGIS.Framework.IPropertyPage pSelPage = new ESRI.ArcGIS.CartoUI.LayerLabelsPropertyPageClass();
            pComPropSheet.AddPage(pSelPage);

            // Joins & Relates....
            ESRI.ArcGIS.Framework.IPropertyPage pJoinPage = new ESRI.ArcGIS.ArcMapUI.JoinRelatePageClass();
            pComPropSheet.AddPage(pJoinPage);

            // Setup layer link
            ESRI.ArcGIS.esriSystem.ISet pMySet = new ESRI.ArcGIS.esriSystem.SetClass();
            pMySet.Add(layer);
            pMySet.Reset();

            // make the symbology tab active
            pComPropSheet.ActivePage = 4;

            // show the property sheet
            bool bOK = pComPropSheet.EditProperties(pMySet, 0);
            m_activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, m_activeView.Extent);
            tocControl.Update(); //更新目录树
        }

        private void SetupRasterPropertySheet(ILayer layer)
        {
            if (layer == null) return;
            ESRI.ArcGIS.Framework.IComPropertySheet pComPropSheet;
            pComPropSheet = new ESRI.ArcGIS.Framework.ComPropertySheet();
            pComPropSheet.Title = layer.Name + " - 属性";

            ESRI.ArcGIS.esriSystem.UID pPPUID = new ESRI.ArcGIS.esriSystem.UIDClass();
            pComPropSheet.AddCategoryID(pPPUID);

            // General....
            ESRI.ArcGIS.Framework.IPropertyPage pGenPage = new ESRI.ArcGIS.CartoUI.GeneralLayerPropPageClass();
            pComPropSheet.AddPage(pGenPage);

            // Source
            //ESRI.ArcGIS.Framework.IPropertyPage pSrcPage = new ESRI.ArcGIS.CartoUI.FeatureLayerSourcePropertyPageClass();
            //pComPropSheet.AddPage(pSrcPage);

            // Selection...
            //ESRI.ArcGIS.Framework.IPropertyPage pSelectPage = new ESRI.ArcGIS.CartoUI.FeatureLayerSelectionPropertyPageClass();
            //pComPropSheet.AddPage(pSelectPage);

            // Display....
            //ESRI.ArcGIS.Framework.IPropertyPage pDispPage = new ESRI.ArcGIS.CartoUI.FeatureLayerDisplayPropertyPageClass();
            //pComPropSheet.AddPage(pDispPage);

            // Symbology....
            //ESRI.ArcGIS.Framework.IPropertyPage pDrawPage = new ESRI.ArcGIS.CartoUI.LayerDrawingPropertyPageClass();
            //pComPropSheet.AddPage(pDrawPage);

            //// Fields... 
            //ESRI.ArcGIS.Framework.IPropertyPage pFieldsPage = new ESRI.ArcGIS.CartoUI.LayerFieldsPropertyPageClass();
            //pComPropSheet.AddPage(pFieldsPage);

            //// Definition Query... 
            //ESRI.ArcGIS.Framework.IPropertyPage pQueryPage = new ESRI.ArcGIS.CartoUI.LayerDefinitionQueryPropertyPageClass();
            //pComPropSheet.AddPage(pQueryPage);

            //// Labels....
            //ESRI.ArcGIS.Framework.IPropertyPage pSelPage = new ESRI.ArcGIS.CartoUI.LayerLabelsPropertyPageClass();
            //pComPropSheet.AddPage(pSelPage);

            //// Joins & Relates....
            ESRI.ArcGIS.Framework.IPropertyPage pJoinPage = new ESRI.ArcGIS.ArcMapUI.JoinRelatePageClass();
            pComPropSheet.AddPage(pJoinPage);

            //ESRI.ArcGIS.Framework.IPropertyPage tePage = new ESRI.ArcGIS.CatalogUI.RasterCoordSysPageClass();
            //pComPropSheet.AddPage(tePage);
            // Setup layer link
            ESRI.ArcGIS.esriSystem.ISet pMySet = new ESRI.ArcGIS.esriSystem.SetClass();
            pMySet.Add(layer);
            pMySet.Reset();

            // make the symbology tab active
            pComPropSheet.ActivePage = 4;

            // show the property sheet
            bool bOK = pComPropSheet.EditProperties(pMySet, 0);
            m_activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, m_activeView.Extent);
            tocControl.Update(); //更新目录树
        }
    }
}