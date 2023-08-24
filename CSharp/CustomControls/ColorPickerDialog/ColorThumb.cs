using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WpfDemosCommonCode.CustomControls
{
    public class ColorThumb : Thumb
    {

        #region Constructors

        static ColorThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorThumb),
                new FrameworkPropertyMetadata(typeof(ColorThumb)));
        }

        #endregion



        #region Dependency Properties

        #region ThumbColor

        public static readonly DependencyProperty ThumbColorProperty =
            DependencyProperty.Register("ThumbColor", typeof(Color), typeof(ColorThumb),
                new FrameworkPropertyMetadata(Colors.Transparent));
        public Color ThumbColor
        {
            get
            {
                return (Color)GetValue(ThumbColorProperty);
            }
            set
            {
                SetValue(ThumbColorProperty, value);
            }
        }

        #endregion


        #region PointerOutlineThickness

        public static readonly DependencyProperty PointerOutlineThicknessProperty =
            DependencyProperty.Register("PointerOutlineThickness", typeof(double), typeof(ColorThumb),
                new FrameworkPropertyMetadata(1.0));
        public double PointerOutlineThickness
        {
            get
            {
                return (double)GetValue(PointerOutlineThicknessProperty);
            }
            set
            {
                SetValue(PointerOutlineThicknessProperty, value);
            }
        }

        #endregion


        #region PointerOutlineBrush

        public static readonly DependencyProperty PointerOutlineBrushProperty =
            DependencyProperty.Register("PointerOutlineBrush", typeof(Brush), typeof(ColorThumb),
                new FrameworkPropertyMetadata(null));
        public Brush PointerOutlineBrush
        {
            get
            {
                return (Brush)GetValue(PointerOutlineBrushProperty);
            }
            set
            {
                SetValue(PointerOutlineBrushProperty, value);
            }
        }

        #endregion

        #endregion

    }
}
