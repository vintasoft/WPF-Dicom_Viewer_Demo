using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Annotation;
using Vintasoft.Imaging.Annotation.Dicom;
using Vintasoft.Imaging.Annotation.Dicom.Wpf.UI;
using Vintasoft.Imaging.Annotation.Dicom.Wpf.UI.VisualTools;
using Vintasoft.Imaging.Annotation.Formatters;
using Vintasoft.Imaging.Annotation.UI;
using Vintasoft.Imaging.Annotation.Wpf.UI;
using Vintasoft.Imaging.Annotation.Wpf.UI.VisualTools;
using Vintasoft.Imaging.Codecs;
using Vintasoft.Imaging.Codecs.Decoders;
using Vintasoft.Imaging.Codecs.Encoders;
using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;
using Vintasoft.Imaging.Dicom.Wpf.UI.VisualTools;
using Vintasoft.Imaging.ImageColors;
using Vintasoft.Imaging.Metadata;
using Vintasoft.Imaging.UI;
using Vintasoft.Imaging.UIActions;
using Vintasoft.Imaging.Wpf.UI.VisualTools;

using WpfDemosCommonCode;
using WpfDemosCommonCode.CustomControls;
using WpfDemosCommonCode.Imaging;
using WpfDemosCommonCode.Imaging.Codecs;
using WpfDemosCommonCode.Imaging.Codecs.Dialogs;

namespace WpfDicomViewerDemo
{
    /// <summary>
    /// Main window of DICOM viewer demo.
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Constants

        /// <summary>
        /// The name of text overlay collection owner.
        /// </summary>
        const string OVERLAY_OWNER_NAME = "Dicom Viewer";

        #endregion



        #region Fields

        /// <summary>
        /// Template of the application title.
        /// </summary>
        string _titlePrefix = "VintaSoft WPF DICOM Viewer Demo v" + ImagingGlobalSettings.ProductVersion + " - {0}";

        /// <summary>
        /// DICOM annotated viewer tool.
        /// </summary>
        WpfDicomAnnotatedViewerTool _dicomViewerTool;

        /// <summary>
        /// The previous interaction mode in DICOM viewer tool.
        /// </summary>
        WpfDicomAnnotatedViewerToolInteractionMode _prevDicomViewerToolInteractionMode;

        /// <summary>
        /// The previous interaction mode in DICOM annotation tool.
        /// </summary>
        AnnotationInteractionMode _prevDicomAnnotationToolInteractionMode;

        /// <summary>
        /// Current rulers unit menu item.
        /// </summary>
        MenuItem _currentRulersUnitOfMeasureMenuItem = null;

        /// <summary>
        /// Determines that window of application is closing.
        /// </summary> 
        bool _isWindowClosing = false;

        /// <summary>
        /// Indicates whether the main window is initialized.
        /// </summary>
        readonly bool _isInitialized = false;


        /// <summary>
        /// Dictionary: the menu item => rulers units of measure.
        /// </summary>
        Dictionary<MenuItem, UnitOfMeasure> _menuItemToRulersUnitOfMeasure =
            new Dictionary<MenuItem, UnitOfMeasure>();

        /// <summary>
        /// Dictionary: the menu item => <see cref="DicomImageVoiLutMouseMoveDirection"/>.
        /// </summary>
        Dictionary<MenuItem, DicomImageVoiLutMouseMoveDirection> _menuItemToMouseMoveDirection =
            new Dictionary<MenuItem, DicomImageVoiLutMouseMoveDirection>();

        /// <summary>
        /// Dictionary: the menu item => VOI LUT.
        /// </summary>
        Dictionary<MenuItem, DicomImageVoiLookupTable> _menuItemToVoiLut =
            new Dictionary<MenuItem, DicomImageVoiLookupTable>();

        /// <summary>
        /// Indicates that the visual tool of <see cref="ImageViewerToolBar"/> is changing.
        /// </summary>
        bool _isVisualToolChanging = false;


        #region File Dialogs

        /// <summary>
        /// The open file dialog for DICOM files.
        /// </summary>
        OpenFileDialog _openDicomFileDialog = new OpenFileDialog();

        /// <summary>
        /// The open file dialog for DICOM annotations.
        /// </summary>
        OpenFileDialog _openDicomAnnotationsFileDialog = new OpenFileDialog();

        /// <summary>
        /// The save file dialog.
        /// </summary>
        SaveFileDialog _saveFileDialog = new SaveFileDialog();

        /// <summary>
        /// The save file dialog for GIF file.
        /// </summary>
        SaveFileDialog _saveGifFileDialog = new SaveFileDialog();

        /// <summary>
        /// The save file dialog for DICOM annotations.
        /// </summary>
        SaveFileDialog _saveDicomAnnotationsFileDialog = new SaveFileDialog();

        #endregion


        #region DICOM file

        /// <summary>
        /// Controller of files in current DICOM series.
        /// </summary>
        DicomSeriesController _dicomSeriesController = new DicomSeriesController();

        /// <summary>
        /// DICOM file without images.
        /// </summary>
        DicomFile _dicomFileWithoutImages = null;

        /// <summary>
        /// Decoding setting of DICOM frame.
        /// </summary>
        DicomDecodingSettings _dicomFrameDecodingSettings = new DicomDecodingSettings(false);

        #endregion


        #region VOI LUT

        /// <summary>
        /// Default VOI LUT menu item.
        /// </summary>
        MenuItem _defaultVoiLutMenuItem = null;

        /// <summary>
        /// Current VOI LUT menu item.
        /// </summary>
        MenuItem _currentVoiLutMenuItem = null;

        /// <summary>
        /// Window that allows to specify VOI LUT with custom parameters.
        /// </summary>
        VoiLutParamsWindow _voiLutParamsWindow = null;

        /// <summary>
        /// The VOI LUT button in toolbar.
        /// </summary>
        MenuItem _voiLutsButton = null;

        #endregion


        #region Animation

        /// <summary>
        /// Indicates whether the animation is cycled.
        /// </summary>
        bool _isAnimationCycled = true;

        /// <summary>
        /// Animation delay in milliseconds.
        /// </summary>
        int _animationDelay = 100;

        /// <summary>
        /// Animation thread.
        /// </summary>
        Thread _animationThread = null;

        /// <summary>
        /// Index of current animated frame.
        /// </summary>
        int _currentAnimatedFrameIndex = 0;

        /// <summary>
        /// A value indicating whether the focused index is changing.
        /// </summary>
        bool _isFocusedIndexChanging = false;

        /// <summary>
        /// A value indicating whether the animation can be stopped.
        /// </summary>
        bool _canStopAnimation = true;

        #endregion


        #region Presentation State Files

        /// <summary>
        /// The extensions of the DICOM presentation state files.
        /// </summary>
        string[] _presentationStateFileExtensions = null;

        #endregion


        #region Annotations

        /// <summary>
        /// Determines that transforming of annotation is started.
        /// </summary>
        bool _isAnnotationTransforming = false;

        /// <summary>
        /// Determines that the annotation property is changing.
        /// </summary>
        bool _isAnnotationPropertyChanging = false;

        /// <summary>
        /// Determines that the annotations are loaded for the current frame.
        /// </summary>
        bool _isAnnotationsLoadedForCurrentFrame = false;

        #endregion


        #region Hot keys

        public static RoutedCommand _openCommand = new RoutedCommand();
        public static RoutedCommand _closeCommand = new RoutedCommand();
        public static RoutedCommand _exitCommand = new RoutedCommand();
        public static RoutedCommand _isNegativeCommand = new RoutedCommand();
        public static RoutedCommand _cutCommand = new RoutedCommand();
        public static RoutedCommand _copyCommand = new RoutedCommand();
        public static RoutedCommand _pasteCommand = new RoutedCommand();
        public static RoutedCommand _deleteCommand = new RoutedCommand();
        public static RoutedCommand _deleteAllCommand = new RoutedCommand();
        public static RoutedCommand _rotateClockwiseCommand = new RoutedCommand();
        public static RoutedCommand _rotateCounterclockwiseCommand = new RoutedCommand();
        public static RoutedCommand _aboutCommand = new RoutedCommand();

        #endregion

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            // register the evaluation license for VintaSoft Imaging .NET SDK
            Vintasoft.Imaging.ImagingGlobalSettings.Register("REG_USER", "REG_EMAIL", "EXPIRATION_DATE", "REG_CODE");

            InitializeComponent();

            Jpeg2000AssemblyLoader.Load();

            AnnotationTypeEditorRegistrator.Register();

            imageViewer1.MasterViewer = thumbnailViewer1;

            InitFileDialogs();

            MoveDicomCodecToFirstPosition();

            _presentationStateFileExtensions = new string[] { ".DCM", ".DIC", ".ACR", ".PRE", "" };

            // init ImageViewerToolBar
            InitImageViewerToolBar();

            dicomAnnotatedViewerToolBar.ImageViewer = imageViewer1;
            MeasurementVisualToolActionFactory.CreateActions(dicomAnnotatedViewerToolBar);


            NoneAction noneAction = dicomAnnotatedViewerToolBar.FindAction<NoneAction>();
            noneAction.Activated += new EventHandler(noneAction_Activated);
            noneAction.Deactivated += new EventHandler(noneAction_Deactivated);

            ImageMeasureToolAction imageMeasureToolAction =
                dicomAnnotatedViewerToolBar.FindAction<ImageMeasureToolAction>();
            imageMeasureToolAction.Activated += new EventHandler(imageMeasureToolAction_Activated);

            WpfMagnifierTool magnifierTool = new WpfMagnifierTool();
            // create action, which allows to magnify of image region in image viewer
            MagnifierToolAction magnifierToolAction = new MagnifierToolAction(
                magnifierTool,
                "Magnifier Tool",
                "Magnifier",
                DemosResourcesManager.GetResourceAsBitmap("WpfDemosCommonCode.Imaging.VisualToolsToolBar.VisualTools.ZoomVisualTools.Resources.WpfMagnifierTool.png"));

            WpfDicomViewerTool dicomViewerTool = new WpfDicomViewerTool();
            _dicomViewerTool = new WpfDicomAnnotatedViewerTool(
               dicomViewerTool,
               new WpfDicomAnnotationTool(),
               (Vintasoft.Imaging.Annotation.Wpf.UI.Measurements.WpfImageMeasureTool)imageMeasureToolAction.VisualTool);

            // add visual tools to tool strip
            dicomAnnotatedViewerToolBar.DicomAnnotatedViewerTool = _dicomViewerTool;
            dicomAnnotatedViewerToolBar.AddVisualToolAction(magnifierToolAction);
            dicomAnnotatedViewerToolBar.MainVisualTool.ActiveTool = _dicomViewerTool;

            magnifierToolAction.Activated += new EventHandler(magnifierToolAction_Activated);

            _dicomViewerTool.DicomViewerTool.TextOverlay.Add(
                new WpfCompressionInfoTextOverlay(AnchorType.Top | AnchorType.Left));

            DemosTools.SetTestFilesFolder(_openDicomFileDialog);

            WpfCompositeVisualTool compositeTool = new WpfCompositeVisualTool(_dicomViewerTool, magnifierTool);
            compositeTool.ActiveTool = _dicomViewerTool;
            imageViewer1.VisualTool = compositeTool;
            annotationsToolBar.Viewer = imageViewer1;

            // init DICOM annotation tool
            InitDicomAnnotationTool();

            _dicomViewerTool.DicomViewerTool.DicomImageVoiLutChanged +=
                new EventHandler<WpfVoiLutChangedEventArgs>(dicomViewerTool_DicomImageVoiLutChanged);

            thumbnailViewer1.ImageDecodingSettings = _dicomFrameDecodingSettings;

            imageViewer1.Images.ImageCollectionSavingProgress += new EventHandler<ProgressEventArgs>(Images_ImageCollectionSavingProgress);
            imageViewer1.Images.ImageCollectionSavingFinished += new EventHandler(Images_ImageCollectionSavingFinished);

            // init rulers unit of measure
            InitUnitOfMeasuresForRulers();

            _defaultVoiLutMenuItem = new MenuItem();
            _defaultVoiLutMenuItem.Header = "Default VOI LUT";
            _defaultVoiLutMenuItem.IsCheckable = true;
            _defaultVoiLutMenuItem.Click += new RoutedEventHandler(voiLutMenuItem_Click);

            _menuItemToMouseMoveDirection.Add(widthHorizontalCenterVerticalMenuItem,
                DicomImageVoiLutMouseMoveDirection.WidthHorizontalCenterVertical);
            _menuItemToMouseMoveDirection.Add(widthVerticalCenterHorizontalMenuItem,
                DicomImageVoiLutMouseMoveDirection.WidthVerticalCenterHorizontal);

            this.Title = string.Format(_titlePrefix, "(Untitled)");

            // update the UI
            UpdateUI();

            _isInitialized = true;
        }

        #endregion



        #region Properties

        bool _isDicomFileOpening = false;
        /// <summary>
        /// Gets or sets a value indicating whether the DICOM file is opening.
        /// </summary>    
        bool IsDicomFileOpening
        {
            get
            {
                return _isDicomFileOpening;
            }
            set
            {
                _isDicomFileOpening = value;
                InvokeUpdateUI();
            }
        }

        bool _isFileSaving = false;
        /// <summary>
        /// Gets or sets a value indicating whether file is saving.
        /// </summary>
        bool IsFileSaving
        {
            get
            {
                return _isFileSaving;
            }
            set
            {
                _isFileSaving = value;
                InvokeUpdateUI();
            }
        }

        bool _isAnimationStarted = false;
        /// <summary>
        /// Gets or sets a value indicating whether the animation is started.
        /// </summary>
        bool IsAnimationStarted
        {
            get
            {
                return _isAnimationStarted;
            }
            set
            {
                if (_isAnimationStarted == value)
                    return;

                if (value)
                    StartAnimation();
                else
                    StopAnimation();

                imageViewerToolBar.IsNavigationEnabled = !value;
                UpdateUI();
            }
        }

        /// <summary>
        /// Gets the DICOM file of focused image.
        /// </summary>    
        DicomFile DicomFile
        {
            get
            {
                VintasoftImage image = imageViewer1.Image;
                if (image != null)
                    return _dicomSeriesController.GetDicomFile(image);

                return _dicomFileWithoutImages;
            }
        }

        /// <summary>
        /// Gets the DICOM frame of focused image.
        /// </summary>
        DicomFrame DicomFrame
        {
            get
            {
                VintasoftImage image = imageViewer1.Image;

                return DicomFrame.GetFrameAssociatedWithImage(image);
            }
        }

        /// <summary>
        /// Gets the DICOM presentation state file of <paramref name="DicomFile"/>.
        /// </summary>
        DicomFile PresentationStateFile
        {
            get
            {
                if (DicomFile == null)
                    return null;

                return PresentationStateFileController.GetPresentationStateFile(DicomFile);
            }
        }

        #endregion



        #region Methods

        #region PRIVATE

        #region UI

        #region Main Window

        /// <summary>
        /// Handles the Closing event of Window object.
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            IsAnimationStarted = false;
            _isWindowClosing = true;

            // close the previously opened DICOM files
            ClosePreviouslyOpenedFiles();
        }

        #endregion


        #region 'File' menu

        /// <summary>
        /// Handles the Click event of OpenDicomFilesMenuItem object.
        /// </summary>
        private void openDicomFilesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenDicomFile();
        }

        /// <summary>
        /// Handles the Click event of SaveDicomFileToImageFileMenuItem object.
        /// </summary>
        private void saveDicomFileToImageFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageCollection images = imageViewer1.Images;

            WpfDicomAnnotationTool annotationTool = _dicomViewerTool.DicomAnnotationTool;
            if (annotationTool.AnnotationDataCollection.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show(
                    "DICOM annotations cannot be converted into Vintasoft annotations but annotations can be burned on image.\r\n" +
                    "Burn annotations on images?\r\n" +
                    "Press 'Yes' if you want save images with burned annotations.\r\n" +
                    "Press 'No' if you want save images without annotations.\r\n" +
                    "Press 'Cancel' to cancel saving.",
                    "Annotations",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Cancel)
                    return;

                if (result == MessageBoxResult.Yes)
                {
                    ImageCollection imagesWithAnnotations = new ImageCollection();

                    using (WpfAnnotationViewController viewController = new WpfAnnotationViewController(
                        annotationTool.AnnotationDataController))
                    {
                        for (int i = 0; i < images.Count; i++)
                            imagesWithAnnotations.Add(viewController.GetImageWithAnnotations(i));
                    }

                    images = imagesWithAnnotations;
                    images.ImageCollectionSavingProgress += new EventHandler<ProgressEventArgs>(Images_ImageCollectionSavingProgress);
                    images.ImageCollectionSavingFinished += new EventHandler(Images_ImageCollectionSavingFinished);
                }
            }

            bool multipage = images.Count > 1;

            CodecsFileFilters.SetFiltersWithAnnotations(_saveFileDialog, multipage);
            if (_saveFileDialog.ShowDialog() == true)
            {
                EncoderBase encoder = null;
                try
                {
                    IsFileSaving = true;
                    string saveFilename = Path.GetFullPath(_saveFileDialog.FileName);
                    if (multipage)
                        encoder = GetMultipageEncoder(saveFilename);
                    else
                        encoder = GetEncoder(saveFilename);
                    if (encoder != null)
                    {
                        progressBar1.Maximum = 100;
                        progressBar1.Value = 0;
                        progressBar1.Visibility = Visibility.Visible;
                        // save the image
                        images.SaveAsync(saveFilename, encoder);
                    }
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                    progressBar1.Visibility = Visibility.Collapsed;
                    IsFileSaving = false;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of CloseDicomSeriesMenuItem object.
        /// </summary>
        private void closeDicomSeriesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // close DICOM file
            CloseDicomSeries();
        }

        /// <summary>
        /// Handles the Click event of ExitMenuItem object.
        /// </summary>
        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion


        #region 'Edit' menu

        /// <summary>
        /// Handles the SubmenuOpened event of EditMenuItem object.
        /// </summary>
        private void editMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            UpdateEditMenuItems();
        }

        /// <summary>
        /// Handles the SubmenuClosed event of EditMenuItem object.
        /// </summary>
        private void editMenuItem_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            EnableEditMenuItems();
        }

        /// <summary>
        /// Handles the Click event of CutMenuItem object.
        /// </summary>
        private void cutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUiAction<CutItemUIAction>();
        }

        /// <summary>
        /// Handles the Click event of CopyMenuItem object.
        /// </summary>
        private void copyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUiAction<CopyItemUIAction>();
        }

        /// <summary>
        /// Handles the Click event of PasteMenuItem object.
        /// </summary>
        private void pasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUiAction<PasteItemUIAction>();
        }

        /// <summary>
        /// Handles the Click event of DeleteMenuItem object.
        /// </summary>
        private void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUiAction<DeleteItemUIAction>();
        }

        /// <summary>
        /// Handles the Click event of DeleteAllMenuItem object.
        /// </summary>
        private void deleteAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUiAction<DeleteAllItemsUIAction>();
        }

        #endregion


        #region 'View' menu

        #region Thumbnail viewer settings

        /// <summary>
        /// Handles the Click event of ThumbnailViewerSettingsMenuItem object.
        /// </summary>
        private void thumbnailViewerSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ThumbnailViewerSettingsWindow dlg = new ThumbnailViewerSettingsWindow(thumbnailViewer1, (Style)Resources["ThumbnailItemStyle"]);
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ShowDialog();
        }

        #endregion


        #region Image viewer settings

        /// <summary>
        /// Handles the Click event of ImageViewerSettingsMenuItem object.
        /// </summary>
        private void imageViewerSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageViewerSettingsWindow dlg = new ImageViewerSettingsWindow(imageViewer1);
            dlg.CanEditMultipageSettings = false;
            dlg.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of RotateClockwiseMenuItem object.
        /// </summary>
        private void rotateClockwiseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RotateViewClockwise();
        }

        /// <summary>
        /// Handles the Click event of RotateCounterclockwiseMenuItem object.
        /// </summary>
        private void rotateCounterclockwiseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RotateViewCounterClockwise();
        }

        #endregion


        #region Overlay images

        /// <summary>
        /// Handles the Click event of ShowOverlayImagesMenuItem object.
        /// </summary>
        private void showOverlayImagesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // change decoding settings
            _dicomFrameDecodingSettings.ShowOverlayImages = showOverlayImagesMenuItem.IsChecked;

            // invalidates images and visual tool
            _dicomViewerTool.DicomViewerTool.Refresh();
        }

        /// <summary>
        /// Handles the Click event of OverlayColorMenuItem object.
        /// </summary>
        private void overlayColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // get filling color from decoding settings
            Rgb24Color rgb24Color = _dicomFrameDecodingSettings.OverlayColor;
            ColorPickerDialog dlg = new ColorPickerDialog();
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Owner = this;
            // init dialog
            dlg.StartingColor = Color.FromArgb(255, rgb24Color.Red, rgb24Color.Green, rgb24Color.Blue);
            // show dialog
            if (dlg.ShowDialog() == true)
            {
                // update filling color in decoding settings
                _dicomFrameDecodingSettings.OverlayColor = new Rgb24Color(dlg.SelectedColor.R,
                    dlg.SelectedColor.G, dlg.SelectedColor.B);

                _dicomViewerTool.DicomViewerTool.Refresh();
            }
        }

        #endregion


        #region Metadata

        /// <summary>
        /// Handles the Click event of ShowMetadataOnViewerMenuItem object.
        /// </summary>
        private void showMetadataOnViewerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showMetadataInViewerMenuItem.IsChecked ^= true;
            _dicomViewerTool.DicomViewerTool.IsTextOverlayVisible = showMetadataInViewerMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the Click event of TextOverlaySettingsMenuItem object.
        /// </summary>
        private void textOverlaySettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DicomOverlaySettingEditorWindow dlg = new DicomOverlaySettingEditorWindow(OVERLAY_OWNER_NAME, _dicomViewerTool.DicomViewerTool);
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Owner = this;
            // show dialog
            dlg.ShowDialog();

            // set text overlay for DICOM viewer tool
            DicomOverlaySettingEditorWindow.SetTextOverlay(OVERLAY_OWNER_NAME, _dicomViewerTool.DicomViewerTool);
            // refresh the DICOM viewer tool
            _dicomViewerTool.DicomViewerTool.Refresh();
        }

        #endregion


        #region Rulers

        /// <summary>
        /// Handles the Click event of ShowRulersOnViewerMenuItem object.
        /// </summary>
        private void showRulersOnViewerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showRulersInViewerMenuItem.IsChecked ^= true;
            _dicomViewerTool.DicomViewerTool.ShowRulers = showRulersInViewerMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the Click event of RulersColorMenuItem object.
        /// </summary>
        private void rulersColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Brush brush = _dicomViewerTool.DicomViewerTool.VerticalImageRuler.RulerPen.Brush;
            SolidColorBrush solidColorBrush = (SolidColorBrush)brush;
            ColorPickerDialog dlg = new ColorPickerDialog();
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Owner = this;
            // init dialog
            dlg.StartingColor = solidColorBrush.Color;
            // show dialog
            if (dlg.ShowDialog() == true)
            {
                // update rulers
                _dicomViewerTool.DicomViewerTool.VerticalImageRuler.RulerPen = new Pen(new SolidColorBrush(dlg.SelectedColor),
                      _dicomViewerTool.DicomViewerTool.VerticalImageRuler.RulerPen.Thickness);
                _dicomViewerTool.DicomViewerTool.HorizontalImageRuler.RulerPen = new Pen(new SolidColorBrush(dlg.SelectedColor),
                      _dicomViewerTool.DicomViewerTool.HorizontalImageRuler.RulerPen.Thickness);

                // refresh DICOM viewer tool
                _dicomViewerTool.DicomViewerTool.Refresh();
            }
        }

        /// <summary>
        /// Handles the Click event of RulersUnitOfMeasureMenuItem object.
        /// </summary>
        private void rulersUnitOfMeasureMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _currentRulersUnitOfMeasureMenuItem.IsChecked = false;
            _currentRulersUnitOfMeasureMenuItem = (MenuItem)e.OriginalSource;
            _dicomViewerTool.DicomViewerTool.RulersUnitOfMeasure =
                _menuItemToRulersUnitOfMeasure[_currentRulersUnitOfMeasureMenuItem];
            _currentRulersUnitOfMeasureMenuItem.IsChecked = true;
        }

        #endregion


        #region VOI LUT

        /// <summary>
        /// Handles the Click event of VoiLutMainMenuItem object.
        /// </summary>
        private void voiLutMainMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowCustomVoiLutForm();
        }

        /// <summary>
        /// Handles the Closing event of VoiLutParamsWindow object.
        /// </summary>
        private void voiLutParamsWindow_Closing(object sender, CancelEventArgs e)
        {
            // if application is not closing
            if (!_isWindowClosing)
            {
                DicomFrameMetadata metadata = GetFocusedImageMetadata();

                // if image viewer contains image 
                if (metadata != null)
                {
                    if (metadata.ColorSpace == DicomImageColorSpaceType.Monochrome1 ||
                        metadata.ColorSpace == DicomImageColorSpaceType.Monochrome2)
                        _voiLutsButton.Visibility = Visibility.Visible;
                    else
                        _voiLutsButton.Visibility = Visibility.Collapsed;
                    voiLutToolBar.Visibility = _voiLutsButton.Visibility;
                }

                voiLutMainMenuItem.IsChecked = false;
                _voiLutParamsWindow = null;
            }
        }

        /// <summary>
        /// Handles the Click event of NegativeImageMenuItem object.
        /// </summary>
        private void negativeImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            negativeImageMenuItem.IsChecked ^= true;
            _dicomViewerTool.DicomViewerTool.IsImageNegative = negativeImageMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the DicomImageVoiLutChanged event of DicomViewerTool object.
        /// </summary>
        private void dicomViewerTool_DicomImageVoiLutChanged(object sender, WpfVoiLutChangedEventArgs e)
        {
            // if there is selected VOI LUT menu item
            if (_currentVoiLutMenuItem != null)
            {
                // clear the selected VOI LUT menu item
                _currentVoiLutMenuItem.IsChecked = false;
                _currentVoiLutMenuItem = null;
            }
            
            ItemCollection items = _voiLutsButton.Items;
            // for each VOI LUT menu item
            foreach (object item in items)
            {
                if (item is MenuItem)
                {
                    // VOI LUT menu item
                    MenuItem toolStripItem = (MenuItem)item;

                    if (_menuItemToVoiLut.ContainsKey(toolStripItem))
                    {
                        // VOI LUT table, which is associated with VOI LUT menu item
                        DicomImageVoiLookupTable window = _menuItemToVoiLut[toolStripItem];

                        // if VOI LUT menu item parameters are equal to the parameters of new VOI LUT
                        if (window.WindowCenter == e.WindowCenter &&
                            window.WindowWidth == e.WindowWidth)
                        {
                            // set the VOI LUT menu item as selected
                            _currentVoiLutMenuItem = toolStripItem;
                            _currentVoiLutMenuItem.IsChecked = true;
                            break;
                        }
                    }
                }
            }

            // the default VOI LUT
            DicomImageVoiLookupTable defaultVoiLut =
                _dicomViewerTool.DicomViewerTool.DefaultDicomImageVoiLut;

            // if the default VOI LUT is equal to new VOI LUT
            if (defaultVoiLut.WindowCenter == e.WindowCenter &&
                defaultVoiLut.WindowWidth == e.WindowWidth)
            {
                // specify that DICOM viewer tool must use VOI LUT from DICOM image metadata for DICOM image
                _dicomViewerTool.DicomViewerTool.AlwaysLoadVoiLutFromMetadataOfDicomFrame = true;
            }
            else
            {
                // specify that DICOM viewer tool must use the same VOI LUT for all DICOM images
                _dicomViewerTool.DicomViewerTool.AlwaysLoadVoiLutFromMetadataOfDicomFrame = false;
            }
        }

        /// <summary>
        /// Handles the Click event of VoiLutMouseMoveDirectionMenuItem object.
        /// </summary>
        private void voiLutMouseMoveDirectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            widthHorizontalCenterVerticalMenuItem.IsChecked = false;
            widthVerticalCenterHorizontalMenuItem.IsChecked = false;
            MenuItem voiLutMouseMoveDirectionMenuItem = (MenuItem)sender;
            voiLutMouseMoveDirectionMenuItem.IsChecked = true;

            _dicomViewerTool.DicomViewerTool.DicomImageVoiLutMouseMoveDirection =
                _menuItemToMouseMoveDirection[voiLutMouseMoveDirectionMenuItem];
        }

        #endregion


        #region Magnifier

        /// <summary>
        /// Handles the Click event of MagnifierSettings object.
        /// </summary>
        private void magnifierSettings_Click(object sender, RoutedEventArgs e)
        {
            MagnifierToolAction magnifierToolAction = dicomAnnotatedViewerToolBar.FindAction<MagnifierToolAction>();

            if (magnifierToolAction != null)
                magnifierToolAction.ShowVisualToolSettings();
        }

        #endregion

        #endregion


        #region 'Metadata' menu

        /// <summary>
        /// Handles the Click event of FileMetadataMenuItem object.
        /// </summary>
        private void fileMetadataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowCurrentFileMetadata();
        }

        #endregion


        #region 'Page' menu

        /// <summary>
        /// Handles the Click event of OverlayImagesMenuItem object.
        /// </summary>
        private void overlayImagesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (imageViewer1.Image != null)
            {
                OverlayImagesViewer window = new OverlayImagesViewer(imageViewer1.Image);
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.Owner = this;
                window.ShowDialog();
            }
        }

        #endregion


        #region 'Animation' menu

        /// <summary>
        /// Handles the Click event of ShowAnimationMenuItem object.
        /// </summary>
        private void showAnimationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IsAnimationStarted = showAnimationMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the Click event of AnimationRepeatMenuItem object.
        /// </summary>
        private void animationRepeatMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _isAnimationCycled = animationRepeatMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the TextChanged event of AnimationDelayComboBox object.
        /// </summary>
        private void animationDelayComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int delay;
            if (int.TryParse(animationDelay_valueComboBox.Text, out delay))
                _animationDelay = Math.Max(1, delay);
            else
                animationDelay_valueComboBox.Text = _animationDelay.ToString();
        }

        /// <summary>
        /// Handles the Click event of SaveAsGifFileToolStripMenuItem object.
        /// </summary>
        private void saveAsGifFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageCollection images = imageViewer1.Images;

            WpfDicomAnnotationTool annotationTool = _dicomViewerTool.DicomAnnotationTool;
            if (annotationTool.AnnotationDataCollection.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show(
                    "DICOM annotations cannot be converted into Vintasoft annotations but annotations can be burned on image.\r\n" +
                    "Burn annotations on images?\r\n" +
                    "Press 'Yes' if you want save images with burned annotations.\r\n" +
                    "Press 'No' if you want save images without annotations.\r\n" +
                    "Press 'Cancel' to cancel saving.",
                    "Annotations",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Cancel)
                    return;

                if (result == MessageBoxResult.Yes)
                {
                    ImageCollection imagesWithAnnotations = new ImageCollection();

                    using (WpfAnnotationViewController viewController = new WpfAnnotationViewController(
                        annotationTool.AnnotationDataController))
                    {
                        for (int i = 0; i < images.Count; i++)
                            imagesWithAnnotations.Add(viewController.GetImageWithAnnotations(i));
                    }

                    images = imagesWithAnnotations;
                    images.ImageCollectionSavingProgress += new EventHandler<ProgressEventArgs>(Images_ImageCollectionSavingProgress);
                    images.ImageCollectionSavingFinished += new EventHandler(Images_ImageCollectionSavingFinished);
                }
            }

            if (_saveGifFileDialog.ShowDialog() == true)
            {
                try
                {
                    // specify that image saving is started
                    IsFileSaving = true;

                    // get filename from Save dialog
                    string saveFilename = _saveGifFileDialog.FileName;
                    // if filename does not have ".GIF" extension
                    if (Path.GetExtension(saveFilename).ToUpperInvariant() != ".GIF")
                    {
                        // change file extension to ".gif"
                        saveFilename = Path.Combine(Path.GetDirectoryName(saveFilename), Path.GetFileNameWithoutExtension(saveFilename) + ".gif");
                    }

                    // create GIF encoder
                    using (GifEncoder gifEncoder = new GifEncoder())
                    {
                        // get the animation delay
                        int animationDelay = int.Parse(animationDelay_valueComboBox.Text);
                        // set animation delay in GIF encoder
                        gifEncoder.Settings.AnimationDelay = Math.Max(1, animationDelay / 10);
                        // set infinite animation flag in GIF encoder
                        gifEncoder.Settings.InfiniteAnimation = animationRepeatMenuItem.IsChecked;

                        progressBar1.Maximum = 100;
                        progressBar1.Minimum = 0;
                        progressBar1.Value = 0;
                        progressBar1.Visibility = Visibility.Visible;

                        // save images to a GIF file
                        images.SaveAsync(saveFilename, gifEncoder);
                    }
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                    progressBar1.Visibility = Visibility.Collapsed;
                    IsFileSaving = false;
                }
            }
        }

        #endregion


        #region 'Annotation' menu

        /// <summary>
        /// Handles the Click event of InfoMenuItem object.
        /// </summary>
        private void infoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AnnotationsInfoWindow dialog = new AnnotationsInfoWindow(_dicomViewerTool.DicomAnnotationTool.AnnotationDataController);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of InteractionModeNoneMenuItem object.
        /// </summary>
        private void interactionModeNoneMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.None;
        }

        /// <summary>
        /// Handles the Click event of InteractionModeViewMenuItem object.
        /// </summary>
        private void interactionModeViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.View;
        }

        /// <summary>
        /// Handles the Click event of InteractionModeAuthorMenuItem object.
        /// </summary>
        private void interactionModeAuthorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.Author;
        }

        /// <summary>
        /// Handles the SubmenuOpened event of AnnotationsMenuItem object.
        /// </summary>
        private void annotationsMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            WpfAnnotationView focusedAnnotationView = _dicomViewerTool.DicomAnnotationTool.FocusedAnnotationView;
            if (focusedAnnotationView != null &&
               (focusedAnnotationView is WpfDicomPolylineAnnotationView ||
                focusedAnnotationView is WpfDicomMultilineAnnotationView ||
                focusedAnnotationView is WpfDicomRangeLineAnnotationView ||
                focusedAnnotationView is WpfDicomInfiniteLineAnnotationView ||
                focusedAnnotationView is WpfDicomArrowAnnotationView ||
                focusedAnnotationView is WpfDicomAxisAnnotationView ||
                focusedAnnotationView is WpfDicomRulerAnnotationView))
            {
                transformationModeMenuItem.IsEnabled = true;
                UpdateTransformationMenu();
            }
            else
            {
                transformationModeMenuItem.IsEnabled = false;
            }
        }
    
        /// <summary>
        /// Handles the Click event of TransformationModeRectangularMenuItem object.
        /// </summary>
        private void transformationModeRectangularMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfAnnotationView focusedAnnotationView = _dicomViewerTool.DicomAnnotationTool.FocusedAnnotationView;
            SetGripMode(focusedAnnotationView, GripMode.Rectangular);
            UpdateTransformationMenu();
        }

        /// <summary>
        /// Handles the Click event of TransformationModePointsMenuItem object.
        /// </summary>
        private void transformationModePointsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            WpfAnnotationView focusedAnnotationView = _dicomViewerTool.DicomAnnotationTool.FocusedAnnotationView;
            SetGripMode(focusedAnnotationView, GripMode.Points);
            UpdateTransformationMenu();
        }

        /// <summary>
        /// Handles the Click event of TransformationModeRectangularAndPointsMenuItem object.
        /// </summary>
        private void transformationModeRectangularAndPointsMenuItem_Click(
            object sender,
            RoutedEventArgs e)
        {
            WpfAnnotationView focusedAnnotationView = _dicomViewerTool.DicomAnnotationTool.FocusedAnnotationView;
            SetGripMode(focusedAnnotationView, GripMode.RectangularAndPoints);
            UpdateTransformationMenu();
        }


        /// <summary>
        /// Handles the Click event of PresentationStateLoadMenuItem object.
        /// </summary>
        private void presentationStateLoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _openDicomAnnotationsFileDialog.FileName = null;
            _openDicomAnnotationsFileDialog.Filter = "Presentation State File(*.pre)|*.pre|All Formats(*.*)|*.*";
            _openDicomAnnotationsFileDialog.FilterIndex = 1;
            _openDicomAnnotationsFileDialog.Multiselect = false;

            if (_openDicomAnnotationsFileDialog.ShowDialog() == true)
            {
                DicomFile presentationStateFile = null;
                try
                {
                    CloseCurrentPresentationStateFile();

                    string fileName = _openDicomAnnotationsFileDialog.FileName;
                    presentationStateFile = new DicomFile(fileName, false);
                    if (presentationStateFile.IsReferencedTo(DicomFile) &&
                        presentationStateFile.Annotations != null)
                    {
                        _isAnnotationsLoadedForCurrentFrame = false;
                        _dicomViewerTool.DicomAnnotationTool.AnnotationDataController.AddAnnotationDataSet(presentationStateFile.Annotations);
                        PresentationStateFileController.UpdatePresentationStateFile(DicomFile, presentationStateFile);
                    }
                    else
                    {
                        presentationStateFile.Dispose();
                        presentationStateFile = null;
                    }

                    UpdateUI();
                }
                catch (Exception ex)
                {
                    if (presentationStateFile != null)
                        presentationStateFile.Dispose();

                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of PresentationStateInfoMenuItem object.
        /// </summary>
        private void presentationStateInfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            PresentationStateInfoWindow dialog = new PresentationStateInfoWindow(PresentationStateFile);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of PresentationStateSaveMenuItem object.
        /// </summary>
        private void presentationStateSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_isAnnotationsLoadedForCurrentFrame)
            {
                DicomAnnotationCodec codec = new DicomAnnotationCodec();
                DicomAnnotationDataCollection collection = (DicomAnnotationDataCollection)
                    _dicomViewerTool.DicomAnnotationTool.AnnotationDataController.GetAnnotations(imageViewer1.Image);
                codec.Encode(PresentationStateFile.Annotations, collection);
                PresentationStateFile.SaveChanges();
            }
            else
            {
                _dicomViewerTool.DicomAnnotationTool.AnnotationDataController.UpdateAnnotationDataSets();
                PresentationStateFile.SaveChanges();
            }
            MessageBox.Show("Presentation state file is saved.");
        }

        /// <summary>
        /// Handles the Click event of PresentationStateSaveToMenuItem object.
        /// </summary>
        private void presentationStateSaveToMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string dicomFilePath = _dicomSeriesController.GetDicomFilePath(DicomFile);
            _saveDicomAnnotationsFileDialog.FileName = Path.GetFileNameWithoutExtension(dicomFilePath) + ".pre";
            _saveDicomAnnotationsFileDialog.Filter = "Presentation State File(*.pre)|*.pre";
            _saveDicomAnnotationsFileDialog.FilterIndex = 1;

            if (_saveDicomAnnotationsFileDialog.ShowDialog() == true)
            {
                try
                {
                    _dicomViewerTool.DicomAnnotationTool.CancelAnnotationBuilding();

                    // get annotations of DICOM file
                    DicomAnnotationDataCollection[] annotations = GetAnnotationsAssociatedWithDicomFileImages(DicomFile);

                    // create presentation state file
                    using (DicomFile presentationStateFile = CreatePresentationStateFile(DicomFile, annotations))
                    {
                        // get file name of presentation state file
                        string fileName = _saveDicomAnnotationsFileDialog.FileName;
                        // if file exist
                        if (File.Exists(fileName))
                            // remove file
                            File.Delete(fileName);

                        // save presentation state file
                        presentationStateFile.Save(fileName);
                    }
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of BinaryFormatLoadMenuItem object.
        /// </summary>
        private void binaryFormatLoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadAnnotationFromBinaryOrXmpFormat(true);
        }

        /// <summary>
        /// Handles the Click event of BinaryFormatSaveToMenuItem object.
        /// </summary>
        private void binaryFormatSaveToMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _saveDicomAnnotationsFileDialog.FileName = null;
            _saveDicomAnnotationsFileDialog.Filter = "Binary Annotations(*.vsab)|*.vsab";
            _saveDicomAnnotationsFileDialog.FilterIndex = 1;

            if (_saveDicomAnnotationsFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (FileStream fs = new FileStream(_saveDicomAnnotationsFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        AnnotationVintasoftBinaryFormatter annotationFormatter = new AnnotationVintasoftBinaryFormatter();
                        //
                        AnnotationDataCollection annotations = _dicomViewerTool.DicomAnnotationTool.AnnotationDataController.GetAnnotations(
                            imageViewer1.Image);
                        //
                        annotationFormatter.Serialize(fs, annotations);
                    }
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of XmpFormatLoadMenuItem object.
        /// </summary>
        private void xmpFormatLoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadAnnotationFromBinaryOrXmpFormat(false);
        }

        /// <summary>
        /// Handles the Click event of XmpFormatSaveToMenuItem object.
        /// </summary>
        private void xmpFormatSaveToMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _saveDicomAnnotationsFileDialog.FileName = null;
            _saveDicomAnnotationsFileDialog.Filter = "XMP Annotations(*.xmp)|*.xmp";
            _saveDicomAnnotationsFileDialog.FilterIndex = 1;

            if (_saveDicomAnnotationsFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (FileStream fs = new FileStream(_saveDicomAnnotationsFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        AnnotationVintasoftXmpFormatter annotationFormatter = new AnnotationVintasoftXmpFormatter();

                        //
                        AnnotationDataCollection annotations = _dicomViewerTool.DicomAnnotationTool.AnnotationDataController.GetAnnotations(
                            imageViewer1.Image);
                        //
                        annotationFormatter.Serialize(fs, annotations);
                    }
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of AddMenuItem object.
        /// </summary>
        private void addMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            if (_dicomViewerTool.DicomAnnotationTool.FocusedAnnotationView != null &&
                _dicomViewerTool.DicomAnnotationTool.FocusedAnnotationView.InteractionController ==
                _dicomViewerTool.DicomAnnotationTool.FocusedAnnotationView.Builder)
                _dicomViewerTool.DicomAnnotationTool.CancelAnnotationBuilding();
            annotationsToolBar.BuildAnnotation(item.Header.ToString());
        }

        #endregion


        #region 'Help' menu

        /// <summary>
        /// Handles the Click event of aboutMenuItem property of Help object.
        /// </summary>
        private void help_aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder description = new StringBuilder();

            description.AppendLine("This project demonstrates how to preview DICOM files and allows to:");
            description.AppendLine();
            description.AppendLine("- Preview single- and multipage DICOM files.");
            description.AppendLine();
            description.AppendLine("- Display DICOM image with necessary VOI LUT (value of interest lookup table).");
            description.AppendLine();
            description.AppendLine("- Display DICOM image with/without overlays.");
            description.AppendLine();
            description.AppendLine("- Animate DICOM images.");
            description.AppendLine();
            description.AppendLine("- View metadata of DICOM file.");
            description.AppendLine();
            description.AppendLine("- Annotate DICOM images.");
            description.AppendLine();

            description.AppendLine();
            description.AppendLine("The project is available in C# and VB.NET for Visual Studio .NET.");

            WpfAboutBoxBaseWindow dlg = new WpfAboutBoxBaseWindow("vsdicom-dotnet");
            dlg.Description = description.ToString();
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.ShowDialog();
        }

        #endregion


        #region File Manipulation

        /// <summary>
        /// Handles the OpenFile event of ImageViewerToolBar object.
        /// </summary>
        private void imageViewerToolBar_OpenFile(object sender, EventArgs e)
        {
            OpenDicomFile();
        }

        #endregion


        #region Image Viewer

        /// <summary>
        /// Handles the ImageLoadingProgress event of ImageViewer1 object.
        /// </summary>
        private void imageViewer1_ImageLoadingProgress(object sender, ProgressEventArgs e)
        {
            if (_isWindowClosing)
            {
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// Handles the FocusedIndexChanged event of ImageViewer1 object.
        /// </summary>
        private void imageViewer1_FocusedIndexChanged(object sender, PropertyChangedEventArgs<int> e)
        {
            if (!_isInitialized)
                return;

            UpdateUI();

            // get the focused image
            VintasoftImage image = imageViewer1.Image;
            string info = string.Empty;
            // if image viewer has focused image
            if (image != null)
            {
                // create information about focused image
                info = string.Format("Size={0}x{1}; PixelFormat={2}; Resolution={3}",
                   image.Width, image.Height, image.PixelFormat, image.Resolution.ToString());

                // get DICOM frame, which is associated with focused image
                DicomFrame frame = DicomFrame.GetFrameAssociatedWithImage(image);
                // if DICOM frame exists
                if (frame != null)
                {
                    // save information about VOI LUT of DICOM frame
                    _menuItemToVoiLut[_defaultVoiLutMenuItem] = frame.VoiLut;
                }
            }

            // update image info
            imageInfoStatusLabel.Content = info;

            if (!_isFocusedIndexChanging)
            {
                _isFocusedIndexChanging = true;
                try
                {
                    // if animation is executed
                    if (IsAnimationStarted)
                    {
                        // disable animation
                        IsAnimationStarted = false;
                        // uncheck the "Show Animation" menu
                        showAnimationMenuItem.IsChecked = false;
                    }
                    else
                    {
                        if (_voiLutParamsWindow != null)
                            _voiLutParamsWindow.DicomFrame = imageViewer1.Image;
                    }

                    imageViewerToolBar.SelectedPageIndex = e.NewValue;
                }
                finally
                {
                    _isFocusedIndexChanging = false;
                }
            }
        }

        /// <summary>
        /// Handles the Activated event of NoneAction object.
        /// </summary>
        private void noneAction_Activated(object sender, EventArgs e)
        {
            // restore the DICOM viewer tool state
            dicomAnnotatedViewerToolBar.MainVisualTool.ActiveTool = dicomAnnotatedViewerToolBar.DicomAnnotatedViewerTool;
            _dicomViewerTool.InteractionMode = _prevDicomViewerToolInteractionMode;
            _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionMode = _prevDicomAnnotationToolInteractionMode;
        }

        /// <summary>
        /// Handles the Deactivated event of NoneAction object.
        /// </summary>
        private void noneAction_Deactivated(object sender, EventArgs e)
        {
            // save the DICOM viewer tool state

            _prevDicomViewerToolInteractionMode = _dicomViewerTool.InteractionMode;
            _prevDicomAnnotationToolInteractionMode = _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionMode;
        }

        /// <summary>
        /// Handles the Activated event of ImageMeasureToolAction object.
        /// </summary>
        private void imageMeasureToolAction_Activated(object sender, EventArgs e)
        {
            _isVisualToolChanging = true;
            dicomAnnotatedViewerToolBar.MainVisualTool.ActiveTool = dicomAnnotatedViewerToolBar.DicomAnnotatedViewerTool;
            _dicomViewerTool.InteractionMode = WpfDicomAnnotatedViewerToolInteractionMode.Measuring;
            _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.None;
            _isVisualToolChanging = false;
        }

        /// <summary>
        /// Handles the Activated event of MagnifierToolAction object.
        /// </summary>
        private void magnifierToolAction_Activated(object sender, EventArgs e)
        {
            _isVisualToolChanging = true;
            dicomAnnotatedViewerToolBar.MainVisualTool.ActiveTool =
                dicomAnnotatedViewerToolBar.MainVisualTool.FindVisualTool<WpfMagnifierTool>();
            _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.None;
            _isVisualToolChanging = false;
        }

        /// <summary>
        /// Handles the PageIndexChanged event of ImageViewerToolBar object.
        /// </summary>
        private void imageViewerToolBar_PageIndexChanged(object sender, PageIndexChangedEventArgs e)
        {
            if (!IsAnimationStarted)
                imageViewer1.FocusedIndex = e.SelectedPageIndex;
        }

        #endregion


        #region Annotations UI

        /// <summary>
        /// Handles the DropDownOpened event of AnnotationComboBox object.
        /// </summary>
        private void annotationComboBox_DropDownOpened(object sender, EventArgs e)
        {
            FillAnnotationComboBox();
        }

        /// <summary>
        /// Handles the SelectionChanged event of AnnotationComboBox object.
        /// </summary>
        private void annotationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (imageViewer1.FocusedIndex != -1 && annotationComboBox.SelectedIndex != -1)
            {
                _dicomViewerTool.DicomAnnotationTool.FocusedAnnotationData =
                    _dicomViewerTool.DicomAnnotationTool.AnnotationDataCollection[annotationComboBox.SelectedIndex];
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of AnnotationInteractionModeComboBox object.
        /// </summary>
        private void annotationInteractionModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionMode =
                (AnnotationInteractionMode)annotationInteractionModeComboBox.SelectedItem;
        }

        /// <summary>
        /// Handles the AnnotationInteractionModeChanged event of AnnotationTool object.
        /// </summary>
        private void annotationTool_AnnotationInteractionModeChanged(object sender, AnnotationInteractionModeChangedEventArgs e)
        {
            if (!_isVisualToolChanging)
                dicomAnnotatedViewerToolBar.Reset();

            interactionModeNoneMenuItem.IsChecked = false;
            interactionModeViewMenuItem.IsChecked = false;
            interactionModeAuthorMenuItem.IsChecked = false;

            WpfDicomAnnotatedViewerToolInteractionMode visualToolInteractionMode =
                 _dicomViewerTool.InteractionMode;

            AnnotationInteractionMode annotationInteractionMode = e.NewValue;
            switch (annotationInteractionMode)
            {
                case AnnotationInteractionMode.None:
                    interactionModeNoneMenuItem.IsChecked = true;
                    visualToolInteractionMode = WpfDicomAnnotatedViewerToolInteractionMode.Dicom;
                    break;

                case AnnotationInteractionMode.View:
                    interactionModeViewMenuItem.IsChecked = true;
                    visualToolInteractionMode = WpfDicomAnnotatedViewerToolInteractionMode.Dicom;
                    break;

                case AnnotationInteractionMode.Author:
                    interactionModeAuthorMenuItem.IsChecked = true;
                    visualToolInteractionMode = WpfDicomAnnotatedViewerToolInteractionMode.Annotation;
                    break;
            }

            if (!_isVisualToolChanging)
                _dicomViewerTool.InteractionMode = visualToolInteractionMode;

            annotationInteractionModeComboBox.SelectedItem = annotationInteractionMode;

            // update the UI
            UpdateUI();
        }

        #endregion


        #region Annotation visual tool

        /// <summary>
        /// Handles the FocusedAnnotationViewChanged event of AnnotationTool object.
        /// </summary>
        private void annotationTool_FocusedAnnotationViewChanged(object sender, WpfAnnotationViewChangedEventArgs e)
        {
            if (e.OldValue != null)
                e.OldValue.Data.PropertyChanging -= new EventHandler<ObjectPropertyChangingEventArgs>(AnnotationdData_PropertyChanging);
            if (e.NewValue != null)
                e.NewValue.Data.PropertyChanging += new EventHandler<ObjectPropertyChangingEventArgs>(AnnotationdData_PropertyChanging);

            FillAnnotationComboBox();
            ShowAnnotationProperties(_dicomViewerTool.DicomAnnotationTool.FocusedAnnotationView);

            // update the UI
            UpdateUI();
        }

        /// <summary>
        /// Handles the AnnotationBuildingFinished event of AnnotationTool object.
        /// </summary>
        private void annotationTool_AnnotationBuildingFinished(object sender, WpfAnnotationViewEventArgs e)
        {
            ShowAnnotationProperties(_dicomViewerTool.DicomAnnotationTool.FocusedAnnotationView);
        }

        /// <summary>
        /// Handles the AnnotationTransformingStarted event of AnnotationTool object.
        /// </summary>
        private void annotationTool_AnnotationTransformingStarted(object sender, WpfAnnotationViewEventArgs e)
        {
            _isAnnotationTransforming = true;
        }

        /// <summary>
        /// Handles the AnnotationTransformingFinished event of AnnotationTool object.
        /// </summary>
        private void annotationTool_AnnotationTransformingFinished(object sender, WpfAnnotationViewEventArgs e)
        {
            _isAnnotationTransforming = false;
            propertyGrid1.Refresh();
        }

        /// <summary>
        /// Handles the Changed event of SelectedAnnotations object.
        /// </summary>
        private void SelectedAnnotations_Changed(object sender, EventArgs e)
        {
            // update the UI
            UpdateUI();
        }

        #endregion


        #region VOI LUT

        /// <summary>
        /// Handles the Click event of VoiLutMenuItem object.
        /// </summary>
        private void voiLutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (imageViewer1.Image != null && sender is MenuItem)
            {
                if (_currentVoiLutMenuItem != null)
                    _currentVoiLutMenuItem.IsChecked = false;

                _currentVoiLutMenuItem = (MenuItem)sender;
                _currentVoiLutMenuItem.IsChecked = true;
                _dicomViewerTool.DicomViewerTool.DicomImageVoiLut = _menuItemToVoiLut[_currentVoiLutMenuItem];
            }
        }

        /// <summary>
        /// Handles the Click event of CustomVoiLutMenuItem object.
        /// </summary>
        private void customVoiLutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowCustomVoiLutForm();
        }

        #endregion


        #region Save Image(s)

        /// <summary>
        /// Handles the ImageCollectionSavingProgress event of Images object.
        /// </summary>
        private void Images_ImageCollectionSavingProgress(object sender, ProgressEventArgs e)
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
                progressBar1.Value = e.Progress;
            else
                Dispatcher.Invoke(new SavingProgressDelegate(Images_ImageCollectionSavingProgress), sender, e);
        }

        /// <summary>
        /// Handles the ImageCollectionSavingFinished event of Images object.
        /// </summary>
        private void Images_ImageCollectionSavingFinished(object sender, EventArgs e)
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
                progressBar1.Visibility = Visibility.Collapsed;
            else
                Dispatcher.Invoke(new SavingFinishedDelegate(Images_ImageCollectionSavingFinished), sender, e);

            IsFileSaving = false;

            ImageCollection images = (ImageCollection)sender;

            if (images != imageViewer1.Images)
                images.ClearAndDisposeItems();
        }

        #endregion

        #endregion


        #region UI state

        /// <summary>
        /// Updates UI safely.
        /// </summary>
        private void InvokeUpdateUI()
        {
            if (Dispatcher.Thread == Thread.CurrentThread)
                UpdateUI();
            else
                Dispatcher.Invoke(new UpdateUIDelegate(UpdateUI));
        }

        /// <summary>
        /// Updates the user interface of this window.
        /// </summary>
        private void UpdateUI()
        {
            // if application is closing
            if (_isWindowClosing)
                // exit
                return;

            bool hasImages = imageViewer1.Images.Count > 0;
            bool isDicomFileLoaded = hasImages || DicomFile != null;
            bool isDicomFileOpening = _isDicomFileOpening;
            bool isAnnotationsFileLoaded = PresentationStateFile != null;
            bool isFileSaving = _isFileSaving;
            bool isMultipageFile = imageViewer1.Images.Count > 1;
            bool isAnimationStarted = IsAnimationStarted;
            bool isImageSelected = imageViewer1.Image != null;
            bool isAnnotationEmpty = true;
            if (isImageSelected)
                isAnnotationEmpty = _dicomViewerTool.DicomAnnotationTool.AnnotationDataController[imageViewer1.FocusedIndex].Count <= 0;
            bool isAnnotationDataControllerEmpty = true;
            DicomAnnotationDataController dataController = _dicomViewerTool.DicomAnnotationTool.AnnotationDataController;
            foreach (VintasoftImage image in imageViewer1.Images)
            {
                if (dataController.GetAnnotations(image).Count > 0)
                {
                    isAnnotationDataControllerEmpty = false;
                    break;
                }
            }
            bool isInteractionModeAuthor = _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionMode == AnnotationInteractionMode.Author;


            bool hasOverlayImages = false;
            bool isMonochromeImage = false;

            DicomFrameMetadata metadata = GetFocusedImageMetadata();
            if (metadata != null)
            {
                hasOverlayImages = metadata.OverlayImages.Length > 0;
                isMonochromeImage = metadata.ColorSpace == DicomImageColorSpaceType.Monochrome1 ||
                                     metadata.ColorSpace == DicomImageColorSpaceType.Monochrome2;
            }

            // 'File' menu
            //
            openDicomFilesMenuItem.IsEnabled = !isDicomFileOpening && !isFileSaving;
            saveDicomFileToImageFileMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving;
            closeDicomSeriesMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving;
            imageViewerToolBar.IsEnabled = !isDicomFileOpening && !isFileSaving;

            // 'View' menu
            //
            showOverlayImagesMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && hasOverlayImages && !isFileSaving;
            overlayColorMenuItem.IsEnabled = showOverlayImagesMenuItem.IsEnabled;
            showMetadataInViewerMenuItem.IsEnabled = !isAnimationStarted;
            showRulersInViewerMenuItem.IsEnabled = !isAnimationStarted;
            rulersUnitOfMeasureMenuItem.IsEnabled = !isAnimationStarted;
            voiLutMainMenuItem.IsEnabled = !isAnimationStarted && isMonochromeImage;

            // 'Metadata' menu
            //
            fileMetadataMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving;

            // 'Page' menu
            //
            overlayImagesMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving && hasOverlayImages;

            // 'Animation' menu
            //
            showAnimationMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving && isMultipageFile;
            animationRepeatMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving && isMultipageFile;
            animationDelayMenuItem.IsEnabled = animationRepeatMenuItem.IsEnabled;
            saveAsGifFileToolStripMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving && isMultipageFile;

            thumbnailViewer1.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving;

            _voiLutsButton.Visibility = (isMonochromeImage && _voiLutParamsWindow == null) ? Visibility.Visible : Visibility.Collapsed;
            voiLutToolBar.Visibility = _voiLutsButton.Visibility;
            _voiLutsButton.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving && !isAnimationStarted;


            // "Annotations" menu
            //
            infoMenuItem.IsEnabled = !isDicomFileOpening && !isFileSaving && isDicomFileLoaded;
            //
            interactionModeMenuItem.IsEnabled = !isDicomFileOpening && !isFileSaving && isDicomFileLoaded;
            //
            presentationStateLoadMenuItem.IsEnabled = !isDicomFileOpening && !isFileSaving && isDicomFileLoaded;
            binaryFormatLoadMenuItem.IsEnabled = !isDicomFileOpening && !isFileSaving && isDicomFileLoaded;
            xmpFormatLoadMenuItem.IsEnabled = !isDicomFileOpening && !isFileSaving && isDicomFileLoaded;
            //
            presentationStateSaveMenuItem.IsEnabled = isAnnotationsFileLoaded;
            presentationStateSaveToMenuItem.IsEnabled = !isAnnotationDataControllerEmpty;
            presentationStateInfoMenuItem.IsEnabled = isAnnotationsFileLoaded;
            binaryFormatSaveToMenuItem.IsEnabled = !isAnnotationEmpty;
            xmpFormatSaveToMenuItem.IsEnabled = !isAnnotationEmpty;

            //
            addMenuItem.IsEnabled = !isDicomFileOpening && !isFileSaving && isDicomFileLoaded && isInteractionModeAuthor;

            // annotation tool strip 
            annotationsToolBar.IsEnabled = !isDicomFileOpening && !isFileSaving && isDicomFileLoaded;

            annotationComboBox.IsEnabled = isInteractionModeAuthor;
            propertyGrid1.Enabled = isInteractionModeAuthor;
        }

        /// <summary>
        /// Updates UI with information about DICOM file.
        /// </summary>
        private void UpdateUIWithInformationAboutDicomFile()
        {
            if (DicomFrame != null)
            {
                UpdateWindowLevelSplitButton(DicomFrame.SourceFile.Modality);
            }
        }

        #endregion


        #region 'Edit' menu

        /// <summary>
        /// Executes the specified type UI action of visual tool.
        /// </summary>
        /// <typeparam name="T">The UI action type.</typeparam>
        private void ExecuteUiAction<T>() where T : UIAction
        {
            // get the UI action
            T uiAction = DemosTools.GetUIAction<T>(imageViewer1.VisualTool);

            if (uiAction != null)
            {
                uiAction.Execute();

                UpdateUI();
            }
        }

        /// <summary>
        /// Enables the "Edit" menu items.
        /// </summary>
        private void EnableEditMenuItems()
        {
            cutMenuItem.IsEnabled = true;
            copyMenuItem.IsEnabled = true;
            pasteMenuItem.IsEnabled = true;
            deleteMenuItem.IsEnabled = true;
            deleteAllMenuItem.IsEnabled = true;
            deleteAllMenuItem.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Updates the "Edit" menu items.
        /// </summary>
        private void UpdateEditMenuItems()
        {
            WpfVisualTool visualTool = imageViewer1.VisualTool;

            UpdateEditMenuItem(cutMenuItem, DemosTools.GetUIAction<CutItemUIAction>(visualTool), "Cut");
            UpdateEditMenuItem(copyMenuItem, DemosTools.GetUIAction<CopyItemUIAction>(visualTool), "Copy");
            UpdateEditMenuItem(pasteMenuItem, DemosTools.GetUIAction<PasteItemUIAction>(visualTool), "Paste");
            UpdateEditMenuItem(deleteMenuItem, DemosTools.GetUIAction<DeleteItemUIAction>(visualTool), "Delete");

            UIAction deleteAllItemsUiAction = DemosTools.GetUIAction<DeleteAllItemsUIAction>(visualTool);
            UpdateEditMenuItem(deleteAllMenuItem, deleteAllItemsUiAction, "Delete All");
            if (deleteAllItemsUiAction == null)
                deleteAllMenuItem.Visibility = Visibility.Collapsed;
            else
                deleteAllMenuItem.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Updates the "Edit" menu item.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <param name="action">The action.</param>
        /// <param name="defaultText">The default text of the menu item.</param>
        private void UpdateEditMenuItem(MenuItem menuItem, UIAction action, string defaultText)
        {
            if (action != null && action.IsEnabled)
            {
                menuItem.IsEnabled = true;
                menuItem.Header = action.Name;
            }
            else
            {
                menuItem.IsEnabled = false;
                menuItem.Header = defaultText;
            }
        }

        #endregion


        #region 'View' menu

        #region VOI LUT

        /// <summary>
        /// Shows the dialog that allows to change VOI LUT of DICOM frame.
        /// </summary>
        private void ShowCustomVoiLutForm()
        {
            voiLutMainMenuItem.IsChecked ^= true;

            if (voiLutMainMenuItem.IsChecked)
            {
                // create window
                _voiLutParamsWindow = new VoiLutParamsWindow(this, _dicomViewerTool.DicomViewerTool);
                // set current DICOM frame
                _voiLutParamsWindow.DicomFrame = imageViewer1.Image;
                _voiLutParamsWindow.Closing += new CancelEventHandler(voiLutParamsWindow_Closing);
                // hide VOI LUT info
                _voiLutsButton.Visibility = Visibility.Collapsed;
                voiLutToolBar.Visibility = _voiLutsButton.Visibility;
                // show window
                _voiLutParamsWindow.Show();
            }
            else
            {
                // close the window
                _voiLutParamsWindow.Close();
                _voiLutParamsWindow = null;
            }
        }

        #endregion

        #endregion


        #region File manipulation

        /// <summary>
        /// Opens a DICOM file.
        /// </summary>
        private void OpenDicomFile()
        {
            if (_openDicomFileDialog.ShowDialog() == true)
            {
                if (_openDicomFileDialog.FileNames.Length > 0)
                {
                    // close the previously opened DICOM files
                    ClosePreviouslyOpenedFiles();

                    // add DICOM files to the DICOM series
                    AddDicomFilesToSeries(_openDicomFileDialog.FileNames);
                    _dicomViewerTool.DicomViewerTool.DicomImageVoiLut =
                        _dicomViewerTool.DicomViewerTool.DefaultDicomImageVoiLut;

                    _openDicomFileDialog.InitialDirectory = null;
                }
            }
        }

        /// <summary>
        /// Adds the DICOM files to the series.
        /// </summary>
        /// <param name="filesPath">Files path.</param>
        private void AddDicomFilesToSeries(params string[] filesPath)
        {
            try
            {
                List<DicomFile> filesForLoadPresentationState = new List<DicomFile>();
                string dirPath = null;

                // show action label and progress bar
                actionLabel.Visibility = Visibility.Visible;
                progressBar1.Visibility = Visibility.Visible;
                progressBar1.Maximum = filesPath.Length;
                progressBar1.Value = 0;

                bool skipCorruptedFiles = false;

                foreach (string filePath in filesPath)
                {
                    if (dirPath == null)
                        dirPath = Path.GetDirectoryName(filePath);

                    // set action info
                    actionLabel.Content = string.Format("Loading {0}", Path.GetFileName(filePath));
                    // update progress bar
                    progressBar1.Value++;
                    DemosTools.DoEvents();

                    DicomFile dicomFile = null;
                    try
                    {
                        // if the series already contains the specified DICOM file
                        if (_dicomSeriesController.Contains(filePath))
                        {
                            DemosTools.ShowInfoMessage(string.Format("The series already contains DICOM file \"{0}\".", Path.GetFileName(filePath)));
                            return;
                        }

                        // instance number of new DICOM file
                        int newDicomFileInstanceNumber = 0;
                        // add DICOM file to the current series of DICOM images and get the DICOM images of new DICOM file
                        ImageCollection newDicomImages =
                            _dicomSeriesController.AddDicomFileToSeries(filePath, out dicomFile, out newDicomFileInstanceNumber);

                        // if DICOM file represents the DICOM directory
                        if (IsDicomDirectory(dicomFile))
                        {
                            // close the DICOM file
                            _dicomSeriesController.CloseDicomFile(dicomFile);
                            // show the error message
                            DemosTools.ShowInfoMessage("The DICOM directory cannot be added to the series of DICOM images.");
                            return;
                        }

                        IsDicomFileOpening = true;

                        // if DICOM file does not contain images
                        if (dicomFile.Pages.Count == 0)
                        {
                            // if image viewer contains images
                            if (imageViewer1.Images.Count > 0)
                            {
                                DemosTools.ShowInfoMessage("The DICOM file cannot be added to the series of DICOM images because the DICOM file does not contain image.");
                            }
                            else
                            {
                                // save reference to the DICOM file
                                _dicomFileWithoutImages = dicomFile;

                                // show message for user
                                DemosTools.ShowInfoMessage("DICOM file does not contain image.");
                                // show metadata of DICOM file
                                ShowCurrentFileMetadata();
                            }
                        }
                        else
                        {
                            // get image index in image collection of current DICOM file
                            int imageIndex = GetImageIndexInImageCollectionForNewImage(newDicomFileInstanceNumber);

                            try
                            {
                                // insert images to the specified index
                                imageViewer1.Images.InsertRange(imageIndex, newDicomImages.ToArray());

                                // update frame count in series
                                imageViewerToolBar.PageCount = imageViewer1.Images.Count;
                            }
                            catch
                            {
                                // remove new DICOM images from image collection of image viewer
                                foreach (VintasoftImage newDicomImage in newDicomImages)
                                    imageViewer1.Images.Remove(newDicomImage);

                                // close new DICOM file
                                _dicomSeriesController.CloseDicomFile(dicomFile);
                                dicomFile = null;

                                // update frame count in series
                                imageViewerToolBar.PageCount = imageViewer1.Images.Count;

                                throw;
                            }

                            // if DICOM presentation state file must be loaded automatically
                            if (presentationState_loadAutomaticallyMenuItem.IsChecked)
                            {
                                filesForLoadPresentationState.Add(dicomFile);
                            }

                            // if image viewer shows the first image in series
                            if (imageViewerToolBar.PageCount == dicomFile.Pages.Count)
                                // update UI of DICOM file
                                UpdateUIWithInformationAboutDicomFile();
                        }

                        // update header of form
                        this.Title = string.Format(_titlePrefix, Path.GetFileName(filePath));
                    }
                    catch (Exception ex)
                    {
                        // close file
                        if (dicomFile != null)
                            _dicomSeriesController.CloseDicomFile(dicomFile);

                        if (!skipCorruptedFiles)
                        {
                            if (filesPath.Length == 1)
                            {
                                DemosTools.ShowErrorMessage(ex);

                                dirPath = null;
                                CloseDicomSeries();
                            }
                            else
                            {
                                string exceptionMessage = string.Format(
                                    "The file '{0}' can not be opened:\r\n\"{1}\"\r\nDo you want to continue anyway?",
                                    Path.GetFileName(filePath), DemosTools.GetFullExceptionMessage(ex).Trim());
                                if (MessageBox.Show(
                                    exceptionMessage,
                                    "Error",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Error) == MessageBoxResult.No)
                                {
                                    dirPath = null;
                                    CloseDicomSeries();
                                    break;
                                }
                            }

                            skipCorruptedFiles = true;
                        }
                    }
                }

                // hide action label and progress bar
                actionLabel.Content = string.Empty;
                actionLabel.Visibility = Visibility.Collapsed;
                progressBar1.Visibility = Visibility.Collapsed;

                if (!string.IsNullOrEmpty(dirPath))
                {
                    // if DICOM presentation files must be loaded automatically
                    if (presentationState_loadAutomaticallyMenuItem.IsChecked)
                        // load presentation state file of DICOM file
                        LoadAnnotationsFromPresentationStateFiles(dirPath, filesForLoadPresentationState.ToArray());
                }

                // update UI
                UpdateUI();
            }
            finally
            {
                // hide action label and progress bar
                actionLabel.Content = string.Empty;
                actionLabel.Visibility = Visibility.Collapsed;
                progressBar1.Visibility = Visibility.Collapsed;

                if (!_isWindowClosing)
                {
                    // update the UI
                    IsDicomFileOpening = false;
                }
            }
        }        

        /// <summary>
        /// Returns the index, in image collection, where the new DICOM image must be inserted.
        /// </summary>
        /// <param name="dicomFileInstanceNumber">The DICOM file instance number of new image.</param>
        /// <returns>
        /// The image index of image collection.
        /// </returns>
        private int GetImageIndexInImageCollectionForNewImage(int newImageDicomFileInstanceNumber)
        {
            int imageIndex = imageViewer1.Images.Count;
            while (imageIndex > 0)
            {
                // get DICOM file instance number for the image from image collection
                int imageDicomFileInstanceNumber =
                    _dicomSeriesController.GetDicomFileInstanceNumber(imageViewer1.Images[imageIndex - 1]);

                // if new image must be inserted after the image from image collection
                if (newImageDicomFileInstanceNumber > imageDicomFileInstanceNumber)
                    break;

                imageIndex--;
            }
            return imageIndex;
        }

        /// <summary>
        /// Determines whether the specified DICOM file contains DICOM directory metadata.
        /// </summary>
        /// <param name="dicomFile">The DICOM file.</param>
        /// <returns>
        /// <returns><b>true</b> if the DICOM file contains DICOM directory metadata; 
        /// otherwise, <b>false</b>.</returns>
        /// </returns>
        private bool IsDicomDirectory(DicomFile dicomFile)
        {
            if (dicomFile.DataSet.DataElements.Contains(DicomDataElementId.DirectoryRecordSequence))
                return true;

            return false;
        }

        /// <summary>
        /// Closes the previously opened DICOM files.
        /// </summary>
        private void ClosePreviouslyOpenedFiles()
        {
            // if DICOM file without images is opened
            if (_dicomFileWithoutImages != null)
            {
                // close the DICOM file without images
                _dicomFileWithoutImages.Dispose();
                _dicomFileWithoutImages = null;
            }
            // if DICOM series has files
            if (_dicomSeriesController.FileCount > 0)
            {
                // close the previously opened DICOM file
                CloseDicomSeries();
            }
        }

        /// <summary>
        /// Closes series of DICOM frames.
        /// </summary>
        private void CloseDicomSeries()
        {
            if (imageViewer1.Images.Count != 0)
                CloseAllPresentationStateFiles();

            // if animation is enabled
            if (IsAnimationStarted)
            {
                // stop animation
                IsAnimationStarted = false;
                showAnimationMenuItem.IsChecked = false;
            }

            imageViewerToolBar.SelectedPageIndex = -1;
            imageViewerToolBar.PageCount = 0;

            // clear image collection of image viewer and dispose all images
            imageViewer1.Images.ClearAndDisposeItems();
            thumbnailViewer1.Images.ClearAndDisposeItems();

            _dicomSeriesController.CloseSeries();
            _dicomFileWithoutImages = null;

            this.Title = string.Format(_titlePrefix, "(Untitled)");

            // update the UI
            UpdateUI();
            UpdateUIWithInformationAboutDicomFile();
        }

        #endregion


        #region Annotations

        /// <summary>
        /// Returns an array of annotations, which are associated with images from DICOM file.
        /// </summary>
        /// <param name="dicomFile">The DICOM file.</param>
        /// <returns>
        /// The annotation data.
        /// </returns>
        private DicomAnnotationDataCollection[] GetAnnotationsAssociatedWithDicomFileImages(DicomFile dicomFile)
        {
            // create result list
            List<DicomAnnotationDataCollection> result = new List<DicomAnnotationDataCollection>();

            // get data controller of annotation tool
            DicomAnnotationDataController controller = _dicomViewerTool.DicomAnnotationTool.AnnotationDataController;
            // get images of DICOM file
            VintasoftImage[] dicomFileImages = _dicomSeriesController.GetImages(dicomFile);
            // for each image
            foreach (VintasoftImage image in dicomFileImages)
            {
                // get annotations of image
                DicomAnnotationDataCollection annotations =
                    (DicomAnnotationDataCollection)controller.GetAnnotations(image);

                // if annotation collection is not empty
                if (annotations.Count > 0)
                    // add annotations
                    result.Add(annotations);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Loads the annotation from binary or XMP packet.
        /// </summary>
        private void LoadAnnotationFromBinaryOrXmpFormat(bool binaryFormat)
        {
            _openDicomAnnotationsFileDialog.FileName = null;
            if (binaryFormat)
                _openDicomAnnotationsFileDialog.Filter = "Binary Annotations(*.vsab)|*.vsab";
            else
                _openDicomAnnotationsFileDialog.Filter = "XMP Annotations(*.xmp)|*.xmp";
            _openDicomAnnotationsFileDialog.FilterIndex = 1;
            _openDicomAnnotationsFileDialog.Multiselect = false;

            if (_openDicomAnnotationsFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (FileStream fs = new FileStream(_openDicomAnnotationsFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        // get the annotation collection
                        AnnotationDataCollection annotations = _dicomViewerTool.DicomAnnotationTool.AnnotationDataCollection;
                        // clear the annotation collection
                        annotations.ClearAndDisposeItems();
                        // add annotations from stream to the annotation collection
                        annotations.AddFromStream(fs, imageViewer1.Image.Resolution);
                    }
                }
                catch (Exception ex)
                {
                    DemosTools.ShowErrorMessage(ex);
                }
            }
        }

        /// <summary>
        /// Sets the grip mode for annotation.
        /// </summary>
        /// <param name="view">The view of annotation.</param>
        /// <param name="mode">The grip mode of annotation.</param>
        private void SetGripMode(WpfAnnotationView view, GripMode mode)
        {
            if (view is WpfDicomPolylineAnnotationView)
                ((WpfDicomPolylineAnnotationView)view).GripMode = mode;
            else if (view is WpfDicomRangeLineAnnotationView)
                ((WpfDicomRangeLineAnnotationView)view).GripMode = mode;
            else if (view is WpfDicomInfiniteLineAnnotationView)
                ((WpfDicomInfiniteLineAnnotationView)view).GripMode = mode;
            else if (view is WpfDicomArrowAnnotationView)
                ((WpfDicomArrowAnnotationView)view).GripMode = mode;
            else if (view is WpfDicomAxisAnnotationView)
                ((WpfDicomAxisAnnotationView)view).GripMode = mode;
            else if (view is WpfDicomRulerAnnotationView)
                ((WpfDicomRulerAnnotationView)view).GripMode = mode;
            else if (view is WpfDicomMultilineAnnotationView)
                ((WpfDicomMultilineAnnotationView)view).GripMode = mode;
        }

        #endregion


        #region Annotations UI

        /// <summary>
        /// Fills combobox with information about annotations of image.
        /// </summary>
        private void FillAnnotationComboBox()
        {
            annotationComboBox.BeginInit();
            annotationComboBox.Items.Clear();
            annotationComboBox.SelectedIndex = -1;

            if (imageViewer1.FocusedIndex >= 0)
            {
                AnnotationDataCollection annotations = _dicomViewerTool.DicomAnnotationTool.AnnotationDataController[imageViewer1.FocusedIndex];
                for (int i = 0; i < annotations.Count; i++)
                {
                    annotationComboBox.Items.Add(string.Format("[{0}] {1}", i, annotations[i].GetType().Name));
                    if (_dicomViewerTool.DicomAnnotationTool.FocusedAnnotationData == annotations[i])
                        annotationComboBox.SelectedIndex = i;
                }
            }
            annotationComboBox.EndInit();
        }

        /// <summary>
        /// Shows information about annotation in property grid.
        /// </summary>
        private void ShowAnnotationProperties(WpfAnnotationView annotation)
        {
            AnnotationData annotationData = null;
            if (annotation != null)
                annotationData = annotation.Data;

            if (propertyGrid1.SelectedObject != annotationData)
                propertyGrid1.SelectedObject = annotationData;
            else if (!_isAnnotationTransforming)
                propertyGrid1.Refresh();
        }

        /// <summary>
        /// Updates the transformation mode menu.
        /// </summary>
        private void UpdateTransformationMenu()
        {
            WpfAnnotationView view = _dicomViewerTool.DicomAnnotationTool.FocusedAnnotationView;

            GripMode mode = GripMode.Rectangular;
            if (view is WpfDicomPolylineAnnotationView)
                mode = ((WpfDicomPolylineAnnotationView)view).GripMode;
            else if (view is WpfDicomRangeLineAnnotationView)
                mode = ((WpfDicomRangeLineAnnotationView)view).GripMode;
            else if (view is WpfDicomInfiniteLineAnnotationView)
                mode = ((WpfDicomInfiniteLineAnnotationView)view).GripMode;
            else if (view is WpfDicomArrowAnnotationView)
                mode = ((WpfDicomArrowAnnotationView)view).GripMode;
            else if (view is WpfDicomAxisAnnotationView)
                mode = ((WpfDicomAxisAnnotationView)view).GripMode;
            else if (view is WpfDicomRulerAnnotationView)
                mode = ((WpfDicomRulerAnnotationView)view).GripMode;
            else if (view is WpfDicomMultilineAnnotationView)
                mode = ((WpfDicomMultilineAnnotationView)view).GripMode;

            if (mode == GripMode.Rectangular)
                transformationModeRectangularMenuItem.IsChecked = true;
            else
                transformationModeRectangularMenuItem.IsChecked = false;
            if (mode == GripMode.Points)
                transformationModePointsMenuItem.IsChecked = true;
            else
                transformationModePointsMenuItem.IsChecked = false;
            if (mode == GripMode.RectangularAndPoints)
                transformationModeRectangularAndPointsMenuItem.IsChecked = true;
            else
                transformationModeRectangularAndPointsMenuItem.IsChecked = false;
        }

        #endregion


        #region Annotation visual tool

        /// <summary>
        /// The annotation property is changing.
        /// </summary>
        private void AnnotationdData_PropertyChanging(object sender, ObjectPropertyChangingEventArgs e)
        {
            if (e.PropertyName == "UnitOfMeasure")
            {
                if (_isAnnotationPropertyChanging)
                    return;

                _isAnnotationPropertyChanging = true;
                DicomAnnotationData data = (DicomAnnotationData)sender;

                data.ChangeUnitOfMeasure((DicomUnitOfMeasure)e.NewValue, imageViewer1.Image);
                _isAnnotationPropertyChanging = false;
            }
        }


        #endregion


        #region Presentation state file

        /// <summary>
        /// Loads the annotations from DICOM presentation state files.
        /// </summary>
        /// <param name="presentationStateFileDirectoryPath">A path to a directory,
        /// where DICOM presentation files must be searched.</param>
        /// <param name="sourceDicomFiles">The source DICOM files.</param>
        private void LoadAnnotationsFromPresentationStateFiles(
           string presentationStateFileDirectoryPath,
           params DicomFile[] sourceDicomFiles)
        {
            // if directory does NOT exist
            if (!Directory.Exists(presentationStateFileDirectoryPath))
                // exit
                return;

            // get paths to the files in directory
            string[] filePaths = Directory.GetFiles(presentationStateFileDirectoryPath);

            // show action label and progress bar
            actionLabel.Visibility = Visibility.Visible;
            progressBar1.Visibility = Visibility.Visible;
            progressBar1.Maximum = filePaths.Length;
            progressBar1.Value = 0;

            try
            {
                // dictionary: DICOM file => path to the DICOM presentation state files, which are referenced to the DICOM file
                Dictionary<DicomFile, List<string>> dicomFileToPresentationStateFilePaths =
                    new Dictionary<DicomFile, List<string>>();

                List<string> dicomFilePaths = new List<string>();
                foreach (DicomFile dicomFile in sourceDicomFiles)
                    dicomFilePaths.Add(_dicomSeriesController.GetDicomFilePath(dicomFile));

                // for each file path in directory
                foreach (string filePath in filePaths)
                {
                    // if file path is NOT a path to a DICOM file
                    if (IsDicomFilePath(filePath, dicomFilePaths))
                        // skip the file
                        continue;

                    // if file path is NOT a path to a DICOM presentation state file
                    if (!IsDicomPresentationStateFilePath(filePath))
                        // skip the file
                        continue;

                    // set action info
                    actionLabel.Content = string.Format("Scanning {0}", Path.GetFileName(filePath));
                    // update progress bar
                    progressBar1.Value++;
                    DemosTools.DoEvents();

                    DicomFile dicomFile = null;
                    try
                    {
                        // open new DICOM file in read-only mode
                        dicomFile = new DicomFile(filePath, true);
                        // if DICOM file has annotations
                        if (dicomFile.Annotations != null)
                        {
                            // for each source DICOM file
                            foreach (DicomFile sourceDicomFile in sourceDicomFiles)
                            {
                                // if DICOM file references to the source DICOM file
                                if (dicomFile.IsReferencedTo(sourceDicomFile))
                                {
                                    // if presentation state file paths for DICOM file are NOT found
                                    if (!dicomFileToPresentationStateFilePaths.ContainsKey(sourceDicomFile))
                                        // create an empty list
                                        dicomFileToPresentationStateFilePaths.Add(sourceDicomFile, new List<string>());
                                    // add file path to a list of presentation state file paths
                                    dicomFileToPresentationStateFilePaths[sourceDicomFile].Add(Path.GetFullPath(filePath));

                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        if (dicomFile != null)
                            dicomFile.Dispose();
                    }
                }

                // hide action label and progress bar
                actionLabel.Content = string.Empty;
                actionLabel.Visibility = Visibility.Collapsed;
                progressBar1.Visibility = Visibility.Collapsed;

                // if presentation state files is searched
                if (dicomFileToPresentationStateFilePaths.Count > 0)
                {
                    foreach (DicomFile sourceDicomFile in dicomFileToPresentationStateFilePaths.Keys)
                    {
                        // load DICOM annotations from DICOM presentation state file
                        SelectDicomPresentationStateFileAndLoadAnnotations(
                            sourceDicomFile,
                            dicomFileToPresentationStateFilePaths[sourceDicomFile].ToArray(),
                            _dicomViewerTool.DicomAnnotationTool.AnnotationDataController);
                    }
                }
            }
            finally
            {
                // hide action label and progress bar
                actionLabel.Content = string.Empty;
                actionLabel.Visibility = Visibility.Collapsed;
                progressBar1.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Selects the DICOM presentation state file and loads annotations from the selected file.
        /// </summary>
        /// <param name="dicomFile">DICOM file.</param>
        /// <param name="presentationStateFilePaths">An array with paths to the DICOM presentation files,
        /// which are associated with <i>dicomFile</i>.</param>
        /// <param name="annotationDataController">The annotation data controller, where information
        /// about annotations must be added.</param>
        private void SelectDicomPresentationStateFileAndLoadAnnotations(
            DicomFile dicomFile,
            string[] presentationStateFilePaths,
            DicomAnnotationDataController annotationDataController)
        {
            // create dialog
            SelectPresentationStateFile dlg =
                new SelectPresentationStateFile(presentationStateFilePaths);

            string dicomFilePath = _dicomSeriesController.GetDicomFilePath(dicomFile);
            dlg.Title += ": " + Path.GetFileName(dicomFilePath);
            dlg.Owner = this;

            string selectedPresentationStateFileName = null;
            if (presentationStateFilePaths.Length > 0)
            {
                string selectedPresentationStateFilePath = presentationStateFilePaths[presentationStateFilePaths.Length - 1];
                selectedPresentationStateFileName = Path.GetFileNameWithoutExtension(selectedPresentationStateFilePath);
            }
            if (selectedPresentationStateFileName != null)
                dlg.SelectedPresentationStateFilename = selectedPresentationStateFileName;

            // show dialog
            if (dlg.ShowDialog() == true)
            {
                // get name of selected presentation state file
                string presentationStateFilename = dlg.SelectedPresentationStateFilename;
                // if name is not empty
                if (!string.IsNullOrEmpty(presentationStateFilename))
                {
                    // create DICOM presentation state file
                    DicomFile presentationStateFile =
                        PresentationStateFileController.LoadPresentationStateFile(
                        dicomFile, presentationStateFilename);
                    // add annotations from DICOM presentation state file to the annotation data controller
                    annotationDataController.AddAnnotationDataSet(presentationStateFile.Annotations);
                }
            }
        }

        /// <summary>
        /// Determines that file path is a path to a DICOM file.
        /// </summary>
        /// <param name="filePath">A file path.</param>
        /// <param name="dicomFilePaths">A list with paths to the DICOM files.</param>
        private bool IsDicomFilePath(string filePath, List<string> dicomFilePaths)
        {
            // for each DICOM file path
            foreach (string dicomFilePath in dicomFilePaths)
            {
                // if file path and DICOM file path are equals
                if (filePath.ToUpperInvariant() == dicomFilePath.ToUpperInvariant())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines that file path is a path to a DICOM presentation state file.
        /// </summary>
        /// <param name="filePath">A file path.</param>
        private bool IsDicomPresentationStateFilePath(string filePath)
        {
            // get file extension
            string fileExtension = Path.GetExtension(filePath);
            // for each supported presentation file extension
            for (int i = 0; i < _presentationStateFileExtensions.Length; i++)
            {
                // if file has presentation file extension
                if (string.Equals(fileExtension, _presentationStateFileExtensions[i], StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Creates the DICOM presentation state file.
        /// </summary>
        /// <param name="dicomFile">The source DICOM file.</param>
        /// <param name="annotations">The annotations of DICOM presentation state file.</param>
        /// <returns>
        /// The presentation state file.
        /// </returns>
        private DicomFile CreatePresentationStateFile(
            DicomFile dicomFile,
            params DicomAnnotationDataCollection[] annotations)
        {
            // create DICOM presentation state file for DICOM image file
            DicomFile presentationStateFile = DicomFile.CreatePresentationState(dicomFile);

            // create annotaiton codec
            DicomAnnotationCodec annoCodec = new DicomAnnotationCodec();
            if (presentationStateFile.Annotations == null)
                presentationStateFile.Annotations = new DicomAnnotationTreeNode(presentationStateFile);

            // encode annotation to the DICOM presentation state file
            annoCodec.Encode(presentationStateFile.Annotations, annotations);

            return presentationStateFile;
        }

        /// <summary>
        /// Closes the DICOM presentation state file of focused DICOM file.
        /// </summary>
        private void CloseCurrentPresentationStateFile()
        {
            ClosePresentationStateFile(DicomFile);
        }

        /// <summary>
        /// Closes all DICOM presentation state files.
        /// </summary>
        private void CloseAllPresentationStateFiles()
        {
            DicomFile[] dicomFiles = _dicomSeriesController.GetFilesOfSeries();

            foreach (DicomFile dicomFile in dicomFiles)
                ClosePresentationStateFile(dicomFile);
        }

        /// <summary>
        /// Closes the DICOM presentation state file of specified DICOM file.
        /// </summary>
        /// <param name="dicomFile">The DICOM file.</param>
        private void ClosePresentationStateFile(DicomFile dicomFile)
        {
            // get the presentation state file of source DICOM file
            DicomFile presentationStateFile = PresentationStateFileController.GetPresentationStateFile(dicomFile);

            if (presentationStateFile == null)
                return;

            // get controller of DicomAnnotationTool
            DicomAnnotationDataController controller = _dicomViewerTool.DicomAnnotationTool.AnnotationDataController;

            // remove annotations from controller
            controller.RemoveAnnotationDataSet(presentationStateFile.Annotations);

            // close the presentation state file of source DICOM file
            PresentationStateFileController.ClosePresentationStateFile(dicomFile);
        }

        #endregion


        #region VOI LUT

        /// <summary>
        /// Updates menu items with VOI LUT information.
        /// </summary>
        /// <param name="modality">Modality of DICOM file.</param>
        private void UpdateWindowLevelSplitButton(DicomFileModality modality)
        {
            ItemCollection items = _voiLutsButton.Items;
            // clear buttons
            items.Clear();

            // is current frame is empty
            if (imageViewer1.Image == null)
                return;

            // add default window level button
            _defaultVoiLutMenuItem.IsChecked = true;
            _currentVoiLutMenuItem = _defaultVoiLutMenuItem;
            items.Add(_defaultVoiLutMenuItem);

            MenuItem menuItem = null;
            _menuItemToVoiLut.Clear();

            DicomFrameMetadata metadata = GetFocusedImageMetadata();
            DicomImageVoiLookupTable defaultVoiLut = metadata.VoiLut;
            if (defaultVoiLut.IsEmpty)
                defaultVoiLut = _dicomViewerTool.DicomViewerTool.DicomImageVoiLut;
            _menuItemToVoiLut.Add(_defaultVoiLutMenuItem, defaultVoiLut);

            if (metadata == null)
                return;

            // get the available VOI LUTs
            DicomImageVoiLookupTable[] voiLuts = metadata.AvailableVoiLuts;
            // if DICOM frame has VOI LUTs
            if (voiLuts.Length > 0)
            {
                bool addSeparator = true;
                // for each VOI LUT
                for (int i = 0; i < voiLuts.Length; i++)
                {
                    // if VOI LUT is equal to the default VOI LUT
                    if (metadata.VoiLut.WindowCenter == voiLuts[i].WindowCenter &&
                        metadata.VoiLut.WindowWidth == voiLuts[i].WindowWidth)
                        continue;

                    if (addSeparator)
                    {
                        items.Add(new Separator());
                        addSeparator = false;
                    }

                    string explanation = voiLuts[i].Explanation;
                    if (explanation == string.Empty)
                        explanation = string.Format("VOI LUT {0}", i + 1);

                    menuItem = new MenuItem();
                    menuItem.Header = explanation;
                    menuItem.IsCheckable = true;
                    _menuItemToVoiLut.Add(menuItem, voiLuts[i]);
                    menuItem.Click += new RoutedEventHandler(voiLutMenuItem_Click);
                    items.Add(menuItem);
                }
            }

            string[] windowExplanation = null;
            voiLuts = null;

            // add standard VOI LUT for specific modalities

            switch (modality)
            {
                case DicomFileModality.CT:
                    windowExplanation = new string[] {
                        "Abdomen",
                        "Angio",
                        "Bone",
                        "Brain",
                        "Chest",
                        "Lungs" };

                    voiLuts = new DicomImageVoiLookupTable[] {
                        new DicomImageVoiLookupTable(60,400),
                        new DicomImageVoiLookupTable(300,600),
                        new DicomImageVoiLookupTable(300,1500),
                        new DicomImageVoiLookupTable(40,80),
                        new DicomImageVoiLookupTable(40,400),
                        new DicomImageVoiLookupTable(-400,1500) };
                    break;

                case DicomFileModality.CR:
                case DicomFileModality.DX:
                case DicomFileModality.MR:
                case DicomFileModality.NM:
                case DicomFileModality.XA:
                    windowExplanation = new string[] {
                        "Center 20   Width 40",
                        "Center 40   Width 80",
                        "Center 80   Width 160",
                        "Center 600  Width 1280",
                        "Center 1280 Width 2560",
                        "Center 2560 Width 5120"};

                    voiLuts = new DicomImageVoiLookupTable[] {
                        new DicomImageVoiLookupTable(20,40),
                        new DicomImageVoiLookupTable(40,80),
                        new DicomImageVoiLookupTable(80,160),
                        new DicomImageVoiLookupTable(600,1280),
                        new DicomImageVoiLookupTable(1280,2560),
                        new DicomImageVoiLookupTable(2560,5120) };
                    break;

                case DicomFileModality.MG:
                case DicomFileModality.PT:
                    windowExplanation = new string[] {
                        "Center 30    Width 60",
                        "Center 125   Width 250",
                        "Center 500   Width 1000",
                        "Center 1875  Width 3750",
                        "Center 3750  Width 7500",
                        "Center 7500  Width 15000",
                        "Center 15000 Width 30000",
                        "Center 30000 Width 60000"};

                    voiLuts = new DicomImageVoiLookupTable[] {
                        new DicomImageVoiLookupTable(30,60),
                        new DicomImageVoiLookupTable(125,250),
                        new DicomImageVoiLookupTable(500,1000),
                        new DicomImageVoiLookupTable(1875,3750),
                        new DicomImageVoiLookupTable(3750,7500),
                        new DicomImageVoiLookupTable(7500,15000),
                        new DicomImageVoiLookupTable(15000,30000),
                        new DicomImageVoiLookupTable(30000,60000) };
                    break;
            }

            if (voiLuts != null)
            {
                items.Add(new Separator());
                for (int i = 0; i < voiLuts.Length; i++)
                {
                    menuItem = new MenuItem();
                    menuItem.Header = windowExplanation[i];
                    menuItem.IsCheckable = true;
                    _menuItemToVoiLut.Add(menuItem, voiLuts[i]);
                    menuItem.Click += new RoutedEventHandler(voiLutMenuItem_Click);
                    items.Add(menuItem);
                }
            }

            items.Add(new Separator());
            menuItem = new MenuItem();
            menuItem.Header = "Custom VOI LUT...";
            menuItem.Click += customVoiLutMenuItem_Click;
            items.Add(menuItem);

            foreach (object item in items)
            {
                if (item is MenuItem)
                    UpdateMenuItemTemplatePartBackground((MenuItem)item);
            }
        }

        /// <summary>
        /// Updates the template of <see cref="MenuItem"/> object for fixing the bug in <see cref="MenuItem"/>.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <remarks>
        /// The <see cref="MenuItem"/> has bug and displays black rectangle in element if MenuItem.IsChecked property is set to True.
        /// This method fixes the bug.
        /// </remarks>
        private void UpdateMenuItemTemplatePartBackground(MenuItem menuItem)
        {
            if (menuItem.Template == null)
                return;

            const string TEMPLATE_PART_NAME = "GlyphPanel";
            Border border = menuItem.Template.FindName(TEMPLATE_PART_NAME, menuItem) as Border;

            if (border == null)
            {
                menuItem.ApplyTemplate();
                border = menuItem.Template.FindName(TEMPLATE_PART_NAME, menuItem) as Border;
            }

            if (border != null)
                border.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
        }

        

        #endregion


        #region Metadata

        /// <summary>
        /// Shows a window that allows to browse DICOM file metadata.
        /// </summary>
        private void ShowCurrentFileMetadata()
        {
            // create metadata editor window
            DicomMetadataEditorWindow window = new DicomMetadataEditorWindow();
            window.CanEdit = false;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Owner = this;

            // get metadata of DICOM image
            DicomFrameMetadata metadata = GetFocusedImageMetadata();
            // if DICOM image does not have metadata
            if (metadata == null)
                // get metadata of DICOM file
                metadata = new DicomFrameMetadata(DicomFile);
            window.RootMetadataNode = metadata;

            // show dialog
            window.ShowDialog();

            // if image viewer has image
            if (imageViewer1.Image != null)
            {
                // update the UI with information about DICOM file
                UpdateUIWithInformationAboutDicomFile();
                // refresh the DICOM viewer tool
                _dicomViewerTool.DicomViewerTool.Refresh();
            }

            UpdateUI();
        }

        /// <summary>
        /// Returns the metadata of focused image.
        /// </summary>
        private DicomFrameMetadata GetFocusedImageMetadata()
        {
            if (imageViewer1.Image == null)
                return null;

            DicomFrameMetadata metadata = imageViewer1.Image.Metadata.MetadataTree as DicomFrameMetadata;

            return metadata;
        }

        #endregion


        #region Frame animation

        /// <summary>
        /// Starts animation.
        /// </summary>
        private void StartAnimation()
        {
            StopAnimation();

            _animationThread = new Thread(AnimationMethod);
            _animationThread.IsBackground = true;

            // disable visual tool
            _dicomViewerTool.Enabled = false;
            _dicomViewerTool.DicomViewerTool.IsTextOverlayVisible = false;
            _dicomViewerTool.DicomViewerTool.ShowRulers = false;
            // disable tool strip
            imageViewerToolBar.IsEnabled = false;
            annotationsToolBar.IsEnabled = false;
            annotationInteractionModeToolBar.IsEnabled = false;
            dicomAnnotatedViewerToolBar.IsEnabled = false;
            dicomAnnotatedViewerToolBar.FindAction<MagnifierToolAction>().VisualTool.Enabled = false;

            if (_voiLutParamsWindow != null)
            {
                _voiLutParamsWindow.IsEnabled = false;
                _voiLutParamsWindow.Hide();
            }

            _animationThread.Start();
            _isAnimationStarted = true;
        }

        /// <summary>
        /// Stops animation.
        /// </summary>
        private void StopAnimation()
        {
            _isAnimationStarted = false;

            if (_animationThread == null)
                return;

            showAnimationMenuItem.IsChecked = false;

            _animationThread = null;

            while (!_canStopAnimation)
                DemosTools.DoEvents();

            _isFocusedIndexChanging = false;

            if (_voiLutParamsWindow != null)
            {
                _voiLutParamsWindow.Show();
                _voiLutParamsWindow.IsEnabled = true;
            }

            // if the focused index in thumbnail viewer was NOT changed during animation
            if (!_isFocusedIndexChanging)
            {
                // if thumbnail viewer and image viewer have different focused images
                if (imageViewer1.FocusedIndex != _currentAnimatedFrameIndex)
                {
                    // change focus in the thumbnail viewer and make it the same as focus in image viewer
                    imageViewer1.FocusedIndex = _currentAnimatedFrameIndex;
                }

                if (imageViewerToolBar.SelectedPageIndex != _currentAnimatedFrameIndex)
                {
                    imageViewerToolBar.SelectedPageIndex = _currentAnimatedFrameIndex;
                }
            }

            // enable tool strip
            imageViewerToolBar.IsEnabled = true;
            annotationsToolBar.IsEnabled = true;
            annotationInteractionModeToolBar.IsEnabled = true;
            dicomAnnotatedViewerToolBar.IsEnabled = true;
            dicomAnnotatedViewerToolBar.FindAction<MagnifierToolAction>().VisualTool.Enabled = true;
            // enable visual tool
            _dicomViewerTool.DicomViewerTool.IsTextOverlayVisible = showMetadataInViewerMenuItem.IsChecked;
            _dicomViewerTool.DicomViewerTool.ShowRulers = showRulersInViewerMenuItem.IsChecked;
            _dicomViewerTool.Enabled = true;
        }

        /// <summary>
        /// Animation thread.
        /// </summary>
        private void AnimationMethod()
        {
            Thread currentThread = Thread.CurrentThread;
            _currentAnimatedFrameIndex = imageViewer1.FocusedIndex;
            int count = imageViewer1.Images.Count;
            for (; _currentAnimatedFrameIndex < count || _isAnimationCycled;)
            {
                if (_animationThread != currentThread)
                    break;

                _isFocusedIndexChanging = true;
                // change focused image in image viewer

                _canStopAnimation = false;
                imageViewer1.SetFocusedIndexSync(_currentAnimatedFrameIndex);
                _canStopAnimation = true;

                _isFocusedIndexChanging = false;
                Thread.Sleep(_animationDelay);

                _currentAnimatedFrameIndex++;
                if (_isAnimationCycled && _currentAnimatedFrameIndex >= count)
                    _currentAnimatedFrameIndex = 0;
            }

            _currentAnimatedFrameIndex--;
            Dispatcher.Invoke(new ThreadStart(StopAnimation));
        }

        #endregion


        #region Save image(s)

        /// <summary>
        /// Returns the encoder for saving of single image.
        /// </summary>
        private EncoderBase GetEncoder(string filename)
        {
            MultipageEncoderBase multipageEncoder = GetMultipageEncoder(filename);
            if (multipageEncoder != null)
                return multipageEncoder;

            switch (Path.GetExtension(filename).ToUpperInvariant())
            {
                case ".JPG":
                case ".JPEG":
                    JpegEncoder jpegEncoder = new JpegEncoder();
                    jpegEncoder.Settings.AnnotationsFormat = AnnotationsFormat.VintasoftBinary;

                    JpegEncoderSettingsWindow jpegEncoderSettingsDlg = new JpegEncoderSettingsWindow();
                    jpegEncoderSettingsDlg.EditAnnotationSettings = true;
                    jpegEncoderSettingsDlg.EncoderSettings = jpegEncoder.Settings;
                    if (jpegEncoderSettingsDlg.ShowDialog() != true)
                        return null;

                    return jpegEncoder;

                case ".PNG":
                    PngEncoder pngEncoder = new PngEncoder();
                    pngEncoder.Settings.AnnotationsFormat = AnnotationsFormat.VintasoftBinary;

                    PngEncoderSettingsWindow pngEncoderSettingsDlg = new PngEncoderSettingsWindow();
                    pngEncoderSettingsDlg.EditAnnotationSettings = true;
                    pngEncoderSettingsDlg.EncoderSettings = pngEncoder.Settings;
                    if (pngEncoderSettingsDlg.ShowDialog() != true)
                        return null;

                    return pngEncoder;

                default:
                    return AvailableEncoders.CreateEncoder(filename);
            }
        }

        /// <summary>
        /// Returns the encoder for saving of image collection.
        /// </summary>
        private MultipageEncoderBase GetMultipageEncoder(string filename)
        {
            bool isFileExist = File.Exists(filename);
            switch (Path.GetExtension(filename).ToUpperInvariant())
            {
                case ".TIF":
                case ".TIFF":
                    TiffEncoder tiffEncoder = new TiffEncoder();
                    tiffEncoder.Settings.AnnotationsFormat = AnnotationsFormat.VintasoftBinary;

                    TiffEncoderSettingsWindow tiffEncoderSettingsDlg = new TiffEncoderSettingsWindow();
                    tiffEncoderSettingsDlg.CanAddImagesToExistingFile = isFileExist;
                    tiffEncoderSettingsDlg.EditAnnotationSettings = true;
                    tiffEncoderSettingsDlg.EncoderSettings = tiffEncoder.Settings;
                    if (tiffEncoderSettingsDlg.ShowDialog() != true)
                        return null;

                    return tiffEncoder;

                case ".GIF":
                    GifEncoder gifEncoder = new GifEncoder();

                    GifEncoderSettingsWindow gifEncoderSettingsDlg = new GifEncoderSettingsWindow();
                    gifEncoderSettingsDlg.CanAddImagesToExistingFile = isFileExist;
                    gifEncoderSettingsDlg.EncoderSettings = gifEncoder.Settings;
                    if (gifEncoderSettingsDlg.ShowDialog() != true)
                        return null;

                    return gifEncoder;

            }

            return null;
        }

        #endregion


        #region View Rotation

        /// <summary>
        /// Rotates images in both annotation viewer and thumbnail viewer by 90 degrees clockwise.
        /// </summary>
        private void RotateViewClockwise()
        {
            if (imageViewer1.ImageRotationAngle != 270)
            {
                imageViewer1.ImageRotationAngle += 90;
                thumbnailViewer1.ImageRotationAngle += 90;
            }
            else
            {
                imageViewer1.ImageRotationAngle = 0;
                thumbnailViewer1.ImageRotationAngle = 0;
            }
        }

        /// <summary>
        /// Rotates images in both annotation viewer and thumbnail viewer by 90 degrees counterclockwise.
        /// </summary>
        private void RotateViewCounterClockwise()
        {
            if (imageViewer1.ImageRotationAngle != 0)
            {
                imageViewer1.ImageRotationAngle -= 90;
                thumbnailViewer1.ImageRotationAngle -= 90;
            }
            else
            {
                imageViewer1.ImageRotationAngle = 270;
                thumbnailViewer1.ImageRotationAngle = 270;
            }
        }

        #endregion


        #region Init

        /// <summary>
        /// Initializes the image viewer tool bar.
        /// </summary>
        private void InitImageViewerToolBar()
        {
            _voiLutsButton = FindVoiLutsButton();

            imageViewerToolBar.ImageViewer = imageViewer1;
            imageViewerToolBar.SelectedPageIndex = -1;
            imageViewerToolBar.UseImageViewerImages = false;
            imageViewerToolBar.PageIndexChanged += new ImageViewerToolBar.PageIndexChangedEventHandler(imageViewerToolBar_PageIndexChanged);
        }

        /// <summary>
        /// Finds the VOI LUT menu item button in toolbar.
        /// </summary>
        /// <returns>
        /// The menu item of VOI LUT.
        /// </returns>
        private MenuItem FindVoiLutsButton()
        {
            foreach (object item in voiLutToolBar.Items)
            {
                if (item is Menu)
                {
                    Menu menu = (Menu)item;
                    foreach (object menuItem in menu.Items)
                    {
                        if (menuItem is MenuItem && ((MenuItem)menuItem).Header is StackPanel)
                        {
                            return (MenuItem)menuItem;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Initializes the DICOM annotation tool.
        /// </summary>
        private void InitDicomAnnotationTool()
        {
            _dicomViewerTool.DicomAnnotationTool.MultiSelect = false;
            _dicomViewerTool.DicomAnnotationTool.FocusedAnnotationViewChanged +=
                new EventHandler<WpfAnnotationViewChangedEventArgs>(annotationTool_FocusedAnnotationViewChanged);
            _dicomViewerTool.DicomAnnotationTool.SelectedAnnotations.Changed += new EventHandler(SelectedAnnotations_Changed);
            _dicomViewerTool.DicomAnnotationTool.AnnotationBuildingFinished +=
                new EventHandler<WpfAnnotationViewEventArgs>(annotationTool_AnnotationBuildingFinished);
            _dicomViewerTool.DicomAnnotationTool.AnnotationTransformingStarted +=
                new EventHandler<WpfAnnotationViewEventArgs>(annotationTool_AnnotationTransformingStarted);
            _dicomViewerTool.DicomAnnotationTool.AnnotationTransformingFinished +=
                new EventHandler<WpfAnnotationViewEventArgs>(annotationTool_AnnotationTransformingFinished);
            _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionModeChanged +=
                new EventHandler<AnnotationInteractionModeChangedEventArgs>(annotationTool_AnnotationInteractionModeChanged);

            _dicomViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.None;

            annotationInteractionModeComboBox.Items.Add(AnnotationInteractionMode.None);
            annotationInteractionModeComboBox.Items.Add(AnnotationInteractionMode.View);
            annotationInteractionModeComboBox.Items.Add(AnnotationInteractionMode.Author);
            // set interaction mode to the View 
            annotationInteractionModeComboBox.SelectedItem = AnnotationInteractionMode.None;
        }

        /// <summary>
        /// Initializes the unit of measures for rulers.
        /// </summary>
        private void InitUnitOfMeasuresForRulers()
        {
            UnitOfMeasure[] unitsOfMeasure = new UnitOfMeasure[] {
                UnitOfMeasure.Centimeters,
                UnitOfMeasure.Inches,
                UnitOfMeasure.Millimeters,
                UnitOfMeasure.Pixels
            };

            _menuItemToRulersUnitOfMeasure.Clear();

            foreach (UnitOfMeasure unit in unitsOfMeasure)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = unit.ToString();
                _menuItemToRulersUnitOfMeasure.Add(menuItem, unit);
                menuItem.Click += new RoutedEventHandler(rulersUnitOfMeasureMenuItem_Click);
                if (unit == _dicomViewerTool.DicomViewerTool.RulersUnitOfMeasure)
                {
                    menuItem.IsChecked = true;
                    _currentRulersUnitOfMeasureMenuItem = menuItem;
                }
                rulersUnitOfMeasureMenuItem.Items.Add(menuItem);
            }
        }

        /// <summary>
        /// Initializes the file dialogs.
        /// </summary>
        private void InitFileDialogs()
        {
            _openDicomFileDialog.Filter =
                "DICOM files|*.dcm;*.dic;*.acr|" +
                "All files|*.*";

            _saveFileDialog.Filter = "Mpeg files|*.mpg";
            _saveGifFileDialog.Filter = "*.gif|*.gif";
            _openDicomAnnotationsFileDialog.Filter =
                "Presentation State File(*.pre)|*.pre|" +
                "Binary Annotations(*.vsab)|*.vsab|" +
                "XMP Annotations(*.xmp)|*.xmp|" +
                "All Formats(*.pre;*.vsab;*.xmp)|*.pre;*.vsab;*.xmp";
            _openDicomAnnotationsFileDialog.FilterIndex = 4;
        }

        #endregion

        /// <summary>
        /// Moves the DICOM codec to the first position in <see cref="AvailableCodecs"/>.
        /// </summary>
        private static void MoveDicomCodecToFirstPosition()
        {
            ReadOnlyCollection<Codec> codecs = AvailableCodecs.Codecs;

            for (int i = codecs.Count - 1; i >= 0; i--)
            {
                Codec codec = codecs[i];

                if (codec.Name.Equals("DICOM", StringComparison.InvariantCultureIgnoreCase))
                {
                    AvailableCodecs.RemoveCodec(codec);
                    AvailableCodecs.InsertCodec(0, codec);
                    break;
                }
            }
        }


        #region Hot keys

        /// <summary>
        /// Handles the CanExecute event of OpenCommandBinding object.
        /// </summary>
        private void openCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = openDicomFilesMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of CloseCommandBinding object.
        /// </summary>
        private void closeCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = closeDicomSeriesMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of ExitCommandBinding object.
        /// </summary>
        private void exitCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = exitMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of IsNegativeCommandBinding object.
        /// </summary>
        private void isNegativeCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = negativeImageMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of CutCommandBinding object.
        /// </summary>
        private void cutCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cutMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of CopyCommandBinding object.
        /// </summary>
        private void copyCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = copyMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of PasteCommandBinding object.
        /// </summary>
        private void pasteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = pasteMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of DeleteCommandBinding object.
        /// </summary>
        private void deleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = deleteMenuItem.IsEnabled && imageViewer1.IsMouseOver;
            e.ContinueRouting = !e.CanExecute;
        }

        /// <summary>
        /// Handles the CanExecute event of DeleteAllCommandBinding object.
        /// </summary>
        private void deleteAllCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = deleteAllMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of RotateClockwiseCommandBinding object.
        /// </summary>
        private void rotateClockwiseCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rotateClockwiseMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of RotateCounterclockwiseCommandBinding object.
        /// </summary>
        private void rotateCounterclockwiseCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rotateCounterclockwiseMenuItem.IsEnabled;
        }

        #endregion

        #endregion

        #endregion



        #region Delegates

        /// <summary>
        /// Represents the <see cref="UpdateUI"/> method.
        /// </summary>
        delegate void UpdateUIDelegate();

        /// <summary>
        /// Represents the method that handles the <see cref="ImageCollection.ImageCollectionSavingProgress"/> event.
        /// </summary>
        delegate void SavingProgressDelegate(object sender, ProgressEventArgs e);

        /// <summary>
        /// Represents the method that handles the <see cref="ImageCollection.ImageCollectionSavingFinished"/> event.
        /// </summary>
        delegate void SavingFinishedDelegate(object sender, EventArgs e);

        #endregion

    }
}
