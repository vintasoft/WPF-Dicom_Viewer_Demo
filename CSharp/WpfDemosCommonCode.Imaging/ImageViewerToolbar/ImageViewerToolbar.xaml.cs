using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Vintasoft.Imaging;
using Vintasoft.Imaging.UI;
using Vintasoft.Imaging.Wpf.UI;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A toolbar of image viewer.
    /// </summary>
    public partial class ImageViewerToolBar : ToolBar
    {

        #region Fields

        /// <summary>
        /// Available zoom values.
        /// </summary>
        int[] _wpfZoomValues = new int[] { 1, 5, 10, 25, 50, 75, 100, 125, 150, 200, 400, 600, 800, 1000, 2000, 4000, 8000, 10000, 20000, 50000, 100000 };

        /// <summary>
        /// Selected "image scale mode" menu item.
        /// </summary>
        MenuItem _imageScaleSelected;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageViewerToolBar"/> class.
        /// </summary>>
        public ImageViewerToolBar()
        {
            InitializeComponent();

            _imageScaleSelected = NormalMenuItem;
            NormalMenuItem.IsChecked = true;

            _images_CollectionChangedEventThreadSafe = new Images_CollectionChangedThreadSafeDelegate(Images_CollectionChangedSafely);

            // init save, scan and print buttons
            _saveButtonEnabled = SaveButton.IsEnabled;
            _scanButtonEnabled = ScanButton.IsEnabled;
            _printButtonEnabled = PrintButton.IsEnabled;
        }

        #endregion



        #region Properties

        WpfImageViewer _imageViewer = null;
        /// <summary>
        /// Gets or sets an image viewer associated with this toolbar.
        /// </summary>
        [Browsable(true)]
        public WpfImageViewer ImageViewer
        {
            get
            {
                return _imageViewer;
            }
            set
            {
                if (_imageViewer != null)
                {
                    // unsubscribe from the ImagesChanging/ImagesChanged events of the viewer
                    _imageViewer.ImagesChanging -= new EventHandler(imageViewer_ImagesChanging);
                    _imageViewer.ImagesChanged -= new EventHandler(imageViewer_ImagesChanged);
                    // unsubscribe from the ImageCollectionChanged event of image collection of the viewer
                    _imageViewer.Images.ImageCollectionChanged -= new EventHandler<ImageCollectionChangeEventArgs>(Images_CollectionChangedSafely);
                    // unsubscribe from the FocusedIndexChanged event of the viewer
                    _imageViewer.FocusedIndexChanged -= new PropertyChangedEventHandler<int>(imageViewer_FocusedIndexChanged);
                    // unsubscribe from the ZoomChanged event of the viewer
                    _imageViewer.ZoomChanged -= new EventHandler<ZoomChangedEventArgs>(imageViewer_ZoomChanged);
                }

                _imageViewer = value;

                if (_imageViewer != null)
                {
                    // subscribe to the ImagesChanging/ImagesChanged events of the viewer
                    _imageViewer.ImagesChanging += new EventHandler(imageViewer_ImagesChanging);
                    _imageViewer.ImagesChanged += new EventHandler(imageViewer_ImagesChanged);
                    // subscribe to the ImageCollectionChanged event of image collection of the viewer
                    _imageViewer.Images.ImageCollectionChanged += new EventHandler<ImageCollectionChangeEventArgs>(Images_CollectionChangedSafely);
                    // subscribe to the FocusedIndexChanged event of the viewer
                    _imageViewer.FocusedIndexChanged += new PropertyChangedEventHandler<int>(imageViewer_FocusedIndexChanged);
                    // subscribe to the ZoomChanged event of the viewer
                    _imageViewer.ZoomChanged += new EventHandler<ZoomChangedEventArgs>(imageViewer_ZoomChanged);

                    if (UseImageViewerImages)
                        PageCountLabel.Text = PageCount.ToString(CultureInfo.InvariantCulture);
                }

                if (_imageViewer != null)
                    imageViewer_FocusedIndexChanged(this, new PropertyChangedEventArgs<int>(-1, _imageViewer.FocusedIndex));
            }
        }

        bool _useImageViewerImages = true;
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar synchronizes with image collection of image viewer.
        /// </summary>
        [Browsable(false)]
        public bool UseImageViewerImages
        {
            get
            {
                return _useImageViewerImages;
            }
            set
            {
                _useImageViewerImages = value;
            }
        }

        Slider _associatedZoomSlider = null;
        /// <summary>
        /// Gets or sets a zoom slider associated with this toolbar.
        /// </summary>
        [Browsable(true)]
        public Slider AssociatedZoomSlider
        {
            get
            {
                return _associatedZoomSlider;
            }
            set
            {
                if (_associatedZoomSlider != null)
                    _associatedZoomSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(_associatedZoomSlider_ValueChanged);

                _associatedZoomSlider = value;

                if (_associatedZoomSlider != null)
                    _associatedZoomSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(_associatedZoomSlider_ValueChanged);
            }
        }

        bool _canOpenFile = true;
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar has button for loading of image from file.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        public bool CanOpenFile
        {
            get
            {
                return _canOpenFile;
            }
            set
            {
                SetVisibility(_canOpenFile = value, OpenButton);

                SetVisibility(value || CanSaveFile, OpenSaveFileSeparator);
            }
        }

        bool _openButtonEnabled;
        /// <summary>
        /// Gets or sets a value indicating whether the button for loading of image from file is enabled.
        /// </summary>
        [Browsable(true)]
        public bool OpenButtonEnabled
        {
            get
            {
                return _openButtonEnabled;
            }
            set
            {
                _openButtonEnabled = value;
                OpenButton.IsEnabled = value;
            }
        }

        bool _canSaveFile = true;
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar has button for saving of image to a file.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        public bool CanSaveFile
        {
            get
            {
                return _canSaveFile;
            }
            set
            {
                SetVisibility(_canSaveFile = value, SaveButton);

                SetVisibility(CanOpenFile || value, OpenSaveFileSeparator);
            }
        }

        bool _saveButtonEnabled;
        /// <summary>
        /// Gets or sets a value indicating whether the button for saving of image is enabled.
        /// </summary>
        [Browsable(true)]
        public bool SaveButtonEnabled
        {
            get
            {
                return _saveButtonEnabled;
            }
            set
            {
                _saveButtonEnabled = value;
                SaveButton.IsEnabled = value;
            }
        }

        bool _canScan = false;
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar has button
        /// for image acquisition from scanner.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        public bool CanScan
        {
            get
            {
                return _canScan;
            }
            set
            {
                SetVisibility(_canScan = value, ScanButton);

                SetVisibility(value || CanCaptureFromCamera, ScanSeparator);
            }
        }

        bool _scanButtonEnabled;
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar has enabled button
        /// for image acquisition from scanner.
        /// </summary>
        [Browsable(false)]
        public bool ScanButtonEnabled
        {
            get
            {
                return _scanButtonEnabled;
            }
            set
            {
                _scanButtonEnabled = value;
                this.ScanButton.IsEnabled = value;
            }
        }

        bool _canCaptureFromCamera = false;
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar has button for capturing of image
        /// from camera.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        public bool CanCaptureFromCamera
        {
            get
            {
                return _canCaptureFromCamera;
            }
            set
            {
                SetVisibility(_canCaptureFromCamera = value, CaptureFromCameraButton);

                SetVisibility(value || CanScan, ScanSeparator);
            }
        }

        bool _captureFromCameraButtonEnabled;
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar has 
        /// enabled button for capturing of image from camera.
        /// </summary>
        [Browsable(false)]
        public bool CaptureFromCameraButtonEnabled
        {
            get
            {
                return _captureFromCameraButtonEnabled;
            }
            set
            {
                _captureFromCameraButtonEnabled = value;
                this.CaptureFromCameraButton.IsEnabled = value;
            }
        }

        bool _canPrint = true;
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar has button for printing of image.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        public bool CanPrint
        {
            get
            {
                return _canPrint;
            }
            set
            {
                SetVisibility(_canPrint = value, PrintButton);

                SetVisibility(value, PrintSeparator);
            }
        }

        bool _printButtonEnabled;
        /// <summary>
        /// Gets or sets a value indicating whether the button for printing of image is enabled.
        /// </summary>
        public bool PrintButtonEnabled
        {
            get
            {
                return _printButtonEnabled;
            }
            set
            {
                _printButtonEnabled = value;
                PrintButton.IsEnabled = value;
            }
        }

        bool _canNavigate = false;
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar has button for image navigation.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        public bool CanNavigate
        {
            get
            {
                return _canNavigate;
            }
            set
            {
                _canNavigate = value;

                SetVisibility(value,
                    FirstPageButton,
                    PreviousPageButton,
                    SelectedPageIndexTextBox,
                    SlashLabel,
                    PageCountLabel,
                    NextPageButton,
                    LastPageButton,
                    NavigationSeparator);
            }
        }

        bool _isNavigationEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether image navigation buttons are
        /// enabled in the toolbar.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(true)]
        public bool IsNavigationEnabled
        {
            get
            {
                return _isNavigationEnabled;
            }
            set
            {
                FirstPageButton.IsEnabled = value;
                PreviousPageButton.IsEnabled = value;
                SelectedPageIndexTextBox.IsEnabled = value;
                SlashLabel.IsEnabled = value;
                PageCountLabel.IsEnabled = value;
                NextPageButton.IsEnabled = value;
                LastPageButton.IsEnabled = value;
                NavigationSeparator.IsEnabled = value;

                _isNavigationEnabled = value;
            }
        }

        bool _canChangeSizeMode = true;
        /// <summary>
        /// Gets or sets a value indicating whether the toolbar has buttons for image size mode changing.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        public bool CanChangeSizeMode
        {
            get
            {
                return _canChangeSizeMode;
            }
            set
            {
                _canChangeSizeMode = value;

                SetVisibility(value,
                    NormalMenuItem,
                    BestFitMenuItem,
                    FitToHeightMenuItem,
                    FitToWidthMenuItem,
                    ScaleMenuItem,
                    PixelToPixelMenuItem,
                    ZoomMenuItemSeparator);
            }
        }

        bool _isChangeSizeModeEnabled = true;
        /// <summary>
        /// Gets or sets a value indicating whether image size mode changing buttons are
        /// enabled in the toolbar.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(true)]
        public bool IsChangeSizeModeEnabled
        {
            get
            {
                return _isChangeSizeModeEnabled;
            }
            set
            {
                NormalMenuItem.IsEnabled = value;
                BestFitMenuItem.IsEnabled = value;
                FitToHeightMenuItem.IsEnabled = value;
                FitToWidthMenuItem.IsEnabled = value;
                PixelToPixelMenuItem.IsEnabled = value;
                ScaleMenuItem.IsEnabled = value;

                _isChangeSizeModeEnabled = value;
            }
        }

        int _pageCount;
        /// <summary>
        /// Gets or sets the image count.
        /// </summary>
        [Browsable(false)]
        public int PageCount
        {
            get
            {
                if (UseImageViewerImages)
                {
                    if (_imageViewer != null)
                        return _imageViewer.Images.Count;
                    else
                        return 0;
                }
                else
                    return _pageCount;
            }
            set
            {
                if (!UseImageViewerImages)
                {
                    if (value >= 0 && value != PageCount)
                    {
                        _pageCount = value;
                        PageCountLabel.Text = PageCount.ToString(CultureInfo.InvariantCulture);

                        if (_selectedPageIndex >= PageCount)
                            UpdateSelectedPageIndex(PageCount - 1);
                        else if (_selectedPageIndex >= 0)
                            UpdateSelectedPageIndex(_selectedPageIndex);
                        else if (PageCount > 0)
                            UpdateSelectedPageIndex(0);
                        UpdateUI();
                    }
                }
            }
        }

        int _selectedPageIndex = -1;
        /// <summary>
        /// Gets or sets an index of selected image.
        /// </summary>
        [Browsable(false)]
        public int SelectedPageIndex
        {
            get
            {
                return _selectedPageIndex;
            }
            set
            {
                if (_selectedPageIndex == value && (_imageViewer == null || _imageViewer.Images.Count > 0))
                    return;

                UpdateSelectedPageIndex(value);

                UpdateUI();
            }
        }

        #endregion



        #region Methods

        #region Update UI

        /// <summary>
        /// Updates the user interface of this form.
        /// </summary>
        private void UpdateUI()
        {
            if (_imageViewer == null)
                return;

            bool isImageLoaded = _imageViewer.Image != null;

            // Navigation buttons
            if (_canNavigate)
            {
                FirstPageButton.IsEnabled = this.SelectedPageIndex > 0;
                PreviousPageButton.IsEnabled = this.SelectedPageIndex > 0;
                NextPageButton.IsEnabled = this.SelectedPageIndex < (this.PageCount - 1);
                LastPageButton.IsEnabled = this.SelectedPageIndex < (this.PageCount - 1);

                SelectedPageIndexTextBox.IsEnabled = this.PageCount > 1;
                SelectedPageIndexTextBox.Text = String.Format("{0}", _selectedPageIndex + 1);
            }

            // Zoom buttons
            ZoomInButton.IsEnabled = isImageLoaded;
            ZoomOutButton.IsEnabled = isImageLoaded;
            ZoomValueTextBox.IsEnabled = isImageLoaded;
            ZoomModesButton.IsEnabled = isImageLoaded;

            if (ZoomValueTextBox.IsEnabled)
                UpdateTextBoxZoom();

            //
            if (UseImageViewerImages)
                PageCountLabel.Text = PageCount.ToString();
        }

        #endregion


        #region Main toolbar

        /// <summary>
        /// Handles the Click event of OpenButton object.
        /// </summary>
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            UIElement uiElement = (UIElement)sender;
            uiElement.IsEnabled = false;
            try
            {
                if (OpenFile != null)
                    OpenFile(sender, e);
            }
            finally
            {
                uiElement.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of SaveButton object.
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            UIElement uiElement = (UIElement)sender;
            uiElement.IsEnabled = false;
            try
            {
                if (SaveFile != null)
                    SaveFile(sender, e);
            }
            finally
            {
                uiElement.IsEnabled = SaveButtonEnabled;
            }
        }

        /// <summary>
        /// Handles the Click event of ScanButton object.
        /// </summary>
        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            UIElement uiElement = (UIElement)sender;
            uiElement.IsEnabled = false;
            try
            {
                if (Scan != null)
                    Scan(sender, e);
            }
            finally
            {
                uiElement.IsEnabled = ScanButtonEnabled;
            }
        }

        /// <summary>
        /// Handles the Click event of CaptureFromCameraButton object.
        /// </summary>
        private void CaptureFromCameraButton_Click(object sender, RoutedEventArgs e)
        {
            UIElement uiElement = (UIElement)sender;
            uiElement.IsEnabled = false;
            try
            {
                if (CaptureFromCamera != null)
                    CaptureFromCamera(sender, e);
            }
            finally
            {
                uiElement.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of PrintButton object.
        /// </summary>
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            UIElement uiElement = (UIElement)sender;
            uiElement.IsEnabled = false;
            try
            {
                if (Print != null)
                    Print(sender, e);
            }
            finally
            {
                uiElement.IsEnabled = PrintButtonEnabled;
            }
        }

        #endregion


        #region Navigation

        /// <summary>
        /// Moves the focus in image viewer to the first image.
        /// </summary>
        private void FirstPageButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPageIndex = 0;
        }

        /// <summary>
        /// Moves the focus in image viewer to the previous image.
        /// </summary>
        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPageIndex--;
        }

        /// <summary>
        /// Moves the focus in image viewer to the next image.
        /// </summary>
        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPageIndex++;
        }

        /// <summary>
        /// Moves the focus in image viewer to the last image.
        /// </summary>
        private void LastPageButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedPageIndex = PageCount - 1;
        }

        /// <summary>
        /// Changes the focus in image viewer from current image to the new image according to the entered page index value.
        /// </summary>
        /// <remarks>
        /// This method does not change focus if entered page index value is not correct.
        /// </remarks>
        private void SelectedPageIndexTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // if enter is pressed
            if (e.Key == Key.Enter)
            {
                int value;
                // if entered index is not correct
                if (int.TryParse(((TextBox)sender).Text, out value) && value > 0 && value <= PageCount)
                {
                    // set last page index
                    SelectedPageIndex = value - 1;
                }
                // if entered index is correct
                else
                {
                    // update selected page index
                    UpdateSelectedPageIndex(_selectedPageIndex);

                    // update user interface
                    UpdateUI();
                }
            }
        }

        /// <summary>
        /// Returns focused page index to the selected index text box.
        /// </summary>
        private void SelectedPageIndexTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // update selected page index
            UpdateSelectedPageIndex(_selectedPageIndex);

            // update user interface
            UpdateUI();
        }

        #endregion


        #region Scale mode

        /// <summary>
        /// Updates zoom value in zoom text box.
        /// </summary>
        private void UpdateTextBoxZoom()
        {
            ZoomValueTextBox.Text = String.Format(CultureInfo.InvariantCulture, "{0}%", _imageViewer.Zoom.ToString("###"));
        }

        /// <summary>
        /// Decreases zoom value.
        /// </summary>
        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            // if zoom value in image viewer greater than minimum zoom value
            if (_imageViewer.Zoom > _wpfZoomValues[0])
            {
                _imageScaleSelected.IsChecked = false;
                // set zoom size mode to the image viewer
                _imageViewer.SizeMode = ImageSizeMode.Zoom;

                int index = 0;
                // search current zoom value in array of available zoom values
                while (index < _wpfZoomValues.Length && _wpfZoomValues[index] < _imageViewer.Zoom)
                {
                    index++;
                }
                // set zoom value
                _imageViewer.Zoom = _wpfZoomValues[index - 1];

                // update value in zoom text box
                UpdateTextBoxZoom();
            }
        }

        /// <summary>
        /// Increases zoom value.
        /// </summary>
        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            // if zoom value in image viewer less than maximum zoom value
            if (_imageViewer.Zoom < _wpfZoomValues[_wpfZoomValues.Length - 1])
            {
                _imageScaleSelected.IsChecked = false;
                // set zoom size mode to the image viewer
                _imageViewer.SizeMode = ImageSizeMode.Zoom;

                int index = 0;
                // search current zoom value in array of available zoom values
                while (_wpfZoomValues[index] <= _imageViewer.Zoom)
                {
                    index++;
                }
                // set zoom value
                _imageViewer.Zoom = _wpfZoomValues[index];

                // update value in zoom text box
                UpdateTextBoxZoom();
            }
        }

        /// <summary>
        /// Changes zoom when new value is entered in zoom text box.
        /// </summary>
        private void ZoomTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // if enter is pressed
            if (e.Key == Key.Enter)
            {
                // get zoom value
                string sourceText = ((TextBox)sender).Text.Replace("%", "");

                _imageScaleSelected.IsChecked = false;
                // set zoom size mode to the image viewer
                _imageViewer.SizeMode = ImageSizeMode.Zoom;

                int value;
                if (int.TryParse(sourceText, out value) && value > 0)
                {
                    // set zoom value
                    _imageViewer.Zoom = value;
                }

                // update value in zoom text box
                UpdateTextBoxZoom();
            }
        }

        /// <summary>
        /// Returns zoom value to the zoom text box. 
        /// </summary>
        private void ZoomValueTextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UpdateTextBoxZoom();
        }

        /// <summary>
        /// Changes image scale mode of image viewer.
        /// </summary>
        private void ScaleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _imageScaleSelected.IsChecked = false;
            _imageScaleSelected = (MenuItem)sender;

            if (_imageScaleSelected == NormalMenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.Normal;
            }
            else if (_imageScaleSelected == BestFitMenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.BestFit;
            }
            else if (_imageScaleSelected == FitToWidthMenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.FitToWidth;
            }
            else if (_imageScaleSelected == FitToHeightMenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.FitToHeight;
            }
            else if (_imageScaleSelected == PixelToPixelMenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.PixelToPixel;
            }
            else if (_imageScaleSelected == ScaleMenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.Zoom;
            }
            else if (_imageScaleSelected == Scale25MenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.Zoom;
                _imageViewer.Zoom = 25;
            }
            else if (_imageScaleSelected == Scale50MenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.Zoom;
                _imageViewer.Zoom = 50;
            }
            else if (_imageScaleSelected == Scale100MenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.Zoom;
                _imageViewer.Zoom = 100;
            }
            else if (_imageScaleSelected == Scale200MenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.Zoom;
                _imageViewer.Zoom = 200;
            }
            else if (_imageScaleSelected == Scale400MenuItem)
            {
                _imageViewer.SizeMode = ImageSizeMode.Zoom;
                _imageViewer.Zoom = 400;
            }

            _imageScaleSelected.IsChecked = true;

            UpdateMenuItemTemplatePartBackground(_imageScaleSelected);

            UpdateTextBoxZoom();
        }

        #endregion


        #region Zoom slider

        /// <summary>
        /// Changes zoom when zoom slider value is changed. 
        /// </summary>
        private void _associatedZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // if slider is captured by mouse
            if ((sender as Slider).IsMouseCaptureWithin)
            {
                // set zoom size mode to the image viewer
                if (_imageViewer.SizeMode != ImageSizeMode.Zoom)
                    _imageViewer.SizeMode = ImageSizeMode.Zoom;
            }
            // set zoom value
            _imageViewer.Zoom = AssociatedZoomSlider.Value;
        }

        #endregion


        #region Image viewer

        /// <summary>
        /// Handles the ImagesChanging event of imageViewer object.
        /// </summary>
        private void imageViewer_ImagesChanging(object sender, EventArgs e)
        {
            if (UseImageViewerImages)
            {
                if (_imageViewer != null)
                    // unsubscribe from the ImageCollectionChanged event of image collection of the viewer
                    _imageViewer.Images.ImageCollectionChanged -= new EventHandler<ImageCollectionChangeEventArgs>(Images_CollectionChangedSafely);
            }
        }

        /// <summary>
        /// Handles the ImagesChanged event of imageViewer object.
        /// </summary>
        private void imageViewer_ImagesChanged(object sender, EventArgs e)
        {
            if (UseImageViewerImages)
                // subscribe to the ImageCollectionChanged event of image collection of the viewer
                _imageViewer.Images.ImageCollectionChanged += new EventHandler<ImageCollectionChangeEventArgs>(Images_CollectionChangedSafely);
        }

        /// <summary>
        /// Handles the FocusedIndexChanged event of imageViewer object.
        /// </summary>
        private void imageViewer_FocusedIndexChanged(object sender, PropertyChangedEventArgs<int> e)
        {
            if (UseImageViewerImages && _imageViewer != null)
                SelectedPageIndex = e.NewValue;
        }

        /// <summary>
        /// Handles the ZoomChanged event of imageViewer object.
        /// </summary>
        private void imageViewer_ZoomChanged(object sender, ZoomChangedEventArgs e)
        {
            if (AssociatedZoomSlider != null)
            {
                AssociatedZoomSlider.Value = Math.Min(e.Zoom, AssociatedZoomSlider.Maximum);
                int zoom = (int)Math.Round(AssociatedZoomSlider.Value);
                ToolTip zoomToolTip = new ToolTip();
                zoomToolTip.Content = zoom.ToString(CultureInfo.InvariantCulture) + "%";
                AssociatedZoomSlider.ToolTip = zoomToolTip;
            }

            UpdateTextBoxZoom();

            _imageScaleSelected.IsChecked = false;
            switch (_imageViewer.SizeMode)
            {
                case ImageSizeMode.BestFit:
                    _imageScaleSelected = BestFitMenuItem;
                    break;

                case ImageSizeMode.FitToHeight:
                    _imageScaleSelected = FitToHeightMenuItem;
                    break;

                case ImageSizeMode.FitToWidth:
                    _imageScaleSelected = FitToWidthMenuItem;
                    break;

                case ImageSizeMode.Normal:
                    _imageScaleSelected = NormalMenuItem;
                    break;

                case ImageSizeMode.PixelToPixel:
                    _imageScaleSelected = PixelToPixelMenuItem;
                    break;

                case ImageSizeMode.Zoom:
                    _imageScaleSelected = ScaleMenuItem;
                    break;
            }

            _imageScaleSelected.IsChecked = true;

            UpdateMenuItemTemplatePartBackground(_imageScaleSelected);
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


        #region Image collection

        /// <summary>
        /// Handles the CollectionChanged event of Images object.
        /// </summary>
        private void Images_CollectionChanged(object sender, ImageCollectionChangeEventArgs e)
        {
            UpdateUI();
        }

        /// <summary>
        /// Handles the CollectionChangedSafely event of Images object.
        /// </summary>
        private void Images_CollectionChangedSafely(object sender, ImageCollectionChangeEventArgs e)
        {
            if (Dispatcher.Thread != System.Threading.Thread.CurrentThread)
                _imageViewer.Dispatcher.Invoke(_images_CollectionChangedEventThreadSafe, new object[] { sender, e });
            else
                this.Images_CollectionChanged(sender, e);
        }

        #endregion


        private void UpdateSelectedPageIndex(int index)
        {
            if (this.PageCount == 0)
            {
                SelectedPageIndexTextBox.Text = "";
                return;
            }

            if (index > PageCount - 1)
                index = PageCount - 1;

            if (index < 0)
            {
                if (!this.UseImageViewerImages || (_imageViewer.FocusedIndex != index))
                    index = 0;
            }

            if (_selectedPageIndex != index)
            {
                _selectedPageIndex = index;

                if (PageIndexChanged != null)
                    PageIndexChanged(this, new PageIndexChangedEventArgs(index));
            }

            if (this.UseImageViewerImages && _imageViewer.FocusedIndex != index)
                _imageViewer.FocusedIndex = index;
        }

        private void SetVisibility(bool visible, params UIElement[] elements)
        {
            Visibility visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            foreach (UIElement element in elements)
                element.Visibility = visibility;
        }

        #endregion



        #region Events

        [Browsable(true)]
        public event EventHandler OpenFile;

        [Browsable(true)]
        public event EventHandler SaveFile;

        [Browsable(true)]
        public event EventHandler Scan;

        [Browsable(true)]
        public event EventHandler CaptureFromCamera;

        [Browsable(true)]
        public event EventHandler Print;

        [Browsable(true)]
        public event PageIndexChangedEventHandler PageIndexChanged;

        #endregion



        #region Delegates

        public delegate void PageIndexChangedEventHandler(object sender, PageIndexChangedEventArgs e);

        private delegate void Images_CollectionChangedThreadSafeDelegate(object sender, ImageCollectionChangeEventArgs e);
        private Images_CollectionChangedThreadSafeDelegate _images_CollectionChangedEventThreadSafe;

        #endregion

    }
}
