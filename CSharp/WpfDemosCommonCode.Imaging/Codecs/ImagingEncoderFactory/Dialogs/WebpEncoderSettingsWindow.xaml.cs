using System;
using System.Windows;

using Vintasoft.Imaging.Codecs.Encoders;
#if NETCOREAPP
using Vintasoft.Imaging.Codecs.Webp;
#endif

namespace WpfDemosCommonCode.Imaging.Codecs.Dialogs
{
    /// <summary>
    /// A window that allows to view and edit the WEBP encoder settings.
    /// </summary>
    public partial class WebpEncoderSettingsWindow : Window
    {

        #region Constructors

        public WebpEncoderSettingsWindow()
        {
            InitializeComponent();
        }

        #endregion



        #region Properties

#if NETCOREAPP
        WebpEncoderSettings _encoderSettings = null;
        /// <summary>
        /// Gets or sets the settings of WEBP encoder.
        /// </summary>
        public WebpEncoderSettings EncoderSettings
        {
            get
            {
                return _encoderSettings;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (!object.Equals(_encoderSettings, value))
                {
                    _encoderSettings = value;

                    encodingComboBox.SelectedItem = _encoderSettings.Method;
                    transparentComboBox.SelectedItem = _encoderSettings.TransparentColorMode;
                }
            }
        }
#endif

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Loaded event of Window object.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#if NETCOREAPP
            for (int i = 0; i < 7; i++)
            {
                WebpEncodingMethod encoding = (WebpEncodingMethod)i;
                encodingComboBox.Items.Add(encoding);
            }

            foreach (WebpTransparentColorMode transparent in Enum.GetValues(typeof(WebpTransparentColorMode)))
            {
                transparentComboBox.Items.Add(transparent);
            }

            EncoderSettings = new WebpEncoderSettings();

            nearLosslessQualityNumericUpDown.IsEnabled = false;
            lossyGroupBox.IsEnabled = false;
            lossyGroupBox.Visibility = Visibility.Hidden;
#endif

            losslessRadioButton.Checked += new RoutedEventHandler(this.losslessRadioButton_Checked);
            lossyRadioButton.Checked += new RoutedEventHandler(this.lossyRadioButton_Checked);
        }

        /// <summary>
        /// Handles the Click event of OkButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
#if NETCOREAPP
            _encoderSettings.Method = (WebpEncodingMethod)encodingComboBox.SelectedItem;
            _encoderSettings.TransparentColorMode = (WebpTransparentColorMode)transparentComboBox.SelectedItem;
            _encoderSettings.Quality = (int)qualityNumericUpDown.Value;
            _encoderSettings.NearLosslessQuality = (int)nearLosslessQualityNumericUpDown.Value;
            _encoderSettings.FilterStrength = (int)filtersStrengthNumericUpDown.Value;
            _encoderSettings.SpatialNoiseShaping = (int)spatialNoiseShapingNumericUpDown.Value;
            _encoderSettings.EntropyPasses = (int)entropyPassesNumericUpDown.Value;
            _encoderSettings.NearLossless = (bool)nearLosslessCheckBox.IsChecked;

            if ((bool)losslessRadioButton.IsChecked)
            {
                _encoderSettings.FileType = WebpFileFormatType.Lossless;
            }
            else
            {
                _encoderSettings.FileType = WebpFileFormatType.Lossy;
            }
#endif

            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of CancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }


        #region Formats

        /// <summary>
        /// Handles the Checked event of LosslessRadioButton object.
        /// </summary>
        private void losslessRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)losslessRadioButton.IsChecked)
            {
                losslessGroupBox.IsEnabled = true;
                losslessGroupBox.Visibility = Visibility.Visible;
                lossyGroupBox.IsEnabled = false;
                lossyGroupBox.Visibility = Visibility.Hidden;
                lossyRadioButton.IsChecked = false;
            }
        }

        /// <summary>
        /// Handles the Checked event of LossyRadioButton object.
        /// </summary>
        private void lossyRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)lossyRadioButton.IsChecked)
            {
                lossyGroupBox.IsEnabled = true;
                lossyGroupBox.Visibility = Visibility.Visible;
                losslessGroupBox.IsEnabled = false;
                losslessGroupBox.Visibility = Visibility.Hidden;
                losslessRadioButton.IsChecked = false;
            }
        }

        #endregion


        #region NearLossless

        /// <summary>
        /// Handles the Checked event of NearLosslessCheckBox object.
        /// </summary>
        private void nearLosslessCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            nearLosslessQualityNumericUpDown.IsEnabled = (bool)nearLosslessCheckBox.IsChecked;
        }

        #endregion

        #endregion

    }
}
