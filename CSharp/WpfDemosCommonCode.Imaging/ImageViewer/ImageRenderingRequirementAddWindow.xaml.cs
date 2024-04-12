using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to add an image rendering requirement.
    /// </summary>
    public partial class ImageRenderingRequirementAddWindow : Window
    {

        #region Constructor

        public ImageRenderingRequirementAddWindow()
        {
            InitializeComponent();

            string[] codecs = new string[] { "Bmp", "Jpeg", "Jpeg2000", "Tiff", "Png", "Pdf" };

            foreach (string codec in codecs)
                codecComboBox.Items.Add(codec);

            codecComboBox.SelectedIndex = 2;
        }

        #endregion



        #region Properties

        public string Codec
        {
            get
            {
                return codecComboBox.SelectedItem.ToString();
            }
        }

        float _value;
        public float Value
        {
            get
            {
                return _value;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateValue();
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of buttonCancel object.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the SelectionChanged event of codecComboBox object.
        /// </summary>
        private void codecComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (codecComboBox.SelectedItem.ToString() == "Jpeg2000")
                megapixelsComboBox.Text = "0.5";
            else
                megapixelsComboBox.Text = "50";
        }

        /// <summary>
        /// Handles the SelectionChanged event of megapixelsComboBox object.
        /// </summary>
        private void megapixelsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateValue();
        }

        /// <summary>
        /// Handles the KeyUp event of megapixelsComboBox object.
        /// </summary>
        private void megapixelsComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateValue();
        }

        private void UpdateValue()
        {
            try
            {
                _value = float.Parse(megapixelsComboBox.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
        }

        #endregion

    }
}
