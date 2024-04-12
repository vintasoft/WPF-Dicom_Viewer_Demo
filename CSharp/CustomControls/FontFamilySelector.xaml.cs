using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// A control that allows to select font family.
    /// </summary>
    public partial class FontFamilySelector : UserControl
    {
        
        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFamilySelector"/> class.
        /// </summary>
        public FontFamilySelector()
        {
            InitializeComponent();
            foreach (FontFamily family in Fonts.SystemFontFamilies)
                fontFamilyComboBox.Items.Add(family);
            fontFamilyComboBox.SelectionChanged += new SelectionChangedEventHandler(fontFamilyComboBox_SelectionChanged);
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets the selected <see cref="FontFamily"/>.
        /// </summary>
        public FontFamily SelectedFamily
        {
            get
            {
                return (FontFamily)fontFamilyComboBox.SelectedItem;
            }
            set
            {
                if (fontFamilyComboBox.SelectedItem != value)
                {
                    fontFamilyComboBox.SelectedItem = value;
                    OnSelectedFamilyChanged();
                }
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the SelectionChanged event of fontFamilyComboBox object.
        /// </summary>
        private void fontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnSelectedFamilyChanged();
        }

        protected virtual void OnSelectedFamilyChanged()
        {
            if (SelectedFamilyChanged != null)
                SelectedFamilyChanged(this, EventArgs.Empty);
        }

        #endregion



        #region Events

        /// <summary>
        /// Occurs when the SelectedFamily property changes.
        /// </summary>
        public event EventHandler SelectedFamilyChanged;

        #endregion

    }
}
