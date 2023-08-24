using System;
using System.Windows;

using Vintasoft.Imaging.Codecs.Encoders;
using Vintasoft.Imaging.Codecs.ImageFiles.Bmp;


namespace WpfDemosCommonCode.Imaging.Codecs.Dialogs
{
    /// <summary>
    /// A window that allows to view and edit the BMP encoder settings.
    /// </summary>
    public partial class BmpEncoderSettingsWindow : Window
    {

        #region Constructors

        public BmpEncoderSettingsWindow()
        {
            InitializeComponent();

            foreach (BmpCompression compression in Enum.GetValues(typeof(BmpCompression)))
            {
                if (compression == BmpCompression.Unsupported)
                    continue;

                compressionComboBox.Items.Add(compression);
            }

            EncoderSettings = new BmpEncoderSettings();
        }

        #endregion



        #region Properties

        BmpEncoderSettings _encoderSettings = null;
        /// <summary>
        /// Gets or sets the settings of BMP encoder.
        /// </summary>
        public BmpEncoderSettings EncoderSettings
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

                    compressionComboBox.SelectedItem = _encoderSettings.Compression;
                }
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// "Ok" button is clicked.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            _encoderSettings.Compression = (BmpCompression)compressionComboBox.SelectedItem;

            DialogResult = true;
        }

        /// <summary>
        /// "Cancel" button is clicked.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

    }
}
