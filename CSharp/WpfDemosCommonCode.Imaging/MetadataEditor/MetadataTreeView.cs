using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

using Vintasoft.Imaging.Metadata;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A control that allows to view the metadata tree.
    /// </summary>
    public class MetadataTreeView : TreeView
    {

        #region Fields

        /// <summary>
        /// The dictionary: metadata node => tree node.
        /// </summary>
        Dictionary<MetadataNode, TreeViewItem> _metadataNodeToTreeNode =
            new Dictionary<MetadataNode, TreeViewItem>();

        /// <summary>
        /// The dictionary: tree node => metadata node.
        /// </summary>
        Dictionary<TreeViewItem, MetadataNode> _treeNodeToMetadataNode =
            new Dictionary<TreeViewItem, MetadataNode>();

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataTreeView"/> class.
        /// </summary>
        public MetadataTreeView()
        {
        }

        #endregion



        #region Properties

        MetadataNode _root;
        /// <summary>
        /// Gets or sets the root metadata node.
        /// </summary>
        public MetadataNode RootMetadataNode
        {
            get
            {
                return _root;
            }
            set
            {
                _root = value;
                FillMetadataTree(_root);

                if (Items.Count > 0)
                    SelectedNode = (TreeViewItem)Items[0];

                UpdateNodeStyle(SelectedNode, true, false);
            }
        }

        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        public TreeViewItem SelectedNode
        {
            get
            {
                return SelectedItem as TreeViewItem;
            }
            set
            {
                if (value == null)
                {
                    if (SelectedItem != null)
                        ((TreeViewItem)SelectedItem).IsSelected = false;
                }
                else
                {
                    TreeViewItem treeViewItem = value;
                    treeViewItem.IsSelected = true;

                    while (treeViewItem.Parent is TreeViewItem)
                    {
                        treeViewItem = (TreeViewItem)treeViewItem.Parent;
                        treeViewItem.IsExpanded = true;
                    }

                    if (!value.IsVisible)
                        UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected metadata node.
        /// </summary>
        public MetadataNode SelectedMetadataNode
        {
            get
            {
                if (SelectedNode == null)
                    return null;

                return _treeNodeToMetadataNode[SelectedNode];
            }
            set
            {
                TreeViewItem node = null;

                if (value != null)
                {
                    if (!_metadataNodeToTreeNode.TryGetValue(value, out node))
                        node = null;
                }

                if (SelectedNode != node)
                    SelectedNode = node;
            }
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Updates the node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void UpdateNode(MetadataNode node)
        {
            if (node == null)
                return;

            TreeViewItem treeNode = null;
            if (_metadataNodeToTreeNode.TryGetValue(node, out treeNode))
                UpdateNode(treeNode);
        }

        /// <summary>
        /// Updates the node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void UpdateNode(TreeViewItem node)
        {
            if (node == null)
                return;

            // get metadata of selected node
            MetadataNode selectedNodeMetadata = null;
            // metadata of next node
            MetadataNode nextNodeMetadata = null;
            // metadata of previous node
            MetadataNode prevNodeMetadata = null;
            // get parent node
            TreeViewItem parent = null;

            // get expanded nodes
            MetadataNode[] expandedNodes = GetExpandedNodes();


            if (SelectedNode != null)
            {
                // get metadata of selected node
                selectedNodeMetadata = _treeNodeToMetadataNode[SelectedNode];

                TreeViewItem nextNode = GetNextItem(SelectedNode);
                // metadata of next node
                if (nextNode != null)
                    nextNodeMetadata = _treeNodeToMetadataNode[nextNode];

                TreeViewItem prevNode = GetPreviousNode(SelectedNode);
                // metadata of previous node
                if (prevNode != null)
                    prevNodeMetadata = _treeNodeToMetadataNode[prevNode];

                // get parent node
                parent = SelectedNode.Parent as TreeViewItem;
            }


            // begin update
            BeginInit();

            // remove specified node children
            Remove(node.Items);

            // update specified node style
            UpdateNodeStyle(node, true, false);

            // add children to specified node
            MetadataNode metadataNode = _treeNodeToMetadataNode[node];
            foreach (MetadataNode childNode in metadataNode)
                FillMetadataTree(node.Items, childNode);


            // restore selected node

            SelectedMetadataNode = selectedNodeMetadata;
            if (SelectedNode == null && nextNodeMetadata != null)
                SelectedMetadataNode = nextNodeMetadata;
            if (SelectedNode == null && prevNodeMetadata != null)
                SelectedMetadataNode = prevNodeMetadata;
            if (SelectedNode == null && parent != null)
                SelectedNode = parent;

            ExpandNodes(expandedNodes);

            // end update
            EndInit();
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Returns the metadata node name.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// The metadata node name.
        /// </returns>
        protected virtual string GetMetadataNodeName(MetadataNode node)
        {
            return node.Name;
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Fills the metadata tree.
        /// </summary>
        /// <param name="metadataTree">The node of metadata.</param>
        private void FillMetadataTree(MetadataNode metadataTree)
        {
            BeginInit();

            // clear tree view
            Items.Clear();
            // add nodes to tree view
            FillMetadataTree(Items, metadataTree);

            EndInit();
        }

        /// <summary>
        /// Updates the node style.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <param name="updateParent">Indicates whether parent must be updated.</param>
        /// <param name="updateChildren">Indicates whether children must be updated.</param>
        private void UpdateNodeStyle(TreeViewItem treeNode, bool updateParent, bool updateChildren)
        {
            if (treeNode == null)
                return;

            // get node of metadata
            MetadataNode metadataNode = _treeNodeToMetadataNode[treeNode];
            // node is not empty?
            bool hasValue = (metadataNode is PageMetadata) || !metadataNode.IsLeafNode || metadataNode.HasValue;
            // if node has not empty value
            if (hasValue)
            {
                // if node is changed
                if (metadataNode.IsChanged)
                {
                    treeNode.Foreground = new SolidColorBrush(Colors.DarkRed);
                }
                // if node can be changed
                else if (!metadataNode.IsReadOnly && metadataNode.IsLeafNode && !(metadataNode is PageMetadata))
                {
                    treeNode.Foreground = new SolidColorBrush(Colors.Green); ;
                }
                else
                {
                    treeNode.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                treeNode.Foreground = new SolidColorBrush(Colors.Gray);
            }

            string metadataNodeName = GetMetadataNodeName(metadataNode);
            // if text of tree node must be updated
            if (!string.Equals(treeNode.Header.ToString(), metadataNodeName, StringComparison.InvariantCulture))
                // update text of thee node
                treeNode.Header = metadataNodeName;

            // if parent of tree node must be updated
            if (updateParent && treeNode.Parent is TreeViewItem)
                // update parent of tree node
                UpdateNodeStyle((TreeViewItem)treeNode.Parent, updateParent, updateChildren);

            // if children of tree node must be updated
            if (updateChildren)
            {
                // update children of tree node
                foreach (TreeViewItem node in treeNode.Items)
                    UpdateNodeStyle(node, updateParent, updateChildren);
            }
        }

        /// <summary>
        /// Fills the metadata tree.
        /// </summary>
        /// <param name="treeNodes">The parent node.</param>
        /// <param name="metadataNode">The node of metadata.</param>
        private void FillMetadataTree(ItemCollection treeNodes, MetadataNode metadataNode)
        {
            if (metadataNode == null)
                return;

            TreeViewItem treeNode = new TreeViewItem();
            treeNode.Header = GetMetadataNodeName(metadataNode);

            treeNodes.Add(treeNode);

            _metadataNodeToTreeNode.Add(metadataNode, treeNode);
            _treeNodeToMetadataNode.Add(treeNode, metadataNode);

            UpdateNodeStyle(treeNode, true, false);

            foreach (MetadataNode childNode in metadataNode)
                FillMetadataTree(treeNode.Items, childNode);
        }

        /// <summary>
        /// Removes the nodes of specified collection.
        /// </summary>
        /// <param name="nodes">The node collection.</param>
        private void Remove(ItemCollection nodes)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
                Remove((TreeViewItem)nodes[i]);
        }

        /// <summary>
        /// Removes the specified node.
        /// </summary>
        /// <param name="node">The node for remove.</param>
        private void Remove(TreeViewItem node)
        {
            if (node == null)
                return;

            Remove(node.Items);

            MetadataNode metadataNode = _treeNodeToMetadataNode[node];
            _metadataNodeToTreeNode.Remove(metadataNode);
            _treeNodeToMetadataNode.Remove(node);

            TreeViewItem parent = node.Parent as TreeViewItem;

            if (parent == null)
            {
                TreeView treeView = (TreeView)node.Parent;
                treeView.Items.Remove(node);
            }
            else
            {
                parent.Items.Remove(node);
            }
        }

        /// <summary>
        /// Returns the all expanded nodes of tree view.
        /// </summary>
        /// <returns>
        /// The expanded nodes array.
        /// </returns>
        private MetadataNode[] GetExpandedNodes()
        {
            List<MetadataNode> nodes = new List<MetadataNode>();

            foreach (TreeViewItem children in Items)
                AddExpandedNodes(children, nodes);

            return nodes.ToArray();
        }

        /// <summary>
        /// Adds the expanded nodes of the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="expandedNodes">The expanded nodes.</param>
        private void AddExpandedNodes(TreeViewItem node, List<MetadataNode> expandedNodes)
        {
            // if specified node is expanded
            if (node.IsExpanded)
            {
                // add to list
                MetadataNode metadataNode = _treeNodeToMetadataNode[node];
                expandedNodes.Add(metadataNode);
            }

            // for each children in specified node
            foreach (TreeViewItem children in node.Items)
                AddExpandedNodes(children, expandedNodes);
        }

        /// <summary>
        /// Expands the specified nodes.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        private void ExpandNodes(params MetadataNode[] nodes)
        {
            TreeViewItem treeNode = null;
            foreach (MetadataNode node in nodes)
            {
                if (_metadataNodeToTreeNode.TryGetValue(node, out treeNode))
                    treeNode.IsExpanded = true;
            }
        }

        /// <summary>
        /// Returns the tree view item, which is following the specified tree view item.
        /// </summary>
        /// <param name="item">The tree view item.</param>
        /// <returns>
        /// The next tree view item.
        /// </returns>
        private TreeViewItem GetNextItem(TreeViewItem item)
        {
            TreeViewItem parent = item.Parent as TreeViewItem;
            if (parent == null)
                return null;

            ItemCollection items = parent.Items;
            int index = items.IndexOf(item) + 1;

            if (index < items.Count)
                return items[index] as TreeViewItem;

            return null;
        }

        /// <summary>
        /// Returns the tree view item, which goes before the specified tree view item.
        /// </summary>
        /// <param name="item">The tree view item.</param>
        /// <returns>
        /// The previous tree view item.
        /// </returns>
        private TreeViewItem GetPreviousNode(TreeViewItem item)
        {
            TreeViewItem parent = item.Parent as TreeViewItem;
            if (parent == null)
                return null;

            ItemCollection items = parent.Items;
            int index = items.IndexOf(item) - 1;

            if (index > 0)
                return items[index] as TreeViewItem;

            return null;
        }

        #endregion

        #endregion

    }
}
