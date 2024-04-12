using System;
using System.IO;
using System.Windows;


namespace WpfDicomViewerDemo
{
    /// <summary>
    /// A window that allows to select a DICOM presentation state file.
    /// </summary>
    public partial class SelectPresentationStateFile : Window
    {

        #region Fields

        /// <summary>
        /// Names of presentation state files.
        /// </summary>
        string[] _presentationStateFilenames = null;

        #endregion



        #region Constructors

        public SelectPresentationStateFile(string[] presentationStateFilenames)
        {
            InitializeComponent();

            _presentationStateFilenames = presentationStateFilenames;

            filenamesComboBox.BeginInit();
            foreach (string filename in presentationStateFilenames)
                filenamesComboBox.Items.Add(Path.GetFileName(filename));
            filenamesComboBox.EndInit();

            if (_presentationStateFilenames.Length > 0)
                filenamesComboBox.SelectedIndex = 0;
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets the name of selected presentation state file.
        /// </summary>
        public string SelectedPresentationStateFilename
        {
            get
            {
                if (filenamesComboBox.SelectedIndex == -1)
                    return string.Empty;

                return _presentationStateFilenames[filenamesComboBox.SelectedIndex];
            }
            set
            {
                int index = Array.IndexOf(_presentationStateFilenames, value);

                filenamesComboBox.SelectedIndex = index;
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

        #endregion

    }
}
