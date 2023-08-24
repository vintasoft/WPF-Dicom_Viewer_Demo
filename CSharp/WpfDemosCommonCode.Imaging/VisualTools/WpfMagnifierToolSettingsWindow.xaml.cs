using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.ImageProcessing.Color;
using Vintasoft.Imaging.ImageProcessing.Effects;

using Vintasoft.Imaging.Wpf.UI.VisualTools;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to edit magnifier tool settings.
    /// </summary>
    public partial class WpfMagnifierToolSettingsWindow : Window
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfMagnifierToolSettingsWindow"/> class.
        /// </summary>
        public WpfMagnifierToolSettingsWindow()
        {
            InitializeComponent();
        }

        #endregion



        #region Properties

        WpfMagnifierTool _magnifier;
        /// <summary>
        /// Gets or sets magnifier tool.
        /// </summary>
        [Description("Magnifier tool.")]
        public WpfMagnifierTool Magnifier
        {
            get
            {
                return _magnifier;
            }
            set
            {
                _magnifier = value;
                UpdateUI();
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Updates the user interface of this form.
        /// </summary>
        private void UpdateUI()
        {
            // set values of size numeric up-downs
            widthNumericUpDown.Value = Magnifier.Size.Width;
            heightNumericUpDown.Value = Magnifier.Size.Height;

            // set value of zoom numeric up-down
            zoomNumericUpDown.Value = Magnifier.Zoom;

            // display pen settings
            if (Magnifier.BorderPenWidth > 0)
            {
                borderColorPanelControl.Color = Magnifier.BorderPenColor;
                borderWidthNumericUpDown.Value = Magnifier.BorderPenWidth;
            }
            else
            {
                borderColorPanelControl.Color = Colors.Transparent;
                borderWidthNumericUpDown.Value = 0;
            }

            // set elliptical outline check box value
            ellipticalOutlineCheckBox.IsChecked = Magnifier.UseEllipticalOutline;

            // set processing commands check boxes to false
            invertCheckBox.IsChecked = false;
            posterizeCheckBox.IsChecked = false;
            oilPaintingCheckBox.IsChecked = false;
            grayscaleCheckBox.IsChecked = false;

            // if the processing command is CompositeCommand
            if (Magnifier.ProcessingCommand is CompositeCommand)
            {
                CompositeCommand compositeCommand = (CompositeCommand)Magnifier.ProcessingCommand;

                // get sub commands
                ProcessingCommandBase[] commands = compositeCommand.GetCommands();

                // set processing commands check boxes
                for (int i = 0; i < commands.Length; i++)
                {
                    if (commands[i] is InvertCommand)
                        invertCheckBox.IsChecked = true;
                    else if (commands[i] is PosterizeCommand)
                        posterizeCheckBox.IsChecked = true;
                    else if (commands[i] is OilPaintingCommand)
                        oilPaintingCheckBox.IsChecked = true;
                    else if (commands[i] is ChangePixelFormatToGrayscaleCommand)
                        grayscaleCheckBox.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// Sets settings to the magnifier tool.
        /// </summary>
        private void SetSettings()
        {
            // set magnifier size
            Magnifier.Size = new Size((int)widthNumericUpDown.Value, (int)heightNumericUpDown.Value);

            // set magnifier zoom
            Magnifier.Zoom = (int)zoomNumericUpDown.Value;

            // if selected width of magnifier pen is more than 0
            if (borderWidthNumericUpDown.Value > 0)
            {
                System.Windows.Media.Color color = borderColorPanelControl.Color;

                // set magnifier settings
                Magnifier.BorderPenWidth = borderWidthNumericUpDown.Value;
                Magnifier.BorderPenColor = borderColorPanelControl.Color;
            }
            else
            {
                Magnifier.BorderPenWidth = 0;
                Magnifier.BorderPenColor = Colors.Transparent;
            }

            // set magnifier UseEllipticalOutline property
            Magnifier.UseEllipticalOutline = ellipticalOutlineCheckBox.IsChecked.Value;

            // create the list of selected processing commands
            List<ProcessingCommandBase> commands = new List<ProcessingCommandBase>();
            if (invertCheckBox.IsChecked.Value)
                commands.Add(new InvertCommand());
            if (posterizeCheckBox.IsChecked.Value)
                commands.Add(new PosterizeCommand());
            if (oilPaintingCheckBox.IsChecked.Value)
                commands.Add(new OilPaintingCommand());
            if (grayscaleCheckBox.IsChecked.Value)
                commands.Add(new ChangePixelFormatToGrayscaleCommand());

            // set the magnifier processing command
            if (commands.Count > 0)
                Magnifier.ProcessingCommand = new CompositeCommand(commands.ToArray());
            else
                Magnifier.ProcessingCommand = null;
        }

        /// <summary>
        /// "OK" button is clicked.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            SetSettings();
            Close();
        }

        /// <summary>
        /// "Cancel" button is clicked.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

    }
}
