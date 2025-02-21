using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

using Vintasoft.Imaging.Wpf.UI.VisualTools;

namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// A panel that allows to show the selected cursor and change the selected cursor.
    /// </summary>
    [DefaultProperty("SelectedCursor")]
    public partial class CursorPanelControl : ComboBox
    {

        #region Fields

        /// <summary>
        /// Available cursors.
        /// </summary>
        List<Cursor> _cursors = new List<Cursor>();

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorPanelControl"/> class.
        /// </summary>
        public CursorPanelControl()
        {
            BeginInit();

            Items.Clear();

            AddCursor("CloseHandCursor (VintaSoft)", WpfToolCursors.CloseHandCursor);
            AddCursor("OpenHandCursor (VintaSoft)", WpfToolCursors.OpenHandCursor);
            AddCursor("CropSelectionCursor (VintaSoft)", WpfToolCursors.CropSelectionCursor);
            AddCursor("MagnifierCursor (VintaSoft)", WpfToolCursors.MagnifierCursor);
            AddCursor("RotateCursor (VintaSoft)", WpfToolCursors.RotateCursor);
            AddCursor("ZoomCursor (VintaSoft)", WpfToolCursors.ZoomCursor);

            AddCursor("AppStarting", Cursors.AppStarting);
            AddCursor("Arrow", Cursors.Arrow);
            AddCursor("ArrowCD", Cursors.ArrowCD);
            AddCursor("Cross", Cursors.Cross);
            AddCursor("Hand", Cursors.Hand);
            AddCursor("Help", Cursors.Help);
            AddCursor("IBeam", Cursors.IBeam);
            AddCursor("No", Cursors.No);
            AddCursor("None", Cursors.None);
            AddCursor("Pen", Cursors.Pen);
            AddCursor("ScrollAll", Cursors.ScrollAll);
            AddCursor("ScrollE", Cursors.ScrollE);
            AddCursor("ScrollN", Cursors.ScrollN);
            AddCursor("ScrollNE", Cursors.ScrollNE);
            AddCursor("ScrollNS", Cursors.ScrollNS);
            AddCursor("ScrollNW", Cursors.ScrollNW);
            AddCursor("ScrollS", Cursors.ScrollS);
            AddCursor("ScrollSE", Cursors.ScrollSE);
            AddCursor("ScrollSW", Cursors.ScrollSW);
            AddCursor("ScrollW", Cursors.ScrollW);
            AddCursor("ScrollWE", Cursors.ScrollWE);
            AddCursor("SizeAll", Cursors.SizeAll);
            AddCursor("SizeNESW", Cursors.SizeNESW);
            AddCursor("SizeNS", Cursors.SizeNS);
            AddCursor("SizeNWSE", Cursors.SizeNWSE);
            AddCursor("SizeWE", Cursors.SizeWE);
            AddCursor("UpArrow", Cursors.UpArrow);
            AddCursor("Wait", Cursors.Wait);

            SelectedCursor = null;

            EndInit();
        }

        #endregion



        #region Properties

        /// <summary>
        /// Gets or sets the current selected cursor.
        /// </summary>
        /// <value>
        /// Default value is <b>null</b>.
        /// </value>
        [Browsable(false)]
        public Cursor SelectedCursor
        {
            get
            {
                if (SelectedIndex == -1)
                    return null;

                return _cursors[SelectedIndex];
            }
            set
            {
                SelectedIndex = _cursors.IndexOf(value);
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Adds a cursor to the control.
        /// </summary>
        /// <param name="name">The cursor name.</param>
        /// <param name="cursor">The cursor.</param>
        private void AddCursor(string name, Cursor cursor)
        {
            _cursors.Add(cursor);
            Items.Add(name);
        }

        #endregion

    }
}
