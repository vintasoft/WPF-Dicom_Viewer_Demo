using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Win32;

using Vintasoft.Imaging;
#if !REMOVE_ANNOTATION_PLUGIN
using Vintasoft.Imaging.Annotation;
using Vintasoft.Imaging.Annotation.Dicom;
using Vintasoft.Imaging.Annotation.Dicom.Wpf.UI;
using Vintasoft.Imaging.Annotation.Dicom.Wpf.UI.VisualTools;
using Vintasoft.Imaging.Annotation.Formatters;
using Vintasoft.Imaging.Annotation.UI;
using Vintasoft.Imaging.Annotation.Wpf.UI;
using Vintasoft.Imaging.Annotation.Wpf.UI.VisualTools;

#endif
using Vintasoft.Imaging.Codecs;
using Vintasoft.Imaging.Codecs.Decoders;
using Vintasoft.Imaging.Codecs.Encoders;
using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;
using Vintasoft.Imaging.Dicom.Wpf.UI;
using Vintasoft.Imaging.Dicom.Wpf.UI.VisualTools;
using Vintasoft.Imaging.ImageColors;
using Vintasoft.Imaging.Metadata;
using Vintasoft.Imaging.UI.VisualTools.UserInteraction;
using Vintasoft.Imaging.UIActions;
using Vintasoft.Imaging.Wpf.UI;
using Vintasoft.Imaging.Wpf.UI.VisualTools;
using Vintasoft.Imaging.Wpf.UI.VisualTools.UserInteraction;
using Vintasoft.Primitives;

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
        /// DICOM viewer tool.
        /// </summary>
        WpfDicomViewerTool _dicomViewerTool;

#if !REMOVE_ANNOTATION_PLUGIN
        /// <summary>
        /// DICOM annotated viewer tool.
        /// </summary>
        WpfDicomAnnotatedViewerTool _dicomAnnotatedViewerTool;

        /// <summary>
        /// The previous interaction mode in DICOM viewer tool.
        /// </summary>
        WpfDicomAnnotatedViewerToolInteractionMode _prevDicomViewerToolInteractionMode;

        /// <summary>
        /// The previous interaction mode in DICOM annotation tool.
        /// </summary>
        AnnotationInteractionMode _prevDicomAnnotationToolInteractionMode;

        /// <summary>
        /// Manager of interaction areas.
        /// </summary>
        WpfAnnotationInteractionAreaAppearanceManager _interactionAreaAppearanceManager;
#endif

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
        bool _isInitialized = false;


        /// <summary>
        /// Dictionary: the menu item => rulers units of measure.
        /// </summary>
        Dictionary<MenuItem, UnitOfMeasure> _menuItemToRulersUnitOfMeasure =
            new Dictionary<MenuItem, UnitOfMeasure>();

        /// <summary>
        /// Dictionary: the menu item => VOI LUT.
        /// </summary>
        Dictionary<MenuItem, DicomImageVoiLookupTable> _menuItemToVoiLut =
            new Dictionary<MenuItem, DicomImageVoiLookupTable>();

        /// <summary>
        /// Indicates that the visual tool of <see cref="ImageViewerToolBar"/> is changing.
        /// </summary>
        bool _isVisualToolChanging = false;

        /// <summary>
        /// Current application window state.
        /// </summary>
        WindowState _windowState;

        /// <summary>
        /// Decoding setting of DICOM frame.
        /// </summary>
        DicomDecodingSettings _dicomFrameDecodingSettings = new DicomDecodingSettings(false);

        /// <summary>
        /// A value indicating whether image coolection must be disposed after save.
        /// </summary>
        bool _disposeImageCollectionAfterSave = false;


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

        /// <summary>
        /// The folder browser dialog.
        /// </summary>
        System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

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
        public static RoutedCommand _showScrollbarsCommand = new RoutedCommand();
        public static RoutedCommand _fullScreenCommand = new RoutedCommand();
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

            InitFileDialogs();

            MoveDicomCodecToFirstPosition();

            _presentationStateFileExtensions = new string[] { ".DCM", ".DIC", ".ACR", ".PRE", "" };

            _voiLutsButton = FindVoiLutsButton();
            // init ImageViewerToolBar
            imageViewerToolBar.ImageViewer = imageViewer1;

            dicomAnnotatedViewerToolBar.ImageViewer = imageViewer1;
            MeasurementVisualToolActionFactory.CreateActions(dicomAnnotatedViewerToolBar);


            NoneAction noneAction = dicomAnnotatedViewerToolBar.FindAction<NoneAction>();
            noneAction.Activated += new EventHandler(noneAction_Activated);
            noneAction.Deactivated += new EventHandler(noneAction_Deactivated);

#if !REMOVE_ANNOTATION_PLUGIN
            ImageMeasureToolAction imageMeasureToolAction =
                  dicomAnnotatedViewerToolBar.FindAction<ImageMeasureToolAction>();
            imageMeasureToolAction.Activated += new EventHandler(imageMeasureToolAction_Activated);
#endif

            WpfMagnifierTool magnifierTool = new WpfMagnifierTool();
            // create action, which allows to magnify of image region in image viewer
            MagnifierToolAction magnifierToolAction = new MagnifierToolAction(
                magnifierTool,
                "Magnifier Tool",
                "Magnifier",
                DemosResourcesManager.GetResourceAsBitmap("WpfDemosCommonCode.Imaging.VisualToolsToolBar.VisualTools.ZoomVisualTools.Resources.WpfMagnifierTool.png"));

            _dicomViewerTool = new WpfDicomViewerTool();
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool = new WpfDicomAnnotatedViewerTool(
                 _dicomViewerTool,
                 new WpfDicomAnnotationTool(),
                 (Vintasoft.Imaging.Annotation.Wpf.UI.Measurements.WpfImageMeasureTool)imageMeasureToolAction.VisualTool);
            _dicomAnnotatedViewerTool.InteractionMode = WpfDicomAnnotatedViewerToolInteractionMode.None;

            _interactionAreaAppearanceManager = new WpfAnnotationInteractionAreaAppearanceManager();
            _interactionAreaAppearanceManager.VisualTool = _dicomAnnotatedViewerTool.DicomAnnotationTool;
#endif

            // add visual tools to tool strip
#if REMOVE_ANNOTATION_PLUGIN
            dicomAnnotatedViewerToolBar.DicomAnnotatedViewerTool = _dicomViewerTool;
#else
            dicomAnnotatedViewerToolBar.DicomAnnotatedViewerTool = _dicomAnnotatedViewerTool;
#endif
            dicomAnnotatedViewerToolBar.AddVisualToolAction(magnifierToolAction);
#if REMOVE_ANNOTATION_PLUGIN
            dicomAnnotatedViewerToolBar.MainVisualTool.ActiveTool = _dicomViewerTool;
#else
            dicomAnnotatedViewerToolBar.MainVisualTool.ActiveTool = _dicomAnnotatedViewerTool;
#endif

            magnifierToolAction.Activated += new EventHandler(magnifierToolAction_Activated);

            _dicomViewerTool.NavigateBySeries = true;
            _dicomViewerTool.ScrollProperties.IsVisible = true;
            _dicomViewerTool.ScrollProperties.Anchor = AnchorType.Left;

            _openDicomFileDialog.Multiselect = true;
            DemosTools.SetTestFilesFolder(_openDicomFileDialog);

#if REMOVE_ANNOTATION_PLUGIN
            WpfCompositeVisualTool compositeTool = new WpfCompositeVisualTool(_dicomViewerTool, magnifierTool);
            compositeTool.ActiveTool = _dicomViewerTool;
            imageViewer1.VisualTool = compositeTool;
#else
            WpfCompositeVisualTool compositeTool = new WpfCompositeVisualTool(_dicomAnnotatedViewerTool, magnifierTool);
            compositeTool.ActiveTool = _dicomAnnotatedViewerTool;
            imageViewer1.VisualTool = compositeTool;
#endif
            annotationsToolBar.Viewer = imageViewer1;
            imageViewer1.IsFastScrollingEnabled = false;
            imageViewer1.ImageDecodingSettings = (DecodingSettings)_dicomFrameDecodingSettings.Clone();


            DicomSrRenderingSettings dicomSrRenderingSettings = new DicomSrRenderingSettings();
            dicomSrRenderingSettings.BackgroundColor = VintasoftColor.Black;
            dicomSrRenderingSettings.ReportHeaderTextColor = VintasoftColor.White;
            dicomSrRenderingSettings.ItemTextColor = VintasoftColor.White;
            imageViewer1.ImageRenderingSettings = dicomSrRenderingSettings;


            dicomSeriesManagerControl1.DicomViewer = imageViewer1;
            dicomSeriesManagerControl1.AddedFileCountChanged += dicomSeriesManagerControl1_AddedFileCountChanged;
            dicomSeriesManagerControl1.AddFilesException += dicomSeriesManagerControl1_AddFilesException;

            dicomViewerToolInteractionButtonToolBar.Tool = _dicomViewerTool;

            // init DICOM annotation tool
            InitDicomAnnotationTool();

            _dicomViewerTool.DicomImageVoiLutChanged +=
                new EventHandler<WpfVoiLutChangedEventArgs>(dicomViewerTool_DicomImageVoiLutChanged);

            SubscribeToImageCollectionEvents(imageViewer1.Images);

            // init rulers unit of measure
            InitUnitOfMeasuresForRulers();

            _defaultVoiLutMenuItem = new MenuItem();
            _defaultVoiLutMenuItem.Header = "Default VOI LUT";
            _defaultVoiLutMenuItem.IsCheckable = true;
            _defaultVoiLutMenuItem.Click += new RoutedEventHandler(voiLutMenuItem_Click);

            Title = "VintaSoft WPF DICOM Viewer Demo v" + ImagingGlobalSettings.ProductVersion;

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
                    return DicomFile.GetFileAssociatedWithImage(image);

                return null;
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
            CloseDicomFiles();
        }

        #endregion


        #region 'File' menu

        /// <summary>
        /// Handles the Click event of openDicomFilesMenuItem object.
        /// </summary>
        private void openDicomFilesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddDicomFiles();
        }

        /// <summary>
        /// Handles the Click event of openDirectoryMenuItem object.
        /// </summary>
        private void openDirectoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenDirectory();
        }

        /// <summary>
        /// Handles the Click event of saveImagesAsMenuItem object.
        /// </summary>
        private void saveImagesAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageCollection images = GetSeriesImages();
            SubscribeToImageCollectionEvents(images);
            _disposeImageCollectionAfterSave = false;
            bool useMultipageEncoderOnly = images.Count > 1;

#if !REMOVE_ANNOTATION_PLUGIN
            WpfDicomAnnotationTool annotationTool = _dicomAnnotatedViewerTool.DicomAnnotationTool;
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
                    SubscribeToImageCollectionEvents(images);
                    _disposeImageCollectionAfterSave = true;
                }
            }
#endif

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
        /// Handles the Click event of saveDisplayedImageMenuItem object.
        /// </summary>
        private void saveDisplayedImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CodecsFileFilters.SetFilters(_saveFileDialog, false);
            // if file is selected in "Save file" dialog
            if (_saveFileDialog.ShowDialog() == true)
            {
                string saveFilename = Path.GetFullPath(_saveFileDialog.FileName);

                using (EncoderBase imageEncoder = GetEncoder(saveFilename))
                {
                    if (imageEncoder != null)
                    {
                        VintasoftImage image = _dicomViewerTool.GetDisplayedImage();
                        if (image == null)
                            image = imageViewer1.Image;

                        IsFileSaving = true;
                        try
                        {
                            // save images to a file
                            image.Save(saveFilename, imageEncoder);
                        }
                        catch (Exception ex)
                        {
                            DemosTools.ShowErrorMessage(ex);
                        }
                        finally
                        {
                            IsFileSaving = false;

                            if (image != imageViewer1.Image)
                                image.Dispose();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Handles the Click event of burnAndSaveToDICOMFileMenuItem object.
        /// </summary>
        private void burnAndSaveToDICOMFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            _saveFileDialog.Filter = "DICOM files|*.dcm";

            try
            {
                if (_saveFileDialog.ShowDialog() == true)
                {
                    // destination file path
                    string destFilePath = Path.GetFullPath(_saveFileDialog.FileName);

                    bool needUpdateFocusedSeries = false;

                    // get file path for focused image
                    string focusedImageFilePath = Path.GetFullPath(imageViewer1.Image.SourceInfo.Filename);
                    // if modified DICOM image must be saved to the source DICOM file
                    if (focusedImageFilePath == destFilePath)
                    {
                        // specify that focused series must be updated
                        needUpdateFocusedSeries = true;
                    }
                    // if modified DICOM image must be saved to a new DICOM file
                    else
                    {
                        // for each DICOM image in image viewer
                        foreach (VintasoftImage image in imageViewer1.Images)
                        {
                            // get file path for current image
                            string currentImageFilePath = Path.GetFullPath(image.SourceInfo.Filename);
                            // if DICOM image must be saved to the source DICOM file
                            if (currentImageFilePath == destFilePath)
                            {
                                throw new InvalidOperationException(
                                    "DICOM images can can be saved to the source file (if source file is focused in viewer) or to a new file.");
                            }
                        }
                    }

                    // burn annotations and measurements on DICOM images and save DICOM images to a file
                    _dicomAnnotatedViewerTool.BurnAndSaveToDicomFile(destFilePath);

                    // if focused series must be updated
                    if (needUpdateFocusedSeries)
                    {
                        // get identifier of focused image
                        string focusedImageId = dicomSeriesManagerControl1.SeriesManager.GetImageIdentifierByImage(imageViewer1.Image);
                        // get series identifier for focused image
                        string focusedImageSeriesId = dicomSeriesManagerControl1.SeriesManager.GetSeriesIdentifierByImage(imageViewer1.Image);
                        // get series images by series identifier
                        VintasoftImage[] seriesImages = dicomSeriesManagerControl1.SeriesManager.GetSeriesImages(focusedImageSeriesId);

                        // remove series images from image viewer
                        imageViewer1.Images.RemoveRange(seriesImages);
                        // for each series image
                        foreach (VintasoftImage imageForDispose in seriesImages)
                            // dispose image
                            imageForDispose.Dispose();

                        // load series images from file (we saved series images to the file in code above)
                        imageViewer1.Images.Add(destFilePath);

                        // get focused image by image identifier
                        VintasoftImage focusedImage = dicomSeriesManagerControl1.SeriesManager.GetImage(focusedImageId);
                        // if focused image is found
                        if (focusedImage != null)
                        {
                            // find image index in image viewer
                            int index = imageViewer1.Images.IndexOf(focusedImage);
                            // if index is found
                            if (index != -1)
                            {
                                // set focused image in image viewer
                                imageViewer1.FocusedIndex = index;
                            }
                        }

                        // update UI
                        UpdateUI();
                        UpdateUIWithInformationAboutDicomFile();
                    }
                }
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
#endif
        }

        /// <summary>
        /// Handles the Click event of saveViewerScreenshotMenuItem object.
        /// </summary>
        private void saveViewerScreenshotMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // get image of image viewer
            using (VintasoftImage image = imageViewer1.RenderViewerImage())
            {
                // save image to a file
                SaveImageFileWindow.SaveImageToFile(image, ImagingEncoderFactory.Default);
            }
        }

        /// <summary>
        /// Handles the Click event of closeDicomSeriesMenuItem object.
        /// </summary>
        private void closeDicomSeriesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // close DICOM file
            CloseDicomFiles();
        }

        /// <summary>
        /// Handles the Click event of exitMenuItem object.
        /// </summary>
        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion


        #region 'Edit' menu

        /// <summary>
        /// Handles the SubmenuOpened event of editMenuItem object.
        /// </summary>
        private void editMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            UpdateEditMenuItems();
        }

        /// <summary>
        /// Handles the SubmenuClosed event of editMenuItem object.
        /// </summary>
        private void editMenuItem_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            EnableEditMenuItems();
        }

        /// <summary>
        /// Handles the Click event of cutMenuItem object.
        /// </summary>
        private void cutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUiAction<CutItemUIAction>();
        }

        /// <summary>
        /// Handles the Click event of copyMenuItem object.
        /// </summary>
        private void copyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUiAction<CopyItemUIAction>();
        }

        /// <summary>
        /// Handles the Click event of pasteMenuItem object.
        /// </summary>
        private void pasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUiAction<PasteItemUIAction>();
        }

        /// <summary>
        /// Handles the Click event of deleteMenuItem object.
        /// </summary>
        private void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUiAction<DeleteItemUIAction>();
        }

        /// <summary>
        /// Handles the Click event of deleteAllMenuItem object.
        /// </summary>
        private void deleteAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ExecuteUiAction<DeleteAllItemsUIAction>();
        }

        #endregion


        #region 'View' menu

        #region Image viewer settings

        /// <summary>
        /// Handles the Click event of imageViewerSettingsMenuItem object.
        /// </summary>
        private void imageViewerSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageViewerSettingsWindow dlg = new ImageViewerSettingsWindow(imageViewer1);
            dlg.CanEditMultipageSettings = false;
            dlg.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of rotateClockwiseMenuItem object.
        /// </summary>
        private void rotateClockwiseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RotateViewClockwise();
        }

        /// <summary>
        /// Handles the Click event of rotateCounterclockwiseMenuItem object.
        /// </summary>
        private void rotateCounterclockwiseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RotateViewCounterClockwise();
        }

        /// <summary>
        /// Handles the Click event of fullScreenMenuItem object.
        /// </summary>
        private void fullScreenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            fullScreenMenuItem.IsChecked ^= true;

            if (fullScreenMenuItem.IsChecked)
            {
                // enable full screen mode
                menu1.Visibility = Visibility.Collapsed;
                toolBarTray1.Visibility = Visibility.Collapsed;
                InfoPanel.Visibility = Visibility.Collapsed;

                // update the form settings
                _windowState = WindowState;

                Topmost = true;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }
            else
            {
                // disable full screen mode
                menu1.Visibility = Visibility.Visible;
                toolBarTray1.Visibility = Visibility.Visible;
                InfoPanel.Visibility = Visibility.Visible;

                // update the form settings
                Topmost = true;
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Normal;
                if (WindowState != _windowState)
                    WindowState = _windowState;
            }
        }

        /// <summary>
        /// Handles the Click event of showViewerScrollbarsMenuItem object.
        /// </summary>
        private void showViewerScrollbarsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showViewerScrollbarsMenuItem.IsChecked ^= true;

            // show/hide scrollbars in image viewer
            imageViewer1.AutoScroll = showViewerScrollbarsMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the Click event of showBrowseScrollbarMenuItem object.
        /// </summary>
        private void showBrowseScrollbarMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showBrowseScrollbarMenuItem.IsChecked ^= true;

            _dicomViewerTool.ScrollProperties.IsVisible = showBrowseScrollbarMenuItem.IsChecked;
        }

        #endregion


        #region Overlay images

        /// <summary>
        /// Handles the Click event of showOverlayImagesMenuItem object.
        /// </summary>
        private void showOverlayImagesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // change decoding settings
            _dicomFrameDecodingSettings.ShowOverlayImages = showOverlayImagesMenuItem.IsChecked;

            // invalidates images and visual tool
            _dicomViewerTool.Refresh();
        }

        /// <summary>
        /// Handles the Click event of overlayColorMenuItem object.
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

                _dicomViewerTool.Refresh();
            }
        }

        #endregion


        #region Metadata

        /// <summary>
        /// Handles the Click event of showMetadataOnViewerMenuItem object.
        /// </summary>
        private void showMetadataOnViewerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showMetadataInViewerMenuItem.IsChecked ^= true;
            _dicomViewerTool.IsTextOverlayVisible = showMetadataInViewerMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the Click event of textOverlaySettingsMenuItem object.
        /// </summary>
        private void textOverlaySettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DicomOverlaySettingEditorWindow dlg = new DicomOverlaySettingEditorWindow(OVERLAY_OWNER_NAME, _dicomViewerTool);
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Owner = this;
            // show dialog
            dlg.ShowDialog();

            // set text overlay for DICOM viewer tool
            DicomOverlaySettingEditorWindow.SetTextOverlay(OVERLAY_OWNER_NAME, _dicomViewerTool);
            // refresh the DICOM viewer tool
            _dicomViewerTool.Refresh();
        }

        #endregion


        #region Rulers

        /// <summary>
        /// Handles the Click event of showRulersOnViewerMenuItem object.
        /// </summary>
        private void showRulersOnViewerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            showRulersInViewerMenuItem.IsChecked ^= true;
            _dicomViewerTool.ShowRulers = showRulersInViewerMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the Click event of rulersColorMenuItem object.
        /// </summary>
        private void rulersColorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Brush brush = _dicomViewerTool.VerticalImageRuler.RulerPen.Brush;
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
                _dicomViewerTool.VerticalImageRuler.RulerPen = new Pen(new SolidColorBrush(dlg.SelectedColor),
                      _dicomViewerTool.VerticalImageRuler.RulerPen.Thickness);
                _dicomViewerTool.HorizontalImageRuler.RulerPen = new Pen(new SolidColorBrush(dlg.SelectedColor),
                      _dicomViewerTool.HorizontalImageRuler.RulerPen.Thickness);

                // refresh DICOM viewer tool
                _dicomViewerTool.Refresh();
            }
        }

        /// <summary>
        /// Handles the Click event of rulersUnitOfMeasureMenuItem object.
        /// </summary>
        private void rulersUnitOfMeasureMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _currentRulersUnitOfMeasureMenuItem.IsChecked = false;
            _currentRulersUnitOfMeasureMenuItem = (MenuItem)e.OriginalSource;
            _dicomViewerTool.RulersUnitOfMeasure =
                _menuItemToRulersUnitOfMeasure[_currentRulersUnitOfMeasureMenuItem];
            _currentRulersUnitOfMeasureMenuItem.IsChecked = true;
        }

        #endregion


        #region VOI LUT

        /// <summary>
        /// Handles the Click event of voiLutMainMenuItem object.
        /// </summary>
        private void voiLutMainMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowCustomVoiLutForm();
        }

        /// <summary>
        /// Handles the Closing event of voiLutParamsWindow object.
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
        /// Handles the Click event of negativeImageMenuItem object.
        /// </summary>
        private void negativeImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            negativeImageMenuItem.IsChecked ^= true;
            _dicomViewerTool.IsImageNegative = negativeImageMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the DicomImageVoiLutChanged event of dicomViewerTool object.
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
                _dicomViewerTool.DefaultDicomImageVoiLut;

            // if the default VOI LUT is equal to new VOI LUT
            if (defaultVoiLut.WindowCenter == e.WindowCenter &&
                defaultVoiLut.WindowWidth == e.WindowWidth)
            {
                // specify that DICOM viewer tool must use VOI LUT from DICOM image metadata for DICOM image
                _dicomViewerTool.AlwaysLoadVoiLutFromMetadataOfDicomFrame = true;
            }
            else
            {
                // specify that DICOM viewer tool must use the same VOI LUT for all DICOM images
                _dicomViewerTool.AlwaysLoadVoiLutFromMetadataOfDicomFrame = false;
            }
        }

        /// <summary>
        /// Handles the Click event of widthHorizontalInvertedCenterVerticalMenuItem object.
        /// </summary>
        private void widthHorizontalInvertedCenterVerticalMenuItem_Click(object sender, RoutedEventArgs e)
        {
            widthHorizontalInvertedCenterVerticalMenuItem.IsChecked = true;
            widthHorizontalCenterVerticalMenuItem.IsChecked = false;
            widthVerticalCenterHorizontalMenuItem.IsChecked = false;

            _dicomViewerTool.DicomImageVoiLutCenterDirection = DicomInteractionDirection.BottomToTop;
            _dicomViewerTool.DicomImageVoiLutWidthDirection = DicomInteractionDirection.LeftToRight;
        }

        /// <summary>
        /// Handles the Click event of widthHorizontalCenterVerticalMenuItem object.
        /// </summary>
        private void widthHorizontalCenterVerticalMenuItem_Click(object sender, RoutedEventArgs e)
        {
            widthHorizontalInvertedCenterVerticalMenuItem.IsChecked = false;
            widthHorizontalCenterVerticalMenuItem.IsChecked = true;
            widthVerticalCenterHorizontalMenuItem.IsChecked = false;

            _dicomViewerTool.DicomImageVoiLutCenterDirection = DicomInteractionDirection.BottomToTop;
            _dicomViewerTool.DicomImageVoiLutWidthDirection = DicomInteractionDirection.RightToLeft;
        }

        /// <summary>
        /// Handles the Click event of widthVerticalCenterHorizontalMenuItem object.
        /// </summary>
        private void widthVerticalCenterHorizontalMenuItem_Click(object sender, RoutedEventArgs e)
        {
            widthHorizontalInvertedCenterVerticalMenuItem.IsChecked = false;
            widthHorizontalCenterVerticalMenuItem.IsChecked = false;
            widthVerticalCenterHorizontalMenuItem.IsChecked = true;

            _dicomViewerTool.DicomImageVoiLutCenterDirection = DicomInteractionDirection.RightToLeft;
            _dicomViewerTool.DicomImageVoiLutWidthDirection = DicomInteractionDirection.BottomToTop;
        }

        #endregion


        #region Magnifier

        /// <summary>
        /// Handles the Click event of magnifierSettings object.
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
        /// Handles the Click event of fileMetadataMenuItem object.
        /// </summary>
        private void fileMetadataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowCurrentFileMetadata();
        }

        #endregion


        #region 'Page' menu

        /// <summary>
        /// Handles the Click event of overlayImagesMenuItem object.
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
        /// Handles the Click event of showAnimationMenuItem object.
        /// </summary>
        private void showAnimationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            IsAnimationStarted = showAnimationMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the Click event of animationRepeatMenuItem object.
        /// </summary>
        private void animationRepeatMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _isAnimationCycled = animationRepeatMenuItem.IsChecked;
        }

        /// <summary>
        /// Handles the TextChanged event of animationDelayComboBox object.
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
        /// Handles the Click event of saveAsGifFileToolStripMenuItem object.
        /// </summary>
        private void saveAsGifFileToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageCollection images = GetSeriesImages();
            SubscribeToImageCollectionEvents(images);
            _disposeImageCollectionAfterSave = false;

#if !REMOVE_ANNOTATION_PLUGIN
            WpfDicomAnnotationTool annotationTool = _dicomAnnotatedViewerTool.DicomAnnotationTool;
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
                    SubscribeToImageCollectionEvents(images);
                    _disposeImageCollectionAfterSave = true;
                }
            }
#endif

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
        /// Handles the Click event of infoMenuItem object.
        /// </summary>
        private void infoMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            AnnotationsInfoWindow dialog = new AnnotationsInfoWindow(_dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataController);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Owner = this;
            dialog.ShowDialog();
#endif
        }

        /// <summary>
        /// Handles the Click event of interactionModeNoneMenuItem object.
        /// </summary>
        private void interactionModeNoneMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.None;
#endif
        }

        /// <summary>
        /// Handles the Click event of interactionModeViewMenuItem object.
        /// </summary>
        private void interactionModeViewMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.View;
#endif
        }

        /// <summary>
        /// Handles the Click event of interactionModeAuthorMenuItem object.
        /// </summary>
        private void interactionModeAuthorMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.Author;
#endif
        }

        /// <summary>
        /// Handles the Click event of interactionModeAnnotationEraserMenuItem object.
        /// </summary>
        private void interactionModeAnnotationEraserMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.AnnotationEraser;
#endif
        }

        /// <summary>
        /// Handles the SubmenuOpened event of annotationsMenuItem object.
        /// </summary>
        private void annotationsMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            WpfAnnotationView focusedAnnotationView = _dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationView;
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
#endif
        }

        /// <summary>
        /// Handles the Click event of transformationModeRectangularMenuItem object.
        /// </summary>
        private void transformationModeRectangularMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            WpfAnnotationView focusedAnnotationView = _dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationView;
            SetGripMode(focusedAnnotationView, GripMode.Rectangular);
            UpdateTransformationMenu();
#endif
        }

        /// <summary>
        /// Handles the Click event of transformationModePointsMenuItem object.
        /// </summary>
        private void transformationModePointsMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            WpfAnnotationView focusedAnnotationView = _dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationView;
            SetGripMode(focusedAnnotationView, GripMode.Points);
            UpdateTransformationMenu();
#endif
        }

        /// <summary>
        /// Handles the Click event of transformationModeRectangularAndPointsMenuItem object.
        /// </summary>
        private void transformationModeRectangularAndPointsMenuItem_Click(
            object sender,
            RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            WpfAnnotationView focusedAnnotationView = _dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationView;
            SetGripMode(focusedAnnotationView, GripMode.RectangularAndPoints);
            UpdateTransformationMenu();
#endif
        }


        /// <summary>
        /// Handles the Click event of presentationStateLoadMenuItem object.
        /// </summary>
        private void presentationStateLoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
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
                        _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataController.AddAnnotationDataSet(presentationStateFile.Annotations);
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
#endif
        }

        /// <summary>
        /// Handles the Click event of presentationStateInfoMenuItem object.
        /// </summary>
        private void presentationStateInfoMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            PresentationStateInfoWindow dialog = new PresentationStateInfoWindow(PresentationStateFile);
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Owner = this;
            dialog.ShowDialog();
#endif
        }

        /// <summary>
        /// Handles the Click event of presentationStateSaveMenuItem object.
        /// </summary>
        private void presentationStateSaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            if (_isAnnotationsLoadedForCurrentFrame)
            {
                DicomAnnotationCodec codec = new DicomAnnotationCodec();
                DicomAnnotationDataCollection collection = (DicomAnnotationDataCollection)
                    _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataController.GetAnnotations(imageViewer1.Image);
                codec.Encode(PresentationStateFile.Annotations, collection);
                PresentationStateFile.SaveChanges();
            }
            else
            {
                _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataController.UpdateAnnotationDataSets();
                PresentationStateFile.SaveChanges();
            }
            MessageBox.Show("Presentation state file is saved.");
#endif
        }

        /// <summary>
        /// Handles the Click event of presentationStateSaveToMenuItem object.
        /// </summary>
        private void presentationStateSaveToMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            string dicomFilePath = imageViewer1.Image.SourceInfo.Filename;
            _saveDicomAnnotationsFileDialog.FileName = Path.GetFileNameWithoutExtension(dicomFilePath) + ".pre";
            _saveDicomAnnotationsFileDialog.Filter = "Presentation State File(*.pre)|*.pre";
            _saveDicomAnnotationsFileDialog.FilterIndex = 1;

            if (_saveDicomAnnotationsFileDialog.ShowDialog() == true)
            {
                try
                {
                    _dicomAnnotatedViewerTool.DicomAnnotationTool.CancelAnnotationBuilding();

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
#endif
        }

        /// <summary>
        /// Handles the Click event of binaryFormatLoadMenuItem object.
        /// </summary>
        private void binaryFormatLoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadAnnotationFromBinaryOrXmpFormat(true);
        }

        /// <summary>
        /// Handles the Click event of binaryFormatSaveToMenuItem object.
        /// </summary>
        private void binaryFormatSaveToMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
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
                        AnnotationDataCollection annotations = _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataController.GetAnnotations(
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
#endif
        }

        /// <summary>
        /// Handles the Click event of xmpFormatLoadMenuItem object.
        /// </summary>
        private void xmpFormatLoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LoadAnnotationFromBinaryOrXmpFormat(false);
        }

        /// <summary>
        /// Handles the Click event of xmpFormatSaveToMenuItem object.
        /// </summary>
        private void xmpFormatSaveToMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
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
                        AnnotationDataCollection annotations = _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataController.GetAnnotations(
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
#endif
        }

        /// <summary>
        /// Handles the Click event of addMenuItem object.
        /// </summary>
        private void addMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            MenuItem item = (MenuItem)sender;
            if (_dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationView != null &&
                _dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationView.InteractionController ==
                _dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationView.Builder)
                _dicomAnnotatedViewerTool.DicomAnnotationTool.CancelAnnotationBuilding();
            annotationsToolBar.BuildAnnotation(item.Header.ToString());
#endif
        }

        /// <summary>
        /// Handles the Click event of propertiesMenuItem object.
        /// </summary>
        private void propertiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            PropertyGridWindow window = new PropertyGridWindow(
                _dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationView,
                "Annotation Properties");
            window.ShowDialog();
#endif
        }

        #endregion


        #region 'Help' menu

        /// <summary>
        /// Handles the aboutMenuItem_Click event of help object.
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
        /// Handles the OpenFile event of imageViewerToolBar object.
        /// </summary>
        private void imageViewerToolBar_OpenFile(object sender, EventArgs e)
        {
            AddDicomFiles();
        }

        #endregion


        #region Image Viewer

        /// <summary>
        /// Handles the ImageLoadingProgress event of imageViewer1 object.
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
        /// Handles the FocusedIndexChanged event of imageViewer1 object.
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
            }

            UpdateUIWithInformationAboutDicomFile();

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
                }
                finally
                {
                    _isFocusedIndexChanging = false;
                }
            }
        }

        /// <summary>
        /// Handles the Activated event of noneAction object.
        /// </summary>
        private void noneAction_Activated(object sender, EventArgs e)
        {
            // restore the DICOM viewer tool state
            dicomAnnotatedViewerToolBar.MainVisualTool.ActiveTool = dicomAnnotatedViewerToolBar.DicomAnnotatedViewerTool;
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool.InteractionMode = _prevDicomViewerToolInteractionMode;
            _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionMode = _prevDicomAnnotationToolInteractionMode;
#endif
        }

        /// <summary>
        /// Handles the Deactivated event of noneAction object.
        /// </summary>
        private void noneAction_Deactivated(object sender, EventArgs e)
        {
            // save the DICOM viewer tool state

#if !REMOVE_ANNOTATION_PLUGIN
            _prevDicomViewerToolInteractionMode = _dicomAnnotatedViewerTool.InteractionMode;
            _prevDicomAnnotationToolInteractionMode = _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionMode;
#endif
        }

        /// <summary>
        /// Handles the Activated event of imageMeasureToolAction object.
        /// </summary>
        private void imageMeasureToolAction_Activated(object sender, EventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            _isVisualToolChanging = true;
            dicomAnnotatedViewerToolBar.MainVisualTool.ActiveTool = dicomAnnotatedViewerToolBar.DicomAnnotatedViewerTool;
            _dicomAnnotatedViewerTool.ActiveTool = null;
            _isVisualToolChanging = false;
#endif
        }

        /// <summary>
        /// Handles the Activated event of magnifierToolAction object.
        /// </summary>
        private void magnifierToolAction_Activated(object sender, EventArgs e)
        {
            _isVisualToolChanging = true;
            dicomAnnotatedViewerToolBar.MainVisualTool.ActiveTool =
                dicomAnnotatedViewerToolBar.MainVisualTool.FindVisualTool<WpfMagnifierTool>();
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.None;
#endif
            _isVisualToolChanging = false;
        }

        #endregion


        #region DICOM Series Manager Control

        /// <summary>
        /// Handles the AddedFileCountChanged event of dicomSeriesManagerControl1 object.
        /// </summary>
        private void dicomSeriesManagerControl1_AddedFileCountChanged(object sender, EventArgs e)
        {
            WpfDicomSeriesManagerControl control = (WpfDicomSeriesManagerControl)sender;

            // if DICOM files loaded
            if (control.AddedFileCount == control.AddingFileCount)
            {
                // hide action label and progress bar
                progressBar1.Visibility = Visibility.Collapsed;
                progressBar1.Maximum = 0;

                // update the UI
                IsDicomFileOpening = false;
            }
            else
            {
                // if DICOM files loading started
                if (control.AddingFileCount != progressBar1.Maximum)
                {
                    progressBar1.Visibility = Visibility.Visible;
                    progressBar1.Maximum = control.AddingFileCount;
                    // update the UI
                    IsDicomFileOpening = true;
                }

                progressBar1.Value = control.AddedFileCount;
            }
        }

        /// <summary>
        /// Handles the AddFilesException event of dicomSeriesManagerControl1 object.
        /// </summary>
        private void dicomSeriesManagerControl1_AddFilesException(object sender, ImageSourceExceptionEventArgs e)
        {
            if (e.Exception.Message != "Image file does not contain pages (Dicom).")
                DemosTools.ShowErrorMessage(e.SourceFilename + ":" + Environment.NewLine + e.Exception.Message);
        }

        #endregion


        #region Annotations UI

        /// <summary>
        /// Handles the SelectionChanged event of annotationInteractionModeComboBox object.
        /// </summary>
        private void annotationInteractionModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionMode =
                   (AnnotationInteractionMode)annotationInteractionModeComboBox.SelectedItem;
#endif
        }

#if !REMOVE_ANNOTATION_PLUGIN
        /// <summary>
        /// Handles the AnnotationInteractionModeChanged event of annotationTool object.
        /// </summary>
        private void annotationTool_AnnotationInteractionModeChanged(object sender, AnnotationInteractionModeChangedEventArgs e)
        {
            if (!_isVisualToolChanging)
                dicomAnnotatedViewerToolBar.Reset();

            interactionModeNoneMenuItem.IsChecked = false;
            interactionModeViewMenuItem.IsChecked = false;
            interactionModeAuthorMenuItem.IsChecked = false;
            interactionModeAnnotationEraserMenuItem.IsChecked = false;

            AnnotationInteractionMode annotationInteractionMode = e.NewValue;
            switch (annotationInteractionMode)
            {
                case AnnotationInteractionMode.None:
                    interactionModeNoneMenuItem.IsChecked = true;
                    break;

                case AnnotationInteractionMode.View:
                    interactionModeViewMenuItem.IsChecked = true;
                    break;

                case AnnotationInteractionMode.Author:
                    interactionModeAuthorMenuItem.IsChecked = true;
                    break;

                case AnnotationInteractionMode.AnnotationEraser:
                    interactionModeAnnotationEraserMenuItem.IsChecked = true;
                    break;
            }

            annotationInteractionModeComboBox.SelectedItem = annotationInteractionMode;

            // update the UI
            UpdateUI();
        }
#endif

        #endregion


        #region Annotation visual tool

#if !REMOVE_ANNOTATION_PLUGIN
        /// <summary>
        /// Handles the FocusedAnnotationViewChanged event of annotationTool object.
        /// </summary>
        private void annotationTool_FocusedAnnotationViewChanged(object sender, WpfAnnotationViewChangedEventArgs e)
        {
            if (e.OldValue != null)
                e.OldValue.Data.PropertyChanging -= new EventHandler<ObjectPropertyChangingEventArgs>(AnnotationdData_PropertyChanging);
            if (e.NewValue != null)
                e.NewValue.Data.PropertyChanging += new EventHandler<ObjectPropertyChangingEventArgs>(AnnotationdData_PropertyChanging);

            // update the UI
            UpdateUI();
        }
#endif

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
        /// Handles the Click event of voiLutMenuItem object.
        /// </summary>
        private void voiLutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (imageViewer1.Image != null && sender is MenuItem)
            {
                if (_currentVoiLutMenuItem != null)
                    _currentVoiLutMenuItem.IsChecked = false;

                _currentVoiLutMenuItem = (MenuItem)sender;
                _currentVoiLutMenuItem.IsChecked = true;
                _dicomViewerTool.DicomImageVoiLut = _menuItemToVoiLut[_currentVoiLutMenuItem];
            }
        }

        /// <summary>
        /// Handles the Click event of customVoiLutMenuItem object.
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

            if (_disposeImageCollectionAfterSave)
            {
                images.ClearAndDisposeItems();
                _disposeImageCollectionAfterSave = false;
            }
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

#if REMOVE_ANNOTATION_PLUGIN
            if (_dicomViewerTool == null)
                return;
#else
            if (_dicomAnnotatedViewerTool == null)
                return;
#endif

            bool hasImages = imageViewer1.Images.Count > 0;
            bool isDicomFileLoaded = hasImages || DicomFile != null;
            bool isDicomFileOpening = _isDicomFileOpening;
            bool isAnnotationsFileLoaded = PresentationStateFile != null;
            bool isFileSaving = _isFileSaving;
            ImageCollection seriesImages = GetSeriesImages();
            bool isMultipageFile = seriesImages.Count > 1;
            bool isAnimationStarted = IsAnimationStarted;
            bool isImageSelected = imageViewer1.Image != null;
            bool isAnnotationEmpty = true;
            bool isImageNegative = _dicomViewerTool.IsImageNegative;
#if !REMOVE_ANNOTATION_PLUGIN
            if (isImageSelected)
                isAnnotationEmpty = _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataController[imageViewer1.FocusedIndex].Count <= 0;
#endif
            bool isAnnotationDataControllerEmpty = true;
#if !REMOVE_ANNOTATION_PLUGIN
            DicomAnnotationDataController dataController = _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataController;
            foreach (VintasoftImage image in seriesImages)
            {
                if (dataController.GetAnnotations(image).Count > 0)
                {
                    isAnnotationDataControllerEmpty = false;
                    break;
                }
            }
#endif
            bool isInteractionModeAuthor = false;
            bool isAnnotationFocused = false;
#if !REMOVE_ANNOTATION_PLUGIN
            isInteractionModeAuthor = _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionMode == AnnotationInteractionMode.Author;
            isAnnotationFocused = _dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationData != null;
#endif


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
            saveImageAsCurrentVOILUTMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving;
            burnAndSaveToDICOMFileMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving;
            saveViewerScreenshotMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && !isFileSaving;
            closeDicomSeriesMenuItem.IsEnabled = isDicomFileLoaded && !isFileSaving;
            imageViewerToolBar.IsEnabled = !isDicomFileOpening && !isFileSaving;

            // 'View' menu
            //
            showOverlayImagesMenuItem.IsEnabled = isDicomFileLoaded && !isDicomFileOpening && hasOverlayImages && !isFileSaving;
            overlayColorMenuItem.IsEnabled = showOverlayImagesMenuItem.IsEnabled;
            showMetadataInViewerMenuItem.IsEnabled = !isAnimationStarted;
            showRulersInViewerMenuItem.IsEnabled = !isAnimationStarted;
            rulersUnitOfMeasureMenuItem.IsEnabled = !isAnimationStarted;
            voiLutMainMenuItem.IsEnabled = !isAnimationStarted && isMonochromeImage;
            negativeImageMenuItem.IsChecked = isImageNegative;

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

            propertiesMenuItem.IsEnabled = isInteractionModeAuthor && isAnnotationFocused;
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
                _voiLutParamsWindow = new VoiLutParamsWindow(this, _dicomViewerTool);
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


        #region Interaction Points

        /// <summary>
        /// Changes settings of annotation interaction points.
        /// </summary>
        private void interactionPointsAppearanceMenuItem_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            WpfInteractionAreaAppearanceManagerWindow window = new WpfInteractionAreaAppearanceManagerWindow();
            window.InteractionAreaSettings = _interactionAreaAppearanceManager;
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            window.ShowDialog();
#endif
        }

        #endregion

        #endregion


        #region File manipulation

        /// <summary>
        /// Opens a DICOM file.
        /// </summary>
        private void AddDicomFiles()
        {
            if (_openDicomFileDialog.ShowDialog() == true)
            {
                if (_openDicomFileDialog.FileNames.Length > 0)
                {
                    // add DICOM files to the DICOM series
                    AddDicomFiles(_openDicomFileDialog.FileNames);

                    _openDicomFileDialog.InitialDirectory = null;
                }
            }
        }

        /// <summary>
        /// Adds the DICOM files.
        /// </summary>
        /// <param name="filesPath">Files path.</param>
        private void AddDicomFiles(params string[] filesPath)
        {
            dicomSeriesManagerControl1.AddFiles(filesPath, false);
        }

        /// <summary>
        /// Open a directory.
        /// </summary>
        private void OpenDirectory()
        {
            _folderBrowserDialog.ShowNewFolderButton = false;
            if (_folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AddDicomFilesFromDirectory(_folderBrowserDialog.SelectedPath);
            }
        }

        /// <summary>
        /// Adds the DICOM files from directory.
        /// </summary>
        /// <param name="filesPath">Files path.</param>
        private void AddDicomFilesFromDirectory(string filesPath)
        {
            dicomSeriesManagerControl1.AddDirectory(filesPath, true, false);
        }

        /// <summary>
        /// Closes series of DICOM frames.
        /// </summary>
        private void CloseDicomFiles()
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

            // clear image collection of image viewer and dispose all images
            dicomSeriesManagerControl1.CloseAllSeries();

            // update the UI
            UpdateUI();
            UpdateUIWithInformationAboutDicomFile();
        }

        #endregion


        #region Annotations

#if !REMOVE_ANNOTATION_PLUGIN
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
            DicomAnnotationDataController controller = _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataController;
            // for each image
            foreach (VintasoftImage image in imageViewer1.Images)
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
#endif

        /// <summary>
        /// Loads the annotation from binary or XMP packet.
        /// </summary>
        private void LoadAnnotationFromBinaryOrXmpFormat(bool binaryFormat)
        {
#if !REMOVE_ANNOTATION_PLUGIN
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
                        AnnotationDataCollection annotations = _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataCollection;
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
#endif
        }

#if !REMOVE_ANNOTATION_PLUGIN
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
#endif

        #endregion


        #region Annotations UI

        /// <summary>
        /// Updates the transformation mode menu.
        /// </summary>
        private void UpdateTransformationMenu()
        {
#if !REMOVE_ANNOTATION_PLUGIN
            WpfAnnotationView view = _dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationView;

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
#endif
        }

        #endregion


        #region Annotation visual tool

        /// <summary>
        /// The annotation property is changing.
        /// </summary>
        private void AnnotationdData_PropertyChanging(object sender, ObjectPropertyChangingEventArgs e)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            if (e.PropertyName == "UnitOfMeasure")
            {
                if (_isAnnotationPropertyChanging)
                    return;

                _isAnnotationPropertyChanging = true;
                DicomAnnotationData data = (DicomAnnotationData)sender;

                data.ChangeUnitOfMeasure((DicomUnitOfMeasure)e.NewValue, imageViewer1.Image);
                _isAnnotationPropertyChanging = false;
            }
#endif
        }


        #endregion


        #region Presentation state file

#if !REMOVE_ANNOTATION_PLUGIN
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
#endif

        /// <summary>
        /// Closes the DICOM presentation state file of focused DICOM file.
        /// </summary>
        private void CloseCurrentPresentationStateFile()
        {
            ClosePresentationStateFileOfFile(DicomFile);
        }

        /// <summary>
        /// Closes all DICOM presentation state files.
        /// </summary>
        private void CloseAllPresentationStateFiles()
        {
            DicomFile[] dicomFiles = DicomFile.GetFilesAssociatedWithImages(imageViewer1.Images.ToArray());
            foreach (DicomFile dicomFile in dicomFiles)
                ClosePresentationStateFileOfFile(dicomFile);
        }

        /// <summary>
        /// Closes the DICOM presentation state file of specified DICOM file.
        /// </summary>
        /// <param name="dicomFile">The DICOM file.</param>
        private void ClosePresentationStateFileOfFile(DicomFile dicomFile)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            // get the presentation state file of source DICOM file
            DicomFile presentationStateFile = PresentationStateFileController.GetPresentationStateFile(dicomFile);

            if (presentationStateFile == null)
                return;

            // get controller of DicomAnnotationTool
            DicomAnnotationDataController controller = _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationDataController;

            // remove annotations from controller
            controller.RemoveAnnotationDataSet(presentationStateFile.Annotations);

            // close the presentation state file of source DICOM file
            PresentationStateFileController.ClosePresentationStateFile(dicomFile);
#endif
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

            _menuItemToVoiLut.Add(
                _defaultVoiLutMenuItem,
                new DicomImageVoiLookupTable(double.NaN, double.NaN));

            DicomFrameMetadata metadata = GetFocusedImageMetadata();
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
            DicomPageMetadata metadata = GetFocusedPageMetadata();
            // if DICOM image does not have metadata
            if (metadata == null)
                // get metadata of DICOM file
                metadata = new DicomPageMetadata(DicomFile);
            window.RootMetadataNode = metadata;

            // show dialog
            window.ShowDialog();

            // if image viewer has image
            if (imageViewer1.Image != null)
            {
                // update the UI with information about DICOM file
                UpdateUIWithInformationAboutDicomFile();
                // refresh the DICOM viewer tool
                _dicomViewerTool.Refresh();
            }

            UpdateUI();
        }

        /// <summary>
        /// Returns the metadata of focused image.
        /// </summary>
        private DicomPageMetadata GetFocusedPageMetadata()
        {
            if (imageViewer1.Image == null)
                return null;

            DicomPageMetadata metadata = imageViewer1.Image.Metadata.MetadataTree as DicomPageMetadata;

            return metadata;
        }

        /// <summary>
        /// Returns the metadata of focused image.
        /// </summary>
        private DicomFrameMetadata GetFocusedImageMetadata()
        {
            return GetFocusedPageMetadata() as DicomFrameMetadata;
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
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool.Enabled = false;
#endif
            _dicomViewerTool.IsTextOverlayVisible = false;
            _dicomViewerTool.ShowRulers = false;
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
            _dicomViewerTool.IsTextOverlayVisible = showMetadataInViewerMenuItem.IsChecked;
            _dicomViewerTool.ShowRulers = showRulersInViewerMenuItem.IsChecked;
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool.Enabled = true;
#endif
        }

        /// <summary>
        /// Animation thread.
        /// </summary>
        private void AnimationMethod()
        {
            Thread currentThread = Thread.CurrentThread;
            _currentAnimatedFrameIndex = imageViewer1.FocusedIndex;

            VintasoftImage[] seriesImages = dicomSeriesManagerControl1.SeriesManager.GetSeriesImages(
                dicomSeriesManagerControl1.SeriesManager.GetSeriesIdentifierByImage(imageViewer1.Image));
            int index = Array.IndexOf(seriesImages, imageViewer1.Image);
            int count = seriesImages.Length;

            for (; _currentAnimatedFrameIndex < count || _isAnimationCycled;)
            {
                if (_animationThread != currentThread)
                    break;

                _isFocusedIndexChanging = true;
                _currentAnimatedFrameIndex = imageViewer1.Images.IndexOf(seriesImages[index]);
                // change focused image in image viewer

                _canStopAnimation = false;
                imageViewer1.SetFocusedIndexSync(_currentAnimatedFrameIndex);
                _canStopAnimation = true;

                _isFocusedIndexChanging = false;
                Thread.Sleep(_animationDelay);

                index++;
                if (_isAnimationCycled && index >= count)
                    index = 0;
            }

            if (index == 0)
                _currentAnimatedFrameIndex = 0;
            else
                _currentAnimatedFrameIndex = imageViewer1.Images.IndexOf(seriesImages[index - 1]);
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
            imageViewer1.RotateViewClockwise();
        }

        /// <summary>
        /// Rotates images in both annotation viewer and thumbnail viewer by 90 degrees counterclockwise.
        /// </summary>
        private void RotateViewCounterClockwise()
        {
            imageViewer1.RotateViewCounterClockwise();
        }

        #endregion


        #region Init

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
#if !REMOVE_ANNOTATION_PLUGIN
            _dicomAnnotatedViewerTool.DicomAnnotationTool.MultiSelect = false;
            _dicomAnnotatedViewerTool.DicomAnnotationTool.FocusedAnnotationViewChanged +=
                new EventHandler<WpfAnnotationViewChangedEventArgs>(annotationTool_FocusedAnnotationViewChanged);
            _dicomAnnotatedViewerTool.DicomAnnotationTool.SelectedAnnotations.Changed += new EventHandler(SelectedAnnotations_Changed);
            _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionModeChanged +=
                new EventHandler<AnnotationInteractionModeChangedEventArgs>(annotationTool_AnnotationInteractionModeChanged);

            _dicomAnnotatedViewerTool.DicomAnnotationTool.AnnotationInteractionMode = AnnotationInteractionMode.None;

            annotationInteractionModeComboBox.Items.Add(AnnotationInteractionMode.None);
            annotationInteractionModeComboBox.Items.Add(AnnotationInteractionMode.View);
            annotationInteractionModeComboBox.Items.Add(AnnotationInteractionMode.Author);
            annotationInteractionModeComboBox.Items.Add(AnnotationInteractionMode.AnnotationEraser);
            // set interaction mode to the View 
            annotationInteractionModeComboBox.SelectedItem = AnnotationInteractionMode.None;
#endif
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
                if (unit == _dicomViewerTool.RulersUnitOfMeasure)
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
        /// Subscribes to the event of image collection.
        /// </summary>
        /// <param name="images">Image collection.</param>
        private void SubscribeToImageCollectionEvents(ImageCollection images)
        {
            images.ImageCollectionSavingProgress += new EventHandler<ProgressEventArgs>(Images_ImageCollectionSavingProgress);
            images.ImageCollectionSavingFinished += new EventHandler(Images_ImageCollectionSavingFinished);
        }

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
        /// Handles the CanExecute event of openCommandBinding object.
        /// </summary>
        private void openCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = openDicomFilesMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of closeCommandBinding object.
        /// </summary>
        private void closeCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = closeDicomSeriesMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of exitCommandBinding object.
        /// </summary>
        private void exitCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = exitMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of isNegativeCommandBinding object.
        /// </summary>
        private void isNegativeCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = negativeImageMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of cutCommandBinding object.
        /// </summary>
        private void cutCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = cutMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of copyCommandBinding object.
        /// </summary>
        private void copyCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = copyMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of pasteCommandBinding object.
        /// </summary>
        private void pasteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = pasteMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of deleteCommandBinding object.
        /// </summary>
        private void deleteCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = deleteMenuItem.IsEnabled && imageViewer1.IsMouseOver;
            e.ContinueRouting = !e.CanExecute;
        }

        /// <summary>
        /// Handles the CanExecute event of deleteAllCommandBinding object.
        /// </summary>
        private void deleteAllCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = deleteAllMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of rotateClockwiseCommandBinding object.
        /// </summary>
        private void rotateClockwiseCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rotateClockwiseMenuItem.IsEnabled;
        }

        /// <summary>
        /// Handles the CanExecute event of rotateCounterclockwiseCommandBinding object.
        /// </summary>
        private void rotateCounterclockwiseCommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = rotateCounterclockwiseMenuItem.IsEnabled;
        }

        #endregion


        #region Drag&Drop

        /// <summary>
        /// Handles the Dragging event of imageViewer1 object.
        /// </summary>
        private void imageViewer1_Dragging(object sender, DragEventArgs e)
        {
            // if image files are dragging
            if (e.Data.GetDataPresent("FileNameW"))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Handles the Drop event of imageViewer1 object.
        /// </summary>
        private void imageViewer1_Drop(object sender, DragEventArgs e)
        {
            // if image viewer allows to drop image files and image files are dropped
            if (e.Data.GetDataPresent("FileDrop"))
            {
                // get image file names
                string[] filenames = (string[])e.Data.GetData("FileDrop");

                if (sender is WpfImageViewer)
                    // close the previously opened DICOM files
                    CloseDicomFiles();

                foreach (string filename in filenames)
                {
                    // if is directory
                    if (Directory.Exists(filename))
                        // add files from directory
                        AddDicomFilesFromDirectory(filename);
                    else
                        // add DICOM files to the DICOM series
                        AddDicomFiles(filename);
                }
            }
        }

        /// <summary>
        /// Returns images for DICOM series that contains focused image.
        /// </summary>
        /// <returns>
        /// The collection of DICOM images.
        /// </returns>
        private ImageCollection GetSeriesImages()
        {
            string seriesIdentifier = dicomSeriesManagerControl1.SeriesManager.GetSeriesIdentifierByImage(imageViewer1.Image);
            VintasoftImage[] seriesImages = dicomSeriesManagerControl1.SeriesManager.GetSeriesImages(seriesIdentifier);
            ImageCollection images = new ImageCollection();
            images.AddRange(seriesImages);
            return images;
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
