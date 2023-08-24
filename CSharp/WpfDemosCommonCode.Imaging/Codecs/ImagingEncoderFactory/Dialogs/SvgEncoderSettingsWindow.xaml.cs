using System;
using System.Windows;
using System.Windows.Controls;
using Vintasoft.Imaging.Codecs.Encoders;


namespace WpfDemosCommonCode.Imaging.Codecs.Dialogs
{
    /// <summary>
    /// A form that allows to view and edit the SVG encoder settings.
    /// </summary>
    public partial class SvgEncoderSettingsWindow : Window
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgEncoderSettingsWindow"/> class.
        /// </summary>
        public SvgEncoderSettingsWindow()
        {
            InitializeComponent();

            pngEncoderSettingsGroupBox.Visibility = Visibility.Collapsed;
            jpegEncoderSettingsGroupBox.Visibility = Visibility.Collapsed;

            encoderNameComboBox.Items.Add("PNG");
            encoderNameComboBox.Items.Add("JPEG");

            pngSettingsComboBox.Items.Add("Fast");
            pngSettingsComboBox.Items.Add("Best Speed");
            pngSettingsComboBox.Items.Add("Normal");
            pngSettingsComboBox.Items.Add("Best Compression");
        }

        #endregion



        #region Properties

        SvgEncoderSettings _encoderSettings = null;
        /// <summary>
        /// Gets or sets the settings of the SVG encoder.
        /// </summary>
        public SvgEncoderSettings EncoderSettings
        {
            get
            {
                return _encoderSettings;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (_encoderSettings != value)
                {
                    _encoderSettings = value;

                    UpdateUI();
                }
            }
        }

        #endregion



        #region Methods

        #region UI

        /// <summary>
        /// Handles the Loaded event of Window object.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (EncoderSettings == null)
                EncoderSettings = new SvgEncoderSettings();
        }

        /// <summary>
        /// Handles the Click event of ButtonOk object.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            // update encoder
            string selectedEncoder = encoderNameComboBox.SelectedItem.ToString();
            switch (selectedEncoder)
            {
                case "PNG":
                    PngEncoder pngEncoder = new PngEncoder();

                    // set encoder settings
                    PngEncoderSettings pngSettings = GetPngEncoderSettings(pngSettingsComboBox.SelectedItem.ToString());
                    pngEncoder.Settings.FilterMethod = pngSettings.FilterMethod;
                    pngEncoder.Settings.CompressionLevel = pngSettings.CompressionLevel;

                    EncoderSettings.EmbeddedImageEncoder = pngEncoder;
                    break;

                case "JPEG":
                    // set encoder settings
                    JpegEncoderSettings jpegSettings = new JpegEncoderSettings();
                    jpegSettings.Quality = (int)jpegQualityNumericUpDown.Value;
                    jpegSettings.SaveAsGrayscale = jpegGrayscaleCheckBox.IsChecked.Value == true;

                    JpegEncoder jpegEncoder = new JpegEncoder(jpegSettings);
                    EncoderSettings.EmbeddedImageEncoder = jpegEncoder;
                    break;
            }

            EncoderSettings.AllowExternalFonts = allowExternalFontsCheckBox.IsChecked.Value == true;

            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of ButtonCancel object.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of EncoderNameComboBox object.
        /// </summary>
        private void encoderNameComboBox_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedEncoder = encoderNameComboBox.SelectedItem.ToString();
            if (selectedEncoder == "PNG")
            {
                pngEncoderSettingsGroupBox.Visibility = Visibility.Visible;
                jpegEncoderSettingsGroupBox.Visibility = Visibility.Collapsed;
                pngSettingsComboBox.SelectedItem = "Normal";
            }
            else if (selectedEncoder == "JPEG")
            {
                jpegEncoderSettingsGroupBox.Visibility = Visibility.Visible;
                pngEncoderSettingsGroupBox.Visibility = Visibility.Collapsed;
            }
        }

        #endregion


        /// <summary>
        /// Updates the user interface of this dialog.
        /// </summary>
        private void UpdateUI()
        {
            if (EncoderSettings.EmbeddedImageEncoder == null)
            {
                encoderNameComboBox.SelectedItem = "PNG";
            }
            else
            {
                encoderNameComboBox.SelectedItem = EncoderSettings.EmbeddedImageEncoder.Name.ToUpper();
            }

            allowExternalFontsCheckBox.IsChecked = EncoderSettings.AllowExternalFonts;
        }

        /// <summary>
        /// Returns an instance of <see cref="PngEncoderSettings"> 
        /// with the default settings, specified by settings name.
        /// </summary>
        /// <param name="settingsName">PNG encoder settings name.</param>
        /// <returns>
        /// An instance of <see cref="PngEncoderSettings"> with 
        /// the default settings, specified by settings name.
        /// </returns>
        private PngEncoderSettings GetPngEncoderSettings(string settingsName)
        {
            switch (settingsName)
            {
                case "Fast":
                    return PngEncoderSettings.Fast;
                case "Best Speed":
                    return PngEncoderSettings.BestSpeed;
                case "Normal":
                    return PngEncoderSettings.Normal;
                case "Best Compression":
                    return PngEncoderSettings.BestCompression;
            }

            encoderNameComboBox.SelectedItem = "Normal";
            return PngEncoderSettings.Normal;
        }

        #endregion

    }
}
