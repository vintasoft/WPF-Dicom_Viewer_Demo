using System.Windows.Media.Imaging;

#if !REMOVE_ANNOTATION_PLUGIN
using Vintasoft.Imaging.Annotation.Wpf.UI.Measurements;
#endif

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Creates visual tool action, which allows to enable/disable visual tool <see cref="WpfImageMeasureTool"/> in image viewer, and adds action to the toolstrip.
    /// </summary>
    public class MeasurementVisualToolActionFactory
    {

        #region Methods

        #region PUBLIC

        /// <summary>
        /// Creates visual tool action, which allows to enable/disable visual tool <see cref="WpfImageMeasureTool"/> in image viewer, and adds action to the toolstrip.
        /// </summary>
        /// <param name="toolBar">The toolbar, where actions must be added.</param>
        public static void CreateActions(VisualToolsToolBar toolBar)
        {
#if !REMOVE_ANNOTATION_PLUGIN
            // create action, which allows to measure objects on image in image viewer
            ImageMeasureToolAction imageMeasureToolAction = new ImageMeasureToolAction(
                 new WpfImageMeasureTool(),
                 "Image Measure Tool",
                 "Image Measure Tool",
                 GetIcon("WpfImageMeasureTool.png"));
            // add the action to the toolstrip
            toolBar.AddAction(imageMeasureToolAction);
#endif
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Returns the visual tool icon of specified name.
        /// </summary>
        /// <param name="iconName">The visual tool icon name.</param>
        /// <returns>
        /// The visual tool icon.
        /// </returns>
        private static BitmapSource GetIcon(string iconName)
        {
            string iconPath =
                string.Format("WpfDemosCommonCode.Imaging.VisualToolsToolBar.VisualTools.MeasurementVisualTools.Resources.{0}", iconName);

            return DemosResourcesManager.GetResourceAsBitmap(iconPath);
        }

        #endregion

        #endregion

    }
}