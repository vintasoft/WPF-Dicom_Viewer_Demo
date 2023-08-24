using System;
using System.ComponentModel;
using System.Windows.Controls;

using Vintasoft.Imaging;


namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// A control that allows to change the padding.
    /// </summary>
    [DefaultEvent("PaddingValueChanged")]
    public partial class PaddingFEditorControl : UserControl
    {

        #region Fields

        /// <summary>
        /// Determines that the the padding value is changing.
        /// </summary>
        bool _paddingValueUpdating = false;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PaddingPanelControl"/> class.
        /// </summary>
        public PaddingFEditorControl()
        {
            InitializeComponent();
        }

        #endregion



        #region Properties

        PaddingF _paddingValue = PaddingF.Empty;
        /// <summary>
        /// Gets or sets the padding value.
        /// </summary>
        /// <value>
        /// Default value is <b>Vintasoft.Imaging.PaddingF.Empty</b>.
        /// </value>
        [Description("The padding value.")]
        public PaddingF PaddingValue
        {
            get
            {
                return _paddingValue;
            }
            set
            {
                if (!_paddingValue.Equals(value))
                {
                    _paddingValue = value;
                    UpdatePaddingPanel(_paddingValue);
                }
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Updates the padding panel.
        /// </summary>
        /// <param name="padding">The padding.</param>
        private void UpdatePaddingPanel(PaddingF padding)
        {
            _paddingValueUpdating = true;

            leftNumericUpDown.Value = Convert.ToInt32(padding.Left);
            topNumericUpDown.Value = Convert.ToInt32(padding.Top);
            rightNumericUpDown.Value = Convert.ToInt32(padding.Right);
            bottomNumericUpDown.Value = Convert.ToInt32(padding.Bottom);
            allNumericUpDown.Value = Convert.ToInt32(padding.All);

            _paddingValueUpdating = false;
        }

        /// <summary>
        /// Field padding is changed.
        /// </summary>
        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_paddingValueUpdating)
                return;

            float left = Convert.ToSingle(leftNumericUpDown.Value);
            float top = Convert.ToSingle(topNumericUpDown.Value);
            float right = Convert.ToSingle(rightNumericUpDown.Value);
            float bottom = Convert.ToSingle(bottomNumericUpDown.Value);

            PaddingValue = new PaddingF(left, top, right, bottom);
            OnPaddingValueChanged();
        }

        /// <summary>
        /// Field padding is changed.
        /// </summary>
        private void allNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_paddingValueUpdating)
                return;

            PaddingValue = new PaddingF(Convert.ToSingle(allNumericUpDown.Value));
            OnPaddingValueChanged();
        }

        /// <summary>
        /// Raises the OnPaddingValueChanged event.
        /// </summary>
        private void OnPaddingValueChanged()
        {
            if (PaddingValueChanged != null)
                PaddingValueChanged(this, EventArgs.Empty);
        }

        #endregion



        #region Events

        /// <summary>
        /// Occurs when value of property is changed.
        /// </summary>
        public event EventHandler PaddingValueChanged;

        #endregion

    }
}
