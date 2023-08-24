using System;
using System.Windows;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs.Encoders;
using Vintasoft.Imaging.Codecs.ImageFiles.Png;

namespace WpfDemosCommonCode.Imaging.Codecs.Dialogs
{
    /// <summary>
    /// A form that allows to view and edit the PNG encoder settings.
    /// </summary>
    public partial class PngEncoderSettingsWindow : Window
    {

        #region Constructors

        public PngEncoderSettingsWindow()
        {
            InitializeComponent();

            foreach (object item in Enum.GetValues(typeof(PngFilterMethod)))
                filterMethodComboBox.Items.Add(item);

            for (int i = 0; i <= 9; i++)
                compressionLevelComboBox.Items.Add(i);
        }

        #endregion



        #region Properties

        PngEncoderSettings _encoderSettings;
        /// <summary>
        /// Gets or sets PNG encoder settings.
        /// </summary>
        public PngEncoderSettings EncoderSettings
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

        public bool EditAnnotationSettings
        {
            get
            {
                return settingsTabControl.Items.Contains(annotationsTabPage);
            }
            set
            {
                if (EditAnnotationSettings != value)
                {
                    if (value)
                        settingsTabControl.Items.Add(annotationsTabPage);
                    else
                        settingsTabControl.Items.Remove(annotationsTabPage);
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
            {
                EncoderSettings = PngEncoderSettings.Fast;
                fastRadioButton.IsChecked = true;
            }
        }

        private void UpdateUI()
        {
            if (EncoderSettings.Equals(PngEncoderSettings.BestSpeed))
                bestSpeedRadioButton.IsChecked = true;
            else if (EncoderSettings.Equals(PngEncoderSettings.Fast))
                fastRadioButton.IsChecked = true;
            else if (EncoderSettings.Equals(PngEncoderSettings.Normal))
                normalRadioButton.IsChecked = true;
            else if (EncoderSettings.Equals(PngEncoderSettings.BestCompression))
                bestCompressionRadioButton.IsChecked = true;
            else
                customRadioButton.IsChecked = true;

            customGroupBox.IsEnabled = customRadioButton.IsChecked.Value == true;
            filterMethodComboBox.SelectedItem = EncoderSettings.FilterMethod;
            compressionLevelComboBox.SelectedItem = EncoderSettings.CompressionLevel;

            adam7InterlacingCheckBox.IsChecked = EncoderSettings.InterlaceMethod == PngInterlaceMethod.Adam7;

            if (EditAnnotationSettings)
            {
                annotationsBinaryCheckBox.IsChecked = (EncoderSettings.AnnotationsFormat | AnnotationsFormat.VintasoftBinary) != 0;
            }
        }

        private void SetEncoderSettings()
        {
            if (bestSpeedRadioButton.IsChecked.Value == true)
            {
                EncoderSettings.FilterMethod = PngEncoderSettings.BestSpeed.FilterMethod;
                EncoderSettings.CompressionLevel = PngEncoderSettings.BestSpeed.CompressionLevel;
            }
            else if (fastRadioButton.IsChecked.Value == true)
            {
                EncoderSettings.FilterMethod = PngEncoderSettings.Fast.FilterMethod;
                EncoderSettings.CompressionLevel = PngEncoderSettings.Fast.CompressionLevel;
            }
            else if (normalRadioButton.IsChecked.Value == true)
            {
                EncoderSettings.FilterMethod = PngEncoderSettings.Normal.FilterMethod;
                EncoderSettings.CompressionLevel = PngEncoderSettings.Normal.CompressionLevel;
            }
            else if (bestCompressionRadioButton.IsChecked.Value == true)
            {
                EncoderSettings.FilterMethod = PngEncoderSettings.BestCompression.FilterMethod;
                EncoderSettings.CompressionLevel = PngEncoderSettings.BestCompression.CompressionLevel;
            }
            else
            {
                EncoderSettings.FilterMethod = (PngFilterMethod)filterMethodComboBox.SelectedItem;
                EncoderSettings.CompressionLevel = (int)compressionLevelComboBox.SelectedItem;
            }

            if (adam7InterlacingCheckBox.IsChecked.Value == true)
                EncoderSettings.InterlaceMethod = PngInterlaceMethod.Adam7;
            else
                EncoderSettings.InterlaceMethod = PngInterlaceMethod.NoInterlace;

            EncoderSettings.AnnotationsFormat = AnnotationsFormat.None;
            if (annotationsBinaryCheckBox.IsChecked.Value == true)
                EncoderSettings.AnnotationsFormat |= AnnotationsFormat.VintasoftBinary;
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

        /// <summary>
        /// Handles the Checked event of RadioButton object.
        /// </summary>
        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            customGroupBox.IsEnabled = sender == customRadioButton;
        }

        #endregion

    }
}
