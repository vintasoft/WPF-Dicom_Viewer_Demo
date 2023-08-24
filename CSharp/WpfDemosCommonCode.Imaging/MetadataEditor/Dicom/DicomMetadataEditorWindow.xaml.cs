using System.Windows;

using Vintasoft.Imaging;
#if !REMOVE_DICOM_PLUGIN
using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;
#endif
using Vintasoft.Imaging.Metadata;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Interaction logic for DicomMetadataEditorWindow.xaml
    /// </summary>
    public partial class DicomMetadataEditorWindow : Window
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DicomMetadataEditorWindow"/> class.
        /// </summary>
        public DicomMetadataEditorWindow()
        {
            InitializeComponent();

            treeViewSearchControl1.TreeView = metadataTreeView;
        }

        #endregion



        #region Properties

        VintasoftImage _image;
        /// <summary>
        /// Gets or sets an image.
        /// </summary>
        public VintasoftImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;

                RootMetadataNode = _image.Metadata.MetadataTree;
            }
        }

        /// <summary>
        /// Gets or sets the root metadata node.
        /// </summary>
        public MetadataNode RootMetadataNode
        {
            get
            {
                return metadataTreeView.RootMetadataNode;
            }
            set
            {
                metadataTreeView.RootMetadataNode = value;

                UpdateUI();
            }
        }

        bool _canEdit = true;
        /// <summary>
        /// Gets or sets a value indicating whether DICOM metadata can be edited.
        /// </summary>
        /// <value>
        /// <b>True</b> if DICOM metadata can be edited; otherwise, <b>false</b>.
        /// </value>
        public bool CanEdit
        {
            get
            {
                return _canEdit;
            }
            set
            {
                _canEdit = value;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Updates the user interface of this form.
        /// </summary>
        private void UpdateUI()
        {
            if (CanEdit)
            {
                this.Title = "DICOM Metadata Editor";
                addButton.Visibility = Visibility.Visible;
                removeButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.Title = "DICOM Metadata Viewer";
                addButton.Visibility = Visibility.Collapsed;
                removeButton.Visibility = Visibility.Collapsed;
            }

            MetadataNode metadataNode = metadataTreeView.SelectedMetadataNode;

            bool canAddSubNode = false;

#if !REMOVE_DICOM_PLUGIN
            if (metadataNode is DicomFrameMetadata ||
                  metadataNode is DicomDataSetMetadata)
                canAddSubNode = true;

            if (metadataNode is DicomDataElementMetadata &&
                ((DicomDataElementMetadata)metadataNode).ValueRepresentation == DicomValueRepresentation.SQ)
                canAddSubNode = true;
#endif

            addButton.IsEnabled = CanEdit && canAddSubNode;
            removeButton.IsEnabled = CanEdit && (metadataNode == null ? false : metadataNode.CanRemove);
        }

        /// <summary>
        /// Shows the specified metadata node properties in <see cref="PropertyGrid"/>.
        /// </summary>
        /// <param name="metadataNode">The metadata node.</param>
        private void ShowMetadataNodeProperties(MetadataNode metadataNode)
        {
            object selectedObject = metadataNode;

#if !REMOVE_DICOM_PLUGIN
            // if selected metadata tree node is DICOM metadata
            if (metadataNode is DicomDataElementMetadata)
            {
                DicomDataElementMetadataConverter metadataConverter =
                    new DicomDataElementMetadataConverter((DicomDataElementMetadata)metadataNode);
                metadataConverter.CanEdit = CanEdit;
                selectedObject = metadataConverter;
            }
#endif

            nodePropertyGrid.SelectedObject = selectedObject;
        }

        /// <summary>
        /// Node of tree view is selected.
        /// </summary>
        private void MetadataTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            MetadataNode metadataNode = metadataTreeView.SelectedMetadataNode;

            ShowMetadataNodeProperties(metadataNode);

            string selectedNodeDescription = string.Empty;
            string addButtonText = "Add DICOM Data Element...";

            if (metadataNode != null)
            {
                selectedNodeDescription = string.Format("{0} ({1})", metadataNode.Name, metadataNode.GetType().Name);

#if !REMOVE_DICOM_PLUGIN
                if (metadataNode is DicomDataElementMetadata &&
                           ((DicomDataElementMetadata)metadataNode).ValueRepresentation == DicomValueRepresentation.SQ)
                    addButtonText = "Add DICOM Sequence Item";
#endif
            }

            selectedNodeGroupBox.Header = selectedNodeDescription;
            addButton.Content = addButtonText;

            UpdateUI();
        }

        /// <summary>
        /// Adds new metadata node to the selected metadata node.
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MetadataNode metadataNode = metadataTreeView.SelectedMetadataNode;
            bool isMetadataNodeChanged = false;

#if !REMOVE_DICOM_PLUGIN
            if (metadataNode is DicomDataElementMetadata)
            {
                DicomDataElementMetadata node = (DicomDataElementMetadata)metadataNode;

                node.AddChild();
                isMetadataNodeChanged = true;
            }
            else
            {
                MetadataNode node = (MetadataNode)metadataNode;

                AddDicomDataElementWindow dialog = new AddDicomDataElementWindow(node);
                dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                    isMetadataNodeChanged = true;
            }
#endif

            if (isMetadataNodeChanged)
            {
                metadataTreeView.UpdateNode(metadataNode);
                metadataTreeView.Focus();

                treeViewSearchControl1.ResetSearchResult();
            }
        }

        /// <summary>
        /// Removes the selected metadata node.
        /// </summary>
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            // get the selected metadata node
            MetadataNode metadataNode = metadataTreeView.SelectedMetadataNode;
            // remove the selected metadata node
            metadataNode.Parent.RemoveChild(metadataNode);

            // update parent of selected node
            metadataTreeView.UpdateNode(metadataNode.Parent);

            metadataTreeView.Focus();
            treeViewSearchControl1.ResetSearchResult();
        }

        #endregion

    }
}
