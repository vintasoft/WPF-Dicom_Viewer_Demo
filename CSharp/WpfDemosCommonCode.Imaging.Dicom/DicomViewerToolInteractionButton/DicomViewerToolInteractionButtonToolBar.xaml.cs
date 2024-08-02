using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Vintasoft.Imaging.Dicom.Wpf.UI.VisualTools;
using Vintasoft.Imaging.UI;
using Vintasoft.Imaging.Wpf;

namespace WpfDicomViewerDemo
{
    /// <summary>
    /// A toolbar for WPF DICOM Viewer tool.
    /// </summary>
    public partial class DicomViewerToolInteractionButtonToolBar : ToolBar
    {

        #region Fields

        /// <summary>
        /// Dictionary: the DICOM Viewer tool interaction mode => menu button.
        /// </summary>
        Dictionary<DicomViewerToolInteractionMode, Control> _interactionModeToMenuButton =
            new Dictionary<DicomViewerToolInteractionMode, Control>();

        /// <summary>
        /// Dictionary: menu button => the DICOM Viewer tool interaction mode.
        /// </summary>
        Dictionary<Control, DicomViewerToolInteractionMode> _menuButtonToInteractionMode =
            new Dictionary<Control, DicomViewerToolInteractionMode>();

        /// <summary>
        /// Dictionary: the DICOM Viewer tool interaction mode => icon name format for menu button.
        /// </summary>
        Dictionary<DicomViewerToolInteractionMode, string> _interactionModeToIconNameFormat =
            new Dictionary<DicomViewerToolInteractionMode, string>();

        /// <summary>
        /// The drop down menu that contains available interaction modes for mouse wheel.
        /// </summary>
        MenuItem _mouseWheelDropDownMenu;

        /// <summary>
        /// The icon name of mouse wheel button.
        /// </summary>
        readonly string MOUSE_WHEEL_BUTTON_ICON_NAME;

        /// <summary>
        /// The available mouse buttons.
        /// </summary>
        VintasoftMouseButtons[] _availableMouseButtons = new VintasoftMouseButtons[] {
            VintasoftMouseButtons.Left, VintasoftMouseButtons.Middle, VintasoftMouseButtons.Right
        };

        #endregion



        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DicomViewerToolInteractionButtonToolBar"/> class.
        /// </summary>
        public DicomViewerToolInteractionButtonToolBar()
        {
            InitializeComponent();

            // initialize interaction mode of WpfDicomViewerTool
            _supportedInteractionModes = new DicomViewerToolInteractionMode[] {
                        DicomViewerToolInteractionMode.Browse,
                        DicomViewerToolInteractionMode.Pan,
                        DicomViewerToolInteractionMode.Zoom,
                        DicomViewerToolInteractionMode.WindowLevel};

            // initilize name of icons

            MOUSE_WHEEL_BUTTON_ICON_NAME = "MouseWheelIcon";

            _interactionModeToIconNameFormat.Add(DicomViewerToolInteractionMode.Browse,
                "Browse_{0}{1}{2}Icon");
            _interactionModeToIconNameFormat.Add(DicomViewerToolInteractionMode.Pan,
                "Pan_{0}{1}{2}Icon");
            _interactionModeToIconNameFormat.Add(DicomViewerToolInteractionMode.WindowLevel,
                "WindowLevel_{0}{1}{2}Icon");
            _interactionModeToIconNameFormat.Add(DicomViewerToolInteractionMode.Zoom,
                "Zoom_{0}{1}{2}Icon");

            // initialize buttons
            InitButtons();
        }

        #endregion



        #region Properties

        WpfDicomViewerTool _tool = null;
        /// <summary>
        /// Gets or sets the visual tool.
        /// </summary>
        /// <value>
        /// Default value is <b>null</b>.
        /// </value>
        public WpfDicomViewerTool Tool
        {
            get
            {
                return _tool;
            }
            set
            {
                // if value is changed
                if (_tool != value)
                {
                    if (_tool != null)
                        UnsubscribeFromDicomViewerToolEvents(_tool);

                    _tool = value;

                    if (value != null)
                        SubscribeToDicomViewerToolEvents(value);

                    ResetUnsupportedInteractionModes();
                    UpdateInteractionButtonIcons();
                }
            }
        }

        DicomViewerToolInteractionMode[] _supportedInteractionModes;
        /// <summary>
        /// Gets or sets the supported interaction modes for toolbar.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <i>value</i> is <b>null</b>.</exception>
        public DicomViewerToolInteractionMode[] SupportedInteractionModes
        {
            get
            {
                return _supportedInteractionModes;
            }
            set
            {
                if (_supportedInteractionModes != value)
                {
                    if (value == null)
                        throw new ArgumentNullException();

                    _supportedInteractionModes = value;

                    InitButtons();

                    ResetUnsupportedInteractionModes();
                }
            }
        }

        DicomViewerToolInteractionMode[] _disabledInteractionModes = null;
        /// <summary>
        /// Gets or sets the disabled interaction modes for toolbar.
        /// </summary>
        /// <value>
        /// Default value is <b>null</b>.
        /// </value>
        public DicomViewerToolInteractionMode[] DisabledInteractionModes
        {
            get
            {
                return _disabledInteractionModes;
            }
            set
            {
                // if value is changed
                if (_disabledInteractionModes != value)
                {
                    // save new value
                    _disabledInteractionModes = value;

                    // for each interaction mode
                    foreach (DicomViewerToolInteractionMode interactionMode in _interactionModeToMenuButton.Keys)
                        // enable button for interaction mode
                        _interactionModeToMenuButton[interactionMode].IsEnabled = true;

                    // if disabled interaction modes are specified
                    if (_disabledInteractionModes != null)
                    {
                        // the menu button of interaction mode
                        Control menuButton = null;

                        // for each interaction mode
                        foreach (DicomViewerToolInteractionMode interactionMode in _disabledInteractionModes)
                        {
                            // if button is enabled
                            if (_interactionModeToMenuButton.TryGetValue(interactionMode, out menuButton))
                                // disable the button for interaction mode
                                menuButton.IsEnabled = false;
                        }
                    }
                }
            }
        }

        #endregion



        #region Methods

        #region PROTECTED

        /// <summary>
        /// Adds hot key commands to the parent window.
        /// </summary>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.InputBindings.AddRange(InputBindings);
                window.CommandBindings.AddRange(CommandBindings);

                window.Loaded += Window_Loaded;
            }

            base.OnVisualParentChanged(oldParent);
        }

        #endregion


        #region PRIVATE

        #region Init

        /// <summary>
        /// Initializes the buttons.
        /// </summary>
        private void InitButtons()
        {
            // remove old buttons
            Items.Clear();

            InitMouseWheelButtons();

            if (Items.Count > 0)
                Items.Add(new Separator());

            InitInteractionModeMenuButtons();
        }

        /// <summary>
        /// Initializes the mouse wheel buttons.
        /// </summary>
        private void InitMouseWheelButtons()
        {
            // the button name
            string name = "Mouse Wheel";
            // create the "Mouse Wheel" button
            Button mouseWheelMenuButton = new Button();
            mouseWheelMenuButton.Content = name;
            mouseWheelMenuButton.ToolTip = name;
            mouseWheelMenuButton.Click += new RoutedEventHandler(mouseWheelMenuButton_Click);

            // set the button icon
            SetToolStripButtonIcon(mouseWheelMenuButton, MOUSE_WHEEL_BUTTON_ICON_NAME);

            // available interaction modes of mouse wheel
            DicomViewerToolMouseWheelInteractionMode[] mouseWheelInteractionMode =
                new DicomViewerToolMouseWheelInteractionMode[] {
                     DicomViewerToolMouseWheelInteractionMode.None,
                     DicomViewerToolMouseWheelInteractionMode.Slide,
                     DicomViewerToolMouseWheelInteractionMode.Zoom };

            Menu mouseWheelDropDownMenuParent = new Menu();
            mouseWheelDropDownMenuParent.Width = 24;
            mouseWheelDropDownMenuParent.VerticalAlignment = VerticalAlignment.Center;
            MenuItem mouseWheelDropDownMenu = new MenuItem();
            mouseWheelDropDownMenu.Background = Brushes.Transparent;
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
            path.Fill = Brushes.Black;
            path.Data = Geometry.Parse("M 0 0 L 4 4 L 8 0 Z");
            mouseWheelDropDownMenu.Header = path;
            mouseWheelDropDownMenuParent.Items.Add(mouseWheelDropDownMenu);

            // for each interaction mode
            foreach (DicomViewerToolMouseWheelInteractionMode interactionMode in mouseWheelInteractionMode)
            {
                // create button
                MenuItem menuButton = new MenuItem();
                menuButton.Header = interactionMode.ToString();

                // if interaction mode is "Slide"
                if (interactionMode == DicomViewerToolMouseWheelInteractionMode.Slide)
                {
                    // mark button as checked
                    menuButton.IsChecked = true;
                }

                // save information about interaction mode in button
                menuButton.Tag = interactionMode;
                // subscribe to the button click event
                menuButton.Click += new RoutedEventHandler(mouseWheelInteractionModeButton_Click);

                // add button
                mouseWheelDropDownMenu.Items.Add(menuButton);
            }

            // add button to ToolStrip
            Items.Add(mouseWheelMenuButton);
            _mouseWheelDropDownMenu = mouseWheelDropDownMenu;
            Items.Add(mouseWheelDropDownMenuParent);
        }

        /// <summary>
        /// Initializes the interaction mode menu buttons.
        /// </summary>
        private void InitInteractionModeMenuButtons()
        {
            // clear dictionaries
            _interactionModeToMenuButton.Clear();
            _menuButtonToInteractionMode.Clear();

            // for each suported interaction mode
            foreach (DicomViewerToolInteractionMode interactionMode in _supportedInteractionModes)
            {
                // create button

                // get button name
                string name = interactionMode.ToString();

                Button menuButton = new Button();
                menuButton.Content = name;
                menuButton.ToolTip = name;

                // set the button icon
                SetButtonIcon(menuButton, interactionMode, VintasoftMouseButtons.None);

                // add button to the dictionaries
                _interactionModeToMenuButton.Add(interactionMode, menuButton);
                _menuButtonToInteractionMode.Add(menuButton, interactionMode);

                // if button must be disabled
                if (_disabledInteractionModes != null &&
                    Array.IndexOf(_disabledInteractionModes, interactionMode) >= 0)
                    // disable the button
                    menuButton.IsEnabled = false;

                menuButton.PreviewMouseDown += new MouseButtonEventHandler(interactionModeButton_PreviewMouseDown);

                // add button to the ToolBar
                Items.Add(menuButton);
            }
        }

        #endregion


        #region Interaction mode

        /// <summary>
        /// Selects the interaction mode button.
        /// </summary>
        private void Tool_InteractionModeChanged(object sender, DicomViewerToolInteractionModeChangedEventArgs e)
        {
            UpdateInteractionMode(e.Button, e.InteractionMode);
        }

        /// <summary>
        /// Selects the interaction mode of DicomViewerTools.
        /// </summary>
        private void interactionModeButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MenuItem splitMenuButton = sender as MenuItem;
            if (splitMenuButton != null && !splitMenuButton.IsPressed)
                return;

            Button menuButton = (Button)sender;
            DicomViewerToolInteractionMode interactionMode = _menuButtonToInteractionMode[menuButton];

            UpdateInteractionMode(WpfObjectConverter.CreateVintasoftMouseButtons(e), interactionMode);
        }

        /// <summary>
        /// Updates the interaction mode in DicomViewerTools.
        /// </summary>
        /// <param name="mouseButton">Mouse button.</param>
        /// <param name="interactionMode">Interaction mode.</param>
        private void UpdateInteractionMode(
            VintasoftMouseButtons mouseButton,
            DicomViewerToolInteractionMode interactionMode)
        {
            // if interaction mode is NOT supported
            if (Array.IndexOf(SupportedInteractionModes, interactionMode) == -1)
                interactionMode = DicomViewerToolInteractionMode.None;

            // if mouse button is NOT supported
            if (Array.IndexOf(_availableMouseButtons, mouseButton) == -1)
                interactionMode = DicomViewerToolInteractionMode.None;

            // set the interaction mode for DICOM Viewer tool
            Tool.SetInteractionMode(mouseButton, interactionMode);

            // update icons of interaction buttons
            UpdateInteractionButtonIcons();
        }

        #endregion


        #region Button icons

        /// <summary>
        /// Updates the icon of interaction buttons.
        /// </summary>
        private void UpdateInteractionButtonIcons()
        {
            // for each interaction mode
            foreach (DicomViewerToolInteractionMode interactionMode in _interactionModeToMenuButton.Keys)
            {
                // get the menu button of interaction mode
                Control menuButton = _interactionModeToMenuButton[interactionMode];

                // get mouse buttons of interaction mode
                VintasoftMouseButtons mouseButtons = GetMouseButtonsForInteractionMode(interactionMode);

                // update icon for menu button
                SetButtonIcon(menuButton, interactionMode, mouseButtons);
            }
        }

        /// <summary>
        /// Returns the icon name of specified interaction mode and buttons.
        /// </summary>
        /// <param name="interactionMode">The interaction mode.</param>
        /// <param name="mouseButtons">The mouse buttons of interaction mode.</param>
        /// <returns>
        /// The icon name.
        /// </returns>
        private string GetInteractionModeIconName(DicomViewerToolInteractionMode interactionMode, VintasoftMouseButtons mouseButtons)
        {
            // indices of action buttons (left, middle, right)
            byte[] indexes = new byte[] { 0, 0, 0 };

            // if mouse buttons are not empty
            if (mouseButtons != VintasoftMouseButtons.None)
            {
                // if left mouse button is active
                if ((mouseButtons & VintasoftMouseButtons.Left) != 0)
                    indexes[0] = 1;
                // if middle mouse button is active
                if ((mouseButtons & VintasoftMouseButtons.Middle) != 0)
                    indexes[1] = 1;
                // if right mouse button is active
                if ((mouseButtons & VintasoftMouseButtons.Right) != 0)
                    indexes[2] = 1;
            }

            // get the icon name format
            string iconNameFormat = _interactionModeToIconNameFormat[interactionMode];

            // return the icon name
            return string.Format(iconNameFormat, indexes[0], indexes[1], indexes[2]);
        }

        /// <summary>
        /// Sets the icon for the tool strip button.
        /// </summary>
        /// <param name="menuButton">The menu button.</param>
        /// <param name="interactionMode">The interaction mode.</param>
        /// <param name="mouseButtons">The mouse button.</param>
        private void SetButtonIcon(
            Control menuButton,
            DicomViewerToolInteractionMode interactionMode,
            VintasoftMouseButtons mouseButtons)
        {
            // get icon name for interaction mode
            string iconName = GetInteractionModeIconName(interactionMode, mouseButtons);

            // set the icon for button
            SetToolStripButtonIcon(menuButton, iconName);
        }

        /// <summary>
        /// Sets the icon for the tool strip button.
        /// </summary>
        /// <param name="menuButton">The menu button.</param>
        /// <param name="iconName">The icon name.</param>
        private void SetToolStripButtonIcon(Control menuButton, string iconName)
        {
            // if the icon name is NOT specified
            if (string.IsNullOrEmpty(iconName))
                return;

            // if menu button contains infomation about the button icon
            if (menuButton.Tag is string)
            {
                // get the icon name
                string currentIconName = menuButton.Tag.ToString();

                // if icon is not changed
                if (String.Equals(currentIconName, iconName, StringComparison.InvariantCultureIgnoreCase))
                    return;
            }

            menuButton.Background = Brushes.Transparent;

            Image image = new Image();
            image.Width = 16;
            image.Height = 16;
            image.Stretch = Stretch.None;
            // load resource
            image.Source = (BitmapImage)Resources[iconName];
            // load image
            if (menuButton is Button)
                ((Button)menuButton).Content = image;
            if (menuButton is MenuItem)
                ((MenuItem)menuButton).Header = image;
            // save icon name
            menuButton.Tag = iconName;
        }

        #endregion


        /// <summary>
        /// Subscribes to the WpfDicomViewerTool events.
        /// </summary>
        /// <param name="tool">The WpfDicomViewerTool.</param>
        private void SubscribeToDicomViewerToolEvents(WpfDicomViewerTool tool)
        {
            tool.ImageViewer.GotFocus += new RoutedEventHandler(imageViewer_GotFocus);
            tool.InteractionModeChanged += Tool_InteractionModeChanged;
        }

        /// <summary>
        /// Unsubscribes from the WpfDicomViewerTool events.
        /// </summary>
        /// <param name="tool">The WpfDicomViewerTool.</param>
        private void UnsubscribeFromDicomViewerToolEvents(WpfDicomViewerTool tool)
        {
            tool.ImageViewer.GotFocus -= new RoutedEventHandler(imageViewer_GotFocus);
            tool.InteractionModeChanged -= Tool_InteractionModeChanged;
        }

        /// <summary>
        /// Opens drop down menu with available interaction modes for mouse wheel.
        /// </summary>
        private void mouseWheelMenuButton_Click(object sender, RoutedEventArgs e)
        {
            _mouseWheelDropDownMenu.IsSubmenuOpen = true;
        }

        /// <summary>
        /// The mouse wheel interaction mode is changed.
        /// </summary>
        private void mouseWheelInteractionModeButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItem currentMenuButton = (MenuItem)sender;
            // get the interaction mode
            DicomViewerToolMouseWheelInteractionMode interactionMode = (DicomViewerToolMouseWheelInteractionMode)currentMenuButton.Tag;

            // update the interaction mode for mouse wheel
            Tool.MouseWheelInteractionMode = interactionMode;


            // uncheck all buttons

            MenuItem parentMenuButton = currentMenuButton.Parent as MenuItem;
            // if parent menu button exists
            if (parentMenuButton != null)
            {
                // for each item in parent menu item
                foreach (Control item in parentMenuButton.Items)
                {
                    // if item is menu button
                    if (item is MenuItem)
                        // uncheck the menu button
                        ((MenuItem)item).IsChecked = false;
                }
            }

            // check the current menu button
            currentMenuButton.IsChecked ^= true;
        }

        /// <summary>
        /// Returns the mouse buttons for interaction mode.
        /// </summary>
        /// <param name="interactionMode">The interaction mode.</param>
        /// <returns>
        /// The mouse buttons for interaction mode.
        /// </returns>
        private VintasoftMouseButtons GetMouseButtonsForInteractionMode(
            DicomViewerToolInteractionMode interactionMode)
        {
            // the result mouse buttons
            VintasoftMouseButtons resultMouseButton = VintasoftMouseButtons.None;

            // for each available mouse button
            foreach (VintasoftMouseButtons button in _availableMouseButtons)
            {
                // get an interaction mode for mouse button
                DicomViewerToolInteractionMode mouseButtonInteractionMode = Tool.GetInteractionMode(button);
                // if interaction mode for mouse button equals to the analyzing interaction mode
                if (mouseButtonInteractionMode == interactionMode)
                    // add mouse button to the result
                    resultMouseButton |= button;
            }

            return resultMouseButton;
        }

        /// <summary>
        /// Resets the unsupported interaction modes.
        /// </summary>
        private void ResetUnsupportedInteractionModes()
        {
            if (Tool == null)
                return;

            // for each mouse button
            foreach (VintasoftMouseButtons mouseButton in _availableMouseButtons)
            {
                // get the interaction mode for mouse button
                DicomViewerToolInteractionMode interactionMode = Tool.GetInteractionMode(mouseButton);

                // if interaction mode is None
                if (interactionMode == DicomViewerToolInteractionMode.None)
                    continue;

                // is interaction mode is not supported
                if (Array.IndexOf(SupportedInteractionModes, interactionMode) == -1)
                    // reset the interaction mode for mouse button
                    Tool.SetInteractionMode(mouseButton, DicomViewerToolInteractionMode.None);
            }
        }

        /// <summary>
        /// Updates the focused measurement annotations.
        /// </summary>
        private void imageViewer_GotFocus(object sender, RoutedEventArgs e)
        {
            UpdateInteractionButtonIcons();
        }

        /// <summary>
        /// Window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateMenuItemTemplatePartBackground(this);
        }

        /// <summary>
        /// Updates the template of <see cref="MenuItem"/> object for fixing the bug in <see cref="MenuItem"/>.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <remarks>
        /// The <see cref="MenuItem"/> has bug and displays black rectangle in element if MenuItem.IsChecked property is set to True.
        /// This method fixes the bug.
        /// </remarks>
        private void UpdateMenuItemTemplatePartBackground(Control control)
        {
            MenuItem menuItem = control as MenuItem;
            if (menuItem != null)
            {
                const string TEMPLATE_PART_NAME = "GlyphPanel";
                Border border = menuItem.Template.FindName(TEMPLATE_PART_NAME, menuItem) as Border;

                if (border == null)
                {
                    menuItem.ApplyTemplate();
                    border = menuItem.Template.FindName(TEMPLATE_PART_NAME, menuItem) as Border;
                }

                if (border != null)
                    border.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            }

            ItemsControl itemsControl = control as ItemsControl;
            if (itemsControl != null)
            {
                foreach (object item in itemsControl.Items)
                {
                    if (item is Control)
                        UpdateMenuItemTemplatePartBackground((Control)item);
                }
            }
        }

        #endregion

        #endregion

    }
}
