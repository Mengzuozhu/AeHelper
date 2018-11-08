using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using stdole;

namespace AeDisplay.Mapping
{
    /// <summary>
    /// 制图
    /// </summary>
    public class MappingClass
    {
        /// <summary>
        /// 制图类型
        /// </summary>
        public enum EnumMapSurroundType
        {
            /// <summary>
            /// 空
            /// </summary>
            None = 0,

            /// <summary>
            /// 指南针
            /// </summary>
            NorthArrow = 1,

            /// <summary>
            /// 比例尺
            /// </summary>
            ScaleBar = 2,

            /// <summary>
            /// 图例
            /// </summary>
            Legend = 3,

            /// <summary>
            /// 标题
            /// </summary>
            Title = 4,

            /// <summary>
            /// 渲染
            /// </summary>
            Renderer = 5
        }

        /// <summary>
        /// 删除指定名称的元素
        /// </summary>
        /// <param name="layoutControl">布局视图</param>
        /// <param name="elementName">元素名称</param>
        public static void DeleteElementByName(AxPageLayoutControl layoutControl, string elementName)
        {
            IGraphicsContainer pGraphicsContainer = layoutControl.PageLayout as IGraphicsContainer;
            if (pGraphicsContainer == null) return;
            IElement pElement = layoutControl.FindElementByName(elementName);
            if (pElement != null)
            {
                pGraphicsContainer.DeleteElement(pElement);  //删除指定元素
            }
        }

        /// <summary>
        /// 添加指定元素，并设置元素名称
        /// </summary>
        /// <param name="pGraphicsContainer">图形容器</param>
        /// <param name="pElement">指定元素</param>
        /// <param name="elementName">元素名称</param>
        public static void AddElementWithName(IGraphicsContainer pGraphicsContainer, IElement pElement, string elementName)
        {
            IElementProperties pElePro = pElement as IElementProperties;
            if (pElePro != null) pElePro.Name = elementName;
            pGraphicsContainer.AddElement(pElement, 0);
        }

        /// <summary>
        /// 获取字体格式
        /// </summary>
        /// <param name="fontType">类型</param>
        /// <param name="fontSize">大小</param>
        /// <param name="isBold">是否加粗</param>
        /// <returns>字体格式</returns>
        public static StdFont GetFont(string fontType = "宋体", decimal fontSize = 18, bool isBold = false)
        {
            StdFont myFont = new StdFontClass();
            myFont.Name = fontType;
            myFont.Size = fontSize;
            myFont.Bold = isBold;
            return myFont;
        }

    }
}
