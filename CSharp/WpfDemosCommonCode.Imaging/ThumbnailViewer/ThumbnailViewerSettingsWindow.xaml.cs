using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Vintasoft.Imaging.UI;
using Vintasoft.Imaging.Wpf.UI;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to edit the thumbnail viewer settings.
    /// </summary>
    public partial class ThumbnailViewerSettingsWindow : Window
    {

        #region Fields

        /// <summary>
        /// The thumbnail viewer;
        /// </summary>
        WpfThumbnailViewer _viewer;

        /// <summary>
        /// Animated thumbnail item style.
        /// </summary>
        Style _animatedThumbnailItemStyle;

        #endregion



        #region Constructors

        /// <summary>
        /// Prevents a default instance of
        /// the <see cref="ThumbnailViewerSettingsWindow"/> class from being created.
        /// </summary>
        private ThumbnailViewerSettingsWindow()
        {
            InitializeComponent();

            foreach (object name in Enum.GetValues(typeof(ThumbnailFlowStyle)))
                thumbnailFlowStyleComboBox.Items.Add(name);

            foreach (object name in Enum.GetValues(typeof(ThumbnailScale)))
                thumbnailScaleComboBox.Items.Add(name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThumbnailViewerSettingsWindow"/> class.
        /// </summary>
        /// <param name="viewer">The thumbnail viewer.</param>
        public ThumbnailViewerSettingsWindow(WpfThumbnailViewer viewer, Style animatedThumbnailItemStyle)
            : this()
        {
            _viewer = viewer;
            _animatedThumbnailItemStyle = animatedThumbnailItemStyle;
            ShowSettings();
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the thumbnail style is animated.
        /// </summary>
        /// <value>
        /// <b>true</b> if the thumbnail style is animated; otherwise, <b>false</b>.
        /// </value>
        private bool IsAnimatedThumbnailItemStyle
        {
            get
            {
                return thumbnailStyleComboBox.Text == "Animated";
            }
            set
            {
                thumbnailStyleComboBox.Text = value ? "Animated" : "Simple";
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Shows the settings of thumbnail viewer.
        /// </summary>
        private void ShowSettings()
        {
            generateOnlyVisibleThumbnailsCheckBox.IsChecked = _viewer.GenerateOnlyVisibleThumbnails;
            thumbnailSizeComboBox.Text = String.Format("{0} x {1}",
                _viewer.ThumbnailSize.Width, _viewer.ThumbnailSize.Height);
            thumbnailFlowStyleComboBox.SelectedItem = _viewer.ThumbnailFlowStyle;
            thumbnailColumnsCountComboBox.SelectedIndex = _viewer.ThumbnailFixedColumnCount - 1;
            thumbnailScaleComboBox.SelectedItem = _viewer.ThumbnailScale;
            Color backgroundColor = Colors.Transparent;
            if (_viewer.Background is SolidColorBrush)
                backgroundColor = (_viewer.Background as SolidColorBrush).Color;
            thumbnailViewerBackColorPanelControl.Color = backgroundColor;
            thumbnailRenderingThreadCountNumericUpDown.Value = _viewer.ThumbnailRenderingThreadCount;

            if (_animatedThumbnailItemStyle == null)
                thumbnailStyleComboBox.Visibility = Visibility.Hidden;
            else
                IsAnimatedThumbnailItemStyle = ThumbnailContainerStyleBaseOn(_animatedThumbnailItemStyle);

            thumbnailPaddingControl.Value = GetThumbnailPadding();
            thumbnailsAnchorEditorControl.SelectedAnchorType = _viewer.ThumbnailsAnchor;

            captionIsVisibleCheckBox.IsChecked = _viewer.ThumbnailCaption.IsVisible;

            captionPaddingFEditorControl.Value = _viewer.ThumbnailCaption.Padding;
            captionAnchorTypeEditor.SelectedAnchorType = _viewer.ThumbnailCaption.Anchor;
            captionFormatTextBox.Text = _viewer.ThumbnailCaption.CaptionFormat;
            captionTextColorPanelControl.Color = _viewer.ThumbnailCaption.TextColor;

            fontSizeNumericUpDown.Value = _viewer.ThumbnailCaption.FontSize;
            fontFamilySelector.SelectedFamily = _viewer.ThumbnailCaption.FontFamily;

            ShowThumbnailCheckBoxCheckBox.IsChecked = _viewer.ShowThumbnailCheckBox;
            thumbnailControlAnchorTypeEditor.SelectedAnchorType = _viewer.ThumbnailControlAnchor;
            thumbnailControlPaddingControl.Value = _viewer.ThumbnailControlPadding;
        }

        /// <summary>
        /// Determines whether the thumbnail style is based on specified style.
        /// </summary>
        /// <param name="style">The thumbnail style.</param>
        /// <returns>
        /// <b>True</b> if the thumbnail style is based on specified style; otherwise, <b>false</b>.
        /// </returns>
        private bool ThumbnailContainerStyleBaseOn(Style style)
        {
            if (_viewer.ThumbnailContainerStyle.Equals(style) ||
               (_viewer.ThumbnailContainerStyle.BasedOn != null &&
                _viewer.ThumbnailContainerStyle.BasedOn.Equals(style)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the thumbnail padding.
        /// </summary>
        /// <returns>
        /// The thumbnail padding.
        /// </returns>
        private Thickness GetThumbnailPadding()
        {
            Setter paddingPropertySetter =
                FindSetter(_viewer.ThumbnailContainerStyle, ThumbnailImageItem.ThumbnailImagePaddingProperty);

            if (paddingPropertySetter != null)
                return (Thickness)paddingPropertySetter.Value;
            return new Thickness(0);
        }

        /// <summary>
        /// Searches setter of specified property in specified style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="property">The property.</param>
        private Setter FindSetter(Style style, DependencyProperty property)
        {
            foreach (Setter setter in style.Setters)
            {
                if (setter.Property == property)
                    return setter;
            }
            return null;
        }

        /// <summary>
        /// Saves the settings of thumbnail viewer.
        /// </summary>
        private bool SetSettings()
        {
            _viewer.GenerateOnlyVisibleThumbnails = generateOnlyVisibleThumbnailsCheckBox.IsChecked.Value == true;

            // Thumbnail Size
            try
            {
                string[] sizeStrings = thumbnailSizeComboBox.Text.Split('x');
                int width;
                int height;
                if (sizeStrings.Length == 1)
                {
                    width = Convert.ToInt32(sizeStrings[0]);
                    height = width;
                }
                else
                {
                    width = Convert.ToInt32(sizeStrings[0]);
                    height = Convert.ToInt32(sizeStrings[1]);
                }
                _viewer.ThumbnailSize = new Size(width, height);
            }
            catch (Exception e)
            {
                DemosTools.ShowErrorMessage(e);
                return false;
            }

            if ((ThumbnailFlowStyle)thumbnailFlowStyleComboBox.SelectedItem == ThumbnailFlowStyle.FixedColumns)
                _viewer.ThumbnailFixedColumnCount = (int)thumbnailColumnsCountComboBox.SelectedIndex + 1;
            _viewer.ThumbnailFlowStyle = (ThumbnailFlowStyle)thumbnailFlowStyleComboBox.SelectedItem;
            _viewer.ThumbnailScale = (ThumbnailScale)thumbnailScaleComboBox.SelectedItem;
            _viewer.Background = new SolidColorBrush(thumbnailViewerBackColorPanelControl.Color);
            _viewer.ThumbnailRenderingThreadCount = (int)thumbnailRenderingThreadCountNumericUpDown.Value;

            Style thumbnailItemStyle;
            if (_animatedThumbnailItemStyle == null)
            {
                thumbnailItemStyle = _viewer.ThumbnailContainerStyle;
            }
            else
            {
                if (IsAnimatedThumbnailItemStyle)
                    thumbnailItemStyle = _animatedThumbnailItemStyle;
                else
                    thumbnailItemStyle = new ThumbnailImageItemStyle();
            }

            Thickness newPadding = thumbnailPaddingControl.Value;
            thumbnailItemStyle = new Style(typeof(ThumbnailImageItem), thumbnailItemStyle);
            thumbnailItemStyle.Setters.Add(new Setter(ThumbnailImageItem.ThumbnailImagePaddingProperty, newPadding));
            _viewer.ThumbnailsAnchor = thumbnailsAnchorEditorControl.SelectedAnchorType;

            try
            {
                _viewer.ThumbnailContainerStyle = thumbnailItemStyle;
            }
            catch (Exception e)
            {
                DemosTools.ShowErrorMessage(e);
                return false;
            }

            _viewer.ThumbnailCaption.IsVisible = captionIsVisibleCheckBox.IsChecked.Value == true;

            _viewer.ThumbnailCaption.Padding = captionPaddingFEditorControl.Value;
            _viewer.ThumbnailCaption.TextColor = captionTextColorPanelControl.Color;
            _viewer.ThumbnailCaption.CaptionFormat = captionFormatTextBox.Text;
            _viewer.ThumbnailCaption.Anchor = captionAnchorTypeEditor.SelectedAnchorType;

            _viewer.ThumbnailCaption.FontSize = fontSizeNumericUpDown.Value;
            _viewer.ThumbnailCaption.FontFamily = fontFamilySelector.SelectedFamily;

            _viewer.ShowThumbnailCheckBox = ShowThumbnailCheckBoxCheckBox.IsChecked.Value == true;
            _viewer.ThumbnailControlAnchor = thumbnailControlAnchorTypeEditor.SelectedAnchorType;
            _viewer.ThumbnailControlPadding = thumbnailControlPaddingControl.Value;

            return true;
        }

        /// <summary>
        /// "OK" button is clicked.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (SetSettings())
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
        /// The flow style of thumbnail viewer is changed.
        /// </summary>
        private void thumbnailFlowStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((ThumbnailFlowStyle)thumbnailFlowStyleComboBox.SelectedItem == ThumbnailFlowStyle.FixedColumns)
            {
                thumbnailColumnsCountComboBox.IsEnabled = true;
            }
            else
            {
                thumbnailColumnsCountComboBox.IsEnabled = false;
            }
        }

        /// <summary>
        /// Shows information about ThumbnailCaption.CaptionFormat property.
        /// </summary>
        private void captionFormatHelpButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Examples:\n" +
                "'File {Filename}, page {PageNumber}'\n" +
                "'{ImageSizeMpx:f2} MPX'\n" +
                "\n" +
                "List of predefined format variables:\n" +
                "{PageLabel} - page label\n" +
                "{PageNumber} - page number, in source image file\n" +
                "{PageIndex} - page index, in source image file\n" +
                "{ImageNumber} - image number, in image collection\n" +
                "{ImageIndex} - image index, in image collection\n" +
                "{Filename} - filename without directory\n" +
                "{FullFilename} - full filename\n" +
                "{DirectoryName} - directory name\n" +
                "{DecoderName} - decoder name\n" +
                "{ImageWidthPx} - source image width, in pixels\n" +
                "{ImageHeightPx} - source image height, in pixels\n" +
                "{ImageSizeMpx} - source image size, in megapixels\n" +
                "{ImageHRes} - source image horizontal resolution, in DPI\n" +
                "{ImageVRes} - source image vertical resolution, in DPI\n" +
                "{ImageBitsPerPixel} - source image bits per pixel",
                "ThumbnailCaption.CaptionFormat property");
        }

        /// <summary>
        /// The thumbnail caption visibility is changed.
        /// </summary>
        private void captionIsVisibleCheckBox_CheckChanged(object sender, RoutedEventArgs e)
        {
            thumbnailCaptionGroupBox.IsEnabled = captionIsVisibleCheckBox.IsChecked.Value == true;
        }

        #endregion

    }
}
