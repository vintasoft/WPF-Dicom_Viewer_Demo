using System.ComponentModel;

using Vintasoft.Imaging.Annotation;
using Vintasoft.Imaging.Annotation.TypeEditors;

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
            TypeDescriptor.AddAttributes(typeof(AnnotationFont), new EditorAttribute(typeof(AnnotationFontEditor), typeof(System.Drawing.Design.UITypeEditor)));
        }

    }
}
