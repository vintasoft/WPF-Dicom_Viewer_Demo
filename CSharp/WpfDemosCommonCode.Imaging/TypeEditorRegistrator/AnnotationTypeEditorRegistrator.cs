using System.ComponentModel;

#if !REMOVE_ANNOTATION_PLUGIN
using Vintasoft.Imaging.Annotation;
using Vintasoft.Imaging.Annotation.TypeEditors; 
#endif

namespace WpfDemosCommonCode.Imaging
{
    /// <summary>
    /// Allows to register the ability to edit properries of <see cref="AnnotationFontEditor"/> in property grid.
    /// </summary>
    class AnnotationTypeEditorRegistrator
    {

        /// <summary>
        /// Registers the ability to edit properries of <see cref="AnnotationFontEditor"/> in property grid.
        /// </summary>
        public static void Register()
        {
#if !REMOVE_ANNOTATION_PLUGIN
            TypeDescriptor.AddAttributes(typeof(AnnotationFont), new EditorAttribute(typeof(AnnotationFontEditor), typeof(System.Drawing.Design.UITypeEditor))); 
#endif
        }

    }
}
