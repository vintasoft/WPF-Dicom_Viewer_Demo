using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// A control that allows to search <see cref="System.Windows.Controls.TreeViewItem"/> in the <see cref="System.Windows.Controls.TreeView"/>.
    /// </summary>
    public partial class TreeViewSearchControl : UserControl
    {

        #region Fields

        /// <summary>
        /// The searched tree nodes.
        /// </summary>
        List<TreeViewItem> _searchedTreeNodes = null;

        /// <summary>
        /// The current tree node index of searched tree nodes.
        /// </summary>
        int _currentSelectedNodeIndex = -1;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewSearchControl"/> class.
        /// </summary>
        public TreeViewSearchControl()
        {
            InitializeComponent();

            ResetSearchResult();
        }

        #endregion



        #region Properties

        TreeView _treeView = null;
        /// <summary> 
        /// Gets or sets the <see cref="System.Windows.Controls.TreeView"/> for search in <see cref="System.Windows.Controls.TreeViewItem"/>.
        /// </summary>
        /// <value>
        /// Default value is <b>null</b>.
        /// </value>
        public TreeView TreeView
        {
            get
            {
                return _treeView;
            }
            set
            {
                if (_treeView != value)
                {
                    _treeView = value;

                    ResetSearchResult();
                }
            }
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Resets the searched result. 
        /// </summary>
        public void ResetSearchResult()
        {
            _searchedTreeNodes = null;
            _currentSelectedNodeIndex = -1;
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// The "Find" button is clicked.
        /// </summary>
        private void FindButton_Click(object sender, RoutedEventArgs e)
        {
            SearchNodes();

            SelectNextSearchedNode();
        }

        /// <summary>
        /// The search pattern text is changed.
        /// </summary>
        private void FindPatternTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetSearchResult();
        }

        /// <summary>
        /// The key is pressed in the "Find pattern" text box.
        /// </summary>
        private void FindPatternTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.None && e.Key == Key.Enter)
            {
                SearchNodes();

                SelectNextSearchedNode();
            }
        }

        /// <summary>
        /// Selects the next searched node.
        /// </summary>
        private void SelectNextSearchedNode()
        {
            if (TreeView == null)
                return;

            if (_searchedTreeNodes == null ||
                _searchedTreeNodes.Count == 0)
                return;

            _currentSelectedNodeIndex++;

            bool isAllNodesSelected = false;

            if (_currentSelectedNodeIndex >= _searchedTreeNodes.Count)
            {
                _currentSelectedNodeIndex = 0;
                isAllNodesSelected = true;
            }

            TreeViewItem selectedTreeViewItem = _searchedTreeNodes[_currentSelectedNodeIndex];
            selectedTreeViewItem.IsSelected = true;

            TreeViewItem treeViewItem = selectedTreeViewItem;
            while (treeViewItem.Parent is TreeViewItem)
            {
                treeViewItem = (TreeViewItem)treeViewItem.Parent;
                treeViewItem.IsExpanded = true;
            }

            if (!selectedTreeViewItem.IsVisible)
                TreeView.UpdateLayout();

            TreeView.Focus();

            if (isAllNodesSelected)
            {
                MessageBox.Show(
                    "Find reached the starting point of the search.",
                    "Tree View Search Control",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Search the tree nodes.
        /// </summary>
        private void SearchNodes()
        {
            if (TreeView == null)
                return;

            if (_searchedTreeNodes == null)
            {
                string searchPattern = findPatternTextBox.Text;

                if (string.IsNullOrEmpty(searchPattern))
                    return;

                _searchedTreeNodes = new List<TreeViewItem>();

                SearchNodes(TreeView.Items, searchPattern.ToLowerInvariant(), _searchedTreeNodes);

                if (_searchedTreeNodes.Count == 0)
                {
                    MessageBox.Show(
                        string.Format("The specified text was not found:\r\n{0}", searchPattern),
                        "Tree View Search Control",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Search the tree node in the specified tree node collection.
        /// </summary>
        /// <param name="nodes">The collection for search.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchedNodes">The searched nodes list.</param>
        private void SearchNodes(
            ItemCollection nodes,
            string searchPattern,
            List<TreeViewItem> searchedNodes)
        {
            foreach (TreeViewItem node in nodes)
            {
                string nodeName = node.Header.ToString().ToLowerInvariant();
                if (nodeName.Contains(searchPattern))
                    searchedNodes.Add(node);

                SearchNodes(node.Items, searchPattern, searchedNodes);
            }
        }

        #endregion

        #endregion

    }
}
