using System;
using System.Windows;
using System.Windows.Controls;

using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;


namespace WpfDicomViewerDemo
{
    /// <summary>
    /// A window that allows to show and edit information about Presentation State file.
    /// </summary>
    public partial class PresentationStateInfoWindow : Window
    {

        #region Fields

        /// <summary>
        /// The Presentation State File.
        /// </summary>
        DicomFile _presentationStateFile = null;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationStateInfoWindow"/> class.
        /// </summary>
        public PresentationStateInfoWindow(DicomFile presentationStateFile)
        {
            InitializeComponent();

            if (presentationStateFile == null)
                throw new ArgumentNullException("presentationStateFile");

            _presentationStateFile = presentationStateFile;
            instanceNumberNumericUpDown.Minimum = int.MinValue;
            instanceNumberNumericUpDown.Maximum = int.MaxValue;

            DicomDataSet dataSet = presentationStateFile.DataSet;

            // find instance number of file
            DicomDataElement instanceNumberDataElement =
                dataSet.DataElements.Find(DicomDataElementId.InstanceNumber);
            if (instanceNumberDataElement != null)
                // get number of instance
                instanceNumberNumericUpDown.Value = Convert.ToInt32(instanceNumberDataElement.Data);

            // update all text boxes

            SetTextBox(presentationCreationDateTextBox, dataSet, DicomDataElementId.PresentationCreationDate);
            SetTextBox(presentationCreationTimeTextBox, dataSet, DicomDataElementId.PresentationCreationTime);
            SetTextBox(presentationCreatorTextBox, dataSet, DicomDataElementId.ContentCreatorName);
            SetTextBox(presentationDescriptionTextBox, dataSet, DicomDataElementId.ContentDescription);
            SetTextBox(presentationLabelTextBox, dataSet, DicomDataElementId.ContentLabel);
        }

        #endregion



        #region Methods

        /// <summary>
        /// "Ok" button is clicked.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DicomDataSet dataSet = _presentationStateFile.DataSet;

            // find instance number of file
            DicomDataElement instanceNumberDataElement = dataSet.DataElements.Find(DicomDataElementId.InstanceNumber);
            if (instanceNumberDataElement != null)
                // set number of instance
                instanceNumberDataElement.Data = instanceNumberNumericUpDown.Value;

            SetDataElement(dataSet, 0x0070, 0x0084, presentationCreatorTextBox);
            SetDataElement(dataSet, 0x0070, 0x0081, presentationDescriptionTextBox);
            SetDataElement(dataSet, 0x0070, 0x0080, presentationLabelTextBox);

            DialogResult = true;
        }

        /// <summary>
        /// "Cancel" button is clicked.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Sets text box, which shows information about DICOM data element value.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="dicomDataSet">The DICOM data set.</param>
        /// <param name="dicomDataElementId">The DICOM data element identifier.</param>
        private void SetTextBox(
            TextBox textBox,
            DicomDataSet dicomDataSet,
            DicomDataElementId dicomDataElementId)
        {
            // find data element of file
            DicomDataElement dataElement = dicomDataSet.DataElements.Find(dicomDataElementId);
            string text = string.Empty;
            // if data element exist
            if (dataElement != null && dataElement.Data != null)
            {
                switch (dataElement.ValueRepresentation)
                {
                    case DicomValueRepresentation.DA:
                        DateTime date = (DateTime)dataElement.Data;
                        text = date.ToShortDateString();
                        break;

                    case DicomValueRepresentation.TM:
                        TimeSpan time = (TimeSpan)dataElement.Data;
                        text = string.Format("{0}:{1}", time.Hours, time.Minutes);
                        break;

                    default:
                        text = dataElement.Data.ToString();
                        break;
                }
            }
            textBox.Text = text;
        }

        /// <summary>
        /// Sets DICOM data element value from text box.
        /// </summary>
        /// <param name="dataSet">The DICOM data set.</param>
        /// <param name="groupNumber">The DICOM group number.</param>
        /// <param name="elementNubmer">The DICOM data element number.</param>
        /// <param name="textBox">The text box.</param>
        private void SetDataElement(
            DicomDataSet dataSet,
            ushort groupNumber,
            ushort elementNumber,
            TextBox textBox)
        {
            // find data element of file
            DicomDataElement dataElement = dataSet.DataElements.Find(groupNumber, elementNumber);
            // if data element not exist
            if (dataElement == null && !string.IsNullOrEmpty(textBox.Text))
                // create data element
                dataElement = dataSet.DataElements.Add(groupNumber, elementNumber);

            // if data element exist
            if (dataElement != null)
                // set data of data element
                dataElement.Data = textBox.Text;
        }

        #endregion

    }
}
