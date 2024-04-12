using System.Windows;
using System.Windows.Forms;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A window that shows the property grid with properties of specified object.
    /// </summary>
    public partial class PropertyGridWindow : Window
    {

        #region Constructor

        public PropertyGridWindow(object obj, string formTitle)
            :this(obj,formTitle, false)
        {
        }

        public PropertyGridWindow(object obj, string formTitle, bool canCancel)
        {
            InitializeComponent();

            Title = formTitle;
            cancelButton.IsEnabled = canCancel;
            _propertyGrid.SelectedObject = obj;
            _propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);
        }

        #endregion



        #region Properties

        public PropertyGrid PropertyGrid
        {
            get
            {
                return _propertyGrid;
            }
        }

        bool _propertyValueChanged = false;
        /// <summary>
        /// Gets a value indicating whether a property value was changed.
        /// </summary>
        /// <value>
        /// <b>true</b> if a property value was changed; otherwise, <b>false</b>.
        /// </value>
        public bool PropertyValueChanged
        {
            get
            {
                return _propertyValueChanged;
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        /// <summary>
        /// Handles the Click event of cancelButton object.
        /// </summary>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>
        /// Handles the PropertyValueChanged event of the _propertyGrid control.
        /// </summary>
        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            _propertyValueChanged = true;
        }

        #endregion

    }
}
