using System;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Provides data for the <see cref="ImageViewerToolbar.PageIndexChanged"/> event.
    /// </summary>
    public class PageIndexChangedEventArgs : EventArgs
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PageIndexChangedEventArgs"/> class.
        /// </summary>
        /// <param name="selectedPageIndex">Index of the selected page.</param>
        public PageIndexChangedEventArgs(int selectedPageIndex)
        {
            _selectedPageIndex = selectedPageIndex;
        }

        #endregion



        #region Properties

        int _selectedPageIndex;
        /// <summary>
        /// Gets the selected page index.
        /// </summary>
        public int SelectedPageIndex
        {
            get
            {
                return _selectedPageIndex;
            }
        }

        #endregion

    }
}
