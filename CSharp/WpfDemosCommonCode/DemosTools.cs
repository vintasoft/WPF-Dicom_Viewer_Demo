using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.Win32;

using Vintasoft.Imaging;
using Vintasoft.Imaging.Codecs;
using Vintasoft.Imaging.ImageRendering;
using Vintasoft.Imaging.Pdf;
using Vintasoft.Imaging.UIActions;
using Vintasoft.Imaging.Wpf.Codecs;
using Vintasoft.Imaging.Wpf.UI;
using Vintasoft.Imaging.Wpf.UI.VisualTools;


namespace WpfDemosCommonCode
{
    /// <summary>
    /// Contains collection of helper-algorithms for demo applications.
    /// </summary>
    public class DemosTools
    {

        #region Constants

        /// <summary>
        /// The brush for selected control.
        /// </summary>
        public static readonly System.Windows.Media.Brush SELECTED_CONTROL_BRUSH;

        /// <summary>
        /// The brush for unselected control.
        /// </summary>
        public static readonly System.Windows.Media.Brush UNSELECTED_CONTROL_BRUSH;

        #endregion



        #region Constructors

        /// <summary>
        /// Initializes the <see cref="DemosTools"/> class.
        /// </summary>
        static DemosTools()
        {
            SELECTED_CONTROL_BRUSH = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(50, 150, 255));
            UNSELECTED_CONTROL_BRUSH = System.Windows.Media.Brushes.Transparent;
        }

        #endregion



        #region Methods

        /// <summary>
        /// Finds the specified string in the <see cref="ListBox"/>.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <see cref="ListBoxItem"/> object if string is found successfully; otherwise, <b>null</b>.
        /// </returns>
        public static ListBoxItem Find(ListBox control, string value)
        {
            foreach (ListBoxItem item in control.Items)
            {
                if (Equals(item.Content, value))
                    return item;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the control contains the object.
        /// </summary>
        /// <param name="control">The <see cref="ListBox"/>.</param>
        /// <param name="value">The value to locate in the <see cref="ListBox" />.</param>
        /// <returns>
        /// <b>True</b> if <paramref name="value" /> is found in the <see cref="ListBox" />; otherwise, <b>false</b>.
        /// </returns>
        public static bool Contains(ListBox control, string value)
        {
            return Find(control, value) != null;
        }

        /// <summary>
        /// Sets the selected item in <see cref="ListBox"/>.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The value.</param>
        public static void SetSelectedItem(ListBox control, string value)
        {
            control.SelectedItem = Find(control, value);
        }

        /// <summary>
        /// Parses the float value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="fieldName">The field name.</param>
        /// <param name="value">The value.</param>
        public static bool ParseFloat(string text, string fieldName, out float value)
        {
            if (ParseFloat(text, out value))
                return true;
            ShowErrorMessage(string.Format("{0} has invalid format.", fieldName));
            return false;
        }

        /// <summary>
        /// Parses the float value.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        public static bool ParseFloat(string text, out float value)
        {
            return float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        /// <summary>
        /// Convert float value to string.
        /// </summary>
        /// <param name="value">The value.</param>
        public static string ToString(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert float value to string.
        /// </summary>
        /// <param name="value">The value.</param>
        public static string ToString(float? value)
        {
            if (value.HasValue)
                return ToString(value.Value);
            return "";
        }


        /// <summary>
        /// Opens the browser with specified URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        public static void OpenBrowser(string url)
        {
            ProcessStartInfo pi = new ProcessStartInfo("cmd", string.Format("/c start {0}", url));
            pi.CreateNoWindow = true;
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(pi);
        }

        /// <summary>
        /// Converts PdfDocumentConformance to a string.
        /// </summary>
        public static string ConvertToString(PdfDocumentConformance documentConformance)
        {
            switch (documentConformance)
            {
                case PdfDocumentConformance.PdfA_1a:
                    return "PDF/A-1a";
                case PdfDocumentConformance.PdfA_1b:
                    return "PDF/A-1b";
                case PdfDocumentConformance.PdfA_2a:
                    return "PDF/A-2a";
                case PdfDocumentConformance.PdfA_2b:
                    return "PDF/A-2b";
                case PdfDocumentConformance.PdfA_2u:
                    return "PDF/A-2u";
                case PdfDocumentConformance.PdfA_3a:
                    return "PDF/A-3a";
                case PdfDocumentConformance.PdfA_3b:
                    return "PDF/A-3b";
                case PdfDocumentConformance.PdfA_3u:
                    return "PDF/A-3u";
                case PdfDocumentConformance.PdfA_4:
                    return "PDF/A-4";
                case PdfDocumentConformance.PdfA_4e:
                    return "PDF/A-4e";
                case PdfDocumentConformance.PdfA_4f:
                    return "PDF/A-4f";
            }
            return null;
        }

        /// <summary>
        /// Converts string to a PdfDocumentConformance.
        /// </summary>
        public static PdfDocumentConformance ConvertFromString(string documentConformance)
        {
            switch (documentConformance)
            {
                case "PDF/A-1a":
                    return PdfDocumentConformance.PdfA_1a;

                case "PDF/A-1b":
                    return PdfDocumentConformance.PdfA_1b;

                case "PDF/A-2a":
                    return PdfDocumentConformance.PdfA_2a;

                case "PDF/A-2b":
                    return PdfDocumentConformance.PdfA_2b;

                case "PDF/A-2u":
                    return PdfDocumentConformance.PdfA_2u;

                case "PDF/A-3a":
                    return PdfDocumentConformance.PdfA_3a;

                case "PDF/A-3b":
                    return PdfDocumentConformance.PdfA_3b;

                case "PDF/A-3u":
                    return PdfDocumentConformance.PdfA_3u;

                case "PDF/A-4":
                    return PdfDocumentConformance.PdfA_4;

                case "PDF/A-4e":
                    return PdfDocumentConformance.PdfA_4e;

                case "PDF/A-4f":
                    return PdfDocumentConformance.PdfA_4f;
            }

            return PdfDocumentConformance.Undefined;
        }

        /// <summary>
        /// Shows a error(exception) message.
        /// </summary>
        /// <param name="caption">MessageBox caption.</param>
        /// <param name="ex">The exception.</param>
        public static void ShowErrorMessage(string caption, Exception ex)
        {
            MessageBox.Show(GetFullExceptionMessage(ex), caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows an error(exception) message.
        /// </summary>
        /// <param name="caption">MessageBox caption.</param>
        /// <param name="message">The text of error.</param>
        public static void ShowErrorMessage(string caption, string message)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows an error(exception) message.
        /// </summary>
        /// <param name="message">The text of error.</param>
        public static void ShowErrorMessage(string message)
        {
            ShowWarningMessage("Error", message);
        }

        /// <summary>
        /// Shows an error(exception) message.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public static void ShowErrorMessage(Exception ex)
        {
            ShowErrorMessage("Error", ex);
        }

        /// <summary>
        /// Shows an error(exception) message.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="filename">A filename.</param>
        public static void ShowErrorMessage(Exception ex, string filename)
        {
            MessageBox.Show(string.Format("{0} ({1})", ex.Message, Path.GetFileName(filename)), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Returns a message of exception and inner exceptions.
        /// </summary>
        public static string GetFullExceptionMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Message);

            Exception innerException = ex.InnerException;
            if (innerException != null && ex.Message != innerException.Message)
            {
                while (innerException != null)
                {
                    sb.AppendLine(string.Format("Inner exception: {0}", innerException.Message));
                    innerException = innerException.InnerException;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Shows a warning message.
        /// </summary>
        /// <param name="caption">MessageBox caption.</param>
        /// <param name="message">The message.</param>
        public static void ShowWarningMessage(string caption, string message)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Shows a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void ShowWarningMessage(string message)
        {
            ShowWarningMessage("Warning", message);
        }

        /// <summary>
        /// Shows an information message.
        /// </summary>
        /// <param name="caption">MessageBox caption.</param>
        /// <param name="message">The message.</param>
        public static void ShowInfoMessage(string caption, string message)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Shows an information message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void ShowInfoMessage(string message)
        {
            ShowInfoMessage("Information", message);
        }

        /// <summary>
        /// Sets the folder with test files as initial folder for the file dialog.
        /// </summary>
        /// <param name="fileDialog">The file dialog.</param>
        public static void SetTestFilesFolder(FileDialog fileDialog)
        {
            string imagesFolder = FindTestFilesFolder();
            if (imagesFolder != "")
                fileDialog.InitialDirectory = imagesFolder;
        }


        /// <summary>
        /// Sets the folder with test XLSX files as initial folder for the file dialog.
        /// </summary>
        /// <param name="fileDialog">The file dialog.</param>
        public static void SetTestXlsxFolder(FileDialog fileDialog)
        {
            string imagesFolder = FindTestFilesFolder();
            if (imagesFolder != "")
                fileDialog.InitialDirectory = Path.Combine(imagesFolder, "Xlsx");
        }

        /// <summary>
        /// Returns a full path to the folder with test files.
        /// </summary>
        /// <returns>
        /// Full path to the folder with test files if folder is found; otherwise, empty string.
        /// </returns>
        public static string FindTestFilesFolder()
        {
            try
            {
                // search folders
                string[] folders = new string[]
                    {
                        @"..\..\..\TestFiles\",
                        @"..\..\..\..\TestFiles\",
                        @"..\..\..\..\..\TestFiles\",
                        @"..\..\..\..\..\..\..\TestFiles\"
                    };
                string demoBinFolder = Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName);
                foreach (string dir in folders)
                {
                    string testFilesFolder = Path.Combine(demoBinFolder, dir);
                    if (Directory.Exists(testFilesFolder))
                        return Path.GetFullPath(testFilesFolder);
                }
            }
            catch
            {
            }

            return "";
        }

        /// <summary>
        /// Reloads the images in the specified image viewer.
        /// </summary>
        /// <param name="imageViewer">The image viewer.</param>
        public static void ReloadImagesInViewer(WpfImageViewerBase imageViewer)
        {
            try
            {
                ImageCollection images = imageViewer.Images;
                int focusedIndex = imageViewer.FocusedIndex;
                VintasoftImage focusedImage = null;
                if (focusedIndex >= 0 && focusedIndex < images.Count)
                {
                    focusedImage = images[focusedIndex];
                    if (focusedImage != null)
                        focusedImage.Reload(true);
                }

                foreach (VintasoftImage nextImage in imageViewer.Images)
                {
                    if (nextImage != focusedImage)
                        nextImage.Reload(true);
                }
            }
            catch (Exception e)
            {
                ShowErrorMessage(e);
            }
        }

        /// <summary>
        /// Loads the XPS codec.
        /// </summary>
        /// <remarks>
        /// Call this method before any of the <see cref="CodecsFileFilters.SetFilters"/> method calls.
        /// </remarks>
        public static void LoadXpsCodec()
        {
            AvailableCodecs.AddCodec(new XpsCodec());
        }

        /// <summary>
        /// Set rendering requirement for the XPS codec.
        /// </summary>
        /// <param name="viewer">The viewer.</param>
        /// <param name="imageSize">Image size, in megapixels, when image must be rendered.</param>
        public static void SetXpsRenderingRequirement(WpfImageViewer viewer, float imageSize)
        {
            viewer.RenderingRequirements.SetRequirement("Xps", new ImageSizeRenderingRequirement(imageSize));
        }

        /// <summary>
        /// Catches the visual tool exceptions of the specified viewer.
        /// </summary>
        /// <param name="imageViewer">The image viewer.</param>
        public static void CatchVisualToolExceptions(WpfImageViewer imageViewer)
        {
            imageViewer.CatchVisualToolExceptions = true;
            imageViewer.VisualToolException += new EventHandler<Vintasoft.Imaging.ExceptionEventArgs>(imageViewer_VisualToolException);
        }

        /// <summary>
        /// Handles the VisualToolException event of the ImageViewer control.
        /// </summary>
        private static void imageViewer_VisualToolException(object sender, Vintasoft.Imaging.ExceptionEventArgs e)
        {
            ShowErrorMessage(e.Exception);
        }

        /// <summary>
        /// Processes all Windows messages currently in the message queue.
        /// </summary>
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object f)
        {
            ((DispatcherFrame)f).Continue = false;
            return null;
        }


        /// <summary>
        /// Returns the UI action of the visual tool.
        /// </summary>
        /// <param name="visualTool">Visual tool.</param>
        /// <returns>The UI action of the visual tool.</returns>
        public static T GetUIAction<T>(WpfVisualTool visualTool)
            where T : UIAction
        {
            IList<UIAction> uiActions = null;
            // if visual tool has actions
            if (TryGetCurrentToolActions(visualTool, out uiActions))
            {
                // for each action in list
                foreach (UIAction uiAction in uiActions)
                {
                    if (uiAction is T)
                        return (T)uiAction;
                }
            }
            return default(T);
        }

        /// <summary>
        /// Returns the UI actions of visual tool.
        /// </summary>
        /// <param name="visualTool">The visual tool.</param>
        /// <param name="uiActions">The list of UI actions supported by the current visual tool.</param>
        /// <returns>
        /// <b>true</b> - UI actions are found; otherwise, <b>false</b>.
        /// </returns>
        public static bool TryGetCurrentToolActions(
            WpfVisualTool visualTool,
            out IList<UIAction> uiActions)
        {
            uiActions = null;
            ISupportUIActions currentToolWithUIActions = visualTool as ISupportUIActions;
            if (currentToolWithUIActions != null)
                uiActions = currentToolWithUIActions.GetSupportedUIActions();

            return uiActions != null;
        }

        /// <summary>
        /// Enables the license exception displaying.
        /// </summary>
        public static void EnableLicenseExceptionDisplaying()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// Handles the UnhandledException event of the AppDomain.CurrentDomain.
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.ComponentModel.LicenseException licenseException = GetLicenseException(e.ExceptionObject);
            if (licenseException != null)
            {
                // show information about licensing exception
                MessageBox.Show(string.Format("{0}: {1}", licenseException.GetType().Name, licenseException.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // open article with information about usage of evaluation license
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "https://www.vintasoft.com/docs/vsimaging-dotnet/Licensing-Evaluation.html";
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }

        /// <summary>
        /// Returns the license exception from specified exception.
        /// </summary>
        /// <param name="exceptionObject">The exception object.</param>
        /// <returns>Instance of <see cref="LicenseException"/>.</returns>
        private static LicenseException GetLicenseException(object exceptionObject)
        {
            Exception ex = exceptionObject as Exception;
            if (ex == null)
                return null;
            if (ex is LicenseException)
                return (LicenseException)exceptionObject;
            if (ex.InnerException != null)
                return GetLicenseException(ex.InnerException);
            return null;
        }

        #endregion

    }
}
