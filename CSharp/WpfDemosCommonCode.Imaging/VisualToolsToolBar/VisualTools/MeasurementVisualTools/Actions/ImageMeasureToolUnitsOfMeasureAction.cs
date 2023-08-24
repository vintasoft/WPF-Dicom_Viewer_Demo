using System.Windows.Media.Imaging;

using Vintasoft.Imaging;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Stores information about a <see cref="UnitOfMeasure"/> action.
    /// </summary>
    public class ImageMeasureToolUnitsOfMeasureAction : VisualToolAction
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageMeasureToolUnitsOfMeasureAction"/> class.
        /// </summary>
        /// <param name="unitsOfMeasure">The unit of measure for all annotations of measure tool.</param>
        /// <param name="text">The action text.</param>
        /// <param name="toolTip">The action tool tip.</param>
        /// <param name="icon">The action icon.</param>
        /// <param name="subActions">The sub-actions of the action.</param>
        public ImageMeasureToolUnitsOfMeasureAction(
            UnitOfMeasure unitsOfMeasure,
            string text,
            string toolTip,
            BitmapSource icon,
            params VisualToolAction[] subActions)
            : base(null, text, toolTip, icon, subActions)
        {
            _unitsOfMeasure = unitsOfMeasure;
        }

        #endregion



        #region Properties

        UnitOfMeasure _unitsOfMeasure;
        /// <summary>
        /// Gets the unit of measure for all annotation of measure tool.
        /// </summary>
        public UnitOfMeasure UnitsOfMeasure
        {
            get
            {
                return _unitsOfMeasure;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this action can change the visual tool in viewer.
        /// </summary>
        /// <value>
        /// <b>true</b> if this action can change the visual tool in viewer; otherwise, <b>false</b>.
        /// </value>
        public override bool CanChangeImageViewerVisualTool
        {
            get
            {
                return false;
            }
        }

        #endregion

    }
}
