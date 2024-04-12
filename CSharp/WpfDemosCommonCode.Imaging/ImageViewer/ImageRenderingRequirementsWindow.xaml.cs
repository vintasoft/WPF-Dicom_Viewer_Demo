using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Vintasoft.Imaging.ImageRendering;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to view and edit the image rendering requirements.
    /// </summary>
    public partial class ImageRenderingRequirementsWindow : Window
    {

        #region Fields

        Dictionary<string, float> _requirements = new Dictionary<string, float>();

        string[] _codes = new string[] { "Bmp", "Jpeg", "Jpeg2000", "Tiff", "Png", "Pdf", "Xps", "Docx" };

        #endregion



        #region Constructors

        public ImageRenderingRequirementsWindow()
        {
            InitializeComponent();
        }

        public ImageRenderingRequirementsWindow(ImageRenderingRequirements renderingRequirements)
            : this()
        {
            _renderingRequirements = renderingRequirements;

            ShowSettings();
        }

        #endregion



        #region Properties

        ImageRenderingRequirements _renderingRequirements;
        public ImageRenderingRequirements RenderingRequirements
        {
            get
            {
                return _renderingRequirements;
            }
        }

        #endregion



        #region Methods

        private void ShowSettings()
        {

            for (int i = 0; i < _codes.Length; i++)
            {
                ImageSizeRenderingRequirement requirement;
                requirement = _renderingRequirements.GetRequirement(_codes[i]) as ImageSizeRenderingRequirement;
                if (requirement != null)
                {
                    codecComboBox.Items.Add(_codes[i]);
                    _requirements.Add(_codes[i], requirement.ImageSize);
                }
            }

            if (codecComboBox.Items.Count > 0)
                codecComboBox.SelectedIndex = 0;
            UpdateUI();
        }

        private bool SetSettings()
        {
            for (int i = 0; i < _codes.Length; i++)
            {
                string codec = _codes[i];
                if (_requirements.ContainsKey(codec))
                {
                    if (codec == "Tiff")
                        _renderingRequirements.SetRequirement(codec, new TiffRenderingRequirement(_requirements[codec]));
                    else
                        _renderingRequirements.SetRequirement(codec, new ImageSizeRenderingRequirement(_requirements[codec]));
                }
                else
                {
                    _renderingRequirements.SetRequirement(codec, null);
                }
            }
            return true;
        }

        private void UpdateUI()
        {
            bool isEmptyRenderingRequirments = _requirements.Count == 0;

            codecComboBox.IsEnabled = !isEmptyRenderingRequirments;
            megapixelsComboBox.IsEnabled = !isEmptyRenderingRequirments;
            removeButton.IsEnabled = !isEmptyRenderingRequirments;
        }

        /// <summary>
        /// Handles the Click event of buttonOk object.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (SetSettings())
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
        /// Handles the SelectionChanged event of megapixelsComboBox object.
        /// </summary>
        private void megapixelsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMegapixels();
        }

        /// <summary>
        /// Handles the KeyUp event of megapixelsComboBox object.
        /// </summary>
        private void megapixelsComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateMegapixels();
        }

        private void UpdateMegapixels()
        {
            try
            {
                if (megapixelsComboBox.Text != "")
                    _requirements[(string)codecComboBox.SelectedItem] = float.Parse(megapixelsComboBox.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                DemosTools.ShowErrorMessage(ex);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of codecComboBox object.
        /// </summary>
        private void codecComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (codecComboBox.SelectedItem != null)
                megapixelsComboBox.Text = _requirements[(string)codecComboBox.SelectedItem].ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Handles the Click event of addButton object.
        /// </summary>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            ImageRenderingRequirementAddWindow form = new ImageRenderingRequirementAddWindow();
            if (form.ShowDialog().Value)
            {
                string codec = form.Codec;
                float value = form.Value;
                if (_requirements.ContainsKey(codec))
                    _requirements[codec] = value;
                else
                {
                    _requirements.Add(codec, value);
                    codecComboBox.Items.Add(codec);
                }
                UpdateUI();
                if (codecComboBox.SelectedItem != null && codecComboBox.SelectedItem.ToString() == codec)
                    codecComboBox_SelectionChanged(this, null);
                else
                    codecComboBox.SelectedItem = codec;
            }
        }

        /// <summary>
        /// Handles the Click event of removeButton object.
        /// </summary>
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            string codec = codecComboBox.SelectedItem.ToString();
            if (_requirements.ContainsKey(codec))
            {
                _requirements.Remove(codec);
                codecComboBox.Items.Remove(codec);
                codecComboBox.SelectedIndex = codecComboBox.Items.Count - 1;
                UpdateUI();
            }
        }

        #endregion

    }
}
