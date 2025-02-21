using System;
using System.Windows;

using Vintasoft.Imaging.Wpf.UI.VisualTools.UserInteraction;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to edit InteractionAreaAppearanceManager settings.
    /// </summary>
    public partial class WpfInteractionAreaAppearanceManagerWindow : Window
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfInteractionAreaAppearanceManagerWindow"/> class.
        /// </summary>
        public WpfInteractionAreaAppearanceManagerWindow()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfInteractionAreaAppearanceManagerWindow"/> class.
        /// </summary>
        /// <param name="selectedTabIndex">The selected tab page index.</param>
        public WpfInteractionAreaAppearanceManagerWindow(int selectedTabIndex)
        {
            InitializeComponent();

            InteractionAreaSettings = new WpfInteractionAreaAppearanceManager();
            tabControl.IsEnabled = false;
            okButton.IsEnabled = tabControl.IsEnabled;

            tabControl.SelectedIndex = Math.Min(Math.Max(0, selectedTabIndex),
                tabControl.Items.Count - 1);
        }

        #endregion



        #region Properties

        WpfInteractionAreaAppearanceManager _interactionAreaSettings = null;
        /// <summary>
        /// Gets or sets the interaction area settings.
        /// </summary>
        public WpfInteractionAreaAppearanceManager InteractionAreaSettings
        {
            get
            {
                return _interactionAreaSettings;
            }
            set
            {
                tabControl.IsEnabled = value != null;
                okButton.IsEnabled = tabControl.IsEnabled;
                _interactionAreaSettings = value;

                UpdateUI();
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// "OK" button is clicked.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            // initialization is started
            _interactionAreaSettings.BeginInit();
            try
            {
                // resize points
                _interactionAreaSettings.ResizePointsRadius = resizePointsRadiusNumericUpDown.Value;
                _interactionAreaSettings.ResizePointsInteractionRadius = resizePointsInteractionRadiusNumericUpDown.Value;
                _interactionAreaSettings.ResizePointsBackgroundColor = resizePointsBackgroundColorPanelControl.Color;
                _interactionAreaSettings.ResizePointsBorderColor = resizePointsBorderColorPanelControl.Color;
                _interactionAreaSettings.ResizePointsBorderPenWidth = resizePointsBorderPenWidthNumericUpDown.Value;
                _interactionAreaSettings.NorthwestSoutheastResizePointCursor = resizePointsNwseCursorPanelControl.SelectedCursor;
                _interactionAreaSettings.NortheastSouthwestResizePointCursor = resizePointsNeswCursorPanelControl.SelectedCursor;
                _interactionAreaSettings.NorthSouthResizePointCursor = resizePointsNsCursorPanelControl.SelectedCursor;
                _interactionAreaSettings.WestEastResizePointCursor = resizePointsWeCursorPanelControl.SelectedCursor;

                // polygon point
                _interactionAreaSettings.PolygonPointRadius = polygonPointRadiusNumericUpDown.Value;
                _interactionAreaSettings.PolygonPointInteractionRadius = polygonPointInteractionRadiusNumericUpDown.Value;
                _interactionAreaSettings.PolygonPointBackgroundColor = polygonPointBackgroundColorPanelControl.Color;
                _interactionAreaSettings.SelectedPolygonPointBackgroundColor = selectedPointBackgroundColorPanelControl.Color;
                _interactionAreaSettings.PolygonPointBorderColor = polygonPointBorderColorPanelControl.Color;
                _interactionAreaSettings.PolygonPointBorderPenWidth = polygonPointBorderPenWidthNumericUpDown.Value;
                _interactionAreaSettings.PolygonPointCursor = polygonPointCursorPanelControl.SelectedCursor;

                // rotation point
                _interactionAreaSettings.RotationPointRadius = rotationPointRadiusNumericUpDown.Value;
                _interactionAreaSettings.RotationPointInteractionRadius = rotationPointInteractionRadiusNumericUpDown.Value;
                _interactionAreaSettings.RotationPointBackgroundColor = rotationPointBackgroundColorPanelControl.Color;
                _interactionAreaSettings.RotationPointBorderColor = rotationPointBorderColorPanelControl.Color;
                _interactionAreaSettings.RotationPointBorderPenWidth = rotationPointBorderPenWidthNumericUpDown.Value;
                _interactionAreaSettings.RotationPointDistance = rotationPointDistanceNumericUpDown.Value;
                _interactionAreaSettings.RotationPointCursor = rotationPointCursorPanelControl.SelectedCursor;

                // rotation assistant
                _interactionAreaSettings.RotationAssistantRadius = rotationAssistantRadiusNumericUpDown.Value;
                _interactionAreaSettings.RotationAssistantBackgroundColor = rotationAssistantBackgroundColorPanelControl.Color;
                _interactionAreaSettings.RotationAssistantBorderColor = rotationAssistantBorderColorPanelControl.Color;
                _interactionAreaSettings.RotationAssistantBorderPenWidth = rotationAssistantBorderPenWidthNumericUpDown.Value;
                _interactionAreaSettings.RotationAssistantDiscreteAngle = rotationAssistantDiscreteAngleNumericUpDown.Value;

                // text box
                _interactionAreaSettings.TextBoxFontFamily = fontFamilySelector.SelectedFamily;
                _interactionAreaSettings.TextBoxFontSize = textBoxFontSizeNumericUpDown.Value;
                _interactionAreaSettings.TextBoxForeColor = textBoxForeColorPanelControl.Color;
                _interactionAreaSettings.TextBoxBackColor = textBoxBackColorPanelControl.Color;
                _interactionAreaSettings.TextBoxCursor = textBoxCursorPanelControl.SelectedCursor;

                // move area
                _interactionAreaSettings.MoveAreaCursor = moveAreaCursorPanelControl.SelectedCursor;

                DialogResult = true;
            }
            finally
            {
                // initialization is finished
                _interactionAreaSettings.EndInit();
            }
        }

        /// <summary>
        /// "Cancel" button is clicked.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Updates the user interface of this form.
        /// </summary>
        private void UpdateUI()
        {
            WpfInteractionAreaAppearanceManager settings = _interactionAreaSettings;

            if (settings == null)
                return;

            // resize points
            resizePointsRadiusNumericUpDown.Value = Convert.ToInt32(settings.ResizePointsRadius);
            resizePointsInteractionRadiusNumericUpDown.Value = Convert.ToInt32(settings.ResizePointsInteractionRadius);
            resizePointsBackgroundColorPanelControl.Color = settings.ResizePointsBackgroundColor;
            resizePointsBorderColorPanelControl.Color = settings.ResizePointsBorderColor;
            resizePointsBorderPenWidthNumericUpDown.Value = Convert.ToInt32(settings.ResizePointsBorderPenWidth);
            resizePointsNwseCursorPanelControl.SelectedCursor = settings.NorthwestSoutheastResizePointCursor;
            resizePointsNeswCursorPanelControl.SelectedCursor = settings.NortheastSouthwestResizePointCursor;
            resizePointsNsCursorPanelControl.SelectedCursor = settings.NorthSouthResizePointCursor;
            resizePointsWeCursorPanelControl.SelectedCursor = settings.WestEastResizePointCursor;

            // polygon point
            polygonPointRadiusNumericUpDown.Value = Convert.ToInt32(settings.PolygonPointRadius);
            polygonPointInteractionRadiusNumericUpDown.Value = Convert.ToInt32(settings.PolygonPointInteractionRadius);
            polygonPointBackgroundColorPanelControl.Color = settings.PolygonPointBackgroundColor;
            polygonPointBorderColorPanelControl.Color = settings.PolygonPointBorderColor;
            polygonPointBorderPenWidthNumericUpDown.Value = Convert.ToInt32(settings.PolygonPointBorderPenWidth);
            selectedPointBackgroundColorPanelControl.Color = settings.SelectedPolygonPointBackgroundColor;
            polygonPointCursorPanelControl.SelectedCursor = settings.PolygonPointCursor;

            // rotation point
            rotationPointRadiusNumericUpDown.Value = Convert.ToInt32(settings.RotationPointRadius);
            rotationPointInteractionRadiusNumericUpDown.Value = Convert.ToInt32(settings.RotationPointInteractionRadius);
            rotationPointBackgroundColorPanelControl.Color = settings.RotationPointBackgroundColor;
            rotationPointBorderColorPanelControl.Color = settings.RotationPointBorderColor;
            rotationPointBorderPenWidthNumericUpDown.Value = Convert.ToInt32(settings.RotationPointBorderPenWidth);
            rotationPointDistanceNumericUpDown.Value = Convert.ToInt32(settings.RotationPointDistance);
            rotationPointCursorPanelControl.SelectedCursor = settings.RotationPointCursor;

            // rotation assistant
            rotationAssistantRadiusNumericUpDown.Value = Convert.ToInt32(settings.RotationAssistantRadius);
            rotationAssistantBackgroundColorPanelControl.Color = settings.RotationAssistantBackgroundColor;
            rotationAssistantBorderColorPanelControl.Color = settings.RotationAssistantBorderColor;
            rotationAssistantBorderPenWidthNumericUpDown.Value = Convert.ToInt32(settings.RotationAssistantBorderPenWidth);
            rotationAssistantDiscreteAngleNumericUpDown.Value = Convert.ToInt32(settings.RotationAssistantDiscreteAngle);

            // text box
            fontFamilySelector.SelectedFamily = settings.TextBoxFontFamily;
            textBoxFontSizeNumericUpDown.Value = Convert.ToInt32(settings.TextBoxFontSize);
            textBoxForeColorPanelControl.Color = settings.TextBoxForeColor;
            textBoxBackColorPanelControl.Color = settings.TextBoxBackColor;
            textBoxCursorPanelControl.SelectedCursor = settings.TextBoxCursor;

            // move area
            moveAreaCursorPanelControl.SelectedCursor = settings.MoveAreaCursor;
        }

        #endregion

    }
}
