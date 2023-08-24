using System;
using System.IO;
using System.Windows;

using Vintasoft.Imaging.Codecs.Encoders;

using WpfDemosCommonCode.Imaging.Codecs.Dialogs;


namespace WpfDemosCommonCode.Imaging.Codecs
{
    /// <summary>
    /// Provides the ability to create the encoder (<see cref="EncoderBase"/> or 
    /// <see cref="MultipageEncoderBase"/>) for filename or encoder name.
    /// </summary>
    public class ImagingEncoderFactory
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImagingEncoderFactory"/> class.
        /// </summary>
        public ImagingEncoderFactory()
        {
        }

        #endregion



        #region Properties

        static ImagingEncoderFactory _default = null;
        /// <summary>
        /// Gets the default <see cref="ImagingEncoderFactory"/>.
        /// </summary>
        public static ImagingEncoderFactory Default
        {
            get
            {
                if (_default == null)
                    _default = new ImagingEncoderFactory();

                return _default;
            }
        }


        WindowStartupLocation _dialogStartPosition = WindowStartupLocation.CenterOwner;
        /// <summary>
        /// Gets or sets the start position of dialog.
        /// </summary>
        /// <value>
        /// Default value is <b>WindowStartupLocation</b>.CenterOwner.
        /// </value>
        public WindowStartupLocation DialogStartPosition
        {
            get
            {
                return _dialogStartPosition;
            }
            set
            {
                _dialogStartPosition = value;
            }
        }

        bool _canAddImagesToExistingFile = true;
        /// <summary>
        /// Gets or sets a value indicating whether encoder can add images to the existing multipage image file.</param>
        /// </summary>
        /// <value>
        /// <b>True</b> - encoder can add images to the existing GIF file;
        /// <b>false</b> - encoder can NOT add images to the existing GIF file.
        /// </value>
        public bool CanAddImagesToExistingFile
        {
            get
            {
                return _canAddImagesToExistingFile;
            }
            set
            {
                _canAddImagesToExistingFile = value;
            }
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary> 
        /// Returns the encoder for the specified <paramref name="filename"/>.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="encoder">The encoder.</param>
        /// <returns>
        /// <b>True</b> if the encoder is created and initialized; otherwise, <b>false</b>.
        /// </returns>
        /// <exception cref="NotSupportedException">Thrown if encoder is not created.</exception>
        public bool GetEncoder(string filename, out EncoderBase encoder)
        {
            // create encoder
            encoder = AvailableEncoders.CreateEncoder(filename);

            // indicates whether the encoder can add images to the existing multipage image file
            bool canAddImagesToExistingFile = false;
            // if encoder can add images to the existing multipage image file
            if (CanAddImagesToExistingFile)
            {
                // if file exists
                if (File.Exists(filename))
                    // specify that encoder can add images to the existing multipage image file
                    canAddImagesToExistingFile = true;
            }

            // if encoder is not found
            if (encoder == null)
                throw new NotSupportedException(string.Format("Can not find encoder for '{0}'.", filename));

            // set encoder settings
            return ShowEncoderSettingsDialogAndCheckOverwrite(
                filename,
                ref encoder,
                canAddImagesToExistingFile,
                true);
        }

        /// <summary>
        /// Returns the encoder for the specified name (<paramref name="name"/>).
        /// </summary>
        /// <param name="name">The encoder name.</param>
        /// <param name="encoder">The encoder.</param>
        /// <returns>
        /// <b>True</b> if the encoder is created and initialized; otherwise, <b>false</b>.
        /// </returns>
        /// <exception cref="NotSupportedException">Thrown if encoder for <i>name</i> is not created.</exception>
        public bool GetEncoderByName(string name, out EncoderBase encoder)
        {
            // create encoder by name
            encoder = AvailableEncoders.CreateEncoderByName(name);

            // if encoder is not found
            if (encoder == null)
                throw new NotSupportedException(string.Format("Can not find encoder for '{0}'.", name));

            // set encoder settings
            return ShowEncoderSettingsDialogAndCheckOverwrite(name, ref encoder, false, false);
        }

        /// <summary> 
        /// Returns the multipage encoder for the specified <paramref name="filename"/>.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="multipageEncoder">The multipage encoder.</param>
        /// <returns>
        /// <b>True</b> if the multipage encoder is created and initialized; otherwise, <b>false</b>.
        /// </returns>
        public bool GetMultipageEncoder(string filename, out MultipageEncoderBase multipageEncoder)
        {
            // create encoder
            EncoderBase encoder = AvailableEncoders.CreateEncoder(filename);

            // indicates whether the encoder can add images to the existing multipage image file
            bool canAddImagesToExistingFile = false;
            // if encoder can add images to the existing multipage image file
            if (CanAddImagesToExistingFile)
            {
                // if file exists
                if (File.Exists(filename))
                    // specify that encoder can add images to the existing multipage image file
                    canAddImagesToExistingFile = true;
            }

            // get the multipage encoder
            multipageEncoder = encoder as MultipageEncoderBase;

            // if encoder is not multipage
            if (multipageEncoder == null)
            {
                return false;
            }

            // set encoder settings
            bool result = ShowEncoderSettingsDialogAndCheckOverwrite(
                filename,
                ref encoder,
                canAddImagesToExistingFile,
                multipageEncoder.CreateNewFile);

            encoder = encoder as MultipageEncoderBase;

            return result;
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Shows the encoder settings dialog and initializes the encoder.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="canAddImagesToExistingFile">The value indicating whether encoder can add images to the existing multipage image file.</param>
        /// <returns>
        /// <b>True</b> if settings are applied to the encoder; otherwise, <b>false</b>.
        /// </returns>
        protected virtual bool ShowEncoderSettingsDialog(
            ref EncoderBase encoder,
            bool canAddImagesToExistingFile)
        {
            // set encoder settings
            switch (encoder.Name)
            {
                case "Bmp":
                    BmpEncoderSettingsWindow bmpEncoderSettingsWindow = new BmpEncoderSettingsWindow();
                    SetEncoderSettingsDialogProperties(bmpEncoderSettingsWindow);
                    if (bmpEncoderSettingsWindow.ShowDialog() == false)
                        return false;

                    BmpEncoder bmpEncoder = (BmpEncoder)encoder;

                    bmpEncoder.Settings = bmpEncoderSettingsWindow.EncoderSettings;
                    return true;

                case "Gif":
                    GifEncoderSettingsWindow gifEncoderSettingsWindow = new GifEncoderSettingsWindow();
                    SetEncoderSettingsDialogProperties(gifEncoderSettingsWindow);
                    gifEncoderSettingsWindow.CanAddImagesToExistingFile = canAddImagesToExistingFile;
                    if (gifEncoderSettingsWindow.ShowDialog() == false)
                        return false;

                    GifEncoder gifEncoder = (GifEncoder)encoder;

                    gifEncoder.Settings = gifEncoderSettingsWindow.EncoderSettings;
                    gifEncoder.CreateNewFile = !gifEncoderSettingsWindow.AddImagesToExistingFile;
                    return true;

                case "Jpeg":
                    JpegEncoderSettingsWindow jpegEncoderSettingsWindow = new JpegEncoderSettingsWindow();
                    SetEncoderSettingsDialogProperties(jpegEncoderSettingsWindow);
                    if (jpegEncoderSettingsWindow.ShowDialog() == false)
                        return false;

                    JpegEncoder jpegEncoder = (JpegEncoder)encoder;

                    jpegEncoder.Settings = jpegEncoderSettingsWindow.EncoderSettings;
                    return true;

                case "Pbm":
                    PbmEncoderSettingsWindow pbmEncoderSettingsWindow = new PbmEncoderSettingsWindow();
                    SetEncoderSettingsDialogProperties(pbmEncoderSettingsWindow);
                    if (pbmEncoderSettingsWindow.ShowDialog() == false)
                        return false;

                    PbmEncoder pbmEncoder = (PbmEncoder)encoder;

                    pbmEncoder.Settings = pbmEncoderSettingsWindow.EncoderSettings;
                    return true;

                case "Pcx":
                    return true;

                case "Png":
                    PngEncoderSettingsWindow pngEncoderSettingsWindow = new PngEncoderSettingsWindow();
                    SetEncoderSettingsDialogProperties(pngEncoderSettingsWindow);
                    if (pngEncoderSettingsWindow.ShowDialog() == false)
                        return false;

                    PngEncoder pngEncoder = (PngEncoder)encoder;

                    pngEncoder.Settings = pngEncoderSettingsWindow.EncoderSettings;
                    return true;

                case "Svg":
                    SvgEncoderSettingsWindow svgEncoderSettingsForm = new SvgEncoderSettingsWindow();
                    SetEncoderSettingsDialogProperties(svgEncoderSettingsForm);
                    if (svgEncoderSettingsForm.ShowDialog() == false)
                        return false;

                    SvgEncoder svgEncoder = (SvgEncoder)encoder;

                    svgEncoder.Settings = svgEncoderSettingsForm.EncoderSettings;
                    return true;

                case "Tga":
                    TgaEncoderSettingsWindow tgaEncoderSettingsWindow = new TgaEncoderSettingsWindow();
                    SetEncoderSettingsDialogProperties(tgaEncoderSettingsWindow);
                    if (tgaEncoderSettingsWindow.ShowDialog() == false)
                        return false;

                    TgaEncoder tgaEncoder = (TgaEncoder)encoder;

                    tgaEncoder.Settings = tgaEncoderSettingsWindow.EncoderSettings;
                    return true;

                case "Tiff":
                    TiffEncoderSettingsWindow tiffEncoderSettingsWindow = new TiffEncoderSettingsWindow();
                    SetEncoderSettingsDialogProperties(tiffEncoderSettingsWindow);
                    tiffEncoderSettingsWindow.CanAddImagesToExistingFile = canAddImagesToExistingFile;
                    if (tiffEncoderSettingsWindow.ShowDialog() == false)
                        return false;

                    TiffEncoder tiffEncoder = (TiffEncoder)encoder;

                    tiffEncoder.Settings = tiffEncoderSettingsWindow.EncoderSettings;
                    tiffEncoder.CreateNewFile = !tiffEncoderSettingsWindow.AddImagesToExistingFile;
                    return true;

#if NETCOREAPP
                case "Webp":
                    WebpEncoderSettingsWindow webpEncoderSettingsWindow = new WebpEncoderSettingsWindow();
                    
                        SetEncoderSettingsDialogProperties(webpEncoderSettingsWindow);
                        if (webpEncoderSettingsWindow.ShowDialog() == false)
                            return false;

                        WebpEncoder webpEncoder = (WebpEncoder)encoder;

                        webpEncoder.Settings = webpEncoderSettingsWindow.EncoderSettings;
                        return true;
                    
#endif
            }

            return false;
        }

        /// <summary>
        /// Sets the properties of encoder settings dialog.
        /// </summary>
        /// <param name="window">The encoder settings dialog.</param>
        protected void SetEncoderSettingsDialogProperties(Window window)
        {
            if (window == null)
                return;

            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = DialogStartPosition;
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Shows the encoder settings dialog and check overwrite exist file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="canAddImagesToExistingFile">The value indicating whether encoder can add images to the existing multipage image file.</param>
        /// <param name="askIfFileCanBeOverwritten">The value indicating whether we need to ask that the existing file must be overwritten.</param>
        /// <returns>
        /// <b>True</b> if encoder is initialized; otherwise, <b>false</b>.
        /// </returns>
        private bool ShowEncoderSettingsDialogAndCheckOverwrite(
            string filename,
            ref EncoderBase encoder,
            bool canAddImagesToExistingFile,
            bool askIfFileCanBeOverwritten)
        {
            //
            // show the encoder settings dialog and initialize the encoder
            bool result = ShowEncoderSettingsDialog(ref encoder, canAddImagesToExistingFile);
            // if encoder is not initialized
            if (!result)
                // exit
                return false;

            // if we need to ask that the existing file must be overwritten and the file exists
            if (askIfFileCanBeOverwritten && File.Exists(filename))
            {
                // if encoder is multipage encoder
                if (encoder is MultipageEncoderBase)
                {
                    // if encoder must create new image file
                    if (((MultipageEncoderBase)encoder).CreateNewFile)
                        return ShowDialogAndAskIfFileMustBeOverwritten(filename);
                }
                // if encoder is NOT multipage encoder
                else
                {
                    return ShowDialogAndAskIfFileMustBeOverwritten(filename);
                }
            }

            return true;
        }

        /// <summary>
        /// Shows a dialog and asks if existing file must be overwritten.
        /// </summary>
        /// <param name="filename">File name.</param>
        /// <returns><b>True</b> - file must be overwritten; otherwise, <b>false</b>.</returns>
        private bool ShowDialogAndAskIfFileMustBeOverwritten(string filename)
        {
            string message = string.Format("File '{0}' already exists. Overwrite it?", Path.GetFileName(filename));

            if (System.Windows.MessageBox.Show(
                message,
                "Confirm overwrite",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #endregion

    }
}
