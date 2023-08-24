using System.Collections.Generic;

using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;


namespace WpfDicomViewerDemo
{
    /// <summary>
    /// Manages opened Presentation State Files.
    /// </summary>
    public class PresentationStateFileController
    {

        #region Fields

        /// <summary>
        /// Dictionary: DICOM image file => DICOM presentation state file.
        /// </summary>
        static Dictionary<DicomFile, DicomFile> _imageFileToPreFile =
            new Dictionary<DicomFile, DicomFile>();

        #endregion



        #region Methods

        /// <summary>
        /// Loads the presentation state file for specified DICOM image file.
        /// </summary>
        /// <param name="imageFile">The DICOM image file.</param>
        /// <param name="presentationStateFilePath">The path to the DICOM presentation state file.</param>
        public static DicomFile LoadPresentationStateFile(
            DicomFile imageFile,
            string presentationStateFilePath)
        {
            // open presentation state file
            DicomFile presentationStateFile = new DicomFile(presentationStateFilePath, false);

            // save reference
            _imageFileToPreFile.Add(imageFile, presentationStateFile);

            return presentationStateFile;
        }

        /// <summary>
        /// Returns the presentation state file, which is associates with the specified DICOM image file.
        /// </summary>
        /// <param name="imageFile">The DICOM image file.</param>
        /// <returns>
        /// The DICOM presentation state file.
        /// </returns>
        public static DicomFile GetPresentationStateFile(DicomFile imageFile)
        {
            DicomFile result = null;
            _imageFileToPreFile.TryGetValue(imageFile, out result);
            return result;
        }

        /// <summary>
        /// Updates the DICOM presentation state file of specified DICOM image file.
        /// </summary>
        /// <param name="imageFile">The DICOM image file.</param>
        /// <param name="presentationStateFile">The DICOM presentation state file.</param>
        public static void UpdatePresentationStateFile(
            DicomFile imageFile,
            DicomFile presentationStateFile)
        {
            // close the DICOM presentation state file
            ClosePresentationStateFile(imageFile);
            // save reference
            _imageFileToPreFile.Add(imageFile, presentationStateFile);
        }

        /// <summary>
        /// Closes the DICOM presentation state file of specified DICOM image file.
        /// </summary>
        /// <param name="imageFile">The DICOM image file.</param>
        public static void ClosePresentationStateFile(DicomFile imageFile)
        {
            // get the DICOM presentation state file for DICOM image file
            DicomFile presentationStateFile = GetPresentationStateFile(imageFile);

            // if DICOM presetation state file is found
            if (presentationStateFile != null)
            {
                // release all resources of DICOM presetation state file
                presentationStateFile.Dispose();
                // remove reference
                _imageFileToPreFile.Remove(imageFile);
            }
        }

        #endregion

    }
}
