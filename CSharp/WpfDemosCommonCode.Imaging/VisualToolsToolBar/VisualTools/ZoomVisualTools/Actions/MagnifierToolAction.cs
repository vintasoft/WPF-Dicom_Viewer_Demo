using System.Windows;
using System.Windows.Media.Imaging;

using Vintasoft.Imaging.Wpf.UI.VisualTools;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Stores information about a <see cref="WpfMagnifierTool"/> action.
    /// </summary>
    public class MagnifierToolAction : VisualToolAction
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MagnifierToolAction"/> class.
        /// </summary>
        /// <param name="visualTool">The visual tool.</param>
        /// <param name="text">The action text.</param>
        /// <param name="toolTip">The action tool tip.</param>
        /// <param name="icon">The action icon.</param>
        /// <param name="subActions">The sub-actions of the action.</param>
        public MagnifierToolAction(
            WpfMagnifierTool visualTool,
            string text,
            string toolTip,
            BitmapSource icon,
            params VisualToolAction[] subActions)
            : base(visualTool, text, toolTip, icon, subActions)
        {
        }

        #endregion



        #region Methods

        /// <summary>
        /// Shows the visual tool settings.
        /// </summary>
        public override void ShowVisualToolSettings()
        {
            WpfMagnifierToolSettingsWindow dlg = new WpfMagnifierToolSettingsWindow();
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Owner = Window.GetWindow(VisualTool);
            dlg.Magnifier = (WpfMagnifierTool)VisualTool;
            dlg.ShowDialog();
        }

        #endregion

    }
}
