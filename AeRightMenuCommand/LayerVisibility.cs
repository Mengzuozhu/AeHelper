using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace AeRightMenuCommand
{
    /// <summary>
    /// 图层可视控制
    /// </summary>
    public sealed class LayerVisibility : BaseCommand, ICommandSubType
    {

        private IHookHelper m_hookHelper = new HookHelperClass();

        private long m_subType;

        public override void OnClick()
        {
            for (int i = 0; i <= m_hookHelper.FocusMap.LayerCount - 1; i++)
            {
                if (m_subType == 1) m_hookHelper.FocusMap.Layer[i].Visible = true;

                if (m_subType == 2) m_hookHelper.FocusMap.Layer[i].Visible = false;
            }
            m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        public override void OnCreate(object hook)
        {
            m_hookHelper.Hook = hook;
        }

        public int GetCount()
        {
            return 2;
        }

        public void SetSubType(int subType)
        {
            m_subType = subType;
        }

        public override string Caption
        {
            get
            {
                return m_subType == 1 ? "Turn all layers on" : "Turn all layers off";
            }
        }

        public override bool Enabled
        {
            get
            {
                bool enabled = false;
                int i;

                if (m_subType == 1)
                {
                    for (i = 0; i <= m_hookHelper.FocusMap.LayerCount - 1; i++)
                    {
                        if (m_hookHelper.ActiveView.FocusMap.Layer[i].Visible) continue;
                        enabled = true;
                        break;
                    }
                }

                else
                {
                    for (i = 0; i <= m_hookHelper.FocusMap.LayerCount - 1; i++)
                    {
                        if (!m_hookHelper.ActiveView.FocusMap.Layer[i].Visible) continue;
                        enabled = true;
                        break;
                    }
                }
                return enabled;
            }
        }

    }

}
