using System;
using System.Windows;
using System.Windows.Controls;

using Vintasoft.Imaging;

namespace WpfDemosCommonCode.CustomControls
{
    /// <summary> 
    /// Defines a UserControl for changing a value of <see cref="AnchorType"/> type.
    /// </summary>
    public partial class AnchorTypeEditorControl : UserControl
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchorTypeEditorControl"/> class.
        /// </summary>
        public AnchorTypeEditorControl()
        {
            InitializeComponent();
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets the type of the selected <see cref="AnchorType"/> property.
        /// </summary>
        /// <value>
        /// The type of the selected AnchorType property.
        /// </value>
        public AnchorType SelectedAnchorType
        {
            get
            {
                if (topLeftRadioButton.IsChecked.Value == true)
                    return AnchorType.TopLeft;

                if (topRadioButton.IsChecked.Value == true)
                    return AnchorType.Top;

                if (topRightRadioButton.IsChecked.Value == true)
                    return AnchorType.TopRight;

                if (leftRadioButton.IsChecked.Value == true)
                    return AnchorType.Left;

                if (centerRadioButton.IsChecked.Value == true)
                    return AnchorType.Center;

                if (rightRadioButton.IsChecked.Value == true)
                    return AnchorType.Right;

                if (bottomLeftRadioButton.IsChecked.Value == true)
                    return AnchorType.BottomLeft;

                if (bottomRadioButton.IsChecked.Value == true)
                    return AnchorType.Bottom;

                if (bottomRightRadioButton.IsChecked.Value == true)
                    return AnchorType.BottomRight;

                return AnchorType.None;
            }
            set
            {
                switch (value)
                {
                    case AnchorType.TopLeft:
                        topLeftRadioButton.IsChecked = true;
                        break;

                    case AnchorType.Top:
                        topRadioButton.IsChecked = true;
                        break;

                    case AnchorType.TopRight:
                        topRightRadioButton.IsChecked = true;
                        break;

                    case AnchorType.Left:
                        leftRadioButton.IsChecked = true;
                        break;

                    case AnchorType.None:
                    case AnchorType.Center:
                        centerRadioButton.IsChecked = true;
                        break;

                    case AnchorType.Right:
                        rightRadioButton.IsChecked = true;
                        break;

                    case AnchorType.BottomLeft:
                        bottomLeftRadioButton.IsChecked = true;
                        break;

                    case AnchorType.Bottom:
                        bottomRadioButton.IsChecked = true;
                        break;

                    case AnchorType.BottomRight:
                        bottomRightRadioButton.IsChecked = true;
                        break;

                    default:
                        centerRadioButton.IsChecked = true;
                        break;
                }
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Another radioButton is checked.
        /// </summary>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (SelectedAnchorTypeChanged != null)
                SelectedAnchorTypeChanged(sender, e);
        }

        #endregion



        #region Events

        /// <summary>
        /// Occurs when <see cref="SelectedAnchorType"/> is changed.
        /// </summary>
        public event EventHandler SelectedAnchorTypeChanged;

        #endregion

    }
}
