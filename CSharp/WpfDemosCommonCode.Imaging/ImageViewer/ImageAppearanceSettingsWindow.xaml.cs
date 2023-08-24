using System.Windows;
using System.Windows.Media;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that allows to edit the appearance settings of image.
    /// </summary>
    public partial class ImageAppearanceSettingsWindow : Window
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAppearanceSettingsWindow"/> class.
        /// </summary>
        public ImageAppearanceSettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageAppearanceSettingsWindow"/> class.
        /// </summary>
        /// <param name="title">The title of the form.</param>
        public ImageAppearanceSettingsWindow(string title)
            : this()
        {
            Title = title;
        }

        #endregion



        #region Properties

        Color _appearanceBackColor;
        /// <summary>
        /// The back color of image appearance. 
        /// </summary>
        public Color AppearanceBackColor
        {
            get
            {
                return _appearanceBackColor;
            }
            set
            {
                _appearanceBackColor = value;
                backColorPanelControl.Color = _appearanceBackColor;
            }
        }

        Color _appearanceBorderColor;
        /// <summary>
        /// The border color of image appearance.
        /// </summary>
        public Color AppearanceBorderColor
        {
            get
            {
                return _appearanceBorderColor;
            }
            set
            {
                _appearanceBorderColor = value;
                borderColorPanelControl.Color = _appearanceBorderColor;
            }
        }

        int _appearanceBorderWidth;
        /// <summary>
        /// The border width of image appearance.
        /// </summary>
        public int AppearanceBorderWidth
        {
            get
            {
                return _appearanceBorderWidth;
            }
            set
            {
                _appearanceBorderWidth = value;
                borderWidthNumericUpDown.Value = _appearanceBorderWidth;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Set the settings of appearance.
        /// </summary>
        private void SetSettings()
        {
            _appearanceBackColor = backColorPanelControl.Color;
            _appearanceBorderColor = borderColorPanelControl.Color;
            _appearanceBorderWidth = (int)borderWidthNumericUpDown.Value;
        }

        /// <summary>
        /// "Ok" button is clicked.
        /// </summary>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            SetSettings();
            DialogResult = true;
        }

        /// <summary>
        /// "Cancel" button is clicked.
        /// </summary>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

    }
}
