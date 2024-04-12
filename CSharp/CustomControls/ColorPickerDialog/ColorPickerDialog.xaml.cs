using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// A dialog that shows a color picker.
    /// </summary>
    public partial class ColorPickerDialog : Window
    {

        #region Fields

        private Color _color = new Color();

        private Color _startingColor = new Color();

        #endregion



        #region Constructors

        public ColorPickerDialog()
        {
            InitializeComponent();
        }

        #endregion



        #region Properties

        public Color SelectedColor
        {
            get
            {
                return _color;
            }
        }

        public Color StartingColor
        {
            get
            {
                return _startingColor;
            }
            set
            {
                if (!CanEditAlphaChannel && value.A != 255)
                    value = Color.FromArgb(255, value.R, value.G, value.B);

                _startingColor = value;
                cPicker.SelectedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the alpha channel, of color, can be edited
        /// </summary>
        /// <value>
        /// <b>true</b> if the alpha channel, of color, can be edited; otherwise, <b>false</b>.
        /// </value>
        public bool CanEditAlphaChannel
        {
            get
            {
                return cPicker.CanEditAlphaChannel;
            }
            set
            {
                cPicker.CanEditAlphaChannel = value;

                Color color = cPicker.SelectedColor;
                if (!value)
                {
                    color = Color.FromArgb(255, color.R, color.G, color.B);
                    cPicker.SelectedColor = color;
                }
            }
        }

        #endregion



        #region Methods

        #region PROTECTED

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Handles the Clicked event of okButton object.
        /// </summary>
        private void okButton_Clicked(object sender, RoutedEventArgs e)
        {
            _color = cPicker.SelectedColor;
            DialogResult = true;
            Hide();
        }

        /// <summary>
        /// Handles the Clicked event of cancelButton object.
        /// </summary>
        private void cancelButton_Clicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

        #endregion

    }
}
