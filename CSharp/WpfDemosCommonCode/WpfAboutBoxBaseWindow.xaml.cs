using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace WpfDemosCommonCode
{
    /// <summary>
    /// A base window for the application about dialog.
    /// </summary>
    public partial class WpfAboutBoxBaseWindow : Window
    {

        #region Constructors

        public WpfAboutBoxBaseWindow()
        {
            InitializeComponent();
        }

        public WpfAboutBoxBaseWindow(string productPrefix)
            : this()
        {
            nameLabel.Content = string.Format(nameLabel.Content.ToString(), AssemblyTitle, AssemblyShortVersion);
            imagingSDKVersionLabel.Content = string.Format(imagingSDKVersionLabel.Content.ToString(), ImagingSDKVersion);
            productLinkLabel.Text = string.Format(productLinkLabel.Text.ToString(), productPrefix);
        }

        #endregion



        #region Properties

        public string Description
        {
            get
            {
                return decriptionTextBox.Text;
            }
            set
            {
                decriptionTextBox.Text = value;
            }
        }

        [Browsable(false)]
        public string AssemblyTitle
        {
            get
            {

                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        [Browsable(false)]
        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        [Browsable(false)]
        public string AssemblyShortVersion
        {
            get
            {
                Version ver = Assembly.GetEntryAssembly().GetName().Version;
                return string.Format("{0}.{1}", ver.Major, ver.Minor);
            }
        }

        [Browsable(false)]
        public string ImagingSDKVersion
        {
            get
            {
                Assembly imagingAssembly = Assembly.GetAssembly(typeof(Vintasoft.Imaging.VintasoftImage));
                return imagingAssembly.GetName().Version.ToString();
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Handles the Click event of okButton object.
        /// </summary>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the MouseDown event of linkLabel object.
        /// </summary>
        private void linkLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DemosTools.OpenBrowser(string.Concat("https://", ((TextBlock)sender).Text.ToString()));
        }

        /// <summary>
        /// Handles the MouseDown event of Image object.
        /// </summary>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DemosTools.OpenBrowser("https://www.vintasoft.com");
        }

        #endregion

    }
}
