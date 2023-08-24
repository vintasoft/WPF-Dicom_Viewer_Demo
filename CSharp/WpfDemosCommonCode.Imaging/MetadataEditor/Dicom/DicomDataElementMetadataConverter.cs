#if !REMOVE_DICOM_PLUGIN
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;
using Vintasoft.Imaging.Metadata;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Represent the metadata converter of DICOM data element.
    /// </summary>
    public class DicomDataElementMetadataConverter
    {

        #region Constants

        /// <summary>
        /// Maximum length of array, which can be edited.
        /// </summary>
        const int MAX_ARRAY_LENGTH = 64;

        #endregion



        #region Fields

        /// <summary>
        /// The DICOM data element.
        /// </summary>
        DicomDataElementMetadata _dataElement;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DicomDataElementMetadataConverter"/> class.
        /// </summary>
        /// <param name="dataElement">The DICOM data element.</param>
        public DicomDataElementMetadataConverter(DicomDataElementMetadata dataElement)
        {
            if (dataElement == null)
                throw new ArgumentNullException();

            _dataElement = dataElement;
        }

        #endregion



        #region Properties

        #region PUBLIC

        bool _canEdit = true;
        /// <summary>
        /// Gets or sets a value indicating whether DICOM metadata can be edited.
        /// </summary>
        /// <value>
        /// <b>True</b> if DICOM metadata can be edited; otherwise, <b>false</b>.
        /// </value>
        [Browsable(false)]
        public bool CanEdit
        {
            get
            {
                return _canEdit;
            }
            set
            {
                _canEdit = value;
            }
        }

        /// <summary> 
        /// Gets a value that indicating whether the metadata node can be copied
        /// to new metadata three.
        /// </summary>
        [Description("Indicates whether the metadata node can be copied to new metadata three.")]
        [Category("Metadata node")]
        public bool CanCopy
        {
            get
            {
                return _dataElement.CanCopy;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the node is read-only.
        /// </summary>
        /// <value>
        /// <b>true</b> - node is read-only;
        /// <b>false</b> - node is not read-only.
        /// </value>
        [Description("Indicates whether the node is read-only.")]
        [Category("Metadata node")]
        public bool IsReadOnly
        {
            get
            {
                return _dataElement.IsReadOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the node is changed.
        /// </summary>
        /// <value>
        /// <b>true</b> - node is changed;
        /// <b>false</b> - node is not changed.
        /// </value>
        [Description("Indicates whether the node is changed.")]
        [Category("Metadata node")]
        public bool IsChanged
        {
            get
            {
                return _dataElement.IsChanged;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the node is leaf of metadata tree.
        /// </summary>
        [Description("Indicates whether the node is leaf of metadata tree.")]
        [Category("Metadata node")]
        public bool IsLeafNode
        {
            get
            {
                return _dataElement.IsLeafNode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the node has value.
        /// </summary>
        [Description("Indicates whether the node has value.")]
        [Category("Metadata node")]
        public bool HasValue
        {
            get
            {
                return _dataElement.HasValue;
            }
        }

        /// <summary>
        /// Gets a value that indicating whether the metadata node can be deleted 
        /// from children collection of parent node.
        /// </summary>
        [Description("Indicates whether the metadata node can be deleted from children collection of parent node.")]
        [Category("Metadata node")]
        public bool CanDelete
        {
            get
            {
                return _dataElement.CanRemove;
            }
        }

        /// <summary>
        /// Gets a name of the node.
        /// </summary>
        [Description("Name of the node.")]
        [Category("Data element information")]
        public string Name
        {
            get
            {
                return _dataElement.Name;
            }
        }

        /// <summary>
        /// Gets the encoding name.
        /// </summary>
        [Description("Encoding name.")]
        [Category("Data element information")]
        public string EncodingName
        {
            get
            {
                return _dataElement.EncodingName;
            }
        }

        /// <summary>
        /// Gets the group number.
        /// </summary>
        [Category("Data element information")]
        [Description("Group number.")]
        public ushort GroupNumber
        {
            get
            {
                return _dataElement.GroupNumber;
            }
        }

        /// <summary>
        /// Gets the element number.
        /// </summary>
        [Category("Data element information")]
        [Description("Element number.")]
        public ushort ElementNumber
        {
            get
            {
                return _dataElement.ElementNumber;
            }
        }

        /// <summary>
        /// Gets the value representation.
        /// </summary>
        [Category("Data element information")]
        [Description("Value representation.")]
        public DicomValueRepresentation ValueRepresentation
        {
            get
            {
                return _dataElement.ValueRepresentation;
            }
        }

        /// <summary>
        /// Gets or sets the node value.
        /// </summary>
        [Description("Node value.")]
        [ReadOnly(true)]
        [Category("Data element information")]
        public object NodeValue
        {
            get
            {
                return _dataElement.Value;
            }
            set
            {
                _dataElement.Value = value;
            }
        }

        /// <summary>
        /// Gets the node value as string.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("Node value as string.")]
        [Category("Data element information")]
        public string ValueAsString
        {
            get
            {
                if (NodeValue != null)
                {
                    if (_dataElement.ValueRepresentation == DicomValueRepresentation.SQ)
                    {
                        return "Sequence";
                    }
                    else
                    {
                        if (NodeValue is Array)
                        {
                            Array array = (Array)NodeValue;
                            string dataString = string.Empty;

                            int length = Math.Min(array.Length, MAX_ARRAY_LENGTH);

                            if (length > 0)
                            {
                                for (int i = 0; i < length - 1; i++)
                                    dataString += ValueToString(ValueRepresentation, array.GetValue(i)) + ' ';
                                dataString += ValueToString(ValueRepresentation, array.GetValue(length - 1));

                                if (array.Length > MAX_ARRAY_LENGTH)
                                    dataString += " ...";
                            }
                            return dataString;
                        }
                        else
                            return ValueToString(ValueRepresentation, NodeValue);
                    }
                }

                return string.Empty;
            }
        }


        /// <summary>
        /// Gets or sets the node value as an array of strings.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [Description("Node value as array of string.")]
        [Category("Data element information")]
        public string[] ValueAsStringArray
        {
            get
            {
                string[] result = new string[0];

                if (!CanEditValue)
                {
                    string message = string.Format(
                        "This demo application can edit array only if array length does not exceed {0}.",
                        MAX_ARRAY_LENGTH);
                    result = new string[] { message };
                }
                else if (NodeValue != null && ValueRepresentation != DicomValueRepresentation.SQ)
                {
                    if (NodeValue is Array)
                    {
                        Array array = (Array)NodeValue;
                        result = new string[array.Length];

                        for (int i = 0; i < array.Length; i++)
                            result[i] = ValueToString(ValueRepresentation, array.GetValue(i));
                    }
                    else
                        result = new string[] { ValueToString(ValueRepresentation, NodeValue) };
                }

                return result;
            }
            set
            {
                if (CanEdit)
                {
                    if (!CanEditValue || ValueRepresentation == DicomValueRepresentation.SQ)
                        return;

                    if (_dataElement.IsReadOnly)
                        throw new Exception("This node is read-only.");

                    try
                    {
                        Array newValue = value;

                        switch (ValueRepresentation)
                        {
                            case DicomValueRepresentation.DA:
                                List<DateTime> dateTimeList = new List<DateTime>();
                                for (int i = 0; i < value.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(value[i]))
                                        dateTimeList.Add(DateTime.Parse(value[i]));
                                }
                                newValue = dateTimeList.ToArray();
                                break;

                            case DicomValueRepresentation.DT:
                                List<DicomDateTime> dicomDateTimeList = new List<DicomDateTime>();
                                for (int i = 0; i < value.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(value[i]))
                                    {
                                        string[] splitedValue = value[i].Split(' ');

                                        if (splitedValue.Length != 2 && splitedValue.Length != 3)
                                            throw new FormatException("Invalid DicomDateTime format.");

                                        DateTime utcDateTime = DateTime.Parse(splitedValue[0] + " " + splitedValue[1]);

                                        TimeSpan? utcOffset = null;
                                        if (splitedValue.Length == 3)
                                        {
                                            bool isNegative = splitedValue[2].StartsWith("-");

                                            utcOffset = TimeSpan.Parse(splitedValue[2].TrimStart('+', '-'));

                                            if (isNegative)
                                                utcOffset = utcOffset.Value.Negate();
                                        }

                                        dicomDateTimeList.Add(new DicomDateTime(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc), utcOffset));
                                    }
                                }
                                newValue = dicomDateTimeList.ToArray();
                                break;

                            case DicomValueRepresentation.TM:
                                List<TimeSpan> timeSpanList = new List<TimeSpan>();
                                for (int i = 0; i < value.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(value[i]))
                                        timeSpanList.Add(TimeSpan.Parse(value[i]));
                                }
                                newValue = timeSpanList.ToArray();
                                break;

                            case DicomValueRepresentation.UI:
                                List<DicomUid> uidList = new List<DicomUid>();
                                for (int i = 0; i < value.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(value[i]))
                                        uidList.Add(new DicomUid(value[i]));
                                }
                                newValue = uidList.ToArray();
                                break;
                        }
                        NodeValue = (object)newValue;
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Gets a value indicating whether the DICOM data element value can be edited.
        /// </summary>
        private bool CanEditValue
        {
            get
            {
                if (NodeValue is Array)
                {
                    Array array = (Array)NodeValue;
                    // if array is large
                    if (array.Length > MAX_ARRAY_LENGTH)
                        return false;
                }
                return true;
            }
        }

        #endregion

        #endregion



        #region Methods

        /// <summary>
        /// Converts the DICOM data element value to a string.
        /// </summary>
        /// <param name="valueRepresentation">The DICOM data element value type.</param>
        /// <param name="value">The DICOM data element value.</param>
        /// <returns>The DICOM data element value as a string.</returns>
        private string ValueToString(DicomValueRepresentation valueRepresentation, object value)
        {
            switch (valueRepresentation)
            {
                case DicomValueRepresentation.DA:
                    DateTime dateTime = (DateTime)value;
                    return dateTime.ToShortDateString();

                case DicomValueRepresentation.UI:
                    DicomUid uid = (DicomUid)value;
                    return uid.Value;

                case DicomValueRepresentation.DT:
                    DicomDateTime dicomDateTime = (DicomDateTime)value;
                    string result = dicomDateTime.UtcDateTime.ToShortDateString() + " " + dicomDateTime.UtcDateTime.ToLongTimeString();
                    if (dicomDateTime.OffsetFromUtc.HasValue)
                    {
                        TimeSpan offsetFromUtc = dicomDateTime.OffsetFromUtc.Value;

                        if (offsetFromUtc.Ticks < 0)
                        {
                            result += " -";
                            offsetFromUtc = offsetFromUtc.Negate();
                        }
                        else
                        {
                            result += " +";
                        }
                        result += offsetFromUtc.ToString();
                    }
                    return result;
            }

            return value.ToString();
        }

        #endregion

    }
}
#endif