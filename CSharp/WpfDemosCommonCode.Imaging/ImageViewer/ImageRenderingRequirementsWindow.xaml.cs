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

        /// <summary>
        /// Dictionary: codec name => the image size in megapixels.
        /// </summary>
        Dictionary<string, float> _codecNameToImageSizeInMegapixels = new Dictionary<string, float>();

        /// <summary>
        /// Dictionary: codec name => the image size in megabytes.
        /// </summary>
        Dictionary<string, float> _codecNameToImageSizeInMegabytes = new Dictionary<string, float>();

        /// <summary>
        /// The available codec names.
        /// </summary>
        string[] _codecNames = new string[] { "Bmp", "Jpeg", "Jpeg2000", "Tiff", "Png", "Pdf", "Psd", "Svg", "Docx", "Xlsx", "Wmf" };

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRenderingRequirementsWindow"/> class.
        /// </summary>
        public ImageRenderingRequirementsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRenderingRequirementsWindow"/> class.
        /// </summary>
        /// <param name="renderingRequirements">The rendering requirements.</param>
        public ImageRenderingRequirementsWindow(ImageRenderingRequirements renderingRequirements)
            : this()
        {
            _renderingRequirements = renderingRequirements;

            ShowSettings();
        }

        #endregion



        #region Properties

        ImageRenderingRequirements _renderingRequirements;
        /// <summary>
        /// Gets the current rendering requirements.
        /// </summary>
        public ImageRenderingRequirements RenderingRequirements
        {
            get
            {
                return _renderingRequirements;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Shows the <see cref="RenderingRequirements"/> settings.
        /// </summary>
        private void ShowSettings()
        {

            for (int i = 0; i < _codecNames.Length; i++)
            {
                // get image size rendering requirements
                ImageSizeRenderingRequirement sizeRequirement =
                    _renderingRequirements.GetRequirement(_codecNames[i]) as ImageSizeRenderingRequirement;
                if (sizeRequirement != null)
                {
                    codecComboBox.Items.Add(_codecNames[i]);
                    _codecNameToImageSizeInMegapixels.Add(_codecNames[i], sizeRequirement.ImageSize);
                }

                // get memory usage rendering requirements
                MemoryUsageRenderingRequirement memoryRequirement =
                    _renderingRequirements.GetRequirement(_codecNames[i]) as MemoryUsageRenderingRequirement;
                if (memoryRequirement != null)
                {
                    codecComboBox.Items.Add(_codecNames[i]);
                    _codecNameToImageSizeInMegabytes.Add(_codecNames[i], memoryRequirement.MemorySize);
                }
            }

            if (codecComboBox.Items.Count > 0)
                codecComboBox.SelectedIndex = 0;
            UpdateUI();
        }

        private bool SetSettings()
        {
            // for each codec in codecs
            for (int i = 0; i < _codecNames.Length; i++)
            {
                // get codec
                string codec = _codecNames[i];
                // if codec has rendering requirement
                if (_codecNameToImageSizeInMegapixels.ContainsKey(codec))
                {
                    // if current codec is "TIFF"
                    if (codec == "Tiff")
                        _renderingRequirements.SetRequirement(codec, new TiffRenderingRequirement(_codecNameToImageSizeInMegapixels[codec]));
                    else
                        _renderingRequirements.SetRequirement(codec, new ImageSizeRenderingRequirement(_codecNameToImageSizeInMegapixels[codec]));
                }
                else if (_codecNameToImageSizeInMegabytes.ContainsKey(codec))
                {
                    // if current codec is "TIFF"
                    if (codec == "Tiff")
                        _renderingRequirements.SetRequirement(codec, new TiffMemoryUsageRenderingRequirement(_codecNameToImageSizeInMegabytes[codec]));
                    else
                        _renderingRequirements.SetRequirement(codec, new MemoryUsageRenderingRequirement(_codecNameToImageSizeInMegabytes[codec]));
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
            bool isEmptyRenderingRequirments = _codecNameToImageSizeInMegapixels.Count == 0;

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
                {
                    // update requirement image size for selected codec
                    string codecName = (string)codecComboBox.SelectedItem;
                    float value = float.Parse(megapixelsComboBox.Text, CultureInfo.InvariantCulture);
                    if (_codecNameToImageSizeInMegapixels.ContainsKey(codecName))
                        _codecNameToImageSizeInMegapixels[codecName] = value;
                    if (_codecNameToImageSizeInMegabytes.ContainsKey(codecName))
                        _codecNameToImageSizeInMegabytes[codecName] = value;
                }
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
            {
                string codecName = (string)codecComboBox.SelectedItem;
                // update image size requirement for selected codec
                if (_codecNameToImageSizeInMegapixels.ContainsKey(codecName))
                {
                    megapixelsComboBox.Text = _codecNameToImageSizeInMegapixels[codecName].ToString(CultureInfo.InvariantCulture);
                    sizeTypeLabel.Content = "Megapixels";
                }
                else if (_codecNameToImageSizeInMegabytes.ContainsKey(codecName))
                {
                    megapixelsComboBox.Text = _codecNameToImageSizeInMegabytes[codecName].ToString(CultureInfo.InvariantCulture);
                    sizeTypeLabel.Content = "Megabytes";
                }
            }
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
                
                // if rendering requirement must be updated
                if (_codecNameToImageSizeInMegapixels.ContainsKey(codec))
                {
                    _codecNameToImageSizeInMegapixels[codec] = value;
                }
                else if (_codecNameToImageSizeInMegabytes.ContainsKey(codec))
                {
                    _codecNameToImageSizeInMegabytes[codec] = value;
                }
                // if rendering requirement must be added
                else
                {
                    _codecNameToImageSizeInMegapixels.Add(codec, value);
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
            if (_codecNameToImageSizeInMegapixels.ContainsKey(codec))
            {
                _codecNameToImageSizeInMegapixels.Remove(codec);
                _codecNameToImageSizeInMegabytes.Remove(codec);
                codecComboBox.Items.Remove(codec);
                codecComboBox.SelectedIndex = codecComboBox.Items.Count - 1;
                UpdateUI();
            }
        }

        #endregion

    }
}
