
namespace WpfDemosCommonCode.CustomControls
{
    /// <summary>
    /// Describes a color in terms of
    /// Hue, Saturation, and Value (brightness).
    /// </summary>
    internal struct HsvColor
    {
        public double H;
        public double S;
        public double V;

        public HsvColor(double h, double s, double v)
        {
            this.H = h;
            this.S = s;
            this.V = v;
        }
    }
}
