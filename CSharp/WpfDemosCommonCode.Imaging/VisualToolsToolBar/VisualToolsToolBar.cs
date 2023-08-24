using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Wpf.UI;
using Vintasoft.Imaging.Wpf.UI.VisualTools;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Represents a tool strip that shows buttons, which allow to enable/disable visual tools in image viewer.
    /// </summary>
    public partial class VisualToolsToolBar : ToolBar
    {

        #region Nested Class

        /// <summary>
        /// A dictionary, which contains links between actions and buttons.
        /// </summary>
        private class VisualToolActionButtonsDictionary
        {

            #region Fields

            /// <summary>
            /// The dictionary from action to the tool strip item.
            /// </summary>
            Dictionary<VisualToolAction, Control> _actionToToolStripItem =
                new Dictionary<VisualToolAction, Control>();

            /// <summary>
            /// The dictionary from the tool strip item to the action.
            /// </summary>
            Dictionary<Control, VisualToolAction> _toolStripItemToAction =
                new Dictionary<Control, VisualToolAction>();

            #endregion



            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="VisualToolActionButtonsDictionary"/> class.
            /// </summary>
            internal VisualToolActionButtonsDictionary()
            {
            }

            #endregion



            #region Methods

            /// <summary>
            /// Adds the specified action and tool strip item.
            /// </summary>
            /// <param name="action">The action.</param>
            /// <param name="item">The tool strip item.</param>
            internal void Add(VisualToolAction action, Control item)
            {
                _actionToToolStripItem.Add(action, item);
                _toolStripItemToAction.Add(item, action);
            }

            /// <summary>
            /// Determines whether the dictionary contains the specified action.
            /// </summary>
            /// <param name="action">The action to locate in the dictionary.</param>
            /// <returns>
            /// <b>True</b> if action is found in the dictionary; otherwise; <b>false</b>.
            /// </returns>
            internal bool Contains(VisualToolAction action)
            {
                return _actionToToolStripItem.ContainsKey(action);
            }

            /// <summary>
            /// Determines whether the dictionary contains the specified tool strip item.
            /// </summary>
            /// <param name="action">The tool strip item to locate in the dictionary.</param>
            /// <returns>
            /// <b>True</b> if tool strip item is found in the dictionary; otherwise; <b>false</b>.
            /// </returns>
            internal bool Contains(Control item)
            {
                return _toolStripItemToAction.ContainsKey(item);
            }

            /// <summary>
            /// Removes all items from the collection.
            /// </summary>
            internal void Clear()
            {
                _actionToToolStripItem.Clear();
                _toolStripItemToAction.Clear();
            }

            /// <summary>
            /// Removes the specified action from the dictionary.
            /// </summary>
            /// <param name="action">The action to remove from the dictionary.</param>
            /// <returns>
            /// <b>True</b> if action was removed successfully; otherwise, <b>false</b>.
            /// </returns>
            internal bool Remove(VisualToolAction action)
            {
                if (Contains(action))
                {
                    Control item = GetItem(action);

                    _actionToToolStripItem.Remove(action);
                    _toolStripItemToAction.Remove(item);

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Removes the specified tool strip item from the dictionary.
            /// </summary>
            /// <param name="item">The tool strip item to remove from the dictionary.</param>
            /// <returns>
            /// <b>True</b> if tool strip item was removed successfully; otherwise, <b>false</b>.
            /// </returns>
            internal bool Remove(Control item)
            {
                if (Contains(item))
                {
                    VisualToolAction action = GetAction(item);

                    _actionToToolStripItem.Remove(action);
                    _toolStripItemToAction.Remove(item);

                    return true;
                }

                return false;
            }

            /// <summary>
            /// Returns the tool strip item at the specified action.
            /// </summary>
            /// <param name="action">The action.</param>
            /// <returns>
            /// The tool strip item of action.
            /// </returns>
            internal Control GetItem(VisualToolAction action)
            {
                return _actionToToolStripItem[action];
            }

            /// <summary>
            /// Returns the action at the specified tool strip item.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <returns>
            /// The action of tool strip item.
            /// </returns>
            internal VisualToolAction GetAction(Control item)
            {
                return _toolStripItemToAction[item];
            }

            #endregion

        }

        #endregion



        #region Fields

        /// <summary>
        /// The none tool item.
        /// </summary>
        NoneAction _noneVisualToolAction = null;

        /// <summary>
        /// The label, which shows the action status.
        /// </summary>
        TextBlock _actionStatusLabel = new TextBlock();

        /// <summary>
        /// Indicates whether the action status label is moving.
        /// </summary>
        bool _isActionStatusLabelMoving = false;

        /// <summary>
        /// The dictionary: visual tool action => visual tool.
        /// </summary>
        Dictionary<VisualToolAction, WpfVisualTool> _visualToolItemToVisualTool =
            new Dictionary<VisualToolAction, WpfVisualTool>();

        /// <summary>
        /// The dictionary, which contains information about visual tools, which are added to the tool strip.
        /// </summary>
        VisualToolActionButtonsDictionary _toolStripDictionary =
            new VisualToolActionButtonsDictionary();

        /// <summary>
        /// The dictionary, which contains information about visual tools, which are added to the "Visual tools" menu.
        /// </summary>
        VisualToolActionButtonsDictionary _visualToolMenuItemDictionary =
            new VisualToolActionButtonsDictionary();

        /// <summary>
        /// The dictionary: button => item collection.
        /// </summary>
        Dictionary<Button, ItemCollection> _dropDownButtonToItemCollection =
            new Dictionary<Button, ItemCollection>();

        /// <summary>
        /// The dictionary: item collection => button.
        /// </summary>
        Dictionary<ItemCollection, Button> _dropItemCollectionToDownButton =
            new Dictionary<ItemCollection, Button>();

        /// <summary>
        /// The dictionary: item collection => menu.
        /// </summary>
        Dictionary<ItemCollection, Menu> _dropDownItemCollectionToMenu =
            new Dictionary<ItemCollection, Menu>();

        /// <summary>
        /// The action activation count.
        /// </summary>
        int _actionActivationCount = 0;

        /// <summary>
        /// A value indicating whether the activating action can change the visual tool of image viewer.
        /// </summary>
        bool _canChangeVisualTool = true;

        /// <summary>
        /// The previous action of visual tool.
        /// </summary>
        VisualToolAction _previousActionOfVisualTool = null;

        #endregion



        #region Constructors

        public VisualToolsToolBar()
        {
            Height = 31;
            Background = Brushes.Transparent;
            SnapsToDevicePixels = true;

            _actionStatusLabel.VerticalAlignment = VerticalAlignment.Center;
            _actionStatusLabel.FontSize = 15;
            _actionStatusLabel.TextTrimming = TextTrimming.CharacterEllipsis;
            _actionStatusLabel.Margin = new Thickness(0, 0, 3, 0);

            IsEnabled = false;

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            // Add "None" action

            BitmapSource icon = DemosResourcesManager.GetResourceAsBitmap(
                "WpfDemosCommonCode.Imaging.VisualToolsToolBar.Resources.None.png");
            _noneVisualToolAction = new NoneAction("None", "None", icon);
            AddAction(_noneVisualToolAction);
        }

        #endregion



        #region Properties

        #region PUBLIC

        WpfImageViewer _imageViewer = null;
        /// <summary>
        /// Gets or sets an image viewer, which is associated with this tool strip.
        /// </summary>
        [Description("The image viewer, which is associated with this toolstrip.")]
        [Browsable(true)]
        public WpfImageViewer ImageViewer
        {
            get
            {
                return _imageViewer;
            }
            set
            {
                if (_imageViewer != null)
                    UnsubscribeFromImageViewerEvents(_imageViewer);

                _imageViewer = value;

                if (_imageViewer == null)
                {
                    IsEnabled = false;
                }
                else
                {
                    if (_imageViewer.FocusedIndex == -1)
                    {
                        IsEnabled = false;
                    }
                    else
                    {
                        IsEnabled = true;

                        UpdateMenuItemTemplatePartBackground(this);
                    }

                    SubscribeToImageViewerEvents(_imageViewer);
                }

                UpdateSelectedItem();
            }
        }

        List<VisualToolAction> _visualToolActions = new List<VisualToolAction>();
        /// <summary>
        /// Gets the read-only collection of <see cref="VisualToolAction"/> objects, which were added to this <see cref="VisualToolsToolBar"/>.
        /// </summary>
        [Browsable(false)]
        public ReadOnlyCollection<VisualToolAction> VisualToolActions
        {
            get
            {
                return _visualToolActions.AsReadOnly();
            }
        }

        WpfVisualTool _mandatoryVisualTool = null;
        /// <summary>
        /// Gets or sets the mandatory visual tool.
        /// </summary>
        /// <value>
        /// Default value is <b>null</b>.
        /// </value>
        /// <remarks>
        /// The mandatory visual tool is always active because is always used in composition with selected visual tool.
        /// </remarks>
        [Browsable(false)]
        public virtual WpfVisualTool MandatoryVisualTool
        {
            get
            {
                return _mandatoryVisualTool;
            }
            set
            {
                if (_mandatoryVisualTool != value)
                {
                    VisualToolAction currentAction = _currentActionOfVisualTool;

                    if (currentAction != null)
                        currentAction.Deactivate();

                    _visualToolItemToVisualTool.Clear();

                    _mandatoryVisualTool = value;

                    if (currentAction != null)
                        currentAction.Activate();
                }
            }
        }

        MenuItem _visualToolsMenuItem = null;
        /// <summary>
        /// Gets or sets the visual tools menu item, which duplicates this toolstrip.
        /// </summary>
        /// <value>
        /// Default value is <b>null</b>.
        /// </value>
        [Description("The visual tools menu item, which duplicates this toolstrip.")]
        public MenuItem VisualToolsMenuItem
        {
            get
            {
                return _visualToolsMenuItem;
            }
            set
            {
                if (_visualToolsMenuItem != value)
                {
                    if (_visualToolsMenuItem != null)
                        RemoveVisualToolActionsFromMenu(_visualToolsMenuItem);

                    _visualToolsMenuItem = value;

                    if (_visualToolsMenuItem != null)
                        AddVisualToolActionsToMenu(_visualToolsMenuItem);
                }
            }
        }

        #endregion


        #region PROTECTED

        VisualToolAction _currentActionOfVisualTool = null;
        /// <summary>
        /// The currently action of visual tool.
        /// </summary>
        protected VisualToolAction CurrentActionOfVisualTool
        {
            get
            {
                return _currentActionOfVisualTool;
            }
        }

        #endregion

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Resets the current visual tool of image viewer to the default visual tool.
        /// </summary>
        public void Reset()
        {
            if (!_noneVisualToolAction.IsActivated)
                _noneVisualToolAction.Activate();
        }

        /// <summary>
        /// Adds the visual tool action to this tool strip.
        /// </summary>
        /// <param name="visualToolAction">The visual tool action.</param>
        public void AddAction(VisualToolAction visualToolAction)
        {
            AddVisualToolActionToToolBar(Items, visualToolAction, _toolStripDictionary);

            if (_visualToolsMenuItem != null)
            {
                AddVisualToolActionToToolBar(
                    _visualToolsMenuItem.Items,
                    visualToolAction,
                    _visualToolMenuItemDictionary);
            }

            SubscribeToVisualToolActions(visualToolAction);

            // save added item
            _visualToolActions.Add(visualToolAction);

            UpdateSelectedItem();
        }

        /// <summary>
        /// Finds the specified action of specified type in tool strip.
        /// </summary>
        /// <typeparam name="T">The action type to find.</typeparam>
        /// <returns>
        /// The item of specified type if action is found; otherwise, <b>null</b>.
        /// </returns>
        public T FindAction<T>()
            where T : VisualToolAction
        {
            foreach (VisualToolAction item in _visualToolActions)
            {
                if (item is T)
                    return (T)item;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the tool strip contains the specified action of specified type.
        /// </summary>
        /// <typeparam name="T">The action type.</typeparam>
        /// <returns>
        /// <b>True</b> the action of specified type is exist; otherwise, <b>false</b>.
        /// </returns>
        public bool ContainsAction<T>()
            where T : VisualToolAction
        {
            if (FindAction<T>() == null)
                return false;

            return true;
        }

        /// <summary>
        /// Sets the previous action of visual tool.
        /// </summary>
        /// <returns>
        /// <b>True</b> the action is changed; otherwise, <b>false</b>.
        /// </returns>
        public bool SetPreviousAction()
        {
            // if previous action of visual tool is not defined
            if (_previousActionOfVisualTool == null)
                return false;

            // activate previous action of visual tool
            _previousActionOfVisualTool.Activate();

            return true;
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Raises the <see cref="E:ItemsChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (!_isActionStatusLabelMoving)
            {
                if (Items.Count - 1 != Items.IndexOf(_actionStatusLabel))
                {
                    _isActionStatusLabelMoving = true;

                    // move action status label

                    this.Items.Remove(_actionStatusLabel);
                    this.Items.Add(_actionStatusLabel);

                    _isActionStatusLabelMoving = false;
                }
            }
        }

        /// <summary>
        /// Returns the visual tool, which is associated with specified visual tool action.
        /// </summary>
        /// <param name="visualToolAction">The visual tool action.</param>
        /// <returns>
        /// The visual tool.
        /// </returns>
        protected virtual WpfVisualTool GetVisualTool(VisualToolAction visualToolAction)
        {
            // if mandatory visual tool is not specified
            if (MandatoryVisualTool == null)
                return visualToolAction.VisualTool;

            // get current visual tool
            WpfVisualTool result = visualToolAction.VisualTool;

            // if visual tool must be created
            if (!_visualToolItemToVisualTool.TryGetValue(visualToolAction, out result))
            {
                // if action visual tool is not specified
                if (visualToolAction.VisualTool == null)
                {
                    // use mandatory visual tool
                    result = MandatoryVisualTool;
                }
                else
                {
                    // create composite visual tool: mandatory visual tool + action visual tool
                    WpfCompositeVisualTool compositeTool = new WpfCompositeVisualTool(
                        MandatoryVisualTool, visualToolAction.VisualTool);
                    // set active visual tool
                    compositeTool.ActiveTool = visualToolAction.VisualTool;
                    result = compositeTool;
                }

                // save current visual tool
                _visualToolItemToVisualTool.Add(visualToolAction, result);
            }

            return result;
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Adds the visual tools actions to the specified <see cref="ToolStripMenuItem"/>.
        /// </summary>
        /// <param name="visualToolsMenuItem">The menu item.</param>
        private void AddVisualToolActionsToMenu(MenuItem visualToolsMenuItem)
        {
            foreach (VisualToolAction visualToolAction in VisualToolActions)
            {
                AddVisualToolActionToToolBar(
                    visualToolsMenuItem.Items,
                    visualToolAction,
                    _visualToolMenuItemDictionary);
            }
        }

        /// <summary>
        /// Removes the visual tools actions from the specified <see cref="ToolStripMenuItem"/>.
        /// </summary>
        /// <param name="visualToolsMenuItem">The menu item.</param>
        private void RemoveVisualToolActionsFromMenu(MenuItem visualToolsMenuItem)
        {
            foreach (VisualToolAction visualToolAction in VisualToolActions)
            {
                if (_visualToolMenuItemDictionary.Contains(visualToolAction))
                {
                    Control item = _visualToolMenuItemDictionary.GetItem(visualToolAction);

                    visualToolsMenuItem.Items.Remove(item);
                }
            }

            _visualToolMenuItemDictionary.Clear();
        }

        /// <summary>
        /// Subscribes to the events of visual tool action and sub-actions.
        /// </summary>
        /// <param name="visualToolAction">The visual tool action.</param>
        private void SubscribeToVisualToolActions(VisualToolAction visualToolAction)
        {
            if (visualToolAction is SeparatorToolBarAction)
                return;

            // subscribe to the visual tool action events
            visualToolAction.Activated += new EventHandler(visualToolAction_Activated);
            visualToolAction.Deactivated += new EventHandler(visualToolAction_Deactivated);
            visualToolAction.StatusChanged += new EventHandler(visualToolAction_StatusChanged);
            visualToolAction.IconChanged += new EventHandler(visualToolAction_IconChanged);

            // if action contains sub-actions
            if (visualToolAction.HasSubactions)
            {
                foreach (VisualToolAction subAction in visualToolAction.SubActions)
                    SubscribeToVisualToolActions(subAction);
            }
        }

        /// <summary>
        /// Adds the specified visual tool action to the specified collection.
        /// </summary>
        /// <param name="toolBarItemCollection">The item collection of toolbar.</param>
        /// <param name="visualToolAction">The visual tool action.</param>
        /// <param name="visualToolActionsDictionary">A dictionary, which contains links between visual tool actions and buttons.</param>
        private void AddVisualToolActionToToolBar(
            ItemCollection toolBarItemCollection,
            VisualToolAction visualToolAction,
            VisualToolActionButtonsDictionary visualToolActionsDictionary)
        {
            Control toolStripItem;
            // if new toolstrip item must be added to the collection of this toolstrip
            if (toolBarItemCollection == Items)
            {
                // create the toolstip item for visual tool action
                toolStripItem = CreateToolStripItem(visualToolAction, true);
            }
            // if new toolstrip item must be added to the collection of other toolstrip
            else
            {
                // create the toolstip item for visual tool action
                toolStripItem = CreateToolStripItem(visualToolAction, false);
            }

            // if action is not separator
            if (!(visualToolAction is SeparatorToolBarAction))
            {
                // save information about link between action and toolstrip item
                visualToolActionsDictionary.Add(visualToolAction, toolStripItem);

                // update action check state
                UpdateActionCheckState(visualToolAction);

                // subscribe to the item click event
                toolStripItem.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ActionItem_Click);

                // if visual tool action contains sub-actions
                if (visualToolAction.HasSubactions)
                {
                    ItemCollection subActions = null;
                    MenuItem menuItem = null;

                    if (toolStripItem is Button)
                    {
                        Button button = (Button)toolStripItem;
                        subActions = _dropDownButtonToItemCollection[(Button)toolStripItem];

                        Menu menu = _dropDownItemCollectionToMenu[subActions];
                        menuItem = (MenuItem)menu.Items[0];
                    }
                    else if (toolStripItem is MenuItem)
                    {
                        menuItem = (MenuItem)toolStripItem;
                        subActions = menuItem.Items;
                    }

                    menuItem.SubmenuOpened += MenuItem_SubmenuOpened;

                    // for each sub-action
                    foreach (VisualToolAction subAction in visualToolAction.SubActions)
                    {
                        AddVisualToolActionToToolBar(subActions, subAction, visualToolActionsDictionary);
                    }
                }
            }

            // add the toolstrip item to the toolstrip
            toolBarItemCollection.Add(toolStripItem);

            ItemCollection dropDownItemCollection = null;
            if (toolStripItem is Button &&
                _dropDownButtonToItemCollection.TryGetValue((Button)toolStripItem, out dropDownItemCollection))
                toolBarItemCollection.Add(_dropDownItemCollectionToMenu[dropDownItemCollection]);
        }

        /// <summary>
        /// Updates action status.
        /// </summary>
        private void visualToolAction_StatusChanged(object sender, EventArgs e)
        {
            _actionStatusLabel.Text = ((VisualToolAction)sender).Status;
        }

        /// <summary>
        /// Changes image viewer visual tool and checks the action.
        /// </summary>
        private void visualToolAction_Activated(object sender, EventArgs e)
        {
            // update previous action of visual tool
            _previousActionOfVisualTool = _currentActionOfVisualTool;

            _actionActivationCount++;

            VisualToolAction action = (VisualToolAction)sender;

            // update action check state
            UpdateActionCheckState(action);

            // if visual tool must be changed
            if (action.CanChangeImageViewerVisualTool)
            {
                if (_currentActionOfVisualTool != null)
                    // deactivate previous visual tool action
                    _currentActionOfVisualTool.Deactivate();

                if (_canChangeVisualTool && ImageViewer != null)
                {
                    // change visual tool
                    ImageViewer.VisualTool = GetVisualTool(action);

                    if (ImageViewer.VisualTool is WpfCompositeVisualTool)
                        ChangeActiveTool((WpfCompositeVisualTool)ImageViewer.VisualTool, action.VisualTool);
                }

                _currentActionOfVisualTool = action;
            }

            _actionActivationCount--;
        }

        /// <summary>
        /// Changes image viewer visual tool and unchecks the action.
        /// </summary>
        private void visualToolAction_Deactivated(object sender, EventArgs e)
        {
            VisualToolAction action = (VisualToolAction)sender;

            // update action check state
            UpdateActionCheckState(action);

            // if visual tool can not be changed
            if (!action.CanChangeImageViewerVisualTool)
                return;

            if (_canChangeVisualTool && ImageViewer != null)
            {
                bool isActiveToolChanged = false;

                if (ImageViewer.VisualTool is WpfCompositeVisualTool)
                    isActiveToolChanged = ChangeActiveTool((WpfCompositeVisualTool)ImageViewer.VisualTool, null);

                if (!isActiveToolChanged)
                {
                    // remove visual tool
                    ImageViewer.VisualTool = null;
                }
            }

            if (_currentActionOfVisualTool == action)
                // remove current visual tool action
                _currentActionOfVisualTool = null;
        }

        /// <summary>
        /// Updates icon of action.
        /// </summary>
        private void visualToolAction_IconChanged(object sender, EventArgs e)
        {
            // get action
            VisualToolAction action = (VisualToolAction)sender;

            if (_toolStripDictionary.Contains(action))
            {
                Control control = _toolStripDictionary.GetItem(action);
                // get tool strip item of action
                Button button = (Button)control;

                Image image = (Image)button.Content;

                // update icon
                image.Source = action.Icon;
            }

            if (_visualToolMenuItemDictionary.Contains(action))
            {
                // get tool strip item of action
                MenuItem visualToolsMenuItem = (MenuItem)_visualToolMenuItemDictionary.GetItem(action);
                // update icon
                SetMenuItemIcon(visualToolsMenuItem, action);
            }
        }

        /// <summary>
        /// Returns the visual tool action of specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The visual tool action.</returns>
        private VisualToolAction GetAction(Control item)
        {
            if (_toolStripDictionary.Contains(item))
                return _toolStripDictionary.GetAction(item);
            else if (_visualToolMenuItemDictionary.Contains(item))
                return _visualToolMenuItemDictionary.GetAction(item);
            else
                throw new InvalidOperationException();
        }

        /// <summary>
        /// Activates the action of drop-down menu.
        /// </summary>
        private void MenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            if (_dropItemCollectionToDownButton.ContainsKey(menuItem.Items))
            {
                Button button = _dropItemCollectionToDownButton[menuItem.Items];

                VisualToolAction action = GetAction(button);

                if (action != null && !action.IsActivated)
                    action.Activate();
            }
        }

        /// <summary>
        /// Activates the action.
        /// </summary>
        private void ActionItem_Click(object sender, MouseButtonEventArgs e)
        {
            VisualToolAction action = GetAction((Control)sender);

            // if action is activated
            if (action.IsActivated)
            {
                // show visual tool settings
                action.ShowVisualToolSettings();
            }
            else
            {
                // activate action
                action.Activate();
            }

            // click on action
            action.Click();
        }

        /// <summary>
        /// Updates the check state of action.
        /// </summary>
        /// <param name="visualToolAction">The visual tool action.</param>
        private void UpdateActionCheckState(VisualToolAction visualToolAction)
        {
            // if action can not be checked
            if (!visualToolAction.CheckActionButtonOnActivate)
                return;

            if (_toolStripDictionary.Contains(visualToolAction))
                UpdateToolStripItemCheckState(visualToolAction, _toolStripDictionary.GetItem(visualToolAction));

            if (_visualToolMenuItemDictionary.Contains(visualToolAction))
                UpdateToolStripItemCheckState(visualToolAction, _visualToolMenuItemDictionary.GetItem(visualToolAction));
        }

        /// <summary>
        /// Updates the check state the specified tool strip item of action.
        /// </summary>
        /// <param name="visualToolAction">The visual tool action.</param>
        /// <param name="toolStripItem">The tool strip item.</param>
        private void UpdateToolStripItemCheckState(VisualToolAction visualToolAction, Control toolStripItem)
        {
            if (toolStripItem is MenuItem)
            {
                MenuItem menuItem = (MenuItem)toolStripItem;

                if (menuItem.Icon == null)
                {
                    menuItem.IsChecked = visualToolAction.IsActivated;
                }
                else
                {
                    Border border = (Border)menuItem.Icon;

                    if (visualToolAction.IsActivated)
                    {
                        border.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 153, 255));
                    }
                    else
                    {
                        border.BorderBrush = Brushes.Transparent;
                    }
                }
            }
            else if (toolStripItem is Button)
            {
                Button button = toolStripItem as Button;

                if (visualToolAction.IsActivated)
                    button.BorderBrush = new SolidColorBrush(Color.FromRgb(51, 153, 255));
                else
                    button.BorderBrush = Brushes.Transparent;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Creates the tool strip item for specified action.
        /// </summary>
        /// <param name="visualToolAction">The visual tool action.</param>
        /// <param name="createToolStripButton">Indicates whether the tool strip button must be created.</param>
        /// <returns>
        /// The tool strip item
        /// </returns>
        private Control CreateToolStripItem(
            VisualToolAction visualToolAction,
            bool createToolStripButton)
        {
            // if tool strip separator must be created
            if (visualToolAction is SeparatorToolBarAction)
            {
                Separator separator = new Separator();

                if (createToolStripButton)
                {
                    separator.Margin = new Thickness(4, 0, 4, 0);
                }

                return separator;
            }

            // if tool strip button must be created
            if (createToolStripButton)
            {
                // create tool strip button
                Button button = new Button();

                if (visualToolAction.HasSubactions)
                {
                    MenuItem menuItem = null;
                    Menu menu = CreateDropDownMenu(out menuItem);

                    _dropDownButtonToItemCollection.Add(button, menuItem.Items);
                    _dropItemCollectionToDownButton.Add(menuItem.Items, button);
                    _dropDownItemCollectionToMenu.Add(menuItem.Items, menu);
                }

                // set tool strip item icon
                button.Content = CreateIconImageControl(visualToolAction);
                // set tool strip item tooltip
                button.ToolTip = visualToolAction.ToolTip;
                button.Background = Brushes.Transparent;
                button.BorderBrush = Brushes.Transparent;
                button.Focusable = false;

                return button;
            }
            else
            {
                // create tool strip menu item
                MenuItem menuItem = new MenuItem();

                SetMenuItemIcon(menuItem, visualToolAction);
                menuItem.Header = visualToolAction.Text;
                // set tool strip item tooltip
                menuItem.ToolTip = visualToolAction.ToolTip;

                return menuItem;
            }
        }

        /// <summary>
        /// Creates drop down menu.
        /// </summary>
        /// <param name="menuItem"></param>
        /// <returns>A drop down menu.</returns>
        private Menu CreateDropDownMenu(out MenuItem menuItem)
        {
            Menu menu = new Menu();
            menu.VerticalAlignment = VerticalAlignment.Center;
            menu.Margin = new Thickness(0, -1, 0, 0);
            menu.Background = Brushes.Transparent;

            menuItem = new MenuItem();

            Polygon polygon = new Polygon();
            polygon.Fill = Brushes.Black;
            polygon.FillRule = FillRule.EvenOdd;
            polygon.Points.Add(new Point(0, 0));
            polygon.Points.Add(new Point(4, 4));
            polygon.Points.Add(new Point(8, 0));

            Canvas canvas = new Canvas();
            canvas.Children.Add(polygon);
            canvas.Height = 16;
            polygon.Margin = new Thickness(-4, 6, 0, 0);

            menuItem.Header = canvas;

            menu.Items.Add(menuItem);

            return menu;
        }

        /// <summary>
        /// Creates a control for displaying an icon.
        /// </summary>
        /// <param name="visualToolAction">A visual tool action.</param>
        /// <returns>A control for displaying an icon.</returns>
        private Image CreateIconImageControl(VisualToolAction visualToolAction)
        {
            if (visualToolAction.Icon == null)
                return null;

            // create a control that displays an image
            System.Windows.Controls.Image image = new Image();
            image.Source = visualToolAction.Icon;
            image.Stretch = Stretch.None;
            image.Width = 16;
            image.Height = 16;
            return image;
        }

        /// <summary>
        /// Sets the icon of menu item.
        /// </summary>
        /// <param name="menuItem">The menu item.</param>
        /// <param name="visualToolAction">The visual tool action, which is associated with menu item.</param>
        private void SetMenuItemIcon(
            MenuItem menuItem,
            VisualToolAction visualToolAction)
        {
            if (visualToolAction.Icon == null)
            {
                menuItem.Icon = null;
            }
            else
            {
                if (menuItem.Icon == null)
                {
                    Border border = new Border();
                    border.BorderThickness = new Thickness(1);
                    border.Background = Brushes.Transparent;
                    border.BorderBrush = Brushes.Transparent;
                    border.Child = CreateIconImageControl(visualToolAction);
                    menuItem.Icon = border;
                }
                else
                {
                    Border border = (Border)menuItem.Icon;
                    Image image = (Image)border.Child;
                    image.Source = visualToolAction.Icon;
                }
            }
        }

        /// <summary>
        /// Changes the active visual tool in composite visual tool.
        /// </summary>
        /// <param name="compositeVisualTool">The composite visual tool.</param>
        /// <param name="activeTool">The active tool.</param>
        /// <returns>
        /// <b>True</b> if active visual tool is changed; otherwise, <b>false</b>.
        /// </returns>
        private bool ChangeActiveTool(WpfCompositeVisualTool compositeVisualTool, WpfVisualTool activeTool)
        {
            if (activeTool == null)
            {
                compositeVisualTool.ActiveTool = null;

                return true;
            }
            else
            {
                foreach (WpfVisualTool tool in compositeVisualTool)
                {
                    if (tool == activeTool)
                    {
                        compositeVisualTool.ActiveTool = activeTool;
                        return true;
                    }
                    else if (tool is WpfCompositeVisualTool)
                    {
                        WpfCompositeVisualTool nestedCompositeTool = (WpfCompositeVisualTool)tool;

                        if (ChangeActiveTool(nestedCompositeTool, activeTool))
                        {
                            compositeVisualTool.ActiveTool = nestedCompositeTool;
                            return true;
                        }
                    }
                }

                return false;
            }
        }


        #region Image Viewer

        /// <summary>
        /// Subscribes to image viewer events.
        /// </summary>
        /// <param name="imageViewer">The image viewer.</param>
        private void SubscribeToImageViewerEvents(WpfImageViewer imageViewer)
        {
            imageViewer.FocusedIndexChanged +=
                new PropertyChangedEventHandler<int>(ImageViewer_FocusedIndexChanged);
            imageViewer.VisualToolChanged +=
                new PropertyChangedEventHandler<WpfVisualTool>(ImageViewer_VisualToolChanged);
        }

        /// <summary>
        /// Unsubscribes from image viewer events.
        /// </summary>
        /// <param name="imageViewer">The image viewer.</param>
        private void UnsubscribeFromImageViewerEvents(WpfImageViewer imageViewer)
        {
            imageViewer.FocusedIndexChanged -= ImageViewer_FocusedIndexChanged;
            imageViewer.VisualToolChanged -= ImageViewer_VisualToolChanged;
        }

        /// <summary>
        /// Updates <see cref="ImageViewerToolStripItem.ToolStripItem"/> properties.
        /// </summary>
        private void ImageViewer_FocusedIndexChanged(object sender, PropertyChangedEventArgs<int> e)
        {
            bool isEnabled = false;
            if (e.NewValue != -1)
                isEnabled = true;

            IsEnabled = isEnabled;
            if (isEnabled)
            {
                UpdateSelectedItem();

                UpdateMenuItemTemplatePartBackground(this);
            }
        }

        /// <summary>
        /// Selects or deselects this item.
        /// </summary>
        private void ImageViewer_VisualToolChanged(object sender, PropertyChangedEventArgs<WpfVisualTool> e)
        {
            if (_actionActivationCount == 0)
                UpdateSelectedItem();
        }

        /// <summary>
        /// Updates the selected visual tool item.
        /// </summary>
        private void UpdateSelectedItem()
        {
            // if image viewer is not specified
            if (ImageViewer == null)
                return;

            // if current visual tool action is specified
            if (_currentActionOfVisualTool != null)
            {
                // if visual tool action is selected
                if (ImageViewer.VisualTool == GetVisualTool(_currentActionOfVisualTool))
                    return;
            }

            // get current visual tool action
            VisualToolAction currentVisualToolAction = _noneVisualToolAction;

            // for each visual tool actions
            foreach (VisualToolAction action in _visualToolActions)
            {
                // if action must be selected
                if (ImageViewer.VisualTool == GetVisualTool(action))
                {
                    currentVisualToolAction = action;
                    break;
                }
            }

            // if action is not activated
            if (!currentVisualToolAction.IsActivated)
            {
                _canChangeVisualTool = false;
                currentVisualToolAction.Activate();
                _canChangeVisualTool = true;
            }
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

        #endregion

    }
}
