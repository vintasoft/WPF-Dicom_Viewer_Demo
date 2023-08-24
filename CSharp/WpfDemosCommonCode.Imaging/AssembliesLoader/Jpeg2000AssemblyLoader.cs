namespace WpfDemosCommonCode
{
    /// <summary>
    /// Loads the Vintasoft.Imaging.Jpeg2000Codec assembly.
    /// </summary>
    public class Jpeg2000AssemblyLoader
    {

        /// <summary>
        /// Loads the Vintasoft.Imaging.Jpeg2000Codec assembly.
        /// </summary>
        public static void Load()
        {
#if REMOVE_JPEG2000_PLUGIN
            Vintasoft.Imaging.Codecs.AvailableCodecs.RemoveCodecByName("Jpeg2000");
#else
            using (Vintasoft.Imaging.Codecs.Decoders.Jpeg2000Decoder decoder =
                new Vintasoft.Imaging.Codecs.Decoders.Jpeg2000Decoder())
            {
            }
#endif
        }

    }
}
