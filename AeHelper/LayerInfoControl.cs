using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;

namespace AeHelper
{
    /// <summary>
    /// 控件中的图层信息
    /// </summary>
    class LayerInfoControl
    {
        /// <summary>
        /// 添加图层名和图层信息到树视图
        /// </summary>
        /// <param name="layers">图层</param>
        /// <param name="treeView">树视图</param>
        public static void AddLayerInfoToTreeView<T>(List<T> layers, TreeView treeView)
            where T : ILayer
        {
            foreach (var layer in layers)
            {
                string layerName = layer.Name; //得到图层的名称
                //新建一个树节点，将图层名称作为树节点的名称
                TreeNode treeNode = new TreeNode(layerName);
                //利用树节点的Tag属性，存储当前图层信息
                treeNode.Tag = layer;
                //将新建的树节点添加到TreeView控件中的根节点下
                treeView.Nodes.Add(treeNode);
            }
        }

        /// <summary>
        /// 添加图层名到下拉框
        /// </summary>
        /// <param name="layers">图层集合集合</param>
        /// <param name="comboBox">下拉框</param>
        public static void AddLayerNameToComboBox<T>(List<T> layers, ComboBox comboBox)
            where T : ILayer
        {
            foreach (var layer in layers)
            {
                comboBox.Items.Add(layer.Name);
            }
        }

        /// <summary>
        /// 添加图层名到复选框
        /// </summary>
        /// <param name="layers">图层集合集合</param>
        /// <param name="checkedListBox">复选框</param>
        public static void AddLayerNameToCheckedListBox<T>(List<T> layers, CheckedListBox checkedListBox)
            where T : ILayer
        {
            foreach (var layer in layers)
            {
                checkedListBox.Items.Add(layer.Name);
            }
        }

        /// <summary>
        /// 从复选框中，获取选中图层
        /// </summary>
        /// <param name="checkedListBox">复选框</param>
        /// <param name="treeView">树视图</param>
        /// <returns></returns>
        public static List<ILayer> GetCheckedListBoxSelectLayers(CheckedListBox checkedListBox, TreeView treeView)
        {
            if (checkedListBox.CheckedItems.Count == 0) return null;
            //获取选中图层名称
            List<string> listCheckedName = GetCheckedItemName(checkedListBox);
            return (from TreeNode node in treeView.Nodes
                    where listCheckedName.Contains(node.Text)
                    select node.Tag as ILayer).ToList();
        }

        /// <summary>
        /// 获取复选框选中项的名称
        /// </summary>
        /// <param name="checkedListBox">复选框</param>
        /// <returns></returns>
        public static List<string> GetCheckedItemName(CheckedListBox checkedListBox)
        {
            //获取选中项名称
            List<string> listCheckedName = checkedListBox.Items.Cast<object>().
                Where((t, i) => checkedListBox.GetItemChecked(i)).
                Select(checkedListBox.GetItemText).ToList();
            return listCheckedName;
        }

    }
}
