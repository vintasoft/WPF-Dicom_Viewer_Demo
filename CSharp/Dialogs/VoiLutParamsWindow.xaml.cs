using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs.Decoders;
using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;
using Vintasoft.Imaging.Dicom.Wpf.UI.VisualTools;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.Metadata;


namespace WpfDicomViewerDemo
{
    /// <summary>
    /// A window that allows to specify VOI LUT (value of interest lookup table) based
    /// on window center and windows width.
    /// </summary>
    public partial class VoiLutParamsWindow : Window
    {

        #region Fields

        /// <summary>
        /// Visual tool of image viewer.
        /// </summary>
        WpfDicomViewerTool _visualTool = null;

        /// <summary>
        /// An array of default VOI LUTs.
        /// </summary>
        List<DicomImageVoiLookupTable> _defaultVoiLuts = null;

        /// <summary>
        /// Determines that DICOM image VOI LUT is changing.
        /// </summary>
        bool _isVoiLutChanging = false;

        /// <summary>
        /// The dictionary: VOI LUT search mode => VOI LUT.
        /// </summary>
        Dictionary<VoiLutSearchMode, DicomImageVoiLookupTable> _searchModeToVoiLut =
            new Dictionary<VoiLutSearchMode, DicomImageVoiLookupTable>();

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiLutParamsWindow"/> class.
        /// </summary>
        /// <param name="owner">Owner of this form.</param>
        /// <param name="visualTool">Visual tool.</param>
        public VoiLutParamsWindow(
            Window owner,
            WpfDicomViewerTool visualTool)
        {
            InitializeComponent();

            if (visualTool == null)
                throw new ArgumentNullException("visualTool");

            // set owner
            Owner = owner;

            _visualTool = visualTool;

            visualTool.DicomImageVoiLutChanged += new EventHandler<WpfVoiLutChangedEventArgs>(visualTool_DicomImageVoiLutChanged);

            windowWidthNumericUpDown.Maximum = int.MaxValue;

            windowCenterNumericUpDown.Minimum = int.MinValue;
            windowCenterNumericUpDown.Maximum = int.MaxValue;


            foreach (VoiLutSearchMode searchMode in Enum.GetValues(typeof(VoiLutSearchMode)))
                voiLutSearchMethodComboBox.Items.Add(searchMode);
            voiLutSearchMethodComboBox.SelectedItem = VoiLutSearchMode.Simple;

            UpdateUI();
        }

        #endregion



        #region Properties

        VintasoftImage _dicomFrame = null;
        /// <summary>
        /// Gets or sets the DICOM frame.
        /// </summary>
        internal VintasoftImage DicomFrame
        {
            get
            {
                return _dicomFrame;
            }
            set
            {
                if (!IsEnabled)
                    return;

                _dicomFrame = value;

                // update UI
                voiLutPanel.IsEnabled = _dicomFrame != null;

                _searchModeToVoiLut.Clear();

                // if DICOM frame is NULL
                if (_dicomFrame == null)
                {
                    voiLutsComboBox.SelectedIndex = -1;
                }
                else
                {
                    DicomFrameMetadata metadata = _dicomFrame.Metadata.MetadataTree as DicomFrameMetadata;
                    if (metadata != null)
                    {
                        // get information about VOI LUTs from DICOM frame
                        _defaultVoiLuts = new List<DicomImageVoiLookupTable>(metadata.AvailableVoiLuts);

                        for (int i = _defaultVoiLuts.Count - 1; i >= 0; i--)
                        {
                            // if VOI LUT is empty
                            if (_defaultVoiLuts[i].WindowCenter == 0 &&
                                _defaultVoiLuts[i].WindowWidth == 0)
                                _defaultVoiLuts.RemoveAt(i);
                        }

                        UpdateVoiLutsComboBox();

                        DicomImageVoiLookupTable voiLut = _visualTool.DicomImageVoiLut;

                        if (!double.IsNaN(voiLut.WindowCenter) && !double.IsNaN(voiLut.WindowWidth))
                        {
                            if (voiLut.WindowCenter != 0 || voiLut.WindowWidth != 0)
                                voiLutsComboBox.SelectedIndex = GetVoiLut(voiLut.WindowCenter, voiLut.WindowWidth);

                            windowCenterNumericUpDown.Value = (int)voiLut.WindowCenter;
                            windowWidthNumericUpDown.Value = (int)voiLut.WindowWidth;
                        }
                    }
                }

                UpdateUI();
            }
        }

        #endregion



        #region Methods

        #region PROTECTED

        /// <summary>
        /// Form is closing.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            _visualTool.DicomImageVoiLutChanged -= visualTool_DicomImageVoiLutChanged;

            base.OnClosed(e);
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Updates the user interface of this form.
        /// </summary>
        private void UpdateUI()
        {
            VoiLutSearchMode voiLutSearchMode = (VoiLutSearchMode)voiLutSearchMethodComboBox.SelectedItem;

            if (_searchModeToVoiLut.ContainsKey(voiLutSearchMode))
                calculateVoiLutButton.IsEnabled = false;
            else
                calculateVoiLutButton.IsEnabled = true;
        }

        /// <summary>
        /// Sets VOI LUT in DICOM viewer tool.
        /// </summary>
        /// <param name="windowCenter">Window center of DICOM frame.</param>
        /// <param name="windowWidth">Window width of DICOM frame.</param>
        private void SetVoiLutInDicomViewerTool(double windowCenter, double windowWidth)
        {
            _visualTool.DicomImageVoiLut = new DicomImageVoiLookupTable(windowCenter, windowWidth);

            // change an index in a list of VOI LUTS
            voiLutsComboBox.SelectedIndex = GetVoiLut(windowCenter, windowWidth);
        }

        /// <summary>
        /// Gets index of VOI LUT if possible.
        /// </summary>
        private int GetVoiLut(double windowCenter, double windowWidth)
        {
            for (int i = 0; i < _defaultVoiLuts.Count; i++)
            {
                if (windowCenter == _defaultVoiLuts[i].WindowCenter &&
                    windowWidth == _defaultVoiLuts[i].WindowWidth)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Selected VOI LUT is changed.
        /// </summary>
        private void voiLutsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isVoiLutChanging)
                return;

            int index = voiLutsComboBox.SelectedIndex;
            if (index == -1)
                return;

            // get selected VOI LUT
            DicomImageVoiLookupTable voiLut = _defaultVoiLuts[index];

            // set new VOI LUT in DICOM viewer tool
            SetVoiLutInDicomViewerTool(voiLut.WindowCenter, voiLut.WindowWidth);
            windowCenterNumericUpDown.Value = (int)voiLut.WindowCenter;
            windowWidthNumericUpDown.Value = (int)voiLut.WindowWidth;
        }

        /// <summary>
        /// Value of window center is changed.
        /// </summary>
        private void windowCenterNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_isVoiLutChanging)
                SetVoiLutInDicomViewerTool(
                    (double)windowCenterNumericUpDown.Value,
                    _visualTool.DicomImageVoiLut.WindowWidth);

        }

        /// <summary>
        /// Value of window width is changed.
        /// </summary>
        private void windowWidthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_isVoiLutChanging)
                SetVoiLutInDicomViewerTool(
                    _visualTool.DicomImageVoiLut.WindowCenter,
                    (double)windowWidthNumericUpDown.Value);
        }

        /// <summary>
        /// VOI LUT is changed.
        /// </summary>
        private void visualTool_DicomImageVoiLutChanged(object sender, WpfVoiLutChangedEventArgs e)
        {
            _isVoiLutChanging = true;
            windowCenterNumericUpDown.Value = (int)e.WindowCenter;
            windowWidthNumericUpDown.Value = (int)e.WindowWidth;
            voiLutsComboBox.SelectedIndex = GetVoiLut(e.WindowCenter, e.WindowWidth);
            _isVoiLutChanging = false;
        }

        /// <summary>
        /// Calculates the VOI LUT of DICOM frame
        /// if VOI LUT is not specified in DICOM file.
        /// </summary>
        private void calculateVoiLutButton_Click(object sender, RoutedEventArgs e)
        {
            DicomDecoder dicomDecoder = _dicomFrame.SourceInfo.Decoder as DicomDecoder;

            if (dicomDecoder == null)
                return;

            DicomFrameMetadata metadata = _dicomFrame.Metadata.MetadataTree as DicomFrameMetadata;
            DicomDecodingSettings decodingSettings = new DicomDecodingSettings(false, false, DicomImagePixelFormat.Source);
            RenderingSettings renderingSettings = new RenderingSettings(ImagingEnvironment.ScreenResolution);
            using (VintasoftImage rawImage = dicomDecoder.GetImage(_dicomFrame.SourceInfo.PageIndex, decodingSettings, renderingSettings))
            {
                GetDefaultVoiLutCommand command = new GetDefaultVoiLutCommand();
                command.VoiLutSearchMode = (VoiLutSearchMode)voiLutSearchMethodComboBox.SelectedItem;
                command.ExecuteInPlace(rawImage);

                DicomImageVoiLookupTable resultVoiLut = command.ResultVoiLut;

                DicomImageVoiLookupTable voiLut = new DicomImageVoiLookupTable(
                    resultVoiLut.WindowCenter, resultVoiLut.WindowWidth,
                    resultVoiLut.FunctionType,
                    string.Format("Calculated {0}", command.VoiLutSearchMode));

                _defaultVoiLuts.Add(voiLut);
                _searchModeToVoiLut.Add(command.VoiLutSearchMode, voiLut);

                UpdateVoiLutsComboBox();

                SetVoiLutInDicomViewerTool(voiLut.WindowCenter, voiLut.WindowWidth);

                UpdateUI();
            }
        }

        /// <summary>
        /// Changes the VOI LUT search method.
        /// </summary>
        private void VoiLutSearchMethodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }

        /// <summary>
        /// Updates the VOI LUT combobox.
        /// </summary>
        private void UpdateVoiLutsComboBox()
        {
            voiLutsComboBox.BeginInit();
            // clear the combo box with information about VOI LUTs
            voiLutsComboBox.Items.Clear();

            // for each VOI LUT
            for (int i = 0; i < _defaultVoiLuts.Count; i++)
            {
                // get information about VOI LUT
                string explanation = _defaultVoiLuts[i].Explanation;
                // if information about VOI LUT is empty
                if (explanation == string.Empty)
                    // create standard information about VOI LUT
                    explanation = "VOI LUT " + (i + 1).ToString();
                // add information about VOI LUT to a combo box
                voiLutsComboBox.Items.Add(explanation);
            }

            voiLutsComboBox.EndInit();
        } 

        #endregion

        #endregion
    }
}
