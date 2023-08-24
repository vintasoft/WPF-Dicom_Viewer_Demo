using System.Windows.Media.Imaging;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Stores information about an empty visual tool action.
    /// </summary>
    public class NoneAction : VisualToolAction
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NoneAction"/> class.
        /// </summary>
        /// <param name="text">The action text.</param>
        /// <param name="toolTip">The action tool tip.</param>
        /// <param name="icon">The action icon.</param>
        /// <param name="subactions">The sub-actions of the action.</param>
        public NoneAction(
            string text,
            string toolTip,
            BitmapSource icon,
            params VisualToolAction[] subactions)
            : base(null, text, toolTip, icon, subactions)
        {
        }

    }
}
