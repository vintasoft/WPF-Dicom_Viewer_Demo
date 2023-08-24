using System;
using System.Collections.Generic;
using System.IO;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;


namespace WpfDicomViewerDemo
{
    /// <summary>
    /// Manages the opened series of DICOM files.
    /// </summary>
    public class DicomSeriesController
    {

        #region Fields

        /// <summary>
        /// Dictionary: DICOM file => stream.
        /// </summary>
        Dictionary<DicomFile, Stream> _fileToStream = new Dictionary<DicomFile, Stream>();

        /// <summary>
        /// Dictionary: DICOM file => file path.
        /// </summary>
        Dictionary<DicomFile, string> _fileToPath = new Dictionary<DicomFile, string>();

        /// <summary>
        /// Dictionary: image of DICOM file => DICOM file.
        /// </summary>
        Dictionary<VintasoftImage, DicomFile> _frameToFile =
            new Dictionary<VintasoftImage, DicomFile>();

        /// <summary>
        /// Dictionary: DICOM file => instance number in series.
        /// </summary>
        Dictionary<DicomFile, int> _fileToInstanceNumber = new Dictionary<DicomFile, int>();

        /// <summary>
        /// Instance UID of DICOM file.
        /// </summary>
        DicomUid _studyInstanceUid = null;

        /// <summary>
        /// Series UID of DICOM file.
        /// </summary>
        DicomUid _seriesInstanceUid = null;

        /// <summary>
        /// Number of DICOM file series.
        /// </summary>
        int _seriesNumber = 0;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DicomSeriesController"/> class.
        /// </summary>
        public DicomSeriesController()
        {
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets the count of DICOM files in series.
        /// </summary>
        public int FileCount
        {
            get { return _fileToPath.Count; }
        }

        /// <summary>
        /// Gets the count of images in series.
        /// </summary>
        public int ImageCount
        {
            get { return _frameToFile.Count; }
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Returns the file path of specified DICOM file.
        /// </summary>
        /// <param name="dicomFile">The DICOM file.</param>
        /// <returns>
        /// The file path of DICOM file.
        /// </returns>
        public string GetDicomFilePath(DicomFile dicomFile)
        {
            string result = string.Empty;
            _fileToPath.TryGetValue(dicomFile, out result);
            return result;
        }

        /// <summary>
        /// Returns the DICOM file of specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>
        /// The DICOM file of image.
        /// </returns>
        public DicomFile GetDicomFile(VintasoftImage image)
        {
            DicomFile result = null;
            _frameToFile.TryGetValue(image, out result);
            return result;
        }

        /// <summary>
        /// Returns the DICOM file instance number of specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>
        /// The DICOM file instance number.
        /// </returns>
        public int GetDicomFileInstanceNumber(VintasoftImage image)
        {
            // get DICOM file of image
            DicomFile dicomFile = GetDicomFile(image);

            int result = 0;
            _fileToInstanceNumber.TryGetValue(dicomFile, out result);
            return result;
        }

        /// <summary>
        /// Determines whether the series already contains the specified DICOM file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// <b>true</b> if the series already contains the specified DICOM file;
        /// otherwise, <b>false</b>.
        /// </returns>
        public bool Contains(string filePath)
        {
            return _fileToPath.ContainsValue(filePath);
        }

        /// <summary>
        /// Returns the images of specified DICOM file.
        /// </summary>
        /// <param name="dicomFile">The DICOM file.</param>
        /// <returns>
        /// The images of DICOM file.
        /// </returns>
        public VintasoftImage[] GetImages(DicomFile dicomFile)
        {
            List<VintasoftImage> images = new List<VintasoftImage>();

            foreach (VintasoftImage image in _frameToFile.Keys)
            {
                if (_frameToFile[image] == dicomFile)
                    images.Add(image);
            }

            return images.ToArray();
        }

        /// <summary>
        /// Adds the DICOM file to the series.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="dicomFile">The DICOM file.</param>
        /// <param name="dicomFileInstanceNumber">The instance number of DICOM file.</param>
        /// <returns>
        /// The images of DICOM file.
        /// </returns>
        public ImageCollection AddDicomFileToSeries(
            string filePath,
            out DicomFile dicomFile,
            out int dicomFileInstanceNumber)
        {
            dicomFileInstanceNumber = 0;

            // open the file stream
            Stream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            // create DICOM file
            dicomFile = DicomFileController.OpenDicomFile(sourceStream);

            // save information about file
            _fileToStream.Add(dicomFile, sourceStream);
            _fileToPath.Add(dicomFile, filePath);

            // if DICOM file does not contain images
            if (dicomFile.Pages.Count == 0)
                return null;

            // check that DICOM file belongs the current series
            CheckThatDicomFileBelongsSeries(dicomFile);

            // get the instance number element from relationship group
            DicomDataElement dataElement = dicomFile.DataSet.DataElements.Find(DicomDataElementId.InstanceNumber);

            // if the instance number element exists
            if (dataElement != null && dataElement.Data != null)
            {
                // get instance number of DICOM file
                dicomFileInstanceNumber = Convert.ToInt32(dataElement.Data);
                // save information about instance number
                _fileToInstanceNumber.Add(dicomFile, dicomFileInstanceNumber);
            }

            // create collection
            ImageCollection collection = new ImageCollection();
            // load images to the collection 
            collection.Add(sourceStream, false);

            // save information about images
            foreach (VintasoftImage image in collection)
                _frameToFile.Add(image, dicomFile);

            return collection;
        }

        /// <summary>
        /// Closes the current series.
        /// </summary>
        public void CloseSeries()
        {
            DicomFile[] dicomFiles = GetFilesOfSeries();

            foreach (DicomFile dicomFile in dicomFiles)
            {
                CloseDicomFile(dicomFile);
            }

            _studyInstanceUid = null;
            _seriesInstanceUid = null;
        }

        /// <summary>
        /// Closes the DICOM file of series.
        /// </summary>
        /// <param name="dicomFile">The DICOM file.</param>
        public void CloseDicomFile(DicomFile dicomFile)
        {
            // if DICOM file is not specified
            if (dicomFile == null)
                return;

            // close DICOM file
            DicomFileController.CloseDicomFile(dicomFile);

            // get images of the DICOM file
            VintasoftImage[] images = GetImages(dicomFile);
            // remove information about images
            foreach (VintasoftImage image in images)
                _frameToFile.Remove(image);

            // get stream of the DICOM file
            Stream fileStream = _fileToStream[dicomFile];
            // close the DICOM file stream
            fileStream.Close();
            fileStream = null;

            // remove information about DICOM file from dictionaries
            _fileToPath.Remove(dicomFile);
            _fileToStream.Remove(dicomFile);
            _fileToInstanceNumber.Remove(dicomFile);
        }

        /// <summary>
        /// Returns the DICOM files of current series.
        /// </summary>
        /// <returns>
        /// The DICOM files of current series.
        /// </returns>
        public DicomFile[] GetFilesOfSeries()
        {
            DicomFile[] dicomFiles = new DicomFile[_fileToStream.Count];
            _fileToStream.Keys.CopyTo(dicomFiles, 0);
            return dicomFiles;
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Checks that DICOM file can be added to the current series.
        /// </summary>
        /// <param name="dicomFile">The DICOM file.</param>
        /// <exception cref="InvalidOperationException">Thrown if
        /// DICOM file does not belong current series.
        /// </exception>
        private void CheckThatDicomFileBelongsSeries(DicomFile dicomFile)
        {
            // the study of DICOM file
            DicomUid studyInstanceUid = null;
            // the series of DICOM file
            DicomUid seriesInstanceUid = null;
            // the series number of DICOM file
            int seriesNumber;

            // get the study UID, series UID and series number of DICOM file
            GetCurrentSeriesInfo(dicomFile, out studyInstanceUid, out seriesInstanceUid, out seriesNumber);

            // if DICOM file is the first file from series
            if (_studyInstanceUid == null && _seriesInstanceUid == null)
            {
                // save series information

                _studyInstanceUid = studyInstanceUid;
                _seriesInstanceUid = seriesInstanceUid;
                _seriesNumber = seriesNumber;
            }
            // if DICOM file is NOT the first file from series
            else
            {
                // if study UID is not equal to the DICOM file study UID
                if (!_studyInstanceUid.Value.Equals(studyInstanceUid.Value))
                {
                    string message = "The file can not be added to the series because the file does not belong current study.";
                    throw new InvalidOperationException(message);
                }

                // is series UID is not equal to the DICOM file series UID
                if (!_seriesInstanceUid.Value.Equals(seriesInstanceUid.Value) ||
                    _seriesNumber != seriesNumber)
                {
                    string message = "The file can not be added to the series because the file does not belong current series.";
                    throw new InvalidOperationException(message);
                }
            }
        }

        /// <summary>
        /// Returns study and series of DICOM file.
        /// </summary>
        /// <param name="dicomFile">The DICOM file.</param>
        /// <param name="studyInstanceUid">The study UID of DICOM file.</param>
        /// <param name="seriesInstanceUid">The series UID of DICOM file.</param>
        /// <param name="seriesNumber">The series number of DICOM file.</param>
        private void GetCurrentSeriesInfo(
            DicomFile dicomFile,
            out DicomUid studyInstanceUid,
            out DicomUid seriesInstanceUid,
            out int seriesNumber)
        {
            // create "empty" UID
            studyInstanceUid = new DicomUid("0.0.0.0.0");
            // create "empty" UID
            seriesInstanceUid = new DicomUid("0.0.0.0.0");
            // set empty number
            seriesNumber = 0;

            // get StudyInstanceUID data element from relationship group
            DicomDataElement studyInstanceDataElement =
                dicomFile.DataSet.DataElements.Find(DicomDataElementId.StudyInstanceUID);
            // if StudyInstanceUID data element exists
            if (studyInstanceDataElement != null)
                // get study of DICOM file
                studyInstanceUid = (DicomUid)studyInstanceDataElement.Data;


            // get SeriesInstanceUID data element from relationship group
            DicomDataElement seriesInstanceDataElement =
                dicomFile.DataSet.DataElements.Find(DicomDataElementId.SeriesInstanceUID);
            // if SeriesInstanceUID data element exists
            if (seriesInstanceDataElement != null)
                // get series of DICOM file
                seriesInstanceUid = (DicomUid)seriesInstanceDataElement.Data;


            // get SeriesNumber data element from relationship group
            DicomDataElement seriesNumberDataElement =
                dicomFile.DataSet.DataElements.Find(DicomDataElementId.SeriesNumber);
            // if SeriesNumber data element exists
            if (seriesNumberDataElement != null)
                // get the series number of DICOM file
                seriesNumber = Convert.ToInt32(seriesNumberDataElement.Data);
        }

        #endregion

        #endregion

    }
}
