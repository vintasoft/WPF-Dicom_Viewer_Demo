using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs.Encoders;
using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;
using Vintasoft.Imaging.Metadata;

using WpfDemosCommonCode;
using WpfDemosCommonCode.CustomControls;


namespace WpfDicomViewerDemo
{
    /// <summary>
    /// A window that allows to preview overlay images of DICOM file.
    /// </summary>
    public partial class OverlayImagesViewer : Window
    {

        #region Fields

        /// <summary>
        /// Dictionary with information about overlay images.
        /// </summary>
        Dictionary<string, DicomOverlayImage> overlayImagesInfo = new Dictionary<string, DicomOverlayImage>();

        /// <summary>
        /// Dictionary with overlay images.
        /// </summary>
        Dictionary<string, VintasoftImage> overlayImages = new Dictionary<string, VintasoftImage>();

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OverlayImagesViewer"/> class.
        /// </summary>
        public OverlayImagesViewer(VintasoftImage image)
        {
            InitializeComponent();

            DicomFrameMetadata metadata = image.Metadata.MetadataTree as DicomFrameMetadata;

            if (metadata == null)
            {
                MessageBox.Show("Current image is not DICOM frame.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }

            // load overlay images
            LoadOverlayImages(metadata);
        }

        #endregion



        #region Methods

        /// <summary> 
        /// Loads overlay images of DICOM file.
        /// </summary>
        /// <param name="file">Source image.</param>
        private void LoadOverlayImages(DicomFrameMetadata metadata)
        {
            // for each overlay image of DICOM page
            for (int overlayImageIndex = 0; overlayImageIndex < metadata.OverlayImages.Length; overlayImageIndex++)
            {
                // get the image name
                string imageName = string.Format("OverlayImage: {0}", overlayImageIndex + 1);

                bool error = false;
                // get the information about overlay image
                DicomOverlayImage imageInfo = metadata.OverlayImages[overlayImageIndex];
                VintasoftImage overlayImage = null;
                try
                {
                    // get the overlay image
                    overlayImage = imageInfo.GetOverlayImage();
                }
                catch (DicomFileException)
                {
                    error = true;
                }
                if (error)
                    continue;

                overlayImagesInfo.Add(imageName, imageInfo);
                overlayImages.Add(imageName, overlayImage);
                overlayImagesComboBox.Items.Add(imageName);
            }

            if (overlayImages.Count == 0)
                DemosTools.ShowErrorMessage("Overlay images are damaged.");

            if (overlayImagesComboBox.Items.Count > 0)
                overlayImagesComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Selected overlay image is changed in combo box.
        /// </summary>
        private void overlayImagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (overlayImagesComboBox.SelectedIndex >= 0)
            {
                string key = (string)overlayImagesComboBox.Items[overlayImagesComboBox.SelectedIndex];
                // load overlay image info
                propertyGrid.SelectedObject = overlayImagesInfo[key];
                // load overlay image
                imageViewer.Image = overlayImages[key];
            }
            else
            {
                propertyGrid.SelectedObject = null;
                imageViewer.Image = null;
            }
        }

        /// <summary>
        /// Saves the overlay image to an image file.
        /// </summary>
        private void saveAsImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (overlayImagesComboBox.SelectedIndex >= 0)
            {
                string key = (string)overlayImagesComboBox.Items[overlayImagesComboBox.SelectedIndex];
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "TIFF Files|*.tif;*.tiff|JPEG Files|*.jpg;*.jpeg|PNG Files|.png";
                saveFileDialog.DefaultExt = ".tiff";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        string saveFilename = Path.GetFullPath(saveFileDialog.FileName);
                        EncoderBase encoder = null;
                        switch (Path.GetExtension(saveFilename).ToUpperInvariant())
                        {
                            case ".TIF":
                            case ".TIFF":
                                encoder = new TiffEncoder();
                                break;

                            case ".JPG":
                            case ".JPEG":
                                encoder = new JpegEncoder();
                                break;

                            case ".PNG":
                                encoder = new PngEncoder();
                                break;
                        }

                        overlayImages[key].Save(saveFilename, encoder);
                    }
                    catch (Exception ex)
                    {
                        DemosTools.ShowErrorMessage(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of ImageViewerBackColorToolStripMenuItem object.
        /// </summary>
        private void imageViewerBackColorToolStripMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog colorDialog = new ColorPickerDialog();
            colorDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            colorDialog.Owner = this;
            SolidColorBrush brush = imageViewer.Background as SolidColorBrush;
            if (brush != null)
                colorDialog.StartingColor = brush.Color;
            else
                colorDialog.StartingColor = Colors.Black;
            if (colorDialog.ShowDialog() == true)
                imageViewer.Background = new SolidColorBrush(colorDialog.SelectedColor);
        }

        #endregion

    }
}
