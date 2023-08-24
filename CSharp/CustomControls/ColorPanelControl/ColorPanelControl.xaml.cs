using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Vintasoft.Imaging.Wpf;


namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// A panel that allows to show the selected color and change the selected color.
    /// </summary>
    [DefaultEvent("ColorChanged")]
    [DefaultProperty("Color")]
    public partial class ColorPanelControl : UserControl
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPanelControl"/> class.
        /// </summary>
        public ColorPanelControl()
        {
            InitializeComponent();

            colorPanel.Background = Brushes.Transparent;

            ColorButtonMargin = 3;
            DefaultColorButtonMargin = 3;

            CanSetColor = true;
            colorButton.ToolTip = "Click the button if the current color must be changed.";
            defaultColorButton.ToolTip = "Click the button if the current color must be set to the default color.";
        }
        #endregion



        #region Properties

        Color _color = Colors.Transparent;
        /// <summary>
        /// Gets or sets the current color.
        /// </summary>
        /// <value>
        /// Default value is <b>Color.Transparent</b>.
        /// </value>
        [Description("The current color.")]
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color != value)
                {
                    _color = value;

                    UpdateColorPanel();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current color can be changed.
        /// </summary>
        /// <value>
        /// <b>True</b> if the button, which allows to change the current color, is visible;
        /// otherwise, <b>false</b>.
        /// </value>
        [Description("Indicates that the current color can be changed.")]
        [DefaultValue(true)]
        public bool CanSetColor
        {
            get
            {
                return colorButton.Visibility == Visibility.Visible;
            }
            set
            {
                Visibility visibility = Visibility.Collapsed;
                Cursor cursor = this.Cursor;                
                string toolTip = null;
                if (value)
                {
                    visibility = Visibility.Visible;
                    toolTip = "Double click on the panel for changing the color.";
                    cursor = Cursors.Hand;
                }

                colorPanel.ToolTip = toolTip;
                colorPanel.Cursor = cursor;

                colorButton.Visibility = visibility;
            }
        }

        /// <summary>
        /// Gets or sets the width of button, which allows to change the current color.
        /// </summary>
        [Description("The width of button, which allows to change the current color.")]
        [DefaultValue(25)]
        public double ColorButtonWidth
        {
            get
            {
                return colorButton.Width;
            }
            set
            {
                colorButton.Width = Math.Max(1.0, value);
            }
        }

        /// <summary>
        /// Gets or sets the margin of button, which allows to change the current color.
        /// </summary>
        /// <value>
        /// Default value is <b>3</b>.
        /// </value>
        [Description("The margin of button, which allows to change the current color.")]
        [DefaultValue(3)]
        public double ColorButtonMargin
        {
            get
            {
                if (_colorRightToLeft)
                    return colorButton.Margin.Right;
                else
                    return colorButton.Margin.Left;
            }
            set
            {
                Thickness margin;
                if (_colorRightToLeft)
                    margin = new Thickness(0, 0, Math.Max(0, value), 0);
                else
                    margin = new Thickness(Math.Max(0, value), 0, 0, 0);

                colorButton.Margin = margin;
            }
        }

        /// <summary>
        /// Gets or sets the text of button, which allows to change the current color.
        /// </summary>
        /// <value>
        /// Default value is <b>...</b>.
        /// </value>
        [Description("The text of button, which allows to change the current color.")]
        [DefaultValue("...")]
        public string ColorButtonText
        {
            get
            {
                if (colorButton.Content == null)
                    return null;
                return colorButton.Content.ToString();
            }
            set
            {
                if (value != null)
                    colorButton.Content = value;
            }
        }

        bool _canEditAlphaChannel = true;
        /// <summary>
        /// Gets or sets a value indicating whether the alpha channel, of color, can be edited.
        /// </summary>
        /// <value>
        /// <b>true</b> if the alpha channel, of color, can be edited; otherwise, <b>false</b>.
        /// </value>
        [Description("Indicates whether the alpha channel, of color, can be edited.")]
        [DefaultValue(true)]
        public bool CanEditAlphaChannel
        {
            get
            {
                return _canEditAlphaChannel;
            }
            set
            {
                _canEditAlphaChannel = value;
            }
        }

        bool _showColorName = false;
        /// <summary>
        /// Gets or sets a value indicating whether the color must can be shown.
        /// </summary>
        /// <value>
        /// <b>true</b> - the color name must be shown; otherwise, <b>false</b>.
        /// </value>
        [Description("Indicates that the color name must be shown.")]
        [DefaultValue(false)]
        public bool ShowColorName
        {
            get
            {
                return _showColorName;
            }
            set
            {
                if (_showColorName != value)
                {
                    _showColorName = value;
                    UpdateColorPanel();
                }
            }
        }

        Color _defaultColor = Color.FromArgb(0, 0, 0, 0);
        /// <summary>
        /// Gets or sets the default color.
        /// </summary>
        /// <value>
        /// Default value is <b>Color.Empty</b>.
        /// </value>
        [Description("The default color.")]
        public Color DefaultColor
        {
            get
            {
                return _defaultColor;
            }
            set
            {
                _defaultColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default color can be changed.
        /// </summary>
        /// <value>
        /// <b>True</b> - the default color can be changed; otherwise, <b>false</b>.
        /// </value>
        [Description("Indicates that the default color can be changed.")]
        [DefaultValue(false)]
        public bool CanSetDefaultColor
        {
            get
            {
                return defaultColorButton.Visibility == Visibility.Visible;
            }
            set
            {
                if (value)
                    defaultColorButton.Visibility = Visibility.Visible;
                else
                    defaultColorButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or sets the width of button, which allows to change the default color.
        /// </summary>
        [Description("The width of button, which allows to change the default color.")]
        [DefaultValue(25)]
        public double DefaultColorButtonWidth
        {
            get
            {
                return defaultColorButton.Width;
            }
            set
            {
                defaultColorButton.Width = Math.Max(1, value);
            }
        }

        /// <summary>
        /// Gets or sets the margin of button, which allows to change the default color.
        /// </summary>
        /// <value>
        /// Default value is <b>3</b>.
        /// </value>
        [Description("The margin of button, which allows to change the default color.")]
        [DefaultValue(3)]
        public double DefaultColorButtonMargin
        {
            get
            {
                if (_colorRightToLeft)
                    return defaultColorButton.Margin.Right;
                else
                    return defaultColorButton.Margin.Left;
            }
            set
            {
                Thickness margin;
                if (_colorRightToLeft)
                    margin = new Thickness(0, 0, Math.Max(0, value), 0);
                else
                    margin = new Thickness(Math.Max(0, value), 0, 0, 0);

                defaultColorButton.Margin = margin;
            }
        }

        /// <summary>
        /// Gets or sets the text of button, which allows to change the default color.
        /// </summary>
        /// <value>
        /// Default value is <b>X</b>.
        /// </value>
        [Description("The text of button, which allows to change the default color.")]
        [DefaultValue("X")]
        public string DefaultColorButtonText
        {
            get
            {
                if (defaultColorButton.Content == null)
                    return null;
                return defaultColorButton.Content.ToString();
            }
            set
            {
                if (value != null)
                    defaultColorButton.Content = value;
            }
        }

        bool _colorRightToLeft = false;
        /// <summary>
        /// Gets or sets a value indicating whether the color panel and buttons
        /// must be positioned from the right to the left.
        /// </summary>
        /// <value>
        /// Default value is <b>false</b>.
        /// </value>
        [Description("Indicates that the color panel and buttons must be positioned from the right to the left.")]
        [DefaultValue(false)]
        public bool ColorRightToLeft
        {
            get
            {
                return _colorRightToLeft;
            }
            set
            {
                if (_colorRightToLeft != value)
                {
                    double colorButtonMargin = ColorButtonMargin;
                    double defaultColorButtonMargin = DefaultColorButtonMargin;

                    _colorRightToLeft = value;

                    if (_colorRightToLeft)
                    {
                        mainGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Auto);
                        Grid.SetColumn(colorButton, 0);

                        mainGrid.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Auto);
                        Grid.SetColumn(defaultColorButton, 1);

                        mainGrid.ColumnDefinitions[2].Width = new GridLength(1.0, GridUnitType.Star);
                        Grid.SetColumn(backgroundPanel, 2);
                    }
                    else
                    {
                        mainGrid.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Star);
                        Grid.SetColumn(backgroundPanel, 0);

                        mainGrid.ColumnDefinitions[1].Width = new GridLength(1.0, GridUnitType.Auto);
                        Grid.SetColumn(colorButton, 1);

                        mainGrid.ColumnDefinitions[2].Width = new GridLength(1.0, GridUnitType.Auto);
                        Grid.SetColumn(defaultColorButton, 2);
                    }

                    ColorButtonMargin = colorButtonMargin;
                    DefaultColorButtonMargin = defaultColorButtonMargin;
                }
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Updates the color panel.
        /// </summary>
        private void UpdateColorPanel()
        {
            colorPanel.Background = new SolidColorBrush(Color);

            if (ShowColorName)
            {
                System.Drawing.Color color = WpfObjectConverter.CreateDrawingColor(Color);

                Color foreColor;
                if (color.IsEmpty)
                {
                    foreColor = Colors.Black;
                }
                else if (Math.Abs(color.R - 128) < 30 &&
                         Math.Abs(color.G - 128) < 30 &&
                         Math.Abs(color.B - 128) < 30)
                {
                    foreColor = Colors.White;
                }
                else
                {
                    foreColor = new Color();

                    foreColor.A = 255;
                    foreColor.R = (byte)(0xFF ^ color.R);
                    foreColor.G = (byte)(0xFF ^ color.G);
                    foreColor.B = (byte)(0xFF ^ color.B);
                }

                string colorName;
                if (CanSetDefaultColor &&
                    Color == DefaultColor)
                {
                    colorName = "Default";
                }
                else if (color.IsNamedColor)
                {
                    colorName = color.Name;
                }
                else
                {
                    colorName = String.Format("#{0}{1}{2}",
                            color.R.ToString("X2"),
                            color.G.ToString("X2"),
                            color.B.ToString("X2"));
                }

                colorPanel.Foreground = new SolidColorBrush(foreColor);
                colorPanel.Content = colorName;
            }
            else if (colorPanel.Content != null)
                colorPanel.Content = null;
        }

        /// <summary>
        /// "..." button is clicked.
        /// </summary>
        private void colorButton_Click(object sender, RoutedEventArgs e)
        {
            ShowColorDialog();
        }

        /// <summary>
        /// Mouse is double clicked on the panel.
        /// </summary>
        private void colorPanel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CanSetColor && e.ChangedButton == MouseButton.Left)
                ShowColorDialog();
        }

        /// <summary>
        /// Shows the color dialog.
        /// </summary>
        private void ShowColorDialog()
        {
            ColorPickerDialog dialog = new ColorPickerDialog();
            dialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dialog.Owner = Window.GetWindow(this);
            dialog.StartingColor = Color;
            dialog.CanEditAlphaChannel = CanEditAlphaChannel;

            if (dialog.ShowDialog() == true)
            {
                if (Color != dialog.SelectedColor)
                {
                    Color = dialog.SelectedColor;
                    if (ColorChanged != null)
                        ColorChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// "X" button is clicked.
        /// </summary>
        private void defaultColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (Color != _defaultColor)
            {
                Color = _defaultColor;

                if (ColorChanged != null)
                    ColorChanged(this, EventArgs.Empty);
            }
        }

        #endregion



        #region Events

        /// <summary>
        /// Occurs when color is changed.
        /// </summary>
        public event EventHandler ColorChanged;

        #endregion

    }
}
