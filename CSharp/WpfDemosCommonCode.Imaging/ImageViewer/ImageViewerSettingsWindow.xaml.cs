using System;
using System.Windows;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs;
using Vintasoft.Imaging.Codecs.Decoders;
using Vintasoft.Imaging.ImageRendering;
using Vintasoft.Imaging.UI;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to view and change settings of the image viewer.
    /// </summary>
    public partial class ImageViewerSettingsWindow : Window
    {

        #region Fields

        /// <summary>
        /// The image viewer.
        /// </summary>
        WpfImageViewer _viewer;

        /// <summary>
        /// The rendering settings of image viewer.
        /// </summary>
        RenderingSettings _renderingSettings;

        /// <summary>
        /// The rendering requirements of image in image viewer.
        /// </summary>
        ImageRenderingRequirements _renderingRequirements;

        /// <summary>
        /// The back color of image appearance in image viewer.
        /// </summary>
        Color _appearanceBackColor;

        /// <summary>
        /// The border color of image appearance in image viewer.
        /// </summary>
        Color _appearanceBorederColor;

        /// <summary>
        /// The border width of image appearance in image viewer.
        /// </summary>
        float _appearanceBorderWidth;

        /// <summary>
        /// The back color of focused image appearance in image viewer.
        /// </summary>
        Color _focusedAppearanceBackColor;

        /// <summary>
        /// The border color of focused image appearance in image viewer.
        /// </summary>
        Color _focusedAppearanceBorederColor;

        /// <summary>
        /// The border width of focused image appearance in image viewer.
        /// </summary>
        float _focusedAppearanceBorderWidth;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageViewerSettingsWindow"/> class.
        /// </summary>
        public ImageViewerSettingsWindow()
        {
            InitializeComponent();

            // init "Rendering quality"
            renderingQualityComboBox.Items.Add(ImageRenderingQuality.Low);
            renderingQualityComboBox.Items.Add(ImageRenderingQuality.Normal);
            renderingQualityComboBox.Items.Add(ImageRenderingQuality.High);

            // init "Layout direction"
            layoutDirectionComboBox.Items.Add(ImagesLayoutDirection.Horizontal);
            layoutDirectionComboBox.Items.Add(ImagesLayoutDirection.Vertical);

            // init "Multipage display mode"
            multipageDisplayModeComboBox.Items.Add(ImageViewerMultipageDisplayMode.FixedImages);
            multipageDisplayModeComboBox.Items.Add(ImageViewerMultipageDisplayMode.FixedImagesContinuous);
            multipageDisplayModeComboBox.Items.Add(ImageViewerMultipageDisplayMode.AllImages);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageViewerSettingsWindow"/> class.
        /// </summary>
        /// <param name="viewer">The image viewer.</param>
        public ImageViewerSettingsWindow(WpfImageViewer viewer)
            : this()
        {
            _viewer = viewer;
            ShowSettings();
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether multipage settings can be edited.
        /// </summary>
        public bool CanEditMultipageSettings
        {
            get
            {
                if (imageDisplayModeGroupBox.Visibility == Visibility.Visible)
                    return true;
                return false;
            }
            set
            {
                if (value)
                    imageDisplayModeGroupBox.Visibility = Visibility.Visible;
                else
                    imageDisplayModeGroupBox.Visibility = Visibility.Collapsed;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Shows settings of the image viewer.
        /// </summary>
        private void ShowSettings()
        {
            // image anchor
            imageAnchorTypeEditor.SelectedAnchorType = _viewer.ImageAnchor;

            // rendering quality
            renderingQualityComboBox.SelectedItem = _viewer.RenderingQuality;

            // focus point
            focusPointAnchorTypeEditor.SelectedAnchorType = _viewer.FocusPointAnchor;
            focusPointIsFixedCheckBox.IsChecked = _viewer.IsFocusPointFixed;

            // buffering
            rendererCacheSizeNumericUpDown.Value = (int)Math.Round(_viewer.RendererCacheSize);
            viewerBufferSizeNumericUpDown.Value = (int)Math.Round(_viewer.ViewerBufferSize);
            minImageSizeWhenZoomBufferUsedNumericUpDown.Value = (int)Math.Round(_viewer.MinImageSizeWhenZoomBufferUsed);

            // backgroud color
            if (_viewer.Background == null)
                backgroundColorPanelControl.Color = Colors.Transparent;
            else
                backgroundColorPanelControl.Color = (_viewer.Background as SolidColorBrush).Color;

            // rendering
            _renderingSettings = _viewer.ImageRenderingSettings;
            previewIntervalOfVectorImagesNumericUpDown.Value = _viewer.IntermediateImagePreviewInterval;
            vectorRenderingQualityFactorSlider.Value = (_viewer.VectorRenderingQualityFactor - 1) * 4.0;
            maxThreadsNumericUpDown.Value = _viewer.MaxThreadsForRendering;
            renderOnlyVisibleImagesCheckBox.IsChecked = _viewer.RenderOnlyVisibleImages;

            // image display mode
            multipageDisplayModeComboBox.SelectedItem = _viewer.MultipageDisplayMode;
            layoutDirectionComboBox.SelectedItem = _viewer.MultipageDisplayLayoutDirection;
            imagesInRowColumnNumericUpDown.Value = _viewer.MultipageDisplayRowCount;
            imagesPaddingNumericUpDown.Value = (int)_viewer.MultipageDisplayImagePadding.All;

            // image appearances
            _appearanceBackColor = _viewer.ImageBackgroundColor;
            _appearanceBorederColor = _viewer.ImageBorderColor;
            _appearanceBorderWidth = _viewer.ImageBorderWidth;
            _focusedAppearanceBackColor = _viewer.FocusedImageBackgroundColor;
            _focusedAppearanceBorederColor = _viewer.FocusedImageBorderColor;
            _focusedAppearanceBorderWidth = _viewer.FocusedImageBorderWidth;

            // use image appearances in single page mode
            useImageAppearancesInSinglepageModeCheckBox.IsChecked = _viewer.UseImageAppearancesInSinglePageMode;

            // keyboard
            keyboardNavigationCheckBox.IsChecked = _viewer.IsKeyboardNavigationEnabled;
            keyboardNavigationGroupBox.IsEnabled = keyboardNavigationCheckBox.IsChecked.Value == true;
            keyboardNavigationScrollStepNumericUpDown.Value = _viewer.KeyboardNavigationScrollStep;
            keyboardNavigationZoomStepNumericUpDown.Value = _viewer.KeyboardNavigationZoomStep;
        }

        /// <summary>
        /// Sets settings to the image viewer.
        /// </summary>
        private void SetSettings()
        {
            // image anchor
            _viewer.ImageAnchor = imageAnchorTypeEditor.SelectedAnchorType;

            // rendering quality
            _viewer.RenderingQuality = (ImageRenderingQuality)renderingQualityComboBox.SelectedItem;

            // focus point
            _viewer.FocusPointAnchor = focusPointAnchorTypeEditor.SelectedAnchorType;
            _viewer.IsFocusPointFixed = focusPointIsFixedCheckBox.IsChecked.Value;

            // buffering
            _viewer.RendererCacheSize = (int)rendererCacheSizeNumericUpDown.Value;
            _viewer.ViewerBufferSize = (int)viewerBufferSizeNumericUpDown.Value;
            _viewer.MinImageSizeWhenZoomBufferUsed = (int)minImageSizeWhenZoomBufferUsedNumericUpDown.Value;

            // rendering
            _viewer.VectorRenderingQualityFactor =
                (float)(1 + (vectorRenderingQualityFactorSlider.Value / 4.0));

            if (_viewer.ImageRenderingSettings == null)
            {
                _viewer.ImageRenderingSettings = _renderingSettings.CreateClone();
            }
            else
            {
                _viewer.ImageRenderingSettings.InterpolationMode = _renderingSettings.InterpolationMode;
                _viewer.ImageRenderingSettings.SmoothingMode = _renderingSettings.SmoothingMode;
                _viewer.ImageRenderingSettings.Resolution = _renderingSettings.Resolution;
                _viewer.ImageRenderingSettings.OptimizeImageDrawing = _renderingSettings.OptimizeImageDrawing;
            }

            if (_renderingRequirements != null)
                _viewer.RenderingRequirements = _renderingRequirements;

            _viewer.IntermediateImagePreviewInterval = (int)previewIntervalOfVectorImagesNumericUpDown.Value;

            _viewer.MaxThreadsForRendering = (int)maxThreadsNumericUpDown.Value;

            _viewer.RenderOnlyVisibleImages = renderOnlyVisibleImagesCheckBox.IsChecked.Value;

            // backgroud color
            _viewer.Background = new SolidColorBrush(backgroundColorPanelControl.Color);

            // image display mode
            _viewer.MultipageDisplayMode = (ImageViewerMultipageDisplayMode)multipageDisplayModeComboBox.SelectedItem;
            _viewer.MultipageDisplayLayoutDirection = (ImagesLayoutDirection)layoutDirectionComboBox.SelectedItem;
            _viewer.MultipageDisplayRowCount = (int)imagesInRowColumnNumericUpDown.Value;
            _viewer.MultipageDisplayImagePadding = new PaddingF((float)imagesPaddingNumericUpDown.Value);

            // image appearances
            _viewer.ImageBackgroundColor = _appearanceBackColor;
            _viewer.ImageBorderColor = _appearanceBorederColor;
            _viewer.ImageBorderWidth = _appearanceBorderWidth;
            _viewer.FocusedImageBackgroundColor = _focusedAppearanceBackColor;
            _viewer.FocusedImageBorderColor = _focusedAppearanceBorederColor;
            _viewer.FocusedImageBorderWidth = _focusedAppearanceBorderWidth;

            // use image appearances in single page mode
            _viewer.UseImageAppearancesInSinglePageMode = ((bool)useImageAppearancesInSinglepageModeCheckBox.IsChecked);

            // keyboard
            _viewer.IsKeyboardNavigationEnabled = keyboardNavigationCheckBox.IsChecked.Value == true;
            _viewer.KeyboardNavigationScrollStep = (int)keyboardNavigationScrollStepNumericUpDown.Value;
            _viewer.KeyboardNavigationZoomStep = (float)keyboardNavigationZoomStepNumericUpDown.Value;
        }

        /// <summary>
        /// Shows Rendering Settings Window.
        /// </summary>
        private void renderingSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            RenderingSettingsWindow renderingSettingsDialog = new RenderingSettingsWindow(_renderingSettings);
            renderingSettingsDialog.Owner = this;
            if (renderingSettingsDialog.ShowDialog().Value)
                _renderingSettings = renderingSettingsDialog.RenderingSettings;
        }

        /// <summary>
        /// "Ok" button is clicked.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            SetSettings();

            DialogResult = true;
        }

        /// <summary>
        /// "Cancel" button is clicked.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Shows Rendering Requirements Window.
        /// </summary>
        private void renderingRequirementsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_renderingRequirements == null)
                _renderingRequirements = new ImageRenderingRequirements(_viewer.RenderingRequirements);
            ImageRenderingRequirementsWindow renderingRequirements = new ImageRenderingRequirementsWindow(_renderingRequirements);
            renderingRequirements.Owner = this;
            if (renderingRequirements.ShowDialog().Value)
                _renderingRequirements = renderingRequirements.RenderingRequirements;
        }

        /// <summary>
        /// Shows Image Appearance Settings window for image appearance.
        /// </summary>
        private void imageAppearanceButton_Click(object sender, RoutedEventArgs e)
        {
            ImageAppearanceSettingsWindow appearanceWindow = new ImageAppearanceSettingsWindow("Image Appearance Settings");
            appearanceWindow.AppearanceBackColor = _appearanceBackColor;
            appearanceWindow.AppearanceBorderColor = _appearanceBorederColor;
            appearanceWindow.AppearanceBorderWidth = (int)_appearanceBorderWidth;

            if (appearanceWindow.ShowDialog().Value)
            {
                _appearanceBackColor = appearanceWindow.AppearanceBackColor;
                _appearanceBorederColor = appearanceWindow.AppearanceBorderColor;
                _appearanceBorderWidth = appearanceWindow.AppearanceBorderWidth;
            }
        }

        /// <summary>
        /// Shows Image Appearance Settings window for focused image appearance.
        /// </summary>
        private void focusedImageAppearanceButton_Click(object sender, RoutedEventArgs e)
        {
            ImageAppearanceSettingsWindow appearanceWindow = new ImageAppearanceSettingsWindow("Focused Image Appearance Settings");
            appearanceWindow.AppearanceBackColor = _focusedAppearanceBackColor;
            appearanceWindow.AppearanceBorderColor = _focusedAppearanceBorederColor;
            appearanceWindow.AppearanceBorderWidth = (int)_focusedAppearanceBorderWidth;

            if (appearanceWindow.ShowDialog().Value)
            {
                _focusedAppearanceBackColor = appearanceWindow.AppearanceBackColor;
                _focusedAppearanceBorederColor = appearanceWindow.AppearanceBorderColor;
                _focusedAppearanceBorderWidth = appearanceWindow.AppearanceBorderWidth;
            }
        }

        /// <summary> 
        /// The keyboard navigation visibility is changed.
        /// </summary>
        private void KeyboardNavigationCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            keyboardNavigationGroupBox.IsEnabled = keyboardNavigationCheckBox.IsChecked.Value == true;
        }

        #endregion
        
    }
}
