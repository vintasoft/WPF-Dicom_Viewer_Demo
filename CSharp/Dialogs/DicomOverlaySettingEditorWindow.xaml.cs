using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;
using Vintasoft.Imaging.Dicom.Wpf.UI.VisualTools;
using Vintasoft.Imaging;
using Vintasoft.Imaging.Wpf.UI.VisualTools;

namespace WpfDicomViewerDemo
{
    /// <summary>
    /// A window that allows to change an overlay text settings of DICOM viewer.
    /// </summary>
    public partial class DicomOverlaySettingEditorWindow : Window
    {

        #region Fields

        /// <summary>
        /// The collection owner name.
        /// </summary>
        string _collectionOwnerName;

        /// <summary>
        /// The customizable visual tool (<see cref="WpfDicomViewerTool"/> or <see cref="WpfMprImageTool"/>).
        /// </summary>
        WpfVisualTool _visualTool;

        /// <summary>
        /// The default color.
        /// </summary>
        Color _defaultColor = Color.FromArgb(255, 255, 255, 64);

        /// <summary>
        /// The current selected anchor.
        /// </summary>
        AnchorType _currentSelectedAnchor = AnchorType.None;

        /// <summary>
        /// The supported text overlay items.
        /// </summary>
        List<WpfTextOverlay> _supportedItems = new List<WpfTextOverlay>();

        /// <summary>
        /// The dictionary: owner name => text overlay collection.
        /// </summary>
        private static Dictionary<string, WpfTextOverlayCollection> _ownerNameToTextOverlayDictionary = new Dictionary<string, WpfTextOverlayCollection>();

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DicomOverlaySettingEditorWindow"/> class.
        /// </summary>
        /// <param name="ownerName">The collection owner name.</param>
        /// <param name="tool">The customizable visual tool (<see cref="WpfDicomViewerTool"/> or <see cref="WpfMprImageTool"/>).</param>
        public DicomOverlaySettingEditorWindow(string ownerName, WpfVisualTool tool)
        {
            InitializeComponent();

            _visualTool = tool;
            _collectionOwnerName = ownerName;

            CheckAndInitOwnerInDictionary(ownerName, tool);
            InitVisualToolItems();
            Init();
            UpdateSelectedItemsListBox();
            UpdateUI();
        }

        #endregion



        #region Methods

        /// <summary>
        /// Sets the text overlay collection for visual tool.
        /// </summary>
        /// <param name="ownerName">The name of collection owner.</param>
        /// <param name="visualTool">The visual tool for which the text overlay collection must be set.</param>
        public static void SetTextOverlay(string ownerName, WpfVisualTool tool)
        {
            // the text overlay collection of visual tool
            WpfTextOverlayCollection toolTextOverlayCollection = null;
            // if visual tool is DICOM viewer tool
            if (tool is WpfDicomViewerTool)
                toolTextOverlayCollection = ((WpfDicomViewerTool)tool).TextOverlay;

            if (toolTextOverlayCollection != null)
            {
                toolTextOverlayCollection.Clear();
                foreach (WpfTextOverlay item in _ownerNameToTextOverlayDictionary[ownerName])
                {
                    toolTextOverlayCollection.Add((WpfTextOverlay)item.Clone());
                }
            }
        }

        /// <summary>
        /// Updates all text overlay elements in visual tool.
        /// </summary>
        private void DicomOverlaySettingEditorWindow_Closing(object sender, CancelEventArgs e)
        {
            SetTextOverlay(_collectionOwnerName, _visualTool);
        }


        #region Update UI

        /// <summary>
        /// Updates the user interface of this window.
        /// </summary>
        private void UpdateUI()
        {
            bool multipleViewIsSelected = selectedItemsListBox.SelectedItems.Count > 1;
            bool singleViewIsSelected = selectedItemsListBox.SelectedIndex != -1 && !multipleViewIsSelected;

            addButton.IsEnabled = supportedItemsListBox.SelectedIndex != -1;

            removeButton.IsEnabled = multipleViewIsSelected || singleViewIsSelected;
            moveUpButton.IsEnabled = singleViewIsSelected && selectedItemsListBox.SelectedIndex > 0;
            moveDownButton.IsEnabled = singleViewIsSelected && selectedItemsListBox.SelectedIndex != selectedItemsListBox.Items.Count - 1;

            groupButton.IsEnabled = multipleViewIsSelected;
            ungroupButton.IsEnabled = (selectedItemsListBox.SelectedItem is WpfTextOverlayGroup) && singleViewIsSelected;
        }

        /// <summary>
        /// Index of selected anchor is changed.
        /// </summary>
        private void anchorTypeEditor_SelectedAnchorTypeChanged(object sender, System.EventArgs e)
        {
            UpdateSelectedItemsListBox();

            UpdateUI();
        }

        /// <summary>
        /// Updates list box that contains text items with selected anchor property.
        /// </summary>
        private void UpdateSelectedItemsListBox()
        {
            // get selected anchor
            AnchorType selectedAnchor = anchorTypeEditor.SelectedAnchorType;

            if (selectedAnchor == _currentSelectedAnchor)
                return;

            _currentSelectedAnchor = selectedAnchor;

            selectedItemsListBox.BeginInit();
            // clear selected items list box
            selectedItemsListBox.Items.Clear();

            // for each text overlay item
            foreach (WpfTextOverlay item in _ownerNameToTextOverlayDictionary[_collectionOwnerName])
            {
                // if item has selected anchor
                if (item.Anchor == selectedAnchor)
                {
                    // add item to the selected items list box
                    selectedItemsListBox.Items.Add(item);
                }
            }
            selectedItemsListBox.EndInit();
        }

        /// <summary>
        /// Index of selected text overlay elements is changed.
        /// </summary>
        private void supportedItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }

        /// <summary>
        /// Index of selected text overlay elements is changed.
        /// </summary>
        private void selectedItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedTextOverlayPropertyGrid.SelectedObject = selectedItemsListBox.SelectedItem;

            UpdateUI();
        }

        /// <summary>
        /// Updates list box with supported items.
        /// </summary>
        private void UpdateSupportedItemsListBox()
        {
            supportedItemsListBox.BeginInit();

            // clear supported items list
            supportedItemsListBox.Items.Clear();

            foreach (WpfTextOverlay textOverlay in _supportedItems)
            {
                supportedItemsListBox.Items.Add(textOverlay);
            }

            supportedItemsListBox.EndInit();
        }

        /// <summary>
        /// Property value is changed.
        /// </summary>
        private void selectedTextOverlayPropertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
        {
            // update selected item in list box
            WpfTextOverlay textOverlay = (WpfTextOverlay)selectedItemsListBox.SelectedItem;
            selectedItemsListBox.Items.Refresh();
        }

        #endregion


        #region Init

        /// <summary>
        /// Initializes window controls.
        /// </summary>
        private void Init()
        {
            // init supported items
            _supportedItems.Add(new WpfTextOverlay());

            _supportedItems.Add(new WpfCustomDicomDataElementTextOverlay(0, 0));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.Unknown));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.PatientName));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.PatientID));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.PatientBirthDate));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.PatientSex));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.Manufacturer));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.StudyID));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.StudyDescription));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.SeriesDescription));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.ImageComments));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.StudyDate));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.StudyTime));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.XRayTubeCurrent, "{0}mA"));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.KVP, "{0}kV"));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.RepetitionTime, "RT: {0}"));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.EchoTime, "ET: {0}"));
            _supportedItems.Add(new WpfStandardDicomDataElementTextOverlay(DicomDataElementId.MagneticFieldStrength, "FS: {0}"));
            _supportedItems.Add(new WpfDicomImageVoiLutTextOverlay());
            _supportedItems.Add(new WpfPatientOrientationTextOverlay());
            _supportedItems.Add(new WpfCompressionInfoTextOverlay());
            _supportedItems.Add(new WpfRuntimeInfoTextOverlay("Im: {SeriesImageNumber}/{SeriesImageCount}"));

            _supportedItems.Sort(TextOverlayComparer);
            foreach (WpfTextOverlay textOverlay in _supportedItems)
                textOverlay.TextColor = _defaultColor;

            UpdateSupportedItemsListBox();
        }

        /// <summary>
        /// Compares the <see cref="WpfTextOverlay"/>.
        /// </summary>
        /// <param name="firstTextOverlay">The first text overlay.</param>
        /// <param name="secondTextOverlay">The second text overlay.</param>
        private int TextOverlayComparer(
            WpfTextOverlay firstTextOverlay,
            WpfTextOverlay secondTextOverlay)
        {
            return string.Compare(
                firstTextOverlay.ToString(),
                secondTextOverlay.ToString());
        }

        /// <summary>
        /// Checks owner name in dictionary and initializes if necessary.
        /// </summary>
        /// <param name="ownerName">The text overlay collection owner name.</param>
        /// <param name="tool">The visual tool.</param>
        private void CheckAndInitOwnerInDictionary(string ownerName, WpfVisualTool tool)
        {
            // if dictionary does not contain owner name
            if (!_ownerNameToTextOverlayDictionary.ContainsKey(ownerName))
            {
                // create new text overlay collection
                WpfTextOverlayCollection textOverlayCollection = new WpfTextOverlayCollection();

                // get visual tool text overlay collection
                WpfTextOverlayCollection toolTextOverlayCollection = null;
                // if tool is DICOM viewer tool
                if (tool is WpfDicomViewerTool)
                    toolTextOverlayCollection = ((WpfDicomViewerTool)tool).TextOverlay;

                if (toolTextOverlayCollection != null)
                {
                    // copy text overlay items
                    foreach (WpfTextOverlay item in toolTextOverlayCollection)
                        textOverlayCollection.Add((WpfTextOverlay)item.Clone());
                }

                // add owner to the dictionary
                _ownerNameToTextOverlayDictionary.Add(ownerName, textOverlayCollection);
            }
        }

        /// <summary>
        /// Initializes the visual tool text overlay elements for updating dynamically.
        /// </summary>
        private void InitVisualToolItems()
        {
            // get visual tool text overlay collection
            WpfTextOverlayCollection toolTextOverlayCollection = null;
            // if tool is dicom viewer tool
            if (_visualTool is WpfDicomViewerTool)
                toolTextOverlayCollection = ((WpfDicomViewerTool)_visualTool).TextOverlay;

            if (toolTextOverlayCollection != null)
            {
                toolTextOverlayCollection.Clear();
                // copy text overlay items
                foreach (WpfTextOverlay item in _ownerNameToTextOverlayDictionary[_collectionOwnerName])
                    toolTextOverlayCollection.Add(item);
            }
        }

        #endregion


        #region Add Selected Item

        /// <summary>
        /// Adds text overlay element into elements, which should be displayed on viewer.
        /// </summary>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            AddSelectedItem();
        }

        /// <summary>
        /// Adds text overlay element into elements, which should be displayed on viewer.
        /// </summary>
        private void supportedItemsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (supportedItemsListBox.SelectedItem != null)
            {
                int itemIndex = GetSupportedListBoxItemIndexHoveredByMouse();

                if (itemIndex != -1 &&
                    supportedItemsListBox.SelectedItem == supportedItemsListBox.Items[itemIndex])
                    AddSelectedItem();
            }
        }

        /// <summary>
        /// Returns item index of supported list box item, which is hovered by mouse.
        /// </summary>
        /// <returns>
        /// Supported list box item index.
        /// </returns>
        private int GetSupportedListBoxItemIndexHoveredByMouse()
        {
            // for each supported list box item
            for (int index = 0; index < supportedItemsListBox.Items.Count; index++)
            {
                // get list box item
                ListBoxItem listBoxItem = supportedItemsListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                if (listBoxItem != null)
                {
                    // if mouse over item
                    if (listBoxItem.IsMouseOver)
                        // return item index
                        return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Adds the selected item to the listbox with selected text overlay elements.
        /// </summary>
        private void AddSelectedItem()
        {
            // copy a text overlay item
            WpfTextOverlay textOverlay = (WpfTextOverlay)((WpfTextOverlay)supportedItemsListBox.SelectedItem).Clone();
            // set anchor to the item
            textOverlay.Anchor = anchorTypeEditor.SelectedAnchorType;
            AddItem(textOverlay);
        }

        /// <summary>
        /// Adds specified item to the listbox with selected text overlay elements.
        /// </summary>
        /// <param name="textOverlay">The text overlay item.</param>
        private void AddItem(WpfTextOverlay textOverlay)
        {
            // add item to the list
            selectedItemsListBox.Items.Add(textOverlay);
            _ownerNameToTextOverlayDictionary[_collectionOwnerName].Add(textOverlay);

            // get visual tool collection
            WpfTextOverlayCollection toolTextOverlayCollection = null;
            if (_visualTool is WpfDicomViewerTool)
                toolTextOverlayCollection = ((WpfDicomViewerTool)_visualTool).TextOverlay;

            if (toolTextOverlayCollection != null)
                toolTextOverlayCollection.Add(textOverlay);

            UpdateUI();
        }

        #endregion


        #region Remove Selected Item

        /// <summary>
        /// Removes the selected item from the listbox with selected text overlay elements.
        /// </summary>
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            // save selected items to the new array
            int selectedItemsCount = selectedItemsListBox.SelectedItems.Count;
            WpfTextOverlay[] selectedItems = new WpfTextOverlay[selectedItemsCount];
            int selectedItemsIndex = 0;
            foreach (object item in selectedItemsListBox.SelectedItems)
            {
                selectedItems[selectedItemsIndex] = (WpfTextOverlay)item;
                selectedItemsIndex++;
            }

            RemoveItems(selectedItems);
        }

        /// <summary>
        /// Removes the specified items from the listbox with selected text overlay elements.
        /// </summary>
        /// <param name="items">The items to remove.</param>
        private void RemoveItems(params WpfTextOverlay[] items)
        {
            // get visual tool collection
            WpfTextOverlayCollection toolTextOverlayCollection = null;
            if (_visualTool is WpfDicomViewerTool)
                toolTextOverlayCollection = ((WpfDicomViewerTool)_visualTool).TextOverlay;

            foreach (WpfTextOverlay item in items)
            {
                // remove text overlay from selected items list box
                selectedItemsListBox.Items.Remove(item);
                _ownerNameToTextOverlayDictionary[_collectionOwnerName].Remove(item);

                if (toolTextOverlayCollection != null)
                    toolTextOverlayCollection.Remove(item);
            }

            UpdateUI();
        }

        #endregion


        #region Move/Group Selected Item

        /// <summary>
        /// Data element view is moved up.
        /// </summary>
        private void moveUpButton_Click(object sender, RoutedEventArgs e)
        {
            MoveItemInSelectedItemsCollection(-1);
        }

        /// <summary>
        /// Data element view is moved down.
        /// </summary>
        private void moveDownButton_Click(object sender, RoutedEventArgs e)
        {
            MoveItemInSelectedItemsCollection(+1);
        }

        /// <summary>
        /// Moves selected item in selected items collection on specified step count.
        /// </summary>
        /// <param name="steps">The step count.</param>
        private void MoveItemInSelectedItemsCollection(int steps)
        {
            int selectedIndex = selectedItemsListBox.SelectedIndex;
            int movedIndex = selectedIndex + steps;
            if (movedIndex < 0)
                movedIndex = 0;
            if (movedIndex > selectedItemsListBox.Items.Count - 1)
                movedIndex = selectedItemsListBox.Items.Count - 1;

            WpfTextOverlay movedItem = (WpfTextOverlay)selectedItemsListBox.Items[movedIndex];
            WpfTextOverlay selectedTextOverlay = (WpfTextOverlay)selectedItemsListBox.Items[selectedIndex];
            selectedItemsListBox.Items.Remove(selectedTextOverlay);
            selectedItemsListBox.Items.Insert(movedIndex, selectedTextOverlay);
            selectedItemsListBox.SelectedIndex = movedIndex;

            // get text overlay collection
            WpfTextOverlayCollection collection = _ownerNameToTextOverlayDictionary[_collectionOwnerName];
            MoveTextOverlayItemInCollection(movedItem, selectedTextOverlay, collection, steps);

            // get visual tool collection
            WpfTextOverlayCollection toolTextOverlayCollection = null;
            if (_visualTool is WpfDicomViewerTool)
                toolTextOverlayCollection = ((WpfDicomViewerTool)_visualTool).TextOverlay;

            if (toolTextOverlayCollection != null)
                MoveTextOverlayItemInCollection(movedItem, selectedTextOverlay, toolTextOverlayCollection, steps);

            UpdateUI();
        }

        /// <summary>
        /// Moves specified item in specified collection in position of moved item.
        /// </summary>
        /// <param name="movedItem">The moved item.</param>
        /// <param name="selectedItem">The selected item.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="direction">The insert direction.</param>
        private void MoveTextOverlayItemInCollection(
            WpfTextOverlay movedItem,
            WpfTextOverlay selectedItem,
            WpfTextOverlayCollection collection,
            int direction)
        {
            // remove from collection
            collection.Remove(selectedItem);
            // get index of previous item
            int indexOfPreviousItem = collection.IndexOf(movedItem);
            int indexToMove = indexOfPreviousItem;
            if (direction > 0)
                indexToMove++;

            if (indexToMove < 0)
                indexToMove = 0;
            if (indexToMove > collection.Count - 1)
                indexToMove = collection.Count - 1;
            // insert item
            collection.Insert(indexToMove, selectedItem);
        }

        /// <summary>
        /// Groups the selected items.
        /// </summary>
        private void groupButton_Click(object sender, RoutedEventArgs e)
        {
            // save selected items to the new array
            int selectedItemsCount = selectedItemsListBox.SelectedItems.Count;
            WpfTextOverlay[] selectedItems = new WpfTextOverlay[selectedItemsCount];
            int selectedItemsIndex = 0;
            foreach (object item in selectedItemsListBox.SelectedItems)
            {
                selectedItems[selectedItemsIndex] = (WpfTextOverlay)item;
                selectedItemsIndex++;
            }

            RemoveItems(selectedItems);

            WpfTextOverlayGroup group = new WpfTextOverlayGroup(anchorTypeEditor.SelectedAnchorType, selectedItems);
            group.TextFont = selectedItems[0].TextFont;
            group.TextColor = selectedItems[0].TextColor;
            AddItem(group);
        }

        /// <summary>
        /// Ungroups the selected items.
        /// </summary>
        private void ungroupButton_Click(object sender, RoutedEventArgs e)
        {
            WpfTextOverlayGroup group = selectedItemsListBox.SelectedItem as WpfTextOverlayGroup;
            RemoveItems(group);

            foreach (WpfTextOverlay item in group.Items)
            {
                AddItem(item);
            }
        }

        #endregion

        #endregion

        
    }
}
