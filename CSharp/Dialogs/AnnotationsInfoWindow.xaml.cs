using System.Windows;

#if !REMOVE_ANNOTATION_PLUGIN
using Vintasoft.Imaging.Annotation;
using Vintasoft.Imaging.Annotation.Dicom; 
#endif


namespace WpfDicomViewerDemo
{
    /// <summary>
    /// A window that allows to show information about annotations of DICOM file.
    /// </summary>
    public partial class AnnotationsInfoWindow : Window
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
            public DicomAnnotationDataView(int pageNumber, AnnotationData annotationData)
            {
                _pageNumber = string.Format("Page {0}", pageNumber);
                _annotationType = annotationData.GetType().ToString();
                _location = annotationData.Location.ToString();
            } 
#endif



            string _pageNumber;
            /// <summary>
            /// Gets the page number.
            /// </summary>
            public string PageNumber
            {
                get
                {
                    return _pageNumber;
                }
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



#if !REMOVE_ANNOTATION_PLUGIN
        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationsInfoWindow"/> class.
        /// </summary>
        /// <param name="annotations">The annotations.</param>
        public AnnotationsInfoWindow(DicomAnnotationDataController annotations)
        {
            InitializeComponent();

            for (int i = 0; i < annotations.Images.Count; i++)
            {
                for (int j = 0; j < annotations[i].Count; j++)
                    annoInfoListView.Items.Add(new DicomAnnotationDataView(i + 1, annotations[i][j]));
            }
        } 
#endif



        /// <summary>
        /// "OK" button is clicked.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

    }
}
