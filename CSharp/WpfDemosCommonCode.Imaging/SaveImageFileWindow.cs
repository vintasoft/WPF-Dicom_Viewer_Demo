using Microsoft.Win32;

using System;
using System.Windows;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs.Encoders;

using WpfDemosCommonCode.Imaging.Codecs;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Allows to show a file save dialog and save an image to a file.
    /// </summary>
    public class SaveImageFileWindow
    {

        #region Methods

        /// <summary>
        /// Shows a file save dialog and saves the specified image to the selected file.
        /// </summary>
        /// <param name="image">The image, which must be saved.</param>
        /// <param name="encoderFactory">The encoder factory.</param>
        /// <returns>The dialog result.</returns>
        public static bool SaveImageToFile(
            VintasoftImage image,
            ImagingEncoderFactory encoderFactory)
        {
            if (image == null)
                return true;

            SaveFileDialog saveFileDlg = new SaveFileDialog();
            CodecsFileFilters.SetFilters(saveFileDlg, false);
            if (saveFileDlg.ShowDialog() == false)
                return false;

            EncoderBase encoder = null;
            if (encoderFactory.GetEncoder(saveFileDlg.FileName, out encoder))
            {
                try
                {
                    image.Save(saveFileDlg.FileName, encoder);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Shows a file save dialog and saves the specified images to the selected file.
        /// </summary>
        /// <param name="images">The images, which must be saved.</param>
        /// <param name="encoderFactory">The encoder factory.</param>
        /// <returns>The dialog result.</returns>
        public static bool SaveImagesToFile(
            ImageCollection images,
            ImagingEncoderFactory encoderFactory)
        {
            if (images.Count == 0)
                return true;
            if (images.Count == 1)
                return SaveImageToFile(images[0], encoderFactory);

            SaveFileDialog saveFileDlg = new SaveFileDialog();
            CodecsFileFilters.SetFilters(saveFileDlg, true);
            if (saveFileDlg.ShowDialog() == false)
                return false;

            MultipageEncoderBase encoder = null;
            if (encoderFactory.GetMultipageEncoder(saveFileDlg.FileName, out encoder))
            {
                try
                {
                    images.SaveSync(saveFileDlg.FileName, encoder);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                return true;
            }
            return false;
        }

        #endregion

    }
}
