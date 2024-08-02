using System;
using System.Windows;
using System.Windows.Controls;

#if !REMOVE_ANNOTATION_PLUGIN
using Vintasoft.Imaging.Annotation;
using Vintasoft.Imaging.Annotation.Dicom; 
#endif
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
#if !REMOVE_ANNOTATION_PLUGIN
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
#endif

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

#if !REMOVE_ANNOTATION_PLUGIN
        /// <summary>
        /// An array of annotation collections.
        /// </summary>
        DicomAnnotationDataCollection[] _collections = null; 
#endif

        #endregion



        #region Constructors

#if !REMOVE_ANNOTATION_PLUGIN
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
#endif

        #endregion



        #region Properties

#if !REMOVE_ANNOTATION_PLUGIN
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
#endif

        #endregion



        #region Methods

        /// <summary>
        /// Focused annotation data collection is changed.
        /// </summary>
        private void selectedAnnotationDataCollectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            SelectedAnnotationDataCollection = _collections[selectedAnnotationDataCollectionComboBox.SelectedIndex]; 
#endif
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
