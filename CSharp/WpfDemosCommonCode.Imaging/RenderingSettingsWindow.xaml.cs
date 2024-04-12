using System;
using System.Drawing.Drawing2D;
using System.Windows;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs.Decoders;
using Vintasoft.Imaging.Codecs.Encoders;
using Vintasoft.Imaging.Drawing;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A form that allows to view and edit the rendering settings.
    /// </summary>
    public partial class RenderingSettingsWindow : Window
    {

        #region Constructor

        public RenderingSettingsWindow()
        {
            InitializeComponent();

            // init "Interpolation Mode"
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.Bicubic);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.Bilinear);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.HighQualityBicubic);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.HighQualityBilinear);
            interpolationModeComboBox.Items.Add(ImageInterpolationMode.NearestNeighbor);

            // init "Smoothing Mode"
            smoothingModeComboBox.Items.Add(DrawingSmoothingMode.AntiAlias);
            smoothingModeComboBox.Items.Add(DrawingSmoothingMode.None);
        }
        
        public RenderingSettingsWindow(RenderingSettings renderingSettings)
            :this()
        {
            _renderingSettings = renderingSettings;

            if (renderingSettings.IsEmpty || renderingSettings.Resolution.IsEmpty())
            {
                cbDefault.IsChecked = true;
                smoothingModeComboBox.SelectedItem = DrawingSmoothingMode.AntiAlias;
                interpolationModeComboBox.SelectedItem = ImageInterpolationMode.HighQualityBilinear;
                optimizeImageDrawingCheckBox.IsChecked = true;
            }
            else
            {
                cbDefault.IsChecked = false;
                gbCustomSettings.IsEnabled = true;
                smoothingModeComboBox.SelectedItem = renderingSettings.SmoothingMode;
                interpolationModeComboBox.SelectedItem = renderingSettings.InterpolationMode;
                horizontalResolution.Value = (int)renderingSettings.Resolution.Horizontal;
                verticalResolution.Value = (int)renderingSettings.Resolution.Vertical;
                optimizeImageDrawingCheckBox.IsChecked = renderingSettings.OptimizeImageDrawing;
            }
        }

        #endregion



        #region Properties

        RenderingSettings _renderingSettings;
        /// <summary>
        /// Gets the rendering settings.
        /// </summary>
        public RenderingSettings RenderingSettings
        {
            get
            {
                return _renderingSettings;
            }
        }

        #endregion
        


        #region Methods

        public static void SetRenderingSettingsIfNeed(ImageCollection images, EncoderBase encoder, RenderingSettings defaultRenderingSettings)
        {
            if (encoder == null || !(encoder is IPdfEncoder))
            {
                for (int i = 0; i < images.Count; i++)
                    if (images[i].IsVectorImage)
                    {
                        RenderingSettingsWindow settingsForm = new RenderingSettingsWindow(defaultRenderingSettings.CreateClone());
                        if (settingsForm.ShowDialog().Value)
                            images.SetRenderingSettings(settingsForm.RenderingSettings);
                        else
                            return;
                        break;
                    }
            }
        }

        public static void SetRenderingSettingsIfNeed(VintasoftImage image, EncoderBase encoder, RenderingSettings defaultRenderingSettings)
        {
            if (encoder == null || !(encoder is IPdfEncoder))
            {
                if (image.IsVectorImage)
                {
                    RenderingSettingsWindow settingsForm = new RenderingSettingsWindow(defaultRenderingSettings.CreateClone());
                    if (settingsForm.ShowDialog().Value)
                        image.RenderingSettings = settingsForm.RenderingSettings;
                }
            }
        }

        /// <summary>
        /// Handles the Click event of cbDefault object.
        /// </summary>
        private void cbDefault_Click(object sender, RoutedEventArgs e)
        {
            gbCustomSettings.IsEnabled = cbDefault.IsChecked.Value != true;
        }

        /// <summary>
        /// Handles the Click event of btOk object.
        /// </summary>
        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            if (cbDefault.IsChecked.Value == true)
                RenderingSettings.Empty.CopyTo(_renderingSettings);
            else
            {
                _renderingSettings.Resolution = new Resolution((float)horizontalResolution.Value, (float)verticalResolution.Value);
                _renderingSettings.InterpolationMode = (ImageInterpolationMode)interpolationModeComboBox.SelectedItem;
                _renderingSettings.SmoothingMode = (DrawingSmoothingMode)smoothingModeComboBox.SelectedItem;
                _renderingSettings.OptimizeImageDrawing = optimizeImageDrawingCheckBox.IsChecked.Value == true;
            }
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of btCancel object.
        /// </summary>
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

    }
}
