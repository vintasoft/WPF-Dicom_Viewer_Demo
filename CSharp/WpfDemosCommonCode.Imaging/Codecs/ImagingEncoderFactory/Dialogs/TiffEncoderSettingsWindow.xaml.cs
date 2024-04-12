using System;
using System.Windows;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs;
using Vintasoft.Imaging.Codecs.ImageFiles.Tiff;
using Vintasoft.Imaging.Codecs.Encoders;
using Vintasoft.Imaging.ImageProcessing;


namespace WpfDemosCommonCode.Imaging.Codecs.Dialogs
{
    /// <summary>
    /// A form that allows to view and edit the TIFF encoder settings.
    /// </summary>
    public partial class TiffEncoderSettingsWindow : Window
    {

        #region Constructors

        public TiffEncoderSettingsWindow()
        {
            InitializeComponent();

            EditAnnotationSettings = false;
            CanAddImagesToExistingFile = false;

            foreach (BinarizationMode mode in Enum.GetValues(typeof(BinarizationMode)))
                binarizationModeComboBox.Items.Add(mode);
        }

        #endregion



        #region Properties

        TiffEncoderSettings _encoderSettings;
        /// <summary>
        /// Gets or sets TIFF encoder settings.
        /// </summary>
        public TiffEncoderSettings EncoderSettings
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

                    InitUI();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether encoder must add images to the existing TIFF file.
        /// </summary>
        /// <value>
        /// <b>True</b> - encoder must add images to the existing TIFF file;
        /// <b>false</b> - encoder must delete the existing TIFF file if necessary, create new TIFF file and add images to the new TIFF file.
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
        /// Gets or sets a value indicating whether encoder can add images to the existing TIFF file.
        /// </summary>
        /// <value>
        /// <b>True</b> - encoder can add images to the existing TIFF file;
        /// <b>false</b> - encoder can NOT add images to the existing TIFF file.
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

        public bool EditAnnotationSettings
        {
            get
            {
                return tabControl1.Items.Contains(annotationsTabItem);
            }
            set
            {
                if (EditAnnotationSettings != value)
                {
                    if (value)
                        tabControl1.Items.Add(annotationsTabItem);
                    else
                        tabControl1.Items.Remove(annotationsTabItem);
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
                EncoderSettings = new TiffEncoderSettings();
        }

        private void InitUI()
        {
            // compression
            switch (EncoderSettings.Compression)
            {
                case TiffCompression.None:
                    noneCompressionRadioButton.IsChecked = true;
                    break;

                case TiffCompression.CcittGroup3:
                case TiffCompression.CcittGroup4:
                    ccitt4CompressionRadioButton.IsChecked = true;
                    break;

                case TiffCompression.Lzw:
                    lzwCompressionRadioButton.IsChecked = true;
                    break;
                case TiffCompression.Zip:
                    zipCompressionRadioButton.IsChecked = true;
                    break;

                case TiffCompression.Jpeg:
                    jpegCompressionRadioButton.IsChecked = true;
                    break;
                case TiffCompression.Jpeg2000:
                    jpeg2000CompressionRadioButton.IsChecked = true;
                    break;

                default:
                    autoCompressionRadioButton.IsChecked = true;
                    break;
            }

            // JPEG advanced settings
            jpegGrayscaleCheckBox.IsChecked = EncoderSettings.SaveJpegAsGrayscale;
            jpegQualityNumericUpDown.Value = EncoderSettings.JpegQuality;
            // LZW advanced settings
            lzwUsePredictorCheckBox.IsChecked = EncoderSettings.UsePredictor;
            // ZIP advanced settings
            zipUsePredictorCheckBox.IsChecked = EncoderSettings.UsePredictor;
            zipLevelNumericUpDown.Value = EncoderSettings.ZipLevel;

            binarizationModeComboBox.SelectedItem = EncoderSettings.BinarizationCommand.BinarizationMode;
            binarizationThresholdNumericUpDown.Value = EncoderSettings.BinarizationCommand.Threshold;

            // annotation settings
            annotationsBinaryCheckBox.IsChecked = (_encoderSettings.AnnotationsFormat & AnnotationsFormat.VintasoftBinary) != 0;
            annotationXmpCheckBox.IsChecked = (_encoderSettings.AnnotationsFormat & AnnotationsFormat.VintasoftXmp) != 0;
            annotationWangCheckBox.IsChecked = (_encoderSettings.AnnotationsFormat & AnnotationsFormat.Wang) != 0;

            // metadata settings
            copyCommonMetadataCheckBox.IsChecked = EncoderSettings.CopyCommonMetadata;
            copyExifMetadataCheckBox.IsChecked = EncoderSettings.CopyExifMetadata;
            copyGpsMetadataCheckBox.IsChecked = EncoderSettings.CopyGpsMetadata;

            // file format
            if (EncoderSettings.FileFormat == TiffFileFormat.LittleEndian)
                littleEndianRadioButton.IsChecked = true;
            else
                bigEndianRadioButton.IsChecked = true;
            // file version
            if (EncoderSettings.FileVersion == TiffFileVersion.StandardTIFF)
                standardTiffRadioButton.IsChecked = true;
            else
                bigTiffRadioButton.IsChecked = true;
        }

        private void UpdateUI()
        {
            if (!IsInitialized)
                return;

            UpdateVisibility(jpegCompressionAdvancedSettingsGroupBox, jpegCompressionRadioButton.IsChecked.Value);
            UpdateVisibility(jpeg2000CompressionAdvancedSettingsGroupBox, jpeg2000CompressionRadioButton.IsChecked.Value);
            UpdateVisibility(lzwCompressionAdvancedSettingsGroupBox, lzwCompressionRadioButton.IsChecked.Value);
            UpdateVisibility(zipCompressionAdvancedSettingsGroupBox, zipCompressionRadioButton.IsChecked.Value);
            UpdateVisibility(binarizationAdvancedSettingsGroupBox, ccitt4CompressionRadioButton.IsChecked.Value);

            UpdateVisibility(rowsPerStripLabel, useStripsRadioButton.IsChecked.Value);
            UpdateVisibility(rowsPerStripNumericUpDown, useStripsRadioButton.IsChecked.Value);
            UpdateVisibility(tileWidthLabel, useTilesRadioButton.IsChecked.Value);
            UpdateVisibility(tileWidthNumericUpDown, useTilesRadioButton.IsChecked.Value);
            UpdateVisibility(tileHeightLabel, useTilesRadioButton.IsChecked.Value);
            UpdateVisibility(tileHeightNumericUpDown, useTilesRadioButton.IsChecked.Value);
        }

        private void UpdateVisibility(UIElement element, bool visible)
        {
            if (visible)
                element.Visibility = Visibility.Visible;
            else
                element.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Handles the Checked event of radioButton object.
        /// </summary>
        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        /// <summary>
        /// Handles the ValueChanged event of tileWidthNumericUpDown object.
        /// </summary>
        private void tileWidthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if ((tileWidthNumericUpDown.Value % 16) != 0)
            {
                MessageBox.Show("Tile width must be multiple 16.");
                tileWidthNumericUpDown.Value = (int)(tileWidthNumericUpDown.Value / 16) * 16;
            }
        }

        /// <summary>
        /// Handles the ValueChanged event of tileHeightNumericUpDown object.
        /// </summary>
        private void tileHeightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if ((tileHeightNumericUpDown.Value % 16) != 0)
            {
                MessageBox.Show("Tile height must be multiple 16.");
                tileHeightNumericUpDown.Value = (int)(tileHeightNumericUpDown.Value / 16) * 16;
            }
        }

        /// <summary>
        /// Handles the Click event of compressionJpeg2000SettingsButton object.
        /// </summary>
        private void compressionJpeg2000SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Jpeg2000EncoderSettingsWindow window = new Jpeg2000EncoderSettingsWindow();
            window.EncoderSettings = EncoderSettings.Jpeg2000EncoderSettings;
            window.Owner = this;
            if (window.ShowDialog().Value)
            {
                EncoderSettings.Jpeg2000EncoderSettings.CompressionRatio = window.EncoderSettings.CompressionRatio;
                EncoderSettings.Jpeg2000EncoderSettings.CompressionType = window.EncoderSettings.CompressionType;
                EncoderSettings.Jpeg2000EncoderSettings.EncodeAlphaChannelInPalette = window.EncoderSettings.EncodeAlphaChannelInPalette;
                EncoderSettings.Jpeg2000EncoderSettings.FileFormat = window.EncoderSettings.FileFormat;
                EncoderSettings.Jpeg2000EncoderSettings.FileSize = window.EncoderSettings.FileSize;
                EncoderSettings.Jpeg2000EncoderSettings.ProgressionOrder = window.EncoderSettings.ProgressionOrder;
                EncoderSettings.Jpeg2000EncoderSettings.QualityLayers = window.EncoderSettings.QualityLayers;
                EncoderSettings.Jpeg2000EncoderSettings.TileHeight = window.EncoderSettings.TileHeight;
                EncoderSettings.Jpeg2000EncoderSettings.TileWidth = window.EncoderSettings.TileWidth;
                EncoderSettings.Jpeg2000EncoderSettings.WaveletLevels = window.EncoderSettings.WaveletLevels;
            }
        }

        /// <summary>
        /// Handles the Click event of buttonOk object.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            SetEncoderSettings();

            if (EditAnnotationSettings && _encoderSettings.AnnotationsFormat == AnnotationsFormat.Wang)
            {
                if (MessageBox.Show(
                    "Important: some data from anotations will be lost. Do you want to continue anyway?",
                    "Warning",
                     MessageBoxButton.OKCancel,
                      MessageBoxImage.Warning) != MessageBoxResult.OK)
                {
                    return;
                }
            }

            DialogResult = true;
        }

        private void SetEncoderSettings()
        {
            // compression
            if (autoCompressionRadioButton.IsChecked.Value)
                EncoderSettings.Compression = TiffCompression.Auto;
            else if (noneCompressionRadioButton.IsChecked.Value)
                EncoderSettings.Compression = TiffCompression.None;
            else if (ccitt4CompressionRadioButton.IsChecked.Value)
            {
                EncoderSettings.Compression = TiffCompression.CcittGroup4;
                EncoderSettings.BinarizationCommand.BinarizationMode = (BinarizationMode)binarizationModeComboBox.SelectedItem;
                EncoderSettings.BinarizationCommand.Threshold = (int)binarizationThresholdNumericUpDown.Value;
            }
            else if (lzwCompressionRadioButton.IsChecked.Value)
                EncoderSettings.Compression = TiffCompression.Lzw;
            else if (zipCompressionRadioButton.IsChecked.Value)
                EncoderSettings.Compression = TiffCompression.Zip;
            else if (jpegCompressionRadioButton.IsChecked.Value)
                EncoderSettings.Compression = TiffCompression.Jpeg;
            else if (jpeg2000CompressionRadioButton.IsChecked.Value)
                EncoderSettings.Compression = TiffCompression.Jpeg2000;

            // strips & tiles
            EncoderSettings.UseStrips = useStripsRadioButton.IsChecked.Value;
            EncoderSettings.RowsPerStrip = (int)rowsPerStripNumericUpDown.Value;
            EncoderSettings.UseTiles = useTilesRadioButton.IsChecked.Value;
            EncoderSettings.TileSize = new System.Drawing.Size((int)tileWidthNumericUpDown.Value, (int)tileHeightNumericUpDown.Value);

            // JPEG advanced settings
            EncoderSettings.JpegQuality = (int)jpegQualityNumericUpDown.Value;
            EncoderSettings.SaveJpegAsGrayscale = jpegGrayscaleCheckBox.IsChecked.Value;
            // LZW advanced settings
            if (lzwCompressionRadioButton.IsChecked.Value)
                EncoderSettings.UsePredictor = lzwUsePredictorCheckBox.IsChecked.Value;
            // ZIP advanced settings
            if (zipCompressionRadioButton.IsChecked.Value)
                EncoderSettings.UsePredictor = zipUsePredictorCheckBox.IsChecked.Value;
            EncoderSettings.ZipLevel = (int)zipLevelNumericUpDown.Value;

            // annotations
            if (EditAnnotationSettings)
            {
                EncoderSettings.AnnotationsFormat = AnnotationsFormat.None;
                if (annotationsBinaryCheckBox.IsChecked.Value)
                    EncoderSettings.AnnotationsFormat |= AnnotationsFormat.VintasoftBinary;
                if (annotationXmpCheckBox.IsChecked.Value)
                    EncoderSettings.AnnotationsFormat |= AnnotationsFormat.VintasoftXmp;
                if (annotationWangCheckBox.IsChecked.Value)
                    EncoderSettings.AnnotationsFormat |= AnnotationsFormat.Wang;
            }

            // metadata
            EncoderSettings.CopyCommonMetadata = copyCommonMetadataCheckBox.IsChecked.Value;
            EncoderSettings.CopyExifMetadata = copyExifMetadataCheckBox.IsChecked.Value;
            EncoderSettings.CopyGpsMetadata = copyGpsMetadataCheckBox.IsChecked.Value;

            // file format
            if (littleEndianRadioButton.IsChecked.Value)
                EncoderSettings.FileFormat = TiffFileFormat.LittleEndian;
            else
                EncoderSettings.FileFormat = TiffFileFormat.BigEndian;
            // file version
            if (standardTiffRadioButton.IsChecked.Value)
                EncoderSettings.FileVersion = TiffFileVersion.StandardTIFF;
            else
                EncoderSettings.FileVersion = TiffFileVersion.BigTIFF;
        }

        /// <summary>
        /// Handles the Click event of buttonCancel object.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the SelectionChanged event of binarizationModeComboBox object.
        /// </summary>
        private void binarizationModeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if ((BinarizationMode)binarizationModeComboBox.SelectedItem == BinarizationMode.Threshold)
            {
                binarizationThresholdNumericUpDown.Visibility = Visibility.Visible;
                binarizationThresholdLabel.Visibility = Visibility.Visible;
            }
            else
            {
                binarizationThresholdNumericUpDown.Visibility = Visibility.Hidden;
                binarizationThresholdLabel.Visibility = Visibility.Hidden;
            }
        }

        #endregion

    }
}
