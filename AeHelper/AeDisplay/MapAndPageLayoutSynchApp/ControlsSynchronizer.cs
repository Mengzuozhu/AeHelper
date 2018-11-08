// Copyright 2010 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions.
// 

using System;
using System.Collections;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace AeHelper.AeDisplay.MapAndPageLayoutSynchApp
{
    /// <summary>
    /// This class is used to synchronize a given PageLayoutControl and a MapControl.
    /// When initialized, the user must pass the reference of these control to the class, bind
    /// the control together by calling 'BindControls' which in turn sets a joined Map referenced
    /// by both control; and set all the buddy controls joined between these two controls.
    /// When alternating between the MapControl and PageLayoutControl, you should activate the visible control 
    /// and deactivate the other by calling ActivateXXX.
    /// This class is limited to a situation where the controls are not simultaneously visible. 
    /// </summary>
    public class ControlsSynchronizer
    {
        #region class members

        private IMapControl3 mapControl;
        private IPageLayoutControl3 pageLayoutControl;
        private ITool mapActiveTool;
        private ITool pageLayoutActiveTool;

        private bool isMapCtrlActive = true;

        //Toolbars and TOC 控件集合
        private readonly ArrayList frameworkControls;

        #endregion

        #region constructor

        /// <summary>
        /// default constructor
        /// </summary>
        public ControlsSynchronizer()
        {
            //initialize the underlying ArrayList
            frameworkControls = new ArrayList();
        }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="mapControl"></param>
        /// <param name="pageLayoutControl"></param>
        public ControlsSynchronizer(IMapControl3 mapControl, IPageLayoutControl3 pageLayoutControl)
            : this()
        {
            //assign the class members
            this.mapControl = mapControl;
            this.pageLayoutControl = pageLayoutControl;
        }
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the MapControl
        /// </summary>
        public IMapControl3 MapControl
        {
            get { return mapControl; }
            set { mapControl = value; }
        }

        /// <summary>
        /// Gets or sets the PageLayoutControl
        /// </summary>
        public IPageLayoutControl3 PageLayoutControl
        {
            get { return pageLayoutControl; }
            set { pageLayoutControl = value; }
        }

        /// <summary>
        /// Get an indication of the type of the currently active view
        /// </summary>
        public string ActiveViewType
        {
            get
            {
                return isMapCtrlActive ? "MapControl" : "PageLayoutControl";
            }
        }

        /// <summary>
        /// get the active control
        /// </summary>
        public object ActiveControl
        {
            get
            {
                if (mapControl == null || pageLayoutControl == null)
                {
                    throw new Exception("ControlsSynchronizer::ActiveControl:\r\nEither MapControl or PageLayoutControl are not initialized!");
                }
                return isMapCtrlActive ? mapControl.Object : pageLayoutControl.Object;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Activate the MapControl and deactivate the PagleLayoutControl
        /// </summary>
        public void ActivateMap()
        {
            try
            {
                if (pageLayoutControl == null || mapControl == null)
                {
                    throw new Exception("ControlsSynchronizer::ActivateMap:\r\nEither MapControl or PageLayoutControl are not initialized!");
                }
                //cache the current tool of the PageLayoutControl
                if (pageLayoutControl.CurrentTool != null) pageLayoutActiveTool = pageLayoutControl.CurrentTool;

                //deactivate the PagleLayout
                pageLayoutControl.ActiveView.Deactivate();

                //activate the MapControl
                mapControl.ActiveView.Activate(mapControl.hWnd);

                //assign the last active tool that has been used on the MapControl back as the active tool
                if (mapActiveTool != null) mapControl.CurrentTool = mapActiveTool;

                isMapCtrlActive = true;

                //on each of the framework controls, set the Buddy control to the MapControl
                this.SetBuddies(mapControl.Object);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ControlsSynchronizer::ActivateMap:\r\n{0}", ex.Message));
            }
        }

        /// <summary>
        /// Activate the PagleLayoutControl and deactivate the MapCotrol
        /// </summary>
        public void ActivatePageLayout()
        {
            try
            {
                if (pageLayoutControl == null || mapControl == null)
                {
                    throw new Exception("ControlsSynchronizer::ActivatePageLayout:\r\nEither MapControl or PageLayoutControl are not initialized!");
                }
                //cache the current tool of the MapControl
                if (mapControl.CurrentTool != null) mapActiveTool = mapControl.CurrentTool;

                //deactivate the MapControl
                mapControl.ActiveView.Deactivate();

                //activate the PageLayoutControl
                pageLayoutControl.ActiveView.Activate(pageLayoutControl.hWnd);

                //assign the last active tool that has been used on the PageLayoutControl back as the active tool
                if (pageLayoutActiveTool != null) pageLayoutControl.CurrentTool = pageLayoutActiveTool;

                isMapCtrlActive = false;

                //on each of the framework controls, set the Buddy control to the PageLayoutControl
                this.SetBuddies(pageLayoutControl.Object);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ControlsSynchronizer::ActivatePageLayout:\r\n{0}", ex.Message));
            }
        }

        /// <summary>
        /// given a new map, replaces the PageLayoutControl and the MapControl's focus map
        /// </summary>
        /// <param name="newMap"></param>
        public void ReplaceMap(IMap newMap)
        {
            if (newMap == null)
            {
                throw new Exception("ControlsSynchronizer::ReplaceMap:\r\nNew map for replacement is not initialized!");
            }
            if (pageLayoutControl == null || mapControl == null)
            {
                throw new Exception("ControlsSynchronizer::ReplaceMap:\r\nEither MapControl or PageLayoutControl are not initialized!");
            }
            //create a new instance of IMaps collection which is needed by the PageLayout
            IMaps maps = new Maps();
            //add the new map to the Maps collection
            maps.Add(newMap);

            bool bIsMapActive = isMapCtrlActive;

            //call replace map on the PageLayout in order to replace the focus map
            //we must call ActivatePageLayout, since it is the control we call 'ReplaceMaps'
            this.ActivatePageLayout();
            pageLayoutControl.PageLayout.ReplaceMaps(maps);

            //assign the new map to the MapControl
            mapControl.Map = newMap;

            //reset the active tools
            pageLayoutActiveTool = null;
            mapActiveTool = null;

            //make sure that the last active control is activated
            if (bIsMapActive)
            {
                this.ActivateMap();
                mapControl.ActiveView.Refresh();
            }
            else
            {
                this.ActivatePageLayout();
                pageLayoutControl.ActiveView.Refresh();
            }
        }

        /// <summary>
        /// 打开地图文档
        /// </summary>
        /// <param name="docFilePath">文档路径</param>
        public void OpenMapDocument(string docFilePath)
        {
            if (string.IsNullOrEmpty(docFilePath)) return;
            IMapDocument mapDoc = new MapDocumentClass();
            if (mapDoc.IsPresent[docFilePath] && !mapDoc.IsPasswordProtected[docFilePath])
            {
                mapDoc.Open(docFilePath, string.Empty);
                // set the first map as the active view
                IMap map = mapDoc.Map[0];
                //设置地图为激活视图
                mapDoc.SetActiveView((IActiveView)map);
//                PageLayoutControl.PageLayout = mapDoc.PageLayout;
                ReplaceMap(map);
                mapDoc.Close();
            }
            else
            {
                MessageBox.Show(docFilePath + "is Invalid map!", "Information");
            }
        }

        /// <summary>
        /// bind the MapControl and PageLayoutControl together by assigning a new joint focus map
        /// </summary>
        /// <param name="inMapControl"></param>
        /// <param name="inPageLayoutControl"></param>
        /// <param name="activateMapFirst">true if the MapControl supposed to be activated first</param>
        public void BindControls(IMapControl3 inMapControl, IPageLayoutControl3 inPageLayoutControl, bool activateMapFirst)
        {
            if (inMapControl == null || inPageLayoutControl == null)
            {
                throw new Exception("ControlsSynchronizer::BindControls:\r\nEither MapControl or PageLayoutControl are not initialized!");
            }
            mapControl = MapControl;
            pageLayoutControl = inPageLayoutControl;

            this.BindControls(activateMapFirst);
        }

        /// <summary>
        /// bind the MapControl and PageLayoutControl together by assigning a new joint focus map 
        /// </summary>
        /// <param name="activateMapFirst">true if the MapControl supposed to be activated first</param>
        public void BindControls(bool activateMapFirst)
        {
            if (pageLayoutControl == null || mapControl == null)
            {
                throw new Exception("ControlsSynchronizer::BindControls:\r\nEither MapControl or PageLayoutControl are not initialized!");
            }
            //create a new instance of IMap
            IMap newMap = new MapClass();
            newMap.Name = "Map";

            //create a new instance of IMaps collection which is needed by the PageLayout
            IMaps maps = new Maps();
            //add the new Map instance to the Maps collection
            maps.Add(newMap);

            //call replace map on the PageLayout in order to replace the focus map
            pageLayoutControl.PageLayout.ReplaceMaps(maps);
            //assign the new map to the MapControl
            mapControl.Map = newMap;

            //reset the active tools
            pageLayoutActiveTool = null;
            mapActiveTool = null;

            //make sure that the last active control is activated
            if (activateMapFirst)
                this.ActivateMap();
            else
                this.ActivatePageLayout();
        }

        /// <summary>
        ///by passing the application's toolbars and TOC to the synchronization class, it saves you the
        ///management of the buddy control each time the active control changes. This method ads the framework
        ///control to an array; once the active control changes, the class iterates through the array and 
        ///calls SetBuddyControl on each of the stored framework control.
        /// </summary>
        /// <param name="control"></param>
        public void AddFrameworkControl(object control)
        {
            if (control == null)
            {
                throw new Exception("ControlsSynchronizer::AddFrameworkControl:\r\nAdded control is not initialized!");
            }
            frameworkControls.Add(control);
        }

        /// <summary>
        /// Remove a framework control from the managed list of controls
        /// </summary>
        /// <param name="control"></param>
        public void RemoveFrameworkControl(object control)
        {
            if (control == null)
            {
                throw new Exception("ControlsSynchronizer::RemoveFrameworkControl:\r\nControl to be removed is not initialized!");
            }
            frameworkControls.Remove(control);
        }

        /// <summary>
        /// Remove a framework control from the managed list of controls by specifying its index in the list
        /// </summary>
        /// <param name="index"></param>
        public void RemoveFrameworkControlAt(int index)
        {
            if (frameworkControls.Count < index)
            {
                throw new Exception("ControlsSynchronizer::RemoveFrameworkControlAt:\r\nIndex is out of range!");
            }
            frameworkControls.RemoveAt(index);
        }

        /// <summary>
        /// when the active control changes, the class iterates through the array of the framework controls
        ///  and calls SetBuddyControl on each of the controls.
        /// </summary>
        /// <param name="buddy">the active control</param>
        private void SetBuddies(object buddy)
        {
            try
            {
                if (buddy == null)
                {
                    throw new Exception("ControlsSynchronizer::SetBuddies:\r\nTarget Buddy Control is not initialized!");
                }
                foreach (object obj in frameworkControls)
                {
                    IToolbarControl toolbarControl = obj as IToolbarControl;
                    if (toolbarControl != null)
                    {
                        toolbarControl.SetBuddyControl(buddy);
                    }
                    else if (obj is ITOCControl)
                    {
                        ((ITOCControl)obj).SetBuddyControl(buddy);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("ControlsSynchronizer::SetBuddies:\r\n{0}", ex.Message));
            }
        }

        #endregion
    }
}