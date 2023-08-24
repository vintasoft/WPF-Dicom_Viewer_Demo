using Vintasoft.Imaging.Metadata;

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// A control that allows to view the DICOM metadata tree.
    /// </summary>
    public partial class DicomMetadataTreeView : MetadataTreeView
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DicomMetadataTreeView"/> class.
        /// </summary>
        public DicomMetadataTreeView()
        {
        }



        /// <summary>
        /// Returns the metadata node name.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        /// The metadata node name.
        /// </returns>
        protected override string GetMetadataNodeName(MetadataNode node)
        {
#if REMOVE_DICOM_PLUGIN
            return base.GetMetadataNodeName(node);
#else
            DicomDataElementMetadata dataElementMetadata = node as DicomDataElementMetadata;

            if (dataElementMetadata == null)
            {
                return base.GetMetadataNodeName(node);
            }
            else
            {
                string nodeName = string.Format("({0:X4},{1:X4}) {2}",
                    dataElementMetadata.GroupNumber,
                    dataElementMetadata.ElementNumber,
                    dataElementMetadata.Name);

                return nodeName;
            }
#endif
        }

    }
}
