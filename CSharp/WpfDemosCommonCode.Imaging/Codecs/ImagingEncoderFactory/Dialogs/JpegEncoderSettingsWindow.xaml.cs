using System;
using System.Windows;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs;
using Vintasoft.Imaging.Codecs.Encoders;


namespace WpfDemosCommonCode.Imaging.Codecs.Dialogs
{
    /// <summary>
    /// A form that allows to view and edit the JPEG encoder settings.
    /// </summary>
    public partial class JpegEncoderSettingsWindow : Window
    {

        #region Constructor

        public JpegEncoderSettingsWindow()
        {
            InitializeComponent();
            EditAnnotationSettings = false;
        }

        #endregion



        #region Properties

        JpegEncoderSettings _encoderSettings;
        /// <summary>
        /// Gets or sets JPEG encoder settings.
        /// </summary>
        public JpegEncoderSettings EncoderSettings
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
                return tabControl1.Items.Contains(annotationsTabPage);
            }
            set
            {
                if (EditAnnotationSettings != value)
                {
                    if (value)
                        tabControl1.Items.Add(annotationsTabPage);
                    else
                        tabControl1.Items.Remove(annotationsTabPage);
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
                EncoderSettings = new JpegEncoderSettings();
        }

        private void UpdateUI()
        {
            jpegGrayscaleCheckBox.IsChecked = EncoderSettings.SaveAsGrayscale;
            jpegQualityNumericUpDown.Value = EncoderSettings.Quality;
            disableSubsamplingCheckBox.IsChecked = EncoderSettings.IsSubsamplingDisabled;
            optimizeHuffmanTablesCheckBox.IsChecked = EncoderSettings.GenerateOptimalHuffmanTables;
            createThumbnailCheckBox.IsChecked = EncoderSettings.CreateThumbnail;
            saveCommentsCheckBox.IsChecked = EncoderSettings.SaveComments;
            copyExifMetadataCheckBox.IsChecked = EncoderSettings.CopyExifMetadata;
            copyUnkwonwnApplicationMetadataCheckBox.IsChecked = EncoderSettings.CopyUnknownApplicationMetadata;

            annotationsBinaryCheckBox.IsChecked = (EncoderSettings.AnnotationsFormat & AnnotationsFormat.VintasoftBinary) != 0;
            annotationXmpCheckBox.IsChecked = (EncoderSettings.AnnotationsFormat & AnnotationsFormat.VintasoftXmp) != 0;
        }

        private void SetEncoderSettings()
        {
            EncoderSettings.SaveAsGrayscale = jpegGrayscaleCheckBox.IsChecked.Value == true;
            EncoderSettings.Quality = (int)jpegQualityNumericUpDown.Value;
            EncoderSettings.IsSubsamplingDisabled = disableSubsamplingCheckBox.IsChecked.Value == true;
            EncoderSettings.GenerateOptimalHuffmanTables = optimizeHuffmanTablesCheckBox.IsChecked.Value == true;
            EncoderSettings.CreateThumbnail = createThumbnailCheckBox.IsChecked.Value == true;
            EncoderSettings.SaveComments = saveCommentsCheckBox.IsChecked.Value == true;
            EncoderSettings.CopyExifMetadata = copyExifMetadataCheckBox.IsChecked.Value == true;
            EncoderSettings.CopyUnknownApplicationMetadata = copyUnkwonwnApplicationMetadataCheckBox.IsChecked.Value == true;

            if (EditAnnotationSettings)
            {
                EncoderSettings.AnnotationsFormat = AnnotationsFormat.None;
                if (annotationsBinaryCheckBox.IsChecked.Value == true)
                    EncoderSettings.AnnotationsFormat |= AnnotationsFormat.VintasoftBinary;
                if (annotationXmpCheckBox.IsChecked.Value == true)
                    EncoderSettings.AnnotationsFormat |= AnnotationsFormat.VintasoftXmp;
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
