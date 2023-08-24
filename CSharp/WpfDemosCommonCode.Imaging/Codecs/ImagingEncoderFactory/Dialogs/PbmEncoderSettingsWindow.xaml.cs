using System;
using System.Windows;

using Vintasoft.Imaging.Codecs.Encoders;
using Vintasoft.Imaging.Codecs.ImageFiles.Pbm;

namespace WpfDemosCommonCode.Imaging.Codecs.Dialogs
{
    /// <summary>
    /// A form that allows to view and edit the PBM encoder settings.
    /// </summary>
    public partial class PbmEncoderSettingsWindow : Window
    {

        #region Constructors

        public PbmEncoderSettingsWindow()
        {
            InitializeComponent();

            foreach (PbmEncoding encoding in Enum.GetValues(typeof(PbmEncoding)))
            {
                encodingComboBox.Items.Add(encoding);
            }

            EncoderSettings = new PbmEncoderSettings();
        }

        #endregion



        #region Properties

        PbmEncoderSettings _encoderSettings = null;
        /// <summary>
        /// Gets or sets the settings of PBM encoder.
        /// </summary>
        public PbmEncoderSettings EncoderSettings
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

                    encodingComboBox.SelectedItem = _encoderSettings.Encoding;
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
            _encoderSettings = new PbmEncoderSettings((PbmEncoding)encodingComboBox.SelectedItem);

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
