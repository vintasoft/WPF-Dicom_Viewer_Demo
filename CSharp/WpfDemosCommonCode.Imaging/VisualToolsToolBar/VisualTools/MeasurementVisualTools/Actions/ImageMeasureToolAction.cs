#if !REMOVE_ANNOTATION_PLUGIN
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Annotation;
using Vintasoft.Imaging.Annotation.Formatters;
using Vintasoft.Imaging.Annotation.Measurements;
using Vintasoft.Imaging.Annotation.Wpf.UI;
using Vintasoft.Imaging.Annotation.Wpf.UI.Measurements;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Stores information about a <see cref="WpfImageMeasureTool"/> action.
    /// </summary>
    public class ImageMeasureToolAction : VisualToolAction
    {

        #region Enums

        /// <summary>
        /// Specifies available measurement annotations.
        /// </summary>
        private enum MeasurementType
        {
            /// <summary>
            /// The line.
            /// </summary>
            Line,

            /// <summary>
            /// The lines.
            /// </summary>
            Lines,

            /// <summary>
            /// The ellipse.
            /// </summary>
            Ellipse,

            /// <summary>
            /// The angle.
            /// </summary>
            Angle
        }

        #endregion



        #region Fields

        /// <summary>
        /// The current activated measurement annotation action.
        /// </summary>
        VisualToolAction _currentActivatedMeasureTypeAction;

        /// <summary>
        /// The dictionary from measurement annotation action to the measurement annotation type.
        /// </summary>
        Dictionary<VisualToolAction, MeasurementType> _measureTypeActionToMeasurementType;

        /// <summary>
        /// The action, which defines current unit of measure.
        /// </summary>
        ImageMeasureToolUnitsOfMeasureAction _currentUnitOfMeasureAction;

        /// <summary>
        /// Dictionary: unit of measure => unit of measure action.
        /// </summary>
        Dictionary<UnitOfMeasure, ImageMeasureToolUnitsOfMeasureAction> _unitOfMeasureToUnitOfMeasureAction;

        /// <summary>
        /// The value indicating whether this action is initialized.
        /// </summary>
        bool _isInitialized;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageMeasureToolAction"/> class.
        /// </summary>
        /// <param name="visualTool">The visual tool.</param>
        /// <param name="text">The action text.</param>
        /// <param name="toolTip">The action tool tip.</param>
        /// <param name="icon">The action icon.</param>
        /// <param name="subActions">The sub-actions of the action.</param>
        public ImageMeasureToolAction(
            WpfImageMeasureTool visualTool,
            string text,
            string toolTip,
            BitmapSource icon,
            params VisualToolAction[] subActions)
            : base(visualTool, text, toolTip, icon, subActions)
        {
            visualTool.UnitsOfMeasureChanged +=
                new EventHandler<PropertyChangedEventArgs<UnitOfMeasure>>(Tool_UnitsOfMeasureChanged);
            visualTool.MeasuringTextTemplateUpdating +=
                new EventHandler<MeasurementAnnotationDataEventArgs>(Tool_MeasuringTextTemplateUpdating);

            _isInitialized = true;
        }


        #endregion



        #region Methods

        #region PROTECTED

        /// <summary>
        /// Sets the sub actions.
        /// </summary>
        /// <param name="actions">The actions.</param>
        protected override void SetSubActions(VisualToolAction[] actions)
        {
            List<VisualToolAction> sourceActions = new List<VisualToolAction>(actions);

            // add the measurement annotation actions to the source actions
            AddMeasurementAnnotationActions(sourceActions);

            // add separator action
            sourceActions.Add(new SeparatorToolBarAction());

            // add measurement properties action to the source actions
            AddPropertiesActions(sourceActions);

            // add separator action
            sourceActions.Add(new SeparatorToolBarAction());

            // add load and save measurement annotation actions to the source actions
            AddLoadAndSaveActions(sourceActions);

            // add separator action
            sourceActions.Add(new SeparatorToolBarAction());

            // add refresh action to the source actions
            AddRefreshActions(sourceActions);

            // set sub actions of this action
            base.SetSubActions(sourceActions.ToArray());
        }

        #endregion


        #region INTERNAL

        /// <summary>
        /// Action is clicked.
        /// </summary>
        internal override void Click()
        {
            base.Click();

            MeasurementType measurementType = _measureTypeActionToMeasurementType[_currentActivatedMeasureTypeAction];

            BeginMeasurement(measurementType);
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Begins the measurement using the specified measuremet annotation.
        /// </summary>
        /// <param name="type">The measurement annotation type.</param>
        private void BeginMeasurement(MeasurementType type)
        {
            WpfImageMeasureTool tool = (WpfImageMeasureTool)VisualTool;

            switch (type)
            {
                case MeasurementType.Line:
                    tool.BeginLineMeasurement();
                    break;

                case MeasurementType.Lines:
                    tool.BeginLinesMeasurement();
                    break;

                case MeasurementType.Ellipse:
                    tool.BeginEllipseMeasurement();
                    break;

                case MeasurementType.Angle:
                    tool.BeginAngleMeasurement();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }


        #region Measured text

        /// <summary>
        /// Units of measure is changed.
        /// </summary>
        private void Tool_UnitsOfMeasureChanged(object sender, PropertyChangedEventArgs<UnitOfMeasure> e)
        {
            WpfImageMeasureTool tool = (WpfImageMeasureTool)VisualTool;

            ImageMeasureToolUnitsOfMeasureAction unitOfMeasureAction =
                _unitOfMeasureToUnitOfMeasureAction[tool.UnitsOfMeasure];
            unitOfMeasureAction.Activate();

            // get the unit of measure as a string
            string unitOfMeasureString = GetUnitOfMeasureString(e.NewValue);
            // if image viewer is not empty
            if (tool.ImageViewer != null)
            {
                // for each image
                foreach (VintasoftImage image in tool.ImageViewer.Images)
                {
                    // get annotation list
                    IList<WpfAnnotationView> annotations = tool.GetAnnotationsFromImage(image);
                    // for each annotation
                    foreach (WpfAnnotationView annotation in annotations)
                    {
                        MeasurementAnnotationData data = annotation.Data as MeasurementAnnotationData;
                        // if annotation is measurement annotation
                        if (data != null)
                            // set new measuring text template
                            data.MeasuringTextTemplate = GetMeasuringTextTemplate(data, unitOfMeasureString);
                    }
                }
            }
        }

        /// <summary>
        /// Measuring text template is updating.
        /// </summary>
        private void Tool_MeasuringTextTemplateUpdating(object sender, MeasurementAnnotationDataEventArgs e)
        {
            string unitOfMeasureString = GetUnitOfMeasureString(e.MeasurementAnnotationData.MeasuringAnnotation.UnitsOfMeasure);
            e.MeasurementAnnotationData.MeasuringTextTemplate = GetMeasuringTextTemplate(e.MeasurementAnnotationData, unitOfMeasureString);
        }

        /// <summary>
        /// Returns a unit of measure string presentation.
        /// </summary>
        /// <param name="unitOfMeasure">A unit of measure.</param>
        /// <returns>Unit of measure string presentation</returns>
        private string GetUnitOfMeasureString(UnitOfMeasure unitOfMeasure)
        {
            switch (unitOfMeasure)
            {
                case UnitOfMeasure.Millimeters:
                    return "mm";

                case UnitOfMeasure.Centimeters:
                    return "cm";

                case UnitOfMeasure.Inches:
                    return "inch";

                case UnitOfMeasure.DeviceIndependentPixels:
                    return "dip";

                case UnitOfMeasure.Pixels:
                    return "px";

                case UnitOfMeasure.Points:
                    return "points";

                case UnitOfMeasure.Twips:
                    return "twip";

                case UnitOfMeasure.Emu:
                    return "emu";
            }

            return unitOfMeasure.ToString();
        }

        /// <summary>
        /// Returns a measuring text template.
        /// </summary>
        /// <param name="data">A measurement annotation data.</param>
        /// <param name="unitOfMeasureString">A unit of measure string.</param>
        /// <returns>Measuring text template.</returns>
        private string GetMeasuringTextTemplate(MeasurementAnnotationData data, string unitOfMeasureString)
        {
            // if data is angle measuring data
            if (data.MeasuringAnnotation is AngleMeasuringData)
            {
                return string.Format("Angle = {1}°\nReflex angle = {2}°\nLength = {3} {0} ",
                    unitOfMeasureString, "{Angle:f2}", "{ReflexAngle:f2}", "{Length:f2}");
            }

            // if data is line measuring data
            if (data.MeasuringAnnotation is LinearMeasuringData)
            {
                return "{Length:f2} " + unitOfMeasureString;
            }

            // if data is ellipse measuring data
            if (data.MeasuringAnnotation is EllipseMeasuringData)
            {
                return string.Format("S = {1} {0}^2\nDx = {2} {0}, Dy = {3} {0}",
                    unitOfMeasureString, "{Square:f2}", "{DiameterX:f2}", "{DiameterY:f2}");
            }

            // if type is unknown
            return data.MeasuringTextTemplate;
        }

        #endregion


        #region Start measurements

        /// <summary>
        /// Adds the measurement annotation actions to the specified action list.
        /// </summary>
        /// <param name="actions">The actions list.</param>
        private void AddMeasurementAnnotationActions(List<VisualToolAction> actions)
        {
            _measureTypeActionToMeasurementType = new Dictionary<VisualToolAction, MeasurementType>();

            VisualToolAction lineMeasureAction = new VisualToolAction(
                "Line Measure", "Starts the measurement using line", null, true);
            _measureTypeActionToMeasurementType.Add(lineMeasureAction, MeasurementType.Line);

            VisualToolAction linesMeasureAction = new VisualToolAction(
                "Lines Measure", "Starts the measurement using lines", null, true);
            _measureTypeActionToMeasurementType.Add(linesMeasureAction, MeasurementType.Lines);

            VisualToolAction ellipseMeasureAction = new VisualToolAction(
                "Ellipse Measure", "Starts the measurement using ellipse", null, true);
            _measureTypeActionToMeasurementType.Add(ellipseMeasureAction, MeasurementType.Ellipse);

            VisualToolAction angleMeasureAction = new VisualToolAction(
                "Angle Measure", "Starts the measurement using angle", null, true);
            _measureTypeActionToMeasurementType.Add(angleMeasureAction, MeasurementType.Angle);

            foreach (VisualToolAction action in _measureTypeActionToMeasurementType.Keys)
            {
                action.Activated += new EventHandler(measureTypeAction_Activated);
                action.Clicked += new EventHandler(measureTypeAction_Clicked);
                actions.Add(action);
            }

            lineMeasureAction.Activate();
        }

        /// <summary>
        /// Begins the measuremenet.
        /// </summary>
        private void measureTypeAction_Clicked(object sender, EventArgs e)
        {
            if (!_isInitialized)
                return;

            VisualToolAction action = (VisualToolAction)sender;

            MeasurementType measurementType = _measureTypeActionToMeasurementType[action];

            ImageMeasureToolAction measureToolAction = (ImageMeasureToolAction)action.Parent;

            if (!measureToolAction.IsActivated)
                measureToolAction.Activate();

            BeginMeasurement(measurementType);
        }

        /// <summary>
        /// The measurement type action is activated.
        /// </summary>
        private void measureTypeAction_Activated(object sender, EventArgs e)
        {
            VisualToolAction action = (VisualToolAction)sender;

            if (_currentActivatedMeasureTypeAction != null)
                _currentActivatedMeasureTypeAction.Deactivate();

            _currentActivatedMeasureTypeAction = action;
        }

        #endregion


        #region Properties

        /// <summary>
        /// Adds the properties actions to the specified action list.
        /// </summary>
        /// <param name="actions">The actions list.</param>
        private void AddPropertiesActions(List<VisualToolAction> actions)
        {
            List<ImageMeasureToolUnitsOfMeasureAction> unitsOfMeasureSubActions =
                new List<ImageMeasureToolUnitsOfMeasureAction>();
            WpfImageMeasureTool tool = (WpfImageMeasureTool)VisualTool;
            foreach (UnitOfMeasure unitOfMeasure in Enum.GetValues(typeof(UnitOfMeasure)))
            {
                ImageMeasureToolUnitsOfMeasureAction unitsOfMeasureAction =
                    new ImageMeasureToolUnitsOfMeasureAction(
                        unitOfMeasure,
                        string.Format("{0}", unitOfMeasure),
                        string.Format("{0} ({1})", unitOfMeasure, GetUnitOfMeasureString(unitOfMeasure)),
                        null);

                unitsOfMeasureAction.Activated += new EventHandler(unitOfMeasureAction_Activated);

                if (unitOfMeasure == tool.UnitsOfMeasure)
                    unitsOfMeasureAction.Activate();

                if (_unitOfMeasureToUnitOfMeasureAction == null)
                    _unitOfMeasureToUnitOfMeasureAction = new Dictionary<UnitOfMeasure, ImageMeasureToolUnitsOfMeasureAction>();
                _unitOfMeasureToUnitOfMeasureAction.Add(unitOfMeasure, unitsOfMeasureAction);
                unitsOfMeasureSubActions.Add(unitsOfMeasureAction);
            }

            actions.Add(new VisualToolAction("Units Of Measure",
                    "Units Of Measure", null, false,
                    unitsOfMeasureSubActions.ToArray()));


            VisualToolAction propertiesAction =
                new VisualToolAction("Properties...",
                    "Show properties form for image measure tool", null, false);
            propertiesAction.Clicked += new EventHandler(propertiesAction_Clicked);
            actions.Add(propertiesAction);


            VisualToolAction measurementPropertiesAction =
                new VisualToolAction("Measurement Properties...",
                    "Show properties form for focused measurement annotation", null, false);
            measurementPropertiesAction.Clicked += new EventHandler(measurementPropertiesAction_Clicked);
            actions.Add(measurementPropertiesAction);
        }

        /// <summary>
        /// Shows properties form for image measure tool.
        /// </summary>
        private void propertiesAction_Clicked(object sender, EventArgs e)
        {
            WpfImageMeasureTool visualTool = (WpfImageMeasureTool)VisualTool;

            PropertyGridWindow dlg = new PropertyGridWindow(visualTool, "Image measure tool settings");
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Owner = Window.GetWindow(visualTool);
            dlg.ShowDialog();
        }

        /// <summary>
        /// Shows properties form for focused measurement annotation.
        /// </summary>
        private void measurementPropertiesAction_Clicked(object sender, EventArgs e)
        {
            WpfImageMeasureTool visualTool = (WpfImageMeasureTool)VisualTool;

            if (visualTool.FocusedAnnotationView == null)
                return;

            PropertyGridWindow dlg = new PropertyGridWindow(
                visualTool.FocusedAnnotationView, "Measurement settings");
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Owner = Window.GetWindow(visualTool);
            dlg.ShowDialog();
        }

        /// <summary>
        /// The "unit of measure" action is activated.
        /// </summary>
        private void unitOfMeasureAction_Activated(object sender, EventArgs e)
        {
            ImageMeasureToolUnitsOfMeasureAction unitsOfMeasureAction =
                (ImageMeasureToolUnitsOfMeasureAction)sender;

            WpfImageMeasureTool tool = (WpfImageMeasureTool)VisualTool;

            tool.UnitsOfMeasure = unitsOfMeasureAction.UnitsOfMeasure;

            if (_currentUnitOfMeasureAction != null)
                _currentUnitOfMeasureAction.Deactivate();

            _currentUnitOfMeasureAction = unitsOfMeasureAction;
        }

        #endregion


        #region Load and save measurement annotations

        /// <summary>
        /// Adds the actions, which allows to load and save measurement annotations, to the specified action list.
        /// </summary>
        /// <param name="button">The button.</param>
        private void AddLoadAndSaveActions(List<VisualToolAction> actions)
        {
            VisualToolAction loadMeasurementsAction =
                new VisualToolAction("Load Measurements...",
                    "Load the measurement annotations from a file", null, false);
            loadMeasurementsAction.Clicked += new EventHandler(loadMeasurementsAction_Clicked);
            actions.Add(loadMeasurementsAction);

            VisualToolAction saveMeasurementsAction =
                new VisualToolAction("Save Measurements...",
                    " Save the measurement annotations to a file", null, false);
            saveMeasurementsAction.Clicked += new EventHandler(saveMeasurementsAction_Clicked);
            actions.Add(saveMeasurementsAction);
        }


        /// <summary>
        /// Loads the measurement annotations from a file.
        /// </summary>
        private void loadMeasurementsAction_Clicked(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.FileName = null;
                openFileDialog.Filter =
                    "Binary Annotations(*.vsabm)|*.vsabm|XMP Annotations(*.xmpm)|*.xmpm|All Formats(*.vsabm;*.xmpm)|*.vsabm;*.xmpm";
                openFileDialog.FilterIndex = 3;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                        {
                            WpfImageMeasureTool visualTool = (WpfImageMeasureTool)VisualTool;
                            // get the annotation collection
                            AnnotationDataCollection annotations = visualTool.AnnotationViewCollection.DataCollection;
                            // clear the annotation collection
                            annotations.ClearAndDisposeItems();
                            // add annotations from stream to the annotation collection
                            annotations.AddFromStream(fs, visualTool.ImageViewer.Image.Resolution);
                        }
                    }
                    catch (Exception ex)
                    {
                        DemosTools.ShowErrorMessage(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Saves the measurement annotations to a file.
        /// </summary>
        private void saveMeasurementsAction_Clicked(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = null;
                saveFileDialog.Filter = "Binary Annotations|*.vsabm|XMP Annotations|*.xmpm";
                saveFileDialog.FilterIndex = 1;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.ReadWrite))
                        {
                            AnnotationFormatter annotationFormatter = null;
                            if (saveFileDialog.FilterIndex == 1)
                                annotationFormatter = new AnnotationVintasoftBinaryFormatter();
                            else if (saveFileDialog.FilterIndex == 2)
                                annotationFormatter = new AnnotationVintasoftXmpFormatter();

                            WpfImageMeasureTool visualTool = (WpfImageMeasureTool)VisualTool;
                            AnnotationDataCollection annotations = visualTool.AnnotationViewCollection.DataCollection;

                            annotationFormatter.Serialize(fs, annotations);
                        }
                    }
                    catch (Exception ex)
                    {
                        DemosTools.ShowErrorMessage(ex);
                    }
                }
            }
        }

        #endregion


        #region Refresh

        /// <summary>
        /// Adds the 'refresh' action to the specified action list.
        /// </summary>
        /// <param name="button">The button.</param>
        private void AddRefreshActions(List<VisualToolAction> actions)
        {
            VisualToolAction refreshAction =
                new VisualToolAction("Refresh Measurements",
                      "Refresh all measurements of focused image", null, false);
            refreshAction.Clicked += new EventHandler(refreshAction_Clicked);
            actions.Add(refreshAction);
        }

        /// <summary>
        /// Refreshes all measurements of focused image.
        /// </summary>
        private void refreshAction_Clicked(object sender, EventArgs e)
        {
            WpfImageMeasureTool visualTool = (WpfImageMeasureTool)VisualTool;

            visualTool.InvalidateMeasuringTextValue();
        }

        #endregion

        #endregion

        #endregion

    }
}
#endif