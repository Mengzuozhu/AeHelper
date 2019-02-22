using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;

namespace AeHelper.RightMenuCommand
{
    /// <summary>
    /// 图层可视控制
    /// </summary>
    public sealed class LayerVisibility : BaseCommand, ICommandSubType
    {
        private readonly IHookHelper m_hookHelper = new HookHelperClass();
        private long m_subType;

        /// <inheritdoc />
        public override void OnClick()
        {
            for (int i = 0; i <= m_hookHelper.FocusMap.LayerCount - 1; i++)
            {
                if (m_subType == 1) m_hookHelper.FocusMap.Layer[i].Visible = true;

                if (m_subType == 2) m_hookHelper.FocusMap.Layer[i].Visible = false;
            }
            m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        /// <inheritdoc />
        public override void OnCreate(object hook)
        {
            m_hookHelper.Hook = hook;
        }

        /// <summary>
        /// 获取个数
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return 2;
        }

        /// <summary>
        /// 设置子类型
        /// </summary>
        /// <param name="subType"></param>
        public void SetSubType(int subType)
        {
            m_subType = subType;
        }

        /// <inheritdoc />
        public override string Caption
        {
            get
            {
                return m_subType == 1 ? "Turn All Layers On" : "Turn All Layers Off";
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
