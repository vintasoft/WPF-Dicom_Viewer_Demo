using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// Represents a Windows spin box (also known as an up-down control) 
    /// that displays numeric values.
    /// </summary>
    /// <remarks>
    /// Equivalent of <see cref="System.Windows.Forms.NumericUpDown"/> class.
    /// </remarks>
    public partial class NumericUpDown : UserControl
    {

        #region Fields

        /// <summary>
        /// Indicates whether the text in the text box should be updated.
        /// </summary>
        bool _updateText = false;

        #endregion



        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericUpDown"/> class.
        /// </summary>
        public NumericUpDown()
        {
            InitializeComponent();

            Value = _minimum;

            UpdateText();

            valueText.TextChanged += new TextChangedEventHandler(valueText_TextChanged);
            valueText.LostFocus += new RoutedEventHandler(valueText_LostFocus);
        }

        #endregion



        #region Properties

        double _increment = 1;
        /// <summary>
        /// Gets or sets the value to increment or decrement the spin box (also known as an 
        /// up-down control) when the up or down buttons are clicked.
        /// </summary>
        /// <value>
        /// The default value is 1.
        /// </value>
        public double Increment
        {
            get
            {
                return _increment;
            }
            set
            {
                _increment = value;
            }
        }

        double _maximum = 100;
        /// <summary>
        /// Gets or set the maximum value.
        /// </summary>
        public double Maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                if (_maximum != value)
                {
                    if (value < _minimum)
                        value = _minimum;
                    _maximum = value;
                    if (Value > _maximum)
                        Value = _maximum;
                    else
                        UpdateUpDownButtonsEnabled();
                }
            }
        }

        double _minimum = 0;
        /// <summary>
        /// Gets or set the min value.
        /// </summary>
        public double Minimum
        {
            get
            {
                return _minimum;
            }
            set
            {
                if (_minimum != value)
                {
                    if (value > _maximum)
                        value = _maximum;
                    _minimum = value;
                    if (Value < _minimum)
                        Value = _minimum;
                    else
                        UpdateUpDownButtonsEnabled();
                }
            }
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
           DependencyProperty.Register("Value", typeof(double), typeof(NumericUpDown),
           new FrameworkPropertyMetadata(
               (double)0,
               FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
               new PropertyChangedCallback(ValuePropertyChanged),
               new CoerceValueCallback(ValuePropertyCoerceValue)));


        private static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown numUpDown = (NumericUpDown)d;
            numUpDown.UpdateText();
            numUpDown.UpdateUpDownButtonsEnabled();
            numUpDown.OnValueChanged(EventArgs.Empty);
        }

        private static object ValuePropertyCoerceValue(DependencyObject d, object baseValue)
        {
            NumericUpDown numUpDown = (NumericUpDown)d;
            return Math.Max(numUpDown.Minimum, Math.Min((double)baseValue, numUpDown.Maximum));
        }

        /// <summary>
        /// Gets or sets the value assigned to the control.
        /// </summary>
        [DefaultValue((double)0)]
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        int _decimalPlaces = 0;
        /// <summary>
        /// Gets or sets the number of decimal places.
        /// </summary>
        [Description("The number of decimal places.")]
        [DefaultValue(0)]
        public int DecimalPlaces
        {
            get
            {
                return _decimalPlaces;
            }
            set
            {
                _decimalPlaces = value;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Raises the ValueChanged event.
        /// </summary>
        /// <param name="args">An EventArgs that contains the event data.</param>
        protected virtual void OnValueChanged(EventArgs args)
        {
            if (ValueChanged != null)
                ValueChanged(this, args);
        }

        /// <summary>
        /// Update enable of up/down buttons.
        /// </summary>
        private void UpdateUpDownButtonsEnabled()
        {
            downButton.IsEnabled = Value > _minimum;
            upButton.IsEnabled = Value < _maximum;
        }

        /// <summary>
        /// upButton.Click event handler.
        /// </summary>
        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value < Maximum)
            {
                Value += Increment;
            }
        }

        /// <summary>
        /// downButton.Click event handler.
        /// </summary>
        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            if (Value > Minimum)
            {
                Value -= Increment;
            }
        }

        /// <summary>
        /// Update value text.
        /// </summary>
        private void UpdateText()
        {
            _updateText = true;
            valueText.Text = Math.Round(Value, DecimalPlaces).ToString(CultureInfo.InvariantCulture);
            _updateText = false;
        }

        /// <summary>
        /// valueText.TextChanged event handler.
        /// </summary>
        private void valueText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_updateText)
                return;
            if (valueText.Text == "" || valueText.Text == "-")
                return;
            double value;
            if (double.TryParse(valueText.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
            {
                if (value >= Minimum && value <= Maximum)
                {
                    Value = value;
                    if (Value != value)
                        SetValueTextInternal(Value);
                    UpdateUpDownButtonsEnabled();
                    OnValueChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// valueText.LostFocus event handler.
        /// </summary>
        private void valueText_LostFocus(object sender, RoutedEventArgs e)
        {
            double value;
            if (double.TryParse(
                valueText.Text,
                NumberStyles.AllowLeadingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign,
                CultureInfo.InvariantCulture,
                out value))
            {
                if (Value != value)
                {
                    if (value > Maximum)
                        value = Maximum;
                    else if (value < Minimum)
                        value = Minimum;
                    Value = value;
                    SetValueTextInternal(Value);
                    UpdateUpDownButtonsEnabled();
                    OnValueChanged(EventArgs.Empty);
                }
            }
            else
            {
                SetValueTextInternal(Value);
            }
        }

        /// <summary>
        /// Sets a value in the TextBox.
        /// </summary>
        private void SetValueTextInternal(double value)
        {
            valueText.Text = value.ToString(CultureInfo.InvariantCulture);
            valueText.CaretIndex = valueText.Text.Length;
        }

        #endregion



        #region Events

        /// <summary>
        /// Occurs when the Value property changes.
        /// </summary>
        public event EventHandler<EventArgs> ValueChanged;

        #endregion

    }
}