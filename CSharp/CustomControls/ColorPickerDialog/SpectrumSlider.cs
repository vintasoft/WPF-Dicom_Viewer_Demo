using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace WpfDemosCommonCode.CustomControls
{
    public class SpectrumSlider : Slider
    {
        #region Fields

        private static string SpectrumDisplayName = "PART_SpectrumDisplay";

        private Rectangle _spectrumDisplay;

        private LinearGradientBrush _pickerBrush;

        #endregion



        #region Constructors

        static SpectrumSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SpectrumSlider),
                new FrameworkPropertyMetadata(typeof(SpectrumSlider)));
        }

        #endregion


        #region Dependency Properties

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(SpectrumSlider),
                new PropertyMetadata(Colors.Transparent));

        public Color SelectedColor
        {
            get
            {
                return (Color)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, value);
            }

        }

        #endregion



        #region Methods

        #region PUBLIC

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _spectrumDisplay = GetTemplateChild(SpectrumDisplayName) as Rectangle;
            updateColorSpectrum();
            OnValueChanged(Double.NaN, Value);
        }

        #endregion


        #region PROTECTED

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            Color theColor = ColorUtilities.ConvertHsvToRgb(360 - newValue, 1, 1);
            SetValue(SelectedColorProperty, theColor);
        }

        #endregion


        #region PRIVATE

        private void updateColorSpectrum()
        {
            if (_spectrumDisplay != null)
            {
                createSpectrum();
            }
        }

        private void createSpectrum()
        {
            _pickerBrush = new LinearGradientBrush();
            _pickerBrush.StartPoint = new Point(0.5, 0);
            _pickerBrush.EndPoint = new Point(0.5, 1);
            _pickerBrush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;

            List<Color> colorsList = ColorUtilities.GenerateHsvSpectrum();
            double stopIncrement = (double)1 / colorsList.Count;

            int i;
            for (i = 0; i < colorsList.Count; i++)
            {
                _pickerBrush.GradientStops.Add(new GradientStop(colorsList[i], i * stopIncrement));
            }

            _pickerBrush.GradientStops[i - 1].Offset = 1.0;
            _spectrumDisplay.Fill = _pickerBrush;
        }

        #endregion

        #endregion

    }
}
