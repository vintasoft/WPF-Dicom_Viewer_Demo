using System;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Wpf.UI.VisualTools;


namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Stores information about an action of <see cref="VisualTool"/>.
    /// </summary>
    public class VisualToolAction
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualToolAction"/> class.
        /// </summary>
        /// <param name="visualTool">The visual tool.</param>
        /// <param name="text">The action text.</param>
        /// <param name="toolTip">The action tool tip.</param>
        /// <param name="icon">The action icon.</param>
        /// <param name="subActions">The sub-actions of the action.</param>
        public VisualToolAction(
            WpfVisualTool visualTool,
            string text,
            string toolTip,
            BitmapSource icon,
            params VisualToolAction[] subActions)
            : this(visualTool, text, toolTip, icon, true, subActions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualToolAction"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="isCheckOnActivate">Indicate whether the action must be checked after activate.</param>
        /// <param name="subActions">The sub action of the action.</param>
        public VisualToolAction(
            string text,
            string toolTip,
            BitmapSource icon,
            bool isCheckOnActivate,
            params VisualToolAction[] subActions)
            : this(null, text, toolTip, icon, isCheckOnActivate, subActions)
        {
            _canChangeImageViewerVisualTool = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualToolAction"/> class.
        /// </summary>
        /// <param name="visualTool">The visual tool.</param>
        /// <param name="text">The action text.</param>
        /// <param name="toolTip">The action tool tip.</param>
        /// <param name="icon">The action icon.</param>
        /// <param name="isCheckOnActivate">Indicate whether the action must be checked after activate.</param>
        /// <param name="subActions">The sub-actions of the action.</param>
        public VisualToolAction(
            WpfVisualTool visualTool,
            string text,
            string toolTip,
            BitmapSource icon,
            bool isCheckOnActivate,
            params VisualToolAction[] subActions)
        {
            _isActivated = false;

            _visualTool = visualTool;
            _text = text;
            _toolTip = toolTip;
            _icon = icon;
            _checkActionButtonOnActivate = isCheckOnActivate;

            if (subActions == null)
                subActions = new VisualToolAction[0];

            SetSubActions(subActions);
        }

        #endregion



        #region Properties

        WpfVisualTool _visualTool;
        /// <summary>
        /// Gets the visual tool.
        /// </summary>
        public virtual WpfVisualTool VisualTool
        {
            get
            {
                return _visualTool;
            }
        }

        string _text;
        /// <summary>
        /// Gets the action text.
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
        }

        string _toolTip;
        /// <summary>
        /// Gets the action tool tip.
        /// </summary>
        public string ToolTip
        {
            get
            {
                return _toolTip;
            }
        }

        BitmapSource _icon;
        /// <summary>
        /// Gets the icon of the action.
        /// </summary>
        public BitmapSource Icon
        {
            get
            {
                return _icon;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this action is enabled.
        /// </summary>
        public virtual bool IsEnabled
        {
            get
            {
                if (VisualTool != null)
                    return VisualTool.Enabled;
                return true;
            }
        }

        bool _isActivated;
        /// <summary>
        /// Gets a value indicating whether this action is activated.
        /// </summary>
        public virtual bool IsActivated
        {
            get
            {
                return _isActivated;
            }
        }

        VisualToolAction[] _subActions;
        /// <summary>
        /// Gets the sub-actions of this action.
        /// </summary>
        public VisualToolAction[] SubActions
        {
            get
            {
                return _subActions;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this action has sub-actions.
        /// </summary>
        /// <value>
        /// <b>True</b> if this action has sub-actions; otherwise, <b>false</b>.
        /// </value>
        public bool HasSubactions
        {
            get
            {
                return _subActions != null && _subActions.Length > 0;
            }
        }

        VisualToolAction _parent;
        /// <summary>
        /// Gets the parent action.
        /// </summary>
        public VisualToolAction Parent
        {
            get
            {
                return _parent;
            }
        }

        string _status = string.Empty;
        /// <summary>
        /// Gets the status of this action.
        /// </summary>
        public string Status
        {
            get
            {
                return _status;
            }
        }

        bool _checkActionButtonOnActivate = true;
        /// <summary>
        /// Gets a value indicating whether the action button must be checked when button is activated.
        /// </summary>
        /// <value>
        /// <b>True</b> if the action button must be checked when button is activated; otherwise, <b>false</b>.
        /// </value>
        public virtual bool CheckActionButtonOnActivate
        {
            get
            {
                return _checkActionButtonOnActivate;
            }
        }

        bool _canChangeImageViewerVisualTool = true;
        /// <summary>
        /// Gets a value indicating whether the action can change the visual tool of image viewer.
        /// </summary>
        /// <value>
        /// <b>True</b> if action can change the visual tool of image viewer; otherwise, <b>false</b>.
        /// </value>
        public virtual bool CanChangeImageViewerVisualTool
        {
            get
            {
                return _canChangeImageViewerVisualTool;
            }
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Activates this action.
        /// </summary>
        public virtual void Activate()
        {
            if (IsActivated)
                return;

            _isActivated = true;

            // if visual tool is specified
            if (VisualTool != null)
                // subscribe to loading exeption of visual tool
                VisualTool.ImageLoadingException += new EventHandler<ExceptionEventArgs>(Tool_ImageLoadingException);

            SetStatus(string.Empty);

            OnActivated(EventArgs.Empty);
        }

        /// <summary>
        /// Deactivates this action.
        /// </summary>
        public virtual void Deactivate()
        {
            if (!IsActivated)
                return;

            // if visual tool is specified
            if (VisualTool != null)
                // unsubscribe to loading exeption of visual tool
                VisualTool.ImageLoadingException -= Tool_ImageLoadingException;

            SetStatus(string.Empty);

            _isActivated = false;

            OnDeactivated(EventArgs.Empty);
        }

        /// <summary>
        /// Shows the visual tool settings.
        /// </summary>
        public virtual void ShowVisualToolSettings()
        {
            OnShowSettings(EventArgs.Empty);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (VisualTool == null)
            {
                if (string.IsNullOrEmpty(Text))
                {
                    if (string.IsNullOrEmpty(ToolTip))
                        return base.ToString();

                    return ToolTip;
                }

                return Text;
            }

            return VisualTool.ToolName;
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Sets the parent of this action.
        /// </summary>
        /// <param name="parent">The parent.</param>
        protected virtual void SetParent(VisualToolAction parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Sets the sub-actions.
        /// </summary>
        /// <param name="actions">The sub-actions.</param>
        protected virtual void SetSubActions(VisualToolAction[] actions)
        {
            foreach (VisualToolAction action in actions)
                action.SetParent(this);

            _subActions = actions;
        }

        /// <summary>
        /// Sets the action status.
        /// </summary>
        /// <param name="status">The status.</param>
        protected virtual void SetStatus(string status)
        {
            if (_status != status)
            {
                _status = status;

                if (IsActivated)
                    OnStatusChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Sets the action icon.
        /// </summary>
        /// <param name="icon">The icon.</param>
        protected virtual void SetIcon(BitmapSource icon)
        {
            if (_icon != icon)
            {
                _icon = icon;

                OnIconChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Activated" /> event.
        /// </summary>
        protected virtual void OnActivated(EventArgs e)
        {
            if (Activated != null)
                Activated(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Deactivated" /> event.
        /// </summary>
        protected virtual void OnDeactivated(EventArgs e)
        {
            if (Deactivated != null)
                Deactivated(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Clicked" /> event.
        /// </summary>
        protected virtual void OnClicked(EventArgs e)
        {
            if (Clicked != null)
                Clicked(this, e);
        }


        /// <summary>
        /// Raises the <see cref="E:ShowSettings" /> event.
        /// </summary>
        protected virtual void OnShowSettings(EventArgs e)
        {
            if (ShowSettings != null)
                ShowSettings(this, e);
        }


        /// <summary>
        /// Raises the <see cref="E:StatusChanged" /> event.
        /// </summary>
        protected virtual void OnStatusChanged(EventArgs e)
        {
            if (StatusChanged != null)
                StatusChanged(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:IconChanged" /> event.
        /// </summary>
        protected virtual void OnIconChanged(EventArgs e)
        {
            if (IconChanged != null)
                IconChanged(this, e);
        }

        #endregion


        #region INTERNAL

        /// <summary>
        /// The action is clicked.
        /// </summary>
        internal virtual void Click()
        {
            OnClicked(EventArgs.Empty);
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Shows the image loaded errors.
        /// </summary>
        private void Tool_ImageLoadingException(object sender, ExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Visual tool");
        }

        #endregion

        #endregion



        #region Events

        /// <summary>
        /// Occurs when action is activated.
        /// </summary>
        public event EventHandler Activated;

        /// <summary>
        /// Occurs when action is deactivated.
        /// </summary>
        public event EventHandler Deactivated;

        /// <summary>
        /// Occurs when action is shows settings.
        /// </summary>
        public event EventHandler ShowSettings;

        /// <summary>
        /// Occurs when action is clicked.
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// Occurs when status is changed.
        /// </summary>
        public event EventHandler StatusChanged;

        /// <summary>
        /// Occurs when the icon is changed.
        /// </summary>
        public event EventHandler IconChanged;

        #endregion

    }
}
