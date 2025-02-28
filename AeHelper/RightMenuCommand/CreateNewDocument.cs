﻿using System.Windows.Forms;
using AeHelper.AeControl.MapAndPageLayoutSynch;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace AeHelper.RightMenuCommand
{
    /// <summary>
    /// 创建新文档
    /// </summary>
    public class CreateNewDocument : BaseCommand
    {
        //private IHookHelper m_hookHelper;
        private ControlsSynchronizer m_controlsSynchronizer;

        /// <summary>
        /// 创建新文档
        /// </summary>
        /// <param name="controlsSynchronizer"></param>
        public CreateNewDocument(ControlsSynchronizer controlsSynchronizer)
        {
            //update the base properties
            base.m_category = ".NET Samples";
            base.m_caption = "NewDocument";
            base.m_message = "Create a new map";
            base.m_toolTip = "Create a new map";
            base.m_name = "DotNetTemplate_NewDocumentCommand";

            m_controlsSynchronizer = controlsSynchronizer;
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {

        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            //check to see if there is an active edit session and whether edits have been made
            IEngineEditor engineEditor = new EngineEditorClass();
            if ((engineEditor.EditState == esriEngineEditState.esriEngineStateEditing) && engineEditor.HasEdits())
            {
                DialogResult result = MessageBox.Show(@"Is save the edit?", @"Save the edit",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (result)
                {

                    case DialogResult.Cancel:
                        return;

                    case DialogResult.No:
                        engineEditor.StopEditing(false);
                        break;

                    case DialogResult.Yes:
                        engineEditor.StopEditing(true);
                        break;
                }
            }
            //allow the user to save the current document
            DialogResult res = MessageBox.Show(@"Is save the current document？", @"Save document", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                //launch the save command
                ICommand command = new ControlsSaveAsDocCommandClass();
                command.OnCreate(m_controlsSynchronizer.PageLayoutControl.Object);
                command.OnClick();
            }

            //create a new Map
            IMap map = new MapClass();
            map.Name = "Map";

            //assign the new map to the MapControl
            //m_controlsSynchronizer.MapControl.DocumentFilename = string.Empty;
            m_controlsSynchronizer.ReplaceMap(map);
        }

        #endregion
    }
}
