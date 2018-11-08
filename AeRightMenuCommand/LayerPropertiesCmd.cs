using System;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace AeRightMenuCommand
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    [Guid("a486a30d-9b8d-4a64-bc39-a3ae5e786543")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("DecisionSystem.Class.PublicClass.LayerPropertiesCmd")]
    public sealed class LayerPropertiesCmd : BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper;
        IActiveView m_activeView;
        ILayer currentLayer;
        ITOCControl2 tocControl;

        public LayerPropertiesCmd(ITOCControl2 mTocControl, ILayer pLayer)
        {
            base.m_category = "ControlsApplication";
            base.m_caption = "Layer properties";
            base.m_message = "Layer properties";
            base.m_toolTip = "Layer properties";
            base.m_name = "LayerPropertiesCmd";
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
            if (hook == null)
                return;
            m_hookHelper = new HookHelperClass();
            m_hookHelper.Hook = hook;
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
            ESRI.ArcGIS.Framework.IPropertyPage pSrcPage = new ESRI.ArcGIS.CartoUI.FeatureLayerSourcePropertyPageClass();
            pComPropSheet.AddPage(pSrcPage);

            // Selection...
            ESRI.ArcGIS.Framework.IPropertyPage pSelectPage = new ESRI.ArcGIS.CartoUI.FeatureLayerSelectionPropertyPageClass();
            pComPropSheet.AddPage(pSelectPage);

            // Display....
            ESRI.ArcGIS.Framework.IPropertyPage pDispPage = new ESRI.ArcGIS.CartoUI.FeatureLayerDisplayPropertyPageClass();
            pComPropSheet.AddPage(pDispPage);

            // Symbology....
            ESRI.ArcGIS.Framework.IPropertyPage pDrawPage = new ESRI.ArcGIS.CartoUI.LayerDrawingPropertyPageClass();
            pComPropSheet.AddPage(pDrawPage);

            // Fields... 
            ESRI.ArcGIS.Framework.IPropertyPage pFieldsPage = new ESRI.ArcGIS.CartoUI.LayerFieldsPropertyPageClass();
            pComPropSheet.AddPage(pFieldsPage);

            // Definition Query... 
            ESRI.ArcGIS.Framework.IPropertyPage pQueryPage = new ESRI.ArcGIS.CartoUI.LayerDefinitionQueryPropertyPageClass();
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
            tocControl.Update();  //更新目录树
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
            tocControl.Update();  //更新目录树
        }

    }
}