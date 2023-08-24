using System;
using System.Windows;
using System.Windows.Controls;

using Vintasoft.Imaging.Annotation;
using Vintasoft.Imaging.Annotation.Dicom;
using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;


namespace WpfDicomViewerDemo
{
    /// <summary>
    /// A window that allows to select annotation collection from the list of annotation collections.
    /// </summary>
    public partial class SelectAnnotationDataCollectionWindow : Window
    {

        #region Nested classes

        /// <summary>
        /// View of DICOM annotation data.
        /// </summary>
        class DicomAnnotationDataView
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DicomAnnotationDataView"/> class.
            /// </summary>
            /// <param name="pageNumber">The page number.</param>
            /// <param name="annotationData">The annotation data.</param>
            public DicomAnnotationDataView(AnnotationData annotationData)
            {
                _annotationType = annotationData.GetType().ToString();
                _location = annotationData.Location.ToString();
            }

            string _annotationType;
            /// <summary>
            /// Gets the annotation type.
            /// </summary>
            public string AnnotationType
            {
                get
                {
                    return _annotationType;
                }
            }

            string _location;
            /// <summary>
            /// Gets the annotation location.
            /// </summary>
            public string Location
            {
                get
                {
                    return _location;
                }
            }
        }

        #endregion



        #region Fields

        /// <summary>
        /// An array of annotation collections.
        /// </summary>
        DicomAnnotationDataCollection[] _collections = null;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAnnotationDataCollectionWindow"/> class.
        /// </summary>
        /// <param name="collections">An array of annotation collections.</param>
        public SelectAnnotationDataCollectionWindow(params DicomAnnotationDataCollection[] collections)
        {
            InitializeComponent();

            _collections = collections;

            if (_collections.Length > 0)
            {
                selectedAnnotationDataCollectionComboBox.BeginInit();
                for (int i = 0; i < _collections.Length; i++)
                {
                    string title = string.Format("Annotation Data Collection N{0}", i + 1);

                    selectedAnnotationDataCollectionComboBox.Items.Add(title);
                }
                selectedAnnotationDataCollectionComboBox.EndInit();

                SelectedAnnotationDataCollection = _collections[0];
            }
        }

        #endregion



        #region Properties

        DicomAnnotationDataCollection _selectedAnnotationDataCollection = null;
        /// <summary>
        /// Gets or sets the selected annotation data collection.
        /// </summary>
        public DicomAnnotationDataCollection SelectedAnnotationDataCollection
        {
            get
            {
                return _selectedAnnotationDataCollection;
            }
            set
            {
                int collectionIndex = Array.IndexOf(_collections, value);
                if (collectionIndex == -1)
                    throw new ArgumentOutOfRangeException();

                if (_selectedAnnotationDataCollection != value)
                {
                    _selectedAnnotationDataCollection = value;

                    selectedAnnotationDataCollectionComboBox.SelectedIndex = collectionIndex;

                    DicomReferencedImage referencedImage = _selectedAnnotationDataCollection.ReferencedImage;
                    sopClassLabel.Content = "Unknown";
                    if (referencedImage.SopClass != null)
                        sopClassLabel.Content = referencedImage.SopClass.Value;

                    sopInstanceLabel.Content = "Unknown";
                    if (referencedImage.SopInstance != null)
                        sopInstanceLabel.Content = referencedImage.SopInstance.Value;

                    frameNumberLabel.Content = referencedImage.FrameNumber.ToString();

                    annoInfoListView.Items.Clear();
                    for (int i = 0; i < _selectedAnnotationDataCollection.Count; i++)
                        annoInfoListView.Items.Add(new DicomAnnotationDataView(_selectedAnnotationDataCollection[i]));
                }
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Focused annotation data collection is changed.
        /// </summary>
        private void selectedAnnotationDataCollectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAnnotationDataCollection = _collections[selectedAnnotationDataCollectionComboBox.SelectedIndex];
        }

        /// <summary>
        /// "OK" button is clicked.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// "Cancel" button is clicked.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

    }
}
