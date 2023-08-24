using System;
using System.Windows;

using Vintasoft.Imaging.Codecs.Encoders;
using Vintasoft.Imaging.Codecs.ImageFiles.Tga;

namespace WpfDemosCommonCode.Imaging.Codecs.Dialogs
{
    /// <summary>
    /// A form that allows to view and edit the TGA encoder settings.
    /// </summary>
    public partial class TgaEncoderSettingsWindow : Window
    {

        #region Constructors

        public TgaEncoderSettingsWindow()
        {
            InitializeComponent();

            foreach (TgaCompression compression in Enum.GetValues(typeof(TgaCompression)))
            {
                compressionComboBox.Items.Add(compression);
            }
            foreach (TgaImageOrigin origin in Enum.GetValues(typeof(TgaImageOrigin)))
            {
                originComboBox.Items.Add(origin);
            }

            EncoderSettings = new TgaEncoderSettings();
        }

        #endregion



        #region Properties

        TgaEncoderSettings _encoderSettings = null;
        /// <summary>
        /// Gets or sets the settings of TGA encoder.
        /// </summary>
        public TgaEncoderSettings EncoderSettings
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
                    originComboBox.SelectedItem = _encoderSettings.ImageOrigin;
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
            _encoderSettings = new TgaEncoderSettings(
                (TgaCompression)compressionComboBox.SelectedItem,
                (TgaImageOrigin)originComboBox.SelectedItem);

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
