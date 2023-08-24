using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;


namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// A control that allows to change the thickness.
    /// </summary>
    [DefaultEvent("ValueChanged")]
    public partial class ThicknessEditorControl : UserControl
    {

        #region Fields

        /// <summary>
        /// Determines that the the thickness value is updating.
        /// </summary>
        bool _isThicknessValueUpdating = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThicknessEditorControl"/> class.
        /// </summary>
        public ThicknessEditorControl()
        {
            InitializeComponent();
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or set the maximum value of thickness.
        /// </summary>
        public double Maximum
        {
            get
            {
                return allNumericUpDown.Maximum;
            }
            set
            {
                leftNumericUpDown.Maximum = value;
                topNumericUpDown.Maximum = value;
                rightNumericUpDown.Maximum = value;
                bottomNumericUpDown.Maximum = value;
                allNumericUpDown.Maximum = value;
            }
        }

        /// <summary>
        /// Gets or set the minimum value of thickness.
        /// </summary>
        public double Minimum
        {
            get
            {
                return allNumericUpDown.Minimum;
            }
            set
            {
                leftNumericUpDown.Minimum = value;
                topNumericUpDown.Minimum = value;
                rightNumericUpDown.Minimum = value;
                bottomNumericUpDown.Minimum = value;
                allNumericUpDown.Minimum = value;
            }
        }

        Thickness _value = new Thickness(0);
        /// <summary>
        /// Gets or sets the current value of thickness.
        /// </summary>
        /// <value>
        /// Default value is <b>0</b>.
        /// </value>
        [Description("The current value of thickness.")]
        public Thickness Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (!_value.Equals(value))
                {
                    _value = value;
                    UpdateThicknessPanel(_value);
                }
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Updates the thickness panel.
        /// </summary>
        /// <param name="padding">The thickness.</param>
        private void UpdateThicknessPanel(Thickness thickness)
        {
            _isThicknessValueUpdating = true;

            leftNumericUpDown.Value = Convert.ToInt32(thickness.Left);
            topNumericUpDown.Value = Convert.ToInt32(thickness.Top);
            rightNumericUpDown.Value = Convert.ToInt32(thickness.Right);
            bottomNumericUpDown.Value = Convert.ToInt32(thickness.Bottom);

            if (thickness.Left == thickness.Top &&
                thickness.Top == thickness.Right &&
                thickness.Right == thickness.Bottom)
            {
                allNumericUpDown.Value = Convert.ToInt32(thickness.Left);
            }
            else
            {
                allNumericUpDown.Value = -1;
            }

            _isThicknessValueUpdating = false;
        }

        /// <summary>
        /// Updates the thickness value.
        /// </summary>
        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_isThicknessValueUpdating)
                return;

            double left = leftNumericUpDown.Value;
            double top = topNumericUpDown.Value;
            double right = rightNumericUpDown.Value;
            double bottom = bottomNumericUpDown.Value;

            Value = new Thickness(left, top, right, bottom);
            OnValueChanged();
        }

        /// <summary>
        /// Updates the thickness value.
        /// </summary>
        private void allNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_isThicknessValueUpdating)
                return;

            Value = new Thickness(allNumericUpDown.Value);
            OnValueChanged();
        }

        /// <summary>
        /// Raises the OnThicknessValueChanged event.
        /// </summary>
        private void OnValueChanged()
        {
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        #endregion



        #region Events

        /// <summary>
        /// Occurs when the property value is changed.
        /// </summary>
        public event EventHandler ValueChanged;

        #endregion

    }
}
