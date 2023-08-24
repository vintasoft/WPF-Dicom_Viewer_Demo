using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// A control that allows to edit value in image processing configuration window.
    /// </summary>
    public partial class ValueEditorControl : UserControl
    {

        #region Fields

        /// <summary>
        /// Indicates whether the value in valueNumericUpDown should be changed.
        /// </summary>
        bool _changeNumericUpDownValue = true;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueEditorControl"/> class.
        /// </summary>
        public ValueEditorControl()
        {
            InitializeComponent();
        }

        #endregion



        #region Properties

        public static readonly DependencyProperty ValueHeaderProperty =
           DependencyProperty.Register("ValueHeader", typeof(object), typeof(ValueEditorControl),
               new PropertyMetadata("Value Header", ValueHeaderPropertyChanged));
        /// <summary>
        /// Gets or sets header of the value.
        /// </summary>
        [Description("Header of the value.")]
        public object ValueHeader
        {
            get
            {
                return GetValue(ValueHeaderProperty);
            }
            set
            {
                SetValue(ValueHeaderProperty, value);
                UpdateGroupBoxHeader();
            }
        }

        public static readonly DependencyProperty IsContentEnabledProperty =
           DependencyProperty.Register("IsContentEnabled", typeof(bool), typeof(ValueEditorControl),
               new PropertyMetadata(true, IsContentEnabledChanged));
        /// <summary>
        /// Gets or sets a value indicating whether the control's content is enabled.
        /// </summary>
        public bool IsContentEnabled
        {
            get
            {
                return (bool)GetValue(IsContentEnabledProperty);
            }
            set
            {
                SetValue(IsContentEnabledProperty, value);
            }
        }

        string _valueUnitOfMeasure = "";
        /// <summary>
        /// Gets or sets the unit of measure of the value.
        /// </summary>
        [Description("Unit of measure of the value.")]
        public string ValueUnitOfMeasure
        {
            get
            {
                return _valueUnitOfMeasure;
            }
            set
            {
                _valueUnitOfMeasure = value;
                UpdateGroupBoxHeader();
            }
        }

        double _value = 0;
        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        [Description("Current value.")]
        [DefaultValue(0)]
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                valueNumericUpDown.Value = value;
            }
        }

        double _minValue = 0;
        /// <summary>
        /// Gets or sets minimum value.
        /// </summary>
        [Description("Minimum value.")]
        [DefaultValue(0)]
        public double MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                _minValue = value;
                minValueLabel.Content = value.ToString(CultureInfo.InvariantCulture);
                valueNumericUpDown.Minimum = value;
                valueSlider.Minimum = value;
                UpdateSliderParameters();
            }
        }

        double _maxValue = 100;
        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        [Description("Maximum value.")]
        [DefaultValue(100)]
        public double MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;
                maxValueLabel.Content = value.ToString(CultureInfo.InvariantCulture);
                valueNumericUpDown.Maximum = value;
                valueSlider.Maximum = value;
                UpdateSliderParameters();
            }
        }

        double _defaultValue = 0;
        /// <summary>
        /// Gets or sets the default value.
        /// </summary>
        [Description("Default value.")]
        [DefaultValue(0)]
        public double DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                _defaultValue = value;
            }
        }

        double _step = 1;
        /// <summary>
        /// Gets or sets the value step.
        /// </summary>
        [Description("The value step.")]
        [DefaultValue(1)]
        public double Step
        {
            get
            {
                return _step;
            }
            set
            {
                _step = value;
                valueNumericUpDown.Increment = value;
                valueSlider.SmallChange = value;
                UpdateSliderParameters();
            }
        }

        /// <summary>
        /// Gets or sets the number of decimal places.
        /// </summary>
        [Description("The number of decimal places.")]
        [DefaultValue(0)]
        public int DecimalPlaces
        {
            get
            {
                return valueNumericUpDown.DecimalPlaces;
            }
            set
            {
                valueNumericUpDown.DecimalPlaces = value;
            }
        }

        #endregion



        #region Methods
        
        /// <summary>
        /// The "ValueHeader" dependency property is changed.
        /// </summary>
        private static void ValueHeaderPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            target.InvalidateProperty(ValueHeaderProperty);
            ((ValueEditorControl)target).UpdateGroupBoxHeader();
        }

        /// <summary>
        /// The "IsContentEnabled" dependency property is changed.
        /// </summary>
        private static void IsContentEnabledChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            target.InvalidateProperty(IsContentEnabledProperty);
            ((ValueEditorControl)target).contentGrid.IsEnabled = ((ValueEditorControl)target).IsContentEnabled;
        }        

        /// <summary>
        /// The slider value is changed.
        /// </summary>
        private void valueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double sliderValue = Math.Round(valueSlider.Value / Step) * Step;
            valueSlider.Value = sliderValue;
            if (valueNumericUpDown != null && _changeNumericUpDownValue)
                valueNumericUpDown.Value = sliderValue;
        }

        /// <summary>
        /// The value in numericUpDown is changed. 
        /// </summary>
        private void valueNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Value = valueNumericUpDown.Value;
            _changeNumericUpDownValue = false;
            valueSlider.Value = Value;
            _changeNumericUpDownValue = true;
            OnValueChanged();
        }

        /// <summary>
        /// "Reset" button is clicked.
        /// </summary>
        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            valueNumericUpDown.Value = DefaultValue;
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        private void OnValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Updates text of the group box.
        /// </summary>
        private void UpdateGroupBoxHeader()
        {
            if (string.IsNullOrEmpty(ValueUnitOfMeasure))
            {
                valueGroupBox.Header = ValueHeader;
            }
            else
            {
                if (ValueHeader is HeaderedContentControl)
                {
                    if (((HeaderedContentControl)ValueHeader).Header is string)
                    {
                        string header = (string)((HeaderedContentControl)ValueHeader).Header;
                        ((HeaderedContentControl)ValueHeader).Content = String.Format("{0} ({1})", header, ValueUnitOfMeasure);
                    }
                }
                else if (ValueHeader is ContentControl)
                {
                    if (((ContentControl)ValueHeader).Content is string)
                    {
                        string content = (string)((ContentControl)ValueHeader).Content;
                        ((ContentControl)ValueHeader).Content = String.Format("{0} ({1})", content, ValueUnitOfMeasure);
                    }
                }
                else
                {
                    valueGroupBox.Header = String.Format("{0} ({1})", ValueHeader, ValueUnitOfMeasure);
                }
            }
        }

        /// <summary>
        /// Updates parameters of the slider.
        /// </summary>
        private void UpdateSliderParameters()
        {
            // set the large change of the slider
            valueSlider.LargeChange = 0.1 * (MaxValue - MinValue);

            // set the frequency of displayed ticks
            valueSlider.TickFrequency = Step;
        }

        #endregion



        #region Events

        /// <summary>
        /// Occurs when the value is changed.
        /// </summary>
        public event EventHandler ValueChanged;

        #endregion

    }
}
