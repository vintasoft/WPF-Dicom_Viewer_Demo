using System.Collections.Generic;

#if !REMOVE_ANNOTATION_PLUGIN
using Vintasoft.Imaging.Annotation.Dicom.Wpf.UI.VisualTools; 
#endif
using Vintasoft.Imaging.Wpf.UI.VisualTools;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Interaction logic for DicomAnnotatedViewerToolBar.xaml
    /// </summary>
    public partial class DicomAnnotatedViewerToolBar : VisualToolsToolBar
    {

        #region Fields

        /// <summary>
        /// Additional visual tools.
        /// </summary>
        List<WpfVisualTool> _additionalVisualTools = new List<WpfVisualTool>();

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DicomAnnotatedViewerToolBar"/> class.
        /// </summary>
        public DicomAnnotatedViewerToolBar()
        {
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets the mandatory visual tool.
        /// </summary>
        /// <value>
        /// Default value is <b>null</b>.
        /// </value>
        /// <remarks>
        /// The mandatory visual tool is always active because is always used in composition with selected visual tool.
        /// </remarks>
        public override WpfVisualTool MandatoryVisualTool
        {
            get
            {
                return base.MandatoryVisualTool;
            }
            set
            {
            }
        }

        WpfCompositeVisualTool _mainVisualTool = null;
        /// <summary>
        /// Gets or sets the main visual tool.
        /// </summary>
        /// <value>
        /// Default value is <b>null</b>.
        /// </value>
        public WpfCompositeVisualTool MainVisualTool
        {
            get
            {
                return _mainVisualTool;
            }
        }

#if REMOVE_ANNOTATION_PLUGIN
        Vintasoft.Imaging.Dicom.Wpf.UI.VisualTools.WpfDicomViewerTool _dicomAnnotatedViewerTool = null;
        /// <summary>
        /// Gets or sets the <see cref="DicomAnnotatedViewerTool"/>.
        /// </summary>
        /// <value>
        /// Default value is <b></b>.
        /// </value>
        public Vintasoft.Imaging.Dicom.Wpf.UI.VisualTools.WpfDicomViewerTool DicomAnnotatedViewerTool
        {
            get
            {
                return _dicomAnnotatedViewerTool;
            }
            set
            {
                if (_dicomAnnotatedViewerTool != value)
                {
                    _dicomAnnotatedViewerTool = value;

                    if (_mainVisualTool == null)
                    {
                        _mainVisualTool = _dicomAnnotatedViewerTool;
                    }
                    else
                    {
                        // update main visual tool
                        List<WpfVisualTool> tools = new List<WpfVisualTool>(_additionalVisualTools);
                        tools.Add(_dicomAnnotatedViewerTool);
                        _mainVisualTool = new WpfCompositeVisualTool(tools.ToArray());
                    }
                }
            }
        }
#else
        WpfDicomAnnotatedViewerTool _dicomAnnotatedViewerTool = null;
        /// <summary>
        /// Gets or sets the <see cref="DicomAnnotatedViewerTool"/>.
        /// </summary>
        /// <value>
        /// Default value is <b></b>.
        /// </value>
        public WpfDicomAnnotatedViewerTool DicomAnnotatedViewerTool
        {
            get
            {
                return _dicomAnnotatedViewerTool;
            }
            set
            {
                if (_dicomAnnotatedViewerTool != value)
                {
                    _dicomAnnotatedViewerTool = value;

                    if (_mainVisualTool == null)
                    {
                        _mainVisualTool = _dicomAnnotatedViewerTool;
                    }
                    else
                    {
                        // update main visual tool
                        List<WpfVisualTool> tools = new List<WpfVisualTool>(_additionalVisualTools);
                        tools.Add(_dicomAnnotatedViewerTool);
                        _mainVisualTool = new WpfCompositeVisualTool(tools.ToArray());
                    }
                }
            }
        } 
#endif

        #endregion



        #region Methods

        /// <summary>
        /// Returns the visual tool for the specified visual tool action.
        /// </summary>
        /// <param name="visualToolAction">The visual tool action.</param>
        /// <returns>
        /// The visual tool.
        /// </returns>
        protected override WpfVisualTool GetVisualTool(VisualToolAction visualToolAction)
        {
            return _mainVisualTool;
        }

        /// <summary>
        /// Adds the visual tool action to this tool strip 
        /// and adds visual tool to <see cref="MainVisualTool"/>.
        /// </summary>
        /// <param name="visualToolAction">Additional visual tool action.</param>
        /// <exception cref="System.Exception"> Thrown if <see cref="MainVisualTool"/> was <b>null</b>.</exception>
        public void AddVisualToolAction(VisualToolAction visualToolAction)
        {
            if (_mainVisualTool == null)
                throw new System.Exception("Add DicomAnnotatedViewerTool first.");

            // add action to tool strip
            base.AddAction(visualToolAction);

            _additionalVisualTools.Add(visualToolAction.VisualTool);

            // update main visual tool
            List<WpfVisualTool> tools = new List<WpfVisualTool>(_additionalVisualTools);
            tools.Add(_dicomAnnotatedViewerTool);
            _mainVisualTool = new WpfCompositeVisualTool(tools.ToArray());
        }

        #endregion

    }
}
