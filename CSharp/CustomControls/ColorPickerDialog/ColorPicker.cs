using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// The color picker.
    /// </summary>
    public class ColorPicker : Control
    {

        #region Fields

        SpectrumSlider _colorSlider;
        static readonly string ColorSliderName = "PART_ColorSlider";
        FrameworkElement _colorDetail;
        static readonly string ColorDetailName = "PART_ColorDetail";
        TranslateTransform _markerTransform = new TranslateTransform();
        Path _colorMarker;
        static readonly string ColorMarkerName = "PART_ColorMarker";
        Point? _colorPosition;
        Color _color;
        bool _shouldFindPoint;
        bool _templateApplied;
        bool _isAlphaChange;
        Slider _opacitySlider;
        TextBox _scAChannelTextBox;
        TextBox _aChannelTextBox;

        #endregion



        #region Constructors

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public ColorPicker()
        {
            _templateApplied = false;
            _color = Colors.White;
            _shouldFindPoint = true;
            SetValue(AProperty, _color.A);
            SetValue(RProperty, _color.R);
            SetValue(GProperty, _color.G);
            SetValue(BProperty, _color.B);
            SetValue(SelectedColorProperty, _color);
        }

        #endregion



        #region Dependency Properties

        bool _canEditAlphaChannel = true;
        /// <summary>
        /// Gets or sets a value indicating whether the alpha channel, of color, can be edited.
        /// </summary>
        /// <value>
        /// <b>true</b> if the alpha channel, of color, can be edited; otherwise, <b>false</b>.
        /// </value>
        public bool CanEditAlphaChannel
        {
            get
            {
                return _canEditAlphaChannel;
            }
            set
            {
                _canEditAlphaChannel = value;

                UpdateUI();
            }
        }


        #region SelectedColor

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker),
                new PropertyMetadata(Colors.Transparent));

        /// <summary>
        /// Gets or sets the selected color.
        /// This is a dependency property.
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                return (Color)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, value);
                SetColor((Color)value);
            }
        }

        #endregion


        #region ScA

        public static readonly DependencyProperty ScAProperty =
            DependencyProperty.Register("ScA", typeof(float), typeof(ColorPicker),
                new PropertyMetadata(
                    (float)1,
                    new PropertyChangedCallback(ScAPropertyChanged),
                    new CoerceValueCallback(ScAPropertyCoerceValue)));

        private static void ScAPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnScAChanged((float)e.NewValue);
        }

        private static object ScAPropertyCoerceValue(DependencyObject d, object baseValue)
        {
            return (float)Math.Round((double)((float)baseValue), 6);
        }

        /// <summary>
        /// Gets or sets the ScRGB alpha value of the selected color.
        /// This is a dependency property.
        /// </summary>
        public double ScA
        {
            get
            {
                return (double)GetValue(ScAProperty);
            }
            set
            {
                SetValue(ScAProperty, value);
            }
        }

        #endregion


        #region ScR

        public static readonly DependencyProperty ScRProperty =
            DependencyProperty.Register("ScR", typeof(float), typeof(ColorPicker),
                new PropertyMetadata(
                    (float)1,
                    new PropertyChangedCallback(ScRPropertyChanged),
                    new CoerceValueCallback(ScRPropertyCoerceValue)));

        private static void ScRPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnScRChanged((float)e.NewValue);
        }

        private static object ScRPropertyCoerceValue(DependencyObject d, object baseValue)
        {
            return (float)Math.Round((double)((float)baseValue), 6);
        }

        /// <summary>
        /// Gets or sets the ScRGB red value of the selected color.
        /// This is a dependency property.
        /// </summary>
        public double ScR
        {
            get
            {
                return (double)GetValue(ScRProperty);
            }
            set
            {
                SetValue(RProperty, value);
            }
        }

        #endregion


        #region ScG

        public static readonly DependencyProperty ScGProperty =
            DependencyProperty.Register("ScG", typeof(float), typeof(ColorPicker),
                new PropertyMetadata(
                    (float)1,
                    new PropertyChangedCallback(ScGPropertyChanged),
                    new CoerceValueCallback(ScGPropertyCoerceValue)));

        private static void ScGPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnScGChanged((float)e.NewValue);
        }

        private static object ScGPropertyCoerceValue(DependencyObject d, object baseValue)
        {
            return (float)Math.Round((double)((float)baseValue), 6);
        }

        /// <summary>
        /// Gets or sets the ScRGB green value of the selected color.
        /// This is a dependency property.
        /// </summary>
        public double ScG
        {
            get
            {
                return (double)GetValue(ScGProperty);
            }
            set
            {
                SetValue(GProperty, value);
            }
        }

        #endregion


        #region ScB

        public static readonly DependencyProperty ScBProperty =
            DependencyProperty.Register("ScB", typeof(float), typeof(ColorPicker),
                new PropertyMetadata(
                    (float)1,
                    new PropertyChangedCallback(ScBPropertyChanged),
                    new CoerceValueCallback(ScBPropertyCoerceValue)));

        private static void ScBPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnScBChanged((float)e.NewValue);
        }

        private static object ScBPropertyCoerceValue(DependencyObject d, object baseValue)
        {
            return (float)Math.Round((double)((float)baseValue), 6);
        }

        /// <summary>
        /// Gets or sets the ScRGB blue value of the selected color.
        /// This is a dependency property.
        /// </summary>
        public double ScB
        {
            get
            {
                return (double)GetValue(BProperty);
            }
            set
            {
                SetValue(BProperty, value);
            }
        }

        #endregion


        #region A

        public static readonly DependencyProperty AProperty =
            DependencyProperty.Register("A", typeof(byte), typeof(ColorPicker),
                new PropertyMetadata(
                    (byte)255,
                    new PropertyChangedCallback(APropertyChanged)));

        private static void APropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnAChanged((byte)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the ARGB alpha value of the selected color.
        /// This is a dependency property.
        /// </summary>
        public byte A
        {
            get
            {
                return (byte)GetValue(AProperty);
            }
            set
            {
                SetValue(AProperty, value);
            }
        }

        #endregion


        #region R

        public static readonly DependencyProperty RProperty =
            DependencyProperty.Register("R", typeof(byte), typeof(ColorPicker),
                new PropertyMetadata(
                    (byte)255,
                    new PropertyChangedCallback(RPropertyChanged)));

        private static void RPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnRChanged((byte)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the ARGB red value of the selected color.
        /// This is a dependency property.
        /// </summary>
        public byte R
        {
            get
            {
                return (byte)GetValue(RProperty);
            }
            set
            {
                SetValue(RProperty, value);
            }
        }

        #endregion


        #region G

        public static readonly DependencyProperty GProperty =
            DependencyProperty.Register("G", typeof(byte), typeof(ColorPicker),
                new PropertyMetadata(
                    (byte)255,
                    new PropertyChangedCallback(GPropertyChanged)));

        private static void GPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnGChanged((byte)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the ARGB green value of the selected color.
        /// This is a dependency property.
        /// </summary>
        public byte G
        {
            get
            {
                return (byte)GetValue(GProperty);
            }
            set
            {
                SetValue(GProperty, value);
            }
        }

        #endregion


        #region B

        public static readonly DependencyProperty BProperty =
            DependencyProperty.Register("B", typeof(byte), typeof(ColorPicker),
                new PropertyMetadata(
                    (byte)255,
                    new PropertyChangedCallback(BPropertyChanged)));

        private static void BPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnBChanged((byte)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the ARGB blue value of the selected color.
        /// This is a dependency property.
        /// </summary>
        public byte B
        {
            get
            {
                return (byte)GetValue(BProperty);
            }
            set
            {
                SetValue(BProperty, value);
            }
        }

        #endregion


        #region HexadecimalString

        public static readonly DependencyProperty HexadecimalStringProperty =
            DependencyProperty.Register("HexadecimalString", typeof(string), typeof(ColorPicker),
                new PropertyMetadata(
                    "#FFFFFFFF",
                    new PropertyChangedCallback(HexadecimalStringPropertyChanged)));

        private static void HexadecimalStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker c = (ColorPicker)d;
            c.OnHexadecimalStringChanged((string)e.OldValue, (string)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the the selected color in hexadecimal notation.
        /// This is a dependency property.
        /// </summary>
        public string HexadecimalString
        {
            get
            {
                return (string)GetValue(HexadecimalStringProperty);
            }
            set
            {
                SetValue(HexadecimalStringProperty, value);
            }
        }

        #endregion

        #endregion



        #region Methods

        #region PUBLIC

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _colorDetail = GetTemplateChild(ColorDetailName) as FrameworkElement;
            _colorMarker = GetTemplateChild(ColorMarkerName) as Path;
            _colorSlider = GetTemplateChild(ColorSliderName) as SpectrumSlider;
            _colorSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(_colorSlider_ValueChanged);

            _colorMarker.RenderTransform = _markerTransform;
            _colorMarker.RenderTransformOrigin = new Point(0.5, 0.5);
            _colorDetail.MouseLeftButtonDown += new MouseButtonEventHandler(_colorDetail_MouseLeftButtonDown);
            _colorDetail.PreviewMouseMove += new MouseEventHandler(_colorDetail_PreviewMouseMove);
            _colorDetail.SizeChanged += new SizeChangedEventHandler(_colorDetail_SizeChanged);

            _opacitySlider = GetTemplateChild("opacitySlider") as Slider;
            _scAChannelTextBox = GetTemplateChild("ScAChannelTextBox") as TextBox;
            _aChannelTextBox = GetTemplateChild("aChannelTextBox") as TextBox;

            _templateApplied = true;
            _shouldFindPoint = true;
            _isAlphaChange = false;

            SelectedColor = _color;

            UpdateUI();
        }

        #endregion


        #region PROTECTED

        protected override void OnTemplateChanged(
            ControlTemplate oldTemplate,
            ControlTemplate newTemplate)
        {
            _templateApplied = false;
            if (oldTemplate != null)
            {
                _colorSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(_colorSlider_ValueChanged);
                _colorDetail.MouseLeftButtonDown -= new MouseButtonEventHandler(_colorDetail_MouseLeftButtonDown);
                _colorDetail.PreviewMouseMove -= new MouseEventHandler(_colorDetail_PreviewMouseMove);
                _colorDetail.SizeChanged -= new SizeChangedEventHandler(_colorDetail_SizeChanged);
                _colorDetail = null;
                _colorMarker = null;
                _colorSlider = null;

                _opacitySlider = null;
                _scAChannelTextBox = null;
                _aChannelTextBox = null;
            }
            base.OnTemplateChanged(oldTemplate, newTemplate);
        }


        #region Property Changed Callbacks

        protected virtual void OnAChanged(byte newValue)
        {
            _color.A = newValue;
            SetValue(ScAProperty, _color.ScA);
            SetValue(SelectedColorProperty, _color);
        }

        protected virtual void OnRChanged(byte newValue)
        {
            _color.R = newValue;
            SetValue(ScRProperty, _color.ScR);
            SetValue(SelectedColorProperty, _color);
        }

        protected virtual void OnGChanged(byte newValue)
        {
            _color.G = newValue;
            SetValue(ScGProperty, _color.ScG);
            SetValue(SelectedColorProperty, _color);
        }

        protected virtual void OnBChanged(byte newValue)
        {
            _color.B = newValue;
            SetValue(ScBProperty, _color.ScB);
            SetValue(SelectedColorProperty, _color);
        }

        protected virtual void OnScAChanged(float newValue)
        {
            _isAlphaChange = true;
            if (_shouldFindPoint)
            {
                _color.ScA = newValue;
                SetValue(AProperty, _color.A);
                SetValue(SelectedColorProperty, _color);
                SetValue(HexadecimalStringProperty, _color.ToString());
            }
            _isAlphaChange = false;
        }

        protected virtual void OnScRChanged(float newValue)
        {
            if (_shouldFindPoint)
            {
                _color.ScR = newValue;
                SetValue(RProperty, _color.R);
                SetValue(SelectedColorProperty, _color);
                SetValue(HexadecimalStringProperty, _color.ToString());
            }
        }

        protected virtual void OnScGChanged(float newValue)
        {
            if (_shouldFindPoint)
            {
                _color.ScG = newValue;
                SetValue(GProperty, _color.G);
                SetValue(SelectedColorProperty, _color);
                SetValue(HexadecimalStringProperty, _color.ToString());
            }
        }

        protected virtual void OnScBChanged(float newValue)
        {
            if (_shouldFindPoint)
            {
                _color.ScB = newValue;
                SetValue(BProperty, _color.B);
                SetValue(SelectedColorProperty, _color);
                SetValue(HexadecimalStringProperty, _color.ToString());
            }
        }

        protected virtual void OnHexadecimalStringChanged(string oldValue, string newValue)
        {
            try
            {
                if (_shouldFindPoint)
                {
                    _color = (Color)ColorConverter.ConvertFromString(newValue);
                }

                SetValue(AProperty, _color.A);
                SetValue(RProperty, _color.R);
                SetValue(GProperty, _color.G);
                SetValue(BProperty, _color.B);

                if (_shouldFindPoint && !_isAlphaChange && _templateApplied)
                {
                    UpdateMarkerPosition(_color);
                }
            }
            catch (FormatException)
            {
                SetValue(HexadecimalStringProperty, oldValue);
            }
        }

        #endregion

        #endregion


        #region PRIVATE

        private void UpdateUI()
        {
            if (_opacitySlider != null)
                _opacitySlider.IsEnabled = _canEditAlphaChannel;

            if (_scAChannelTextBox != null)
                _scAChannelTextBox.IsEnabled = _canEditAlphaChannel;

            if (_aChannelTextBox != null)
                _aChannelTextBox.IsEnabled = _canEditAlphaChannel;
        }

        private void SetColor(Color theColor)
        {
            _color = theColor;

            if (_templateApplied)
            {
                SetValue(AProperty, _color.A);
                SetValue(RProperty, _color.R);
                SetValue(GProperty, _color.G);
                SetValue(BProperty, _color.B);
                UpdateMarkerPosition(theColor);
            }
        }

        private void UpdateMarkerPosition(Point p)
        {
            _markerTransform.X = p.X;
            _markerTransform.Y = p.Y;
            p.X = p.X / _colorDetail.ActualWidth;
            p.Y = p.Y / _colorDetail.ActualHeight;
            _colorPosition = p;
            DetermineColor(p);
        }

        private void UpdateMarkerPosition(Color theColor)
        {
            _colorPosition = null;

            HsvColor hsv = ColorUtilities.ConvertRgbToHsv(theColor.R, theColor.G, theColor.B);

            _colorSlider.Value = hsv.H;

            Point p = new Point(hsv.S, 1 - hsv.V);

            _colorPosition = p;
            p.X = p.X * _colorDetail.ActualWidth;
            p.Y = p.Y * _colorDetail.ActualHeight;
            _markerTransform.X = p.X;
            _markerTransform.Y = p.Y;
        }

        private void DetermineColor(Point p)
        {
            HsvColor hsv = new HsvColor(360 - _colorSlider.Value, 1, 1);
            hsv.S = p.X;
            hsv.V = 1 - p.Y;
            _color = ColorUtilities.ConvertHsvToRgb(hsv.H, hsv.S, hsv.V);
            _shouldFindPoint = false;
            _color.ScA = (float)GetValue(ScAProperty);
            SetValue(HexadecimalStringProperty, _color.ToString());
            _shouldFindPoint = true;
        }


        #region Template Part Event Handlers

        /// <summary>
        /// Handles the ValueChanged event of _colorSlider object.
        /// </summary>
        private void _colorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            if (_colorPosition != null)
            {
                DetermineColor((Point)_colorPosition);
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of _colorDetail object.
        /// </summary>
        private void _colorDetail_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(_colorDetail);
            UpdateMarkerPosition(p);
        }

        /// <summary>
        /// Handles the PreviewMouseMove event of _colorDetail object.
        /// </summary>
        private void _colorDetail_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(_colorDetail);
                UpdateMarkerPosition(p);
                Mouse.Synchronize();
            }
        }

        /// <summary>
        /// Handles the SizeChanged event of _colorDetail object.
        /// </summary>
        private void _colorDetail_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (args.PreviousSize != Size.Empty &&
                args.PreviousSize.Width != 0 &&
                args.PreviousSize.Height != 0)
            {
                double widthDifference = args.NewSize.Width / args.PreviousSize.Width;
                double heightDifference = args.NewSize.Height / args.PreviousSize.Height;
                _markerTransform.X = _markerTransform.X * widthDifference;
                _markerTransform.Y = _markerTransform.Y * heightDifference;
            }
            else if (_colorPosition != null)
            {
                _markerTransform.X = ((Point)_colorPosition).X * args.NewSize.Width;
                _markerTransform.Y = ((Point)_colorPosition).Y * args.NewSize.Height;
            }
        }

        #endregion

        #endregion

        #endregion

    }
}
