using System;
using System.Windows;

using Vintasoft.Imaging.Codecs;
using Vintasoft.Imaging.Codecs.ImageFiles.Gif;
using Vintasoft.Imaging.Codecs.Encoders;


namespace WpfDemosCommonCode.Imaging.Codecs.Dialogs
{
    /// <summary>
    /// A window that allows to view and edit the GIF encoder settings.
    /// </summary>
    public partial class GifEncoderSettingsWindow : Window
    {

        #region Constructor

        public GifEncoderSettingsWindow()
        {
            InitializeComponent();

            foreach (CreatePageMethod value in Enum.GetValues(typeof(CreatePageMethod)))
                createPageMethodComboBox.Items.Add(value);
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether encoder must add images to the existing GIF file.
        /// </summary>
        /// <value>
        /// <b>True</b> - encoder must add images to the existing GIF file;
        /// <b>false</b> - encoder must delete the existing GIF file if necessary, create new GIF file and add images to the new GIF file.
        /// </value>
        public bool AddImagesToExistingFile
        {
            get
            {
                return appendCheckBox.IsChecked.Value == true;
            }
            set
            {
                appendCheckBox.IsChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether encoder can add images to the existing GIF file.
        /// </summary>
        /// <value>
        /// <b>True</b> - encoder can add images to the existing GIF file;
        /// <b>false</b> - encoder can NOT add images to the existing GIF file.
        /// </value>
        public bool CanAddImagesToExistingFile
        {
            get
            {
                return appendCheckBox.IsEnabled;
            }
            set
            {
                appendCheckBox.IsEnabled = value;
            }
        }


        GifEncoderSettings _encoderSettings;
        /// <summary>
        /// Gets or sets a GIF encoder settings.
        /// </summary>
        public GifEncoderSettings EncoderSettings
        {
            get
            {
                return _encoderSettings;
            }
            set
            {
                if (value == null)
                    throw new ArgumentOutOfRangeException();
                if (_encoderSettings != value)
                {
                    _encoderSettings = value;
                    UpdateUI();
                }
            }
        }

        private PageAlignMode ImagesAlign
        {
            get
            {
                if (centerPositionRadioButton.IsChecked.Value == true)
                    return PageAlignMode.Center;
                if (leftTopPositionRadioButton.IsChecked.Value == true)
                    return PageAlignMode.LeftTop;
                if (topPositionRadioButton.IsChecked.Value == true)
                    return PageAlignMode.Top;
                if (rightTopPositionRadioButton.IsChecked.Value == true)
                    return PageAlignMode.RightTop;
                if (rightPositionRadioButton.IsChecked.Value == true)
                    return PageAlignMode.Right;
                if (rightBottomPositionRadioButton.IsChecked.Value == true)
                    return PageAlignMode.RightButtom;
                if (bottomPositionRadioButton.IsChecked.Value == true)
                    return PageAlignMode.Bottom;
                if (letfBottomPositionRadioButton.IsChecked.Value == true)
                    return PageAlignMode.LeftBottom;
                if (leftPositionRadioButton.IsChecked.Value == true)
                    return PageAlignMode.Left;
                return PageAlignMode.LeftTop;
            }
            set
            {
                switch (value)
                {
                    case PageAlignMode.Center:
                        centerPositionRadioButton.IsChecked = true;
                        break;
                    case PageAlignMode.LeftTop:
                        leftTopPositionRadioButton.IsChecked = true;
                        break;
                    case PageAlignMode.Top:
                        topPositionRadioButton.IsChecked = true;
                        break;
                    case PageAlignMode.RightTop:
                        rightTopPositionRadioButton.IsChecked = true;
                        break;
                    case PageAlignMode.Right:
                        rightPositionRadioButton.IsChecked = true;
                        break;
                    case PageAlignMode.RightButtom:
                        rightBottomPositionRadioButton.IsChecked = true;
                        break;
                    case PageAlignMode.Bottom:
                        bottomPositionRadioButton.IsChecked = true;
                        break;
                    case PageAlignMode.LeftBottom:
                        letfBottomPositionRadioButton.IsChecked = true;
                        break;
                    case PageAlignMode.Left:
                        leftPositionRadioButton.IsChecked = true;
                        break;
                }
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Loaded event of Window object.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (EncoderSettings == null)
                EncoderSettings = new GifEncoderSettings();
        }

        private void UpdateUI()
        {
            createPageMethodComboBox.SelectedItem = _encoderSettings.CreatePageMethod;

            infiniteIterationsCheckBox.IsChecked = _encoderSettings.InfiniteAnimation;
            infiniteIterationsCheckBox_Click(infiniteIterationsCheckBox, null);
            animationCyclesNumericUpDown.Value = _encoderSettings.NumberOfAnimationCycles;

            animationDelayNumericUpDown.Value = _encoderSettings.AnimationDelay;

            autoSizeCheckBox.IsChecked = _encoderSettings.LogicalScreenWidth == 0 && _encoderSettings.LogicalScreenHeight == 0;
            autoSizeCheckBox_Click(autoSizeCheckBox, null);
            logicalScreenWidthNumericUpDown.Value = _encoderSettings.LogicalScreenWidth;
            logicalScreenHeightNumericUpDown.Value = _encoderSettings.LogicalScreenHeight;

            ImagesAlign = _encoderSettings.PageAlign;
        }

        private void SetEncoderSettings()
        {
            _encoderSettings.CreatePageMethod = (CreatePageMethod)createPageMethodComboBox.SelectedItem;

            _encoderSettings.NumberOfAnimationCycles = (int)animationCyclesNumericUpDown.Value;
            _encoderSettings.InfiniteAnimation = infiniteIterationsCheckBox.IsChecked.Value == true;

            _encoderSettings.AnimationDelay = (int)animationDelayNumericUpDown.Value;

            _encoderSettings.LogicalScreenWidth = (int)logicalScreenWidthNumericUpDown.Value;
            _encoderSettings.LogicalScreenHeight = (int)logicalScreenHeightNumericUpDown.Value;

            _encoderSettings.PageAlign = ImagesAlign;
        }

        /// <summary>
        /// Handles the Click event of AutoSizeCheckBox object.
        /// </summary>
        private void autoSizeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (autoSizeCheckBox.IsChecked.Value == true)
            {
                logicalScreenWidthNumericUpDown.IsEnabled = false;
                logicalScreenHeightNumericUpDown.IsEnabled = false;
                logicalScreenWidthNumericUpDown.Value = 0;
                logicalScreenHeightNumericUpDown.Value = 0;
            }
            else
            {
                logicalScreenWidthNumericUpDown.IsEnabled = true;
                logicalScreenHeightNumericUpDown.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of InfiniteIterationsCheckBox object.
        /// </summary>
        private void infiniteIterationsCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (infiniteIterationsCheckBox.IsChecked.Value == true)
            {
                animationCyclesNumericUpDown.IsEnabled = false;
                animationCyclesNumericUpDown.Value = 0;
            }
            else
            {
                animationCyclesNumericUpDown.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles the Click event of ButtonOk object.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            SetEncoderSettings();

            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of ButtonCancel object.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

    }
}
