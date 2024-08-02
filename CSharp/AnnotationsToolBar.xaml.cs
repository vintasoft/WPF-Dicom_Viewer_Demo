using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

#if !REMOVE_ANNOTATION_PLUGIN
using Vintasoft.Imaging.Annotation.Dicom;
using Vintasoft.Imaging.Annotation.UI;
using Vintasoft.Imaging.Annotation.Wpf.UI;
using Vintasoft.Imaging.Annotation.Wpf.UI.VisualTools; 
#endif
using Vintasoft.Imaging.Wpf.UI;
using Vintasoft.Imaging.Wpf.UI.VisualTools;


namespace WpfDicomViewerDemo
{

    /// <summary>
    /// Toolbar with DICOM annotations.
    /// </summary>
    public partial class AnnotationsToolBar : ToolBar
    {

        #region Fields

        Button _buildingAnnotationButton;

        const string SEPARATOR = "SEPARATOR";
        const string PointButtonName = "Point";
        const string CircleButtonName = "Circle";
        const string PolylineButtonName = "Polyline";
        const string InterpolatedButtonName = "Interpolated";

        const string EllipseButtonName = "Ellipse";
        const string MultilineButtonName = "Multiline";
        const string RangelineButtonName = "Rangeline";
        const string InifinitelineButtonName = "Infiniteline";
        const string CutlineButtonName = "Cutline";
        const string ArrowButtonName = "Arrow";
        const string RectangleButtonName = "Rectangle";
        const string AxisButtonName = "Axis";
        const string RulerButtonName = "Ruler";
        const string CrosshairButtonName = "Crosshair";

        const string TextButtonName = "Text";

        const string IconResourceFormat = "{0}Icon";

        string[] AnnotationNames = { 
            PointButtonName,
            CircleButtonName,
            PolylineButtonName,
            InterpolatedButtonName,
            SEPARATOR,
            
            RectangleButtonName,
            EllipseButtonName,
            MultilineButtonName,
            RangelineButtonName,
            InifinitelineButtonName,
            CutlineButtonName,
            ArrowButtonName,
            AxisButtonName,
            RulerButtonName,
            CrosshairButtonName,
            SEPARATOR,

            TextButtonName,
        };

#if !REMOVE_ANNOTATION_PLUGIN
        WpfAnnotationVisualTool _annotationTool; 
#endif

        #endregion



        #region Constructors

        public AnnotationsToolBar()
        {
            InitializeComponent();

            for (int i = 0; i < AnnotationNames.Length; i++)
            {
                string name = AnnotationNames[i];
                if (name == SEPARATOR)
                {
                    StackPanel.Children.Add(new Separator());
                }
                else
                {
                    Button button = new Button();
                    button.Background = Brushes.Transparent;
                    button.BorderThickness = new Thickness(1);
                    button.BorderBrush = Brushes.Transparent;
                    button.Focusable = false;
                    if (i == 0)
                        button.Margin = new Thickness(0, 0, 0, 1);
                    else if (i == AnnotationNames.Length - 1)
                        button.Margin = new Thickness(1, 0, 0, 0);
                    else
                        button.Margin = new Thickness(1, 0, 0, 1);
                    button.ToolTip = name;
                    button.Click += new RoutedEventHandler(buildAnnotationButton_Click);

                    Image image = new Image();
                    image.Width = 16;
                    image.Height = 16;
                    string iconName = string.Format(IconResourceFormat, name);
                    image.Source = (BitmapImage)Resources[iconName];
                    button.Content = image;

                    StackPanel.Children.Add(button);
                }
            }

            Viewer = null;
        }

        #endregion



        #region Properties

        WpfImageViewer _viewer = null;
        /// <summary>
        /// Gets or sets the <see cref="AnnotationViewer"/> associated with
        /// this <see cref="AnnotationsToolStrip"/>.
        /// </summary>        
        public WpfImageViewer Viewer
        {
            get
            {
                return _viewer;
            }
            set
            {
#if !REMOVE_ANNOTATION_PLUGIN
                if (_annotationTool != null)
                {
                    _annotationTool.AnnotationBuildingFinished -= viewer_AnnotationBuildingFinished;
                    _annotationTool.AnnotationBuildingCanceled -= viewer_AnnotationBuildingCanceled;
                }

                _annotationTool = null;
                _viewer = value;
                if (_viewer != null)
                    _annotationTool = GetAnnotationVisualTool(_viewer.VisualTool);

                if (_annotationTool != null)
                {
                    _annotationTool.AnnotationBuildingFinished += new EventHandler<WpfAnnotationViewEventArgs>(viewer_AnnotationBuildingFinished);
                    _annotationTool.AnnotationBuildingCanceled += new EventHandler<WpfAnnotationViewEventArgs>(viewer_AnnotationBuildingCanceled);
                } 
#endif
            }
        }

        Button _selectedButton = null;
        private Button SelectedButton
        {
            get            
            {
                return _selectedButton;
            }
            set
            {
                if (_selectedButton != null)
                {
                    _selectedButton.BorderBrush = Brushes.Transparent;
                }

                _selectedButton = value;

                if (_selectedButton != null)
                {
                    _selectedButton.BorderBrush = Brushes.Blue;
                }
            }
        }

        #endregion



        #region Methods

#if !REMOVE_ANNOTATION_PLUGIN
        /// <summary>
        /// Returns an annotation object by the annotation type name.
        /// </summary>
        protected virtual WpfAnnotationView GetAnnotation(string annotationName)
        {
            DicomAnnotationData data = null;

            switch (annotationName)
            {
                case PointButtonName:
                    data = new DicomPointAnnotationData();
                    break;

                case CircleButtonName:
                    data = new DicomCircleAnnotationData();
                    break;

                case PolylineButtonName:
                    data = new DicomPolylineAnnotationData();
                    break;

                case InterpolatedButtonName:
                    data = new DicomPolylineAnnotationData();
                    ((DicomPolylineAnnotationData)data).UseInterpolation = true;
                    break;

                case EllipseButtonName:
                    data = new DicomEllipseAnnotationData();
                    break;

                case MultilineButtonName:
                    data = new DicomMultilineAnnotationData();
                    break;

                case RangelineButtonName:
                    data = new DicomRangeLineAnnotationData();
                    break;

                case InifinitelineButtonName:
                    data = new DicomInfiniteLineAnnotationData();
                    break;

                case CutlineButtonName:
                    data = new DicomCutLineAnnotationData();
                    break;

                case ArrowButtonName:
                    data = new DicomArrowAnnotationData();
                    break;

                case RectangleButtonName:
                    data = new DicomRectangleAnnotationData();
                    break;

                case AxisButtonName:
                    data = new DicomAxisAnnotationData();
                    break;

                case RulerButtonName:
                    data = new DicomRulerAnnotationData();
                    break;

                case CrosshairButtonName:
                    data = new DicomCrosshairAnnotationData();
                    break;

                case TextButtonName:
                    data = new DicomTextAnnotationData();
                    ((DicomTextAnnotationData)data).UnformattedTextValue = "Text";
                    break;
            }

            return WpfAnnotationViewFactory.CreateView(data);
        } 
#endif

        /// <summary>
        /// User started the annotation building.
        /// </summary>
        private void buildAnnotationButton_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            Button annotationButton = (Button)sender;
            SelectedButton = annotationButton;

            if (_annotationTool.FocusedAnnotationView != null &&
                _annotationTool.FocusedAnnotationView.InteractionController ==
                _annotationTool.FocusedAnnotationView.Builder)
                _annotationTool.CancelAnnotationBuilding();

            if (annotationButton == _buildingAnnotationButton)
            {
                _buildingAnnotationButton = null;
            }
            else
            {
                if (_annotationTool.AnnotationInteractionMode != AnnotationInteractionMode.Author)
                    _annotationTool.AnnotationInteractionMode = AnnotationInteractionMode.Author;

                WpfAnnotationView annotationView = BuildAnnotation(annotationButton.ToolTip.ToString());

                if (annotationView != null)
                    _buildingAnnotationButton = annotationButton;
                else
                    _buildingAnnotationButton = null;
            } 
#endif
        }

#if !REMOVE_ANNOTATION_PLUGIN
        /// <summary>
        /// Adds an annotation to an image and starts building of annotation.
        /// </summary>
        public WpfAnnotationView BuildAnnotation(string annotationName)
        {
            if (Viewer == null || Viewer.Image == null)
                return null;

            WpfAnnotationView annotationView = null;
            try
            {
                annotationView = GetAnnotation(annotationName);

                if (annotationView != null)
                {
                    //
                    _annotationTool.AddAndBuildAnnotation(annotationView);
                }
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Building annotation", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return annotationView;
        } 

        /// <summary>
        /// Annotation building is finished.
        /// </summary>
        private void viewer_AnnotationBuildingFinished(object sender, WpfAnnotationViewEventArgs e)
        {
            Button buildingAnnotationButton = _buildingAnnotationButton;
            if (buildingAnnotationButton != null)
            {
                if (SelectedButton != null && 
                    SelectedButton.ToolTip == buildingAnnotationButton.ToolTip)
                {
                    _buildingAnnotationButton = null;
                    SelectedButton = null;
                }
            }
        }

        /// <summary>
        /// Annotation building is canceled.
        /// </summary>
        private void viewer_AnnotationBuildingCanceled(object sender, WpfAnnotationViewEventArgs e)
        {
            Button buildingAnnotationButton = _buildingAnnotationButton;
            if (buildingAnnotationButton != null)
            {
                if (SelectedButton != null &&
                    SelectedButton.ToolTip == buildingAnnotationButton.ToolTip)
                {
                    _buildingAnnotationButton = null;
                    SelectedButton = null;
                }
            }
        }

        private WpfAnnotationVisualTool GetAnnotationVisualTool(WpfVisualTool visualTool)
        {
            if (visualTool is WpfCompositeVisualTool)
            {
                WpfCompositeVisualTool compositeVisualTool = (WpfCompositeVisualTool)visualTool;
                foreach (WpfVisualTool tool in compositeVisualTool)
                {
                    WpfAnnotationVisualTool result = GetAnnotationVisualTool(tool);
                    if (result != null)
                        return result;
                }

                return null;
            }

            return visualTool as WpfAnnotationVisualTool;
        }
#endif

        #endregion

    }
}
