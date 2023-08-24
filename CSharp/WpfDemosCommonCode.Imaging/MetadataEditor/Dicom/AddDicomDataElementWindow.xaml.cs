using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

#if !REMOVE_DICOM_PLUGIN
using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;
#endif
using Vintasoft.Imaging.Metadata;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Represents a form that allows to add a DICOM data element to a DICOM data set of DICOM file.
    /// </summary>
    public partial class AddDicomDataElementWindow : Window
    {

        #region Fields

#if !REMOVE_DICOM_PLUGIN
        /// <summary>
        /// The metadata of DICOM file.
        /// </summary>
        MetadataNode _metadata = null;
#endif

        #endregion



        #region Constructors

#if !REMOVE_DICOM_PLUGIN
        /// <summary>
        /// Initializes a new instance of the <see cref="AddDicomDataElementForm"/> class.
        /// </summary>
        /// <param name="metadata">The metadata of DICOM file.</param>
        public AddDicomDataElementWindow(MetadataNode metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException();

            InitializeComponent();


            // init combo-box with supported DICOM value types

            valueRepresentationComboBox.BeginInit();

            DicomValueRepresentation[] array = (DicomValueRepresentation[])Enum.GetValues(typeof(DicomValueRepresentation));
            Array.Sort(array, new Comparison<DicomValueRepresentation>(ValueRepresentationComparer));
            foreach (DicomValueRepresentation vr in array)
            {
                if (vr == DicomValueRepresentation.UN)
                    continue;
                valueRepresentationComboBox.Items.Add(vr);
            }
            valueRepresentationComboBox.SelectedItem = DicomValueRepresentation.UT;
            valueRepresentationComboBox.EndInit();

            _metadata = metadata;
        }
#endif

        #endregion



        #region Methods

        /// <summary>
        /// Value type is changed.
        /// </summary>
        private void valueRepresentationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if !REMOVE_DICOM_PLUGIN
            if (valueRepresentationComboBox.SelectedItem == null)
                return;

            // get DICOM value type from combo-box
            DicomValueRepresentation vr = (DicomValueRepresentation)valueRepresentationComboBox.SelectedItem;


            // reset the user interface

            valueLabel.Visibility = Visibility.Visible;
            valueLabel.Content = "Value";

            valueTextBox.Visibility = Visibility.Hidden;
            valueTextBox.Text = string.Empty;
            valueTextBox.MaxLength = int.MaxValue;

            valueDatePickerPanel.Visibility = Visibility.Collapsed;
            valueDatePicker.Value = DateTime.Now;

            valueTimePickerPanel.Visibility = Visibility.Collapsed;
            valueTimePicker.Value = DateTime.Now;

            valueAgeComboBox.Visibility = Visibility.Collapsed;
            valueAgeComboBox.SelectedIndex = 0;

            valueAgeNumericUpDown.Visibility = Visibility.Collapsed;
            valueAgeNumericUpDown.Value = 0;


            // init the user interface

            switch (vr)
            {
                // Age String
                case DicomValueRepresentation.AS:
                    valueAgeNumericUpDown.Visibility = Visibility.Visible;
                    valueAgeComboBox.Visibility = Visibility.Visible;
                    break;

                // Date
                case DicomValueRepresentation.DA:
                    valueDatePickerPanel.Visibility = Visibility.Visible;
                    break;

                // Date Time
                case DicomValueRepresentation.DT:
                    valueDatePickerPanel.Visibility = Visibility.Visible;
                    valueTimePickerPanel.Visibility = Visibility.Visible;
                    break;

                // Sequence of Items
                case DicomValueRepresentation.SQ:
                    valueLabel.Visibility = Visibility.Hidden;
                    break;

                // Time
                case DicomValueRepresentation.TM:
                    valueTimePickerPanel.Visibility = Visibility.Visible;
                    break;

                default:
                    valueTextBox.Visibility = Visibility.Visible;
                    valueLabel.Content = "Enter string values (value per line)";
                    break;

            }
#endif
        }

        /// <summary>
        /// Key is pressed in text box.
        /// </summary>
        private void valueTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
#if !REMOVE_DICOM_PLUGIN
            if (string.IsNullOrEmpty(e.Text))
                return;

            char keyChar = e.Text[0];

            // get selected type of value
            DicomValueRepresentation valueRepresentation =
                (DicomValueRepresentation)valueRepresentationComboBox.SelectedItem;

            // key is a digit?
            bool isDigit = char.IsDigit(keyChar);
            // key is a letter?
            bool isLetter = char.IsLetter(keyChar);
            // key is negative char
            bool isNegativeChar = false;
            if (keyChar == CultureInfo.CurrentCulture.NumberFormat.NegativeSign[0])
                isNegativeChar = true;
            // key is a separator of decimal?
            bool isDecimalSeparatorChar = false;
            if (keyChar == CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0])
                isDecimalSeparatorChar = true;

            switch (valueRepresentation)
            {
                // Application Entity
                case DicomValueRepresentation.AE:
                    if (!isDigit && !isLetter && keyChar != '\\')
                        e.Handled = true;
                    break;

                // Code String
                case DicomValueRepresentation.CS:
                    if (!isDigit && !isLetter && keyChar != '_' && keyChar != ' ')
                        e.Handled = true;
                    break;

                // Attribute Tag
                case DicomValueRepresentation.AT:
                // Unsigned Long
                case DicomValueRepresentation.UL:
                // Unsigned Short
                case DicomValueRepresentation.US:
                    if (!isDigit)
                        e.Handled = true;
                    break;

                // Integer String
                case DicomValueRepresentation.IS:
                // Signed Long
                case DicomValueRepresentation.SL:
                // Signed Short
                case DicomValueRepresentation.SS:
                    if (!isDigit && !isNegativeChar)
                        e.Handled = true;
                    break;

                // DecimalString
                case DicomValueRepresentation.DS:
                // Floating PointSingle
                case DicomValueRepresentation.FL:
                // Floating Point Double
                case DicomValueRepresentation.FD:
                // Other Double String
                case DicomValueRepresentation.OD:
                // Other Float String
                case DicomValueRepresentation.OF:
                    if (!isDigit && !isNegativeChar && !isDecimalSeparatorChar)
                        e.Handled = true;
                    break;

                // Unique Identifier(UID)
                case DicomValueRepresentation.UI:
                    if (!isDigit && keyChar != '.')
                        e.Handled = true;
                    break;
            }
#endif
        }

        /// <summary>
        /// "OK" button is clicked.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
#if !REMOVE_DICOM_PLUGIN
            // group numer
            ushort groupNumber = (ushort)groupNumberNumericUpDown.Value;
            // element number
            ushort elementNumber = (ushort)elementNumberNumericUpDown.Value;
            // value type
            DicomValueRepresentation valueRepresentation = (DicomValueRepresentation)valueRepresentationComboBox.SelectedItem;
            // DataElement value
            object value = null;

            try
            {
                switch (valueRepresentation)
                {
                    // Unique Identifier(UID)
                    case DicomValueRepresentation.UI:
                        List<DicomUid> uidList = new List<DicomUid>();
                        for (int lineIndex = 0; lineIndex < valueTextBox.LineCount; lineIndex++)
                        {
                            string line = valueTextBox.GetLineText(lineIndex);
                            if (!string.IsNullOrEmpty(line))
                            {
                                DicomUid uid = new DicomUid(line);
                                uidList.Add(uid);
                            }
                        }
                        value = uidList.ToArray();
                        break;

                    // Age String
                    case DicomValueRepresentation.AS:
                        string ageString = valueAgeNumericUpDown.Value.ToString();
                        string selectedItem = ((ComboBoxItem)valueAgeComboBox.SelectedItem).Content.ToString();
                        value = ageString + selectedItem[0];
                        break;

                    // Date
                    case DicomValueRepresentation.DA:
                        value = valueDatePicker.Value;
                        break;

                    // Date Time
                    case DicomValueRepresentation.DT:
                        DateTime date = valueDatePicker.Value;
                        DateTime time = valueTimePicker.Value;

                        DateTime dateTime = new DateTime(
                            date.Year, date.Month, date.Day,
                            time.Hour, time.Minute, time.Second);

                        value = new DicomDateTime(
                            TimeZone.CurrentTimeZone.ToUniversalTime(dateTime),
                            TimeZone.CurrentTimeZone.GetUtcOffset(dateTime));
                        break;

                    // Sequence of Items
                    case DicomValueRepresentation.SQ:
                        break;

                    // Time
                    case DicomValueRepresentation.TM:
                        value = valueTimePicker.Value.Subtract(valueTimePicker.Value.Date);
                        break;

                    default:
                        string[] lines = new string[valueTextBox.LineCount];
                        for (int lineIndex = 0; lineIndex < valueTextBox.LineCount; lineIndex++)
                            lines[lineIndex] = valueTextBox.GetLineText(lineIndex);
                        value = lines;
                        break;
                }

                // if type of value is sequence
                if (valueRepresentation == DicomValueRepresentation.SQ)
                {
                    DicomDataSetMetadata collectionMetadata = _metadata as DicomDataSetMetadata;
                    if (collectionMetadata != null)
                        // sequence not contains value
                        collectionMetadata.AddChild(groupNumber, elementNumber, DicomValueRepresentation.SQ);
                    else
                    {
                        DicomFrameMetadata frameMetadata = _metadata as DicomFrameMetadata;
                        if (frameMetadata != null)
                            frameMetadata.AddChild(groupNumber, elementNumber, DicomValueRepresentation.SQ);
                    }
                }
                else
                {
                    DicomDataSetMetadata collectionMetadata = _metadata as DicomDataSetMetadata;
                    if (collectionMetadata != null)
                        // sequence not contains value
                        collectionMetadata.AddChild(groupNumber, elementNumber, valueRepresentation, value);
                    else
                    {
                        DicomFrameMetadata frameMetadata = _metadata as DicomFrameMetadata;
                        if (frameMetadata != null)
                            frameMetadata.AddChild(groupNumber, elementNumber, valueRepresentation, value);
                    }
                }

                DialogResult = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
#endif
        }

        /// <summary>
        /// "Cancel" button is clicked.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

#if !REMOVE_DICOM_PLUGIN
        /// <summary>
        /// Array comparer.
        /// </summary>
        private int ValueRepresentationComparer(DicomValueRepresentation vr1, DicomValueRepresentation vr2)
        {
            return string.Compare(vr1.ToString(), vr2.ToString());
        }

#endif

        #endregion

    }
}
