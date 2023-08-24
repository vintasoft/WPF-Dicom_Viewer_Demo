using Vintasoft.Imaging.Codecs.ImageFiles.Dicom;
using Vintasoft.Imaging.Dicom.Wpf.UI.VisualTools;
using Vintasoft.Imaging.Metadata;
using Vintasoft.Imaging;


namespace WpfDicomViewerDemo
{
    /// <summary>
    /// Represents a text object, which shows DICOM frame compression.
    /// </summary>
    public class WpfCompressionInfoTextOverlay : WpfDicomMetadataTextOverlay
    {

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfCompressionInfoTextOverlay"/> class.
        /// </summary>
        public WpfCompressionInfoTextOverlay()
            : this(AnchorType.Top | AnchorType.Left)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfCompressionInfoTextOverlay"/> class.
        /// </summary>
        /// <param name="anchor">The text anchor in viewer.</param>
        public WpfCompressionInfoTextOverlay(AnchorType anchor)
            : base(anchor)
        {
        }

        #endregion



        #region Methods

        #region PUBLIC

        /// <summary>
        /// Creates a new <see cref="WpfCompressionInfoTextOverlay"/> that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="WpfCompressionInfoTextOverlay"/> that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            WpfCompressionInfoTextOverlay textOverlay = new WpfCompressionInfoTextOverlay();
            CopyTo(textOverlay);
            return textOverlay;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return "Compression Info";
        }

        #endregion


        #region PROTECTED

        /// <summary>
        /// Returns the text of text overlay from DICOM frame metadata.
        /// </summary>
        /// <param name="frameMetadata">The DICOM frame metadata.</param>
        /// <returns>
        /// The text of text overlay.
        /// </returns>
        protected override string GetOverlayText(DicomFrameMetadata frameMetadata)
        {
            string compressionAlgorithm;

            if (frameMetadata.IsLosslessCompression)
                compressionAlgorithm = "Lossless";
            else
                compressionAlgorithm = "Lossy";

            return string.Format("{0} ({1})", compressionAlgorithm,
                GetCompressionName(frameMetadata.Compression));
        }

        #endregion


        #region PRIVATE

        /// <summary>
        /// Returns the name of the compression.
        /// </summary>
        /// <param name="compression">The compression.</param>
        /// <returns>
        /// The name of the compression.
        /// </returns>
        private string GetCompressionName(DicomImageCompressionType compression)
        {
            switch (compression)
            {
                case DicomImageCompressionType.Uncompressed:
                    return "Uncompressed";

                case DicomImageCompressionType.JpegLossy:
                case DicomImageCompressionType.JpegLossless:
                    return "Jpeg";

                case DicomImageCompressionType.JpegLsLossy:
                case DicomImageCompressionType.JpegLsLossless:
                    return "Jpeg-Ls";

                case DicomImageCompressionType.Jpeg2000:
                case DicomImageCompressionType.Jpeg2000InteractiveProtocol:
                    return "Jpeg 2000";

                case DicomImageCompressionType.RLE:
                    return "RLE";

                default:
                    return "Unknown";
            }
        }

        #endregion

        #endregion

    }
}
