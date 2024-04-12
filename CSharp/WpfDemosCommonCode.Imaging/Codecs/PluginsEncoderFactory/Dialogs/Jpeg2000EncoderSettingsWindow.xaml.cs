using System;
using System.Windows;

using Vintasoft.Imaging.Codecs;
using Vintasoft.Imaging.Codecs.ImageFiles.Jpeg2000;
using Vintasoft.Imaging.Codecs.Encoders;


namespace WpfDemosCommonCode.Imaging.Codecs.Dialogs
{
    /// <summary>
    /// A form that allows to view and edit the JPEG2000 encoder settings.
    /// </summary>
    public partial class Jpeg2000EncoderSettingsWindow : Window
    {

        #region Constructors

        public Jpeg2000EncoderSettingsWindow()
        {
            InitializeComponent();

            foreach (object item in Enum.GetValues(typeof(Jpeg2000FileFormat)))
                formatComboBox.Items.Add(item);

            foreach (object item in Enum.GetValues(typeof(ProgressionOrder)))
                progressionOrderComboBox.Items.Add(item);
        }

        #endregion



        #region Properties

        Jpeg2000EncoderSettings _encoderSettings;
        /// <summary>
        /// Gets or sets the JPEG 2000 Encoder Settings.
        /// </summary>
        public Jpeg2000EncoderSettings EncoderSettings
        {
            get
            {
                return _encoderSettings;
            }
            set
            {
                if (value == null)
                    value = new Jpeg2000EncoderSettings();
                _encoderSettings = value;
                UpdateUI();
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
                EncoderSettings = new Jpeg2000EncoderSettings();
        }

        /// <summary>
        /// Handles the ValueChanged event of compressionRatioNumericUpDown object.
        /// </summary>
        private void compressionRatioNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            compressionRatioLabel.Content = string.Format("(1 : {0})", compressionRatioNumericUpDown.Value);
        }

        /// <summary>
        /// Handles the Click event of waveletTransformCheckBox object.
        /// </summary>
        private void waveletTransformCheckBox_Click(object sender, RoutedEventArgs e)
        {
            waveletLevelsNumericUpDown.IsEnabled = waveletTransformCheckBox.IsChecked.Value == true;
            if (waveletLevelsNumericUpDown.Value == 0)
                waveletLevelsNumericUpDown.Value = 5;
        }

        private void UpdateUI()
        {
            formatComboBox.SelectedItem = _encoderSettings.FileFormat;
            waveletLevelsNumericUpDown.Value = _encoderSettings.WaveletLevels;
            waveletTransformCheckBox.IsChecked = _encoderSettings.WaveletLevels > 0;
            waveletTransformCheckBox_Click(waveletTransformCheckBox, null);
            qualityLayersNumericUpDown.Value = _encoderSettings.QualityLayers.Length;
            progressionOrderComboBox.SelectedItem = _encoderSettings.ProgressionOrder;
            useTilesCheckBox.IsChecked = _encoderSettings.TileWidth != 0 && _encoderSettings.TileHeight != 0;
            lossyCompressionCheckBox.IsChecked = _encoderSettings.CompressionType == Jpeg2000CompressionType.Lossy;
            if (useTilesCheckBox.IsChecked.Value == true)
            {
                tileWidthNumericUpDown.Value = Math.Max(tileWidthNumericUpDown.Minimum, _encoderSettings.TileWidth);
                tileHeightNumericUpDown.Value = Math.Max(tileHeightNumericUpDown.Minimum, _encoderSettings.TileHeight);
            }
            compressionRatioNumericUpDown.Value = (int)Math.Round(_encoderSettings.CompressionRatio);
            if (_encoderSettings.FileSize == 0)
                compressionRatioRadioButton.IsChecked = true;
            else
                imageDataSizeRadioButton.IsChecked = true;

            UpdateEnabledState();
        }

        private void SetEncoderSettings()
        {
            _encoderSettings.FileFormat = (Jpeg2000FileFormat)formatComboBox.SelectedItem;
            if (waveletTransformCheckBox.IsChecked.Value == true)
                _encoderSettings.WaveletLevels = (int)waveletLevelsNumericUpDown.Value;
            else
                _encoderSettings.WaveletLevels = 0;
            _encoderSettings.QualityLayers = new double[(int)qualityLayersNumericUpDown.Value];
            for (int i = 0; i < _encoderSettings.QualityLayers.Length; i++)
                _encoderSettings.QualityLayers[i] = 1;
            _encoderSettings.ProgressionOrder = (ProgressionOrder)progressionOrderComboBox.SelectedItem;
            if (lossyCompressionCheckBox.IsChecked.Value == true)
                _encoderSettings.CompressionType = Jpeg2000CompressionType.Lossy;
            else
                _encoderSettings.CompressionType = Jpeg2000CompressionType.Lossless;
            if (useTilesCheckBox.IsChecked.Value == true)
            {
                _encoderSettings.TileWidth = (int)tileWidthNumericUpDown.Value;
                _encoderSettings.TileHeight = (int)tileHeightNumericUpDown.Value;
            }
            else
            {
                _encoderSettings.TileWidth = 0;
                _encoderSettings.TileHeight = 0;
            }
            if (_encoderSettings.CompressionType == Jpeg2000CompressionType.Lossy)
            {
                if (imageDataSizeRadioButton.IsChecked.Value == true)
                {
                    _encoderSettings.FileSize = (long)imageDataSizeNumericUpDown.Value * 1024;
                }
                else
                {
                    _encoderSettings.FileSize = 0;
                    _encoderSettings.CompressionRatio = (double)compressionRatioNumericUpDown.Value;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of useTilesCheckBox object.
        /// </summary>
        private void useTilesCheckBox_Click(object sender, RoutedEventArgs e)
        {
            UpdateEnabledState();
        }

        private void UpdateEnabledState()
        {
            lossyGroupBox.IsEnabled = lossyCompressionCheckBox.IsChecked.Value == true;
            imageDataSizeNumericUpDown.IsEnabled = imageDataSizeRadioButton.IsChecked.Value == true;
            compressionRatioNumericUpDown.IsEnabled = compressionRatioRadioButton.IsChecked.Value == true;
            tileWidthNumericUpDown.IsEnabled = useTilesCheckBox.IsChecked.Value == true;
            tileHeightNumericUpDown.IsEnabled = useTilesCheckBox.IsChecked.Value == true;
        }

        /// <summary>
        /// Handles the Click event of buttonOk object.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetEncoderSettings();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Handles the Click event of buttonCancel object.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

    }
}
