using System;
using System.Linq;
using System.Windows.Media;

using AW.Visual.ColorTheme.ColorSet;

using DColor = System.Drawing.Color;
using MColor = System.Windows.Media.Color;

namespace AW.Visual
{
    /// <summary>
    /// *Set contains all material colors 
    /// https://www.materialpalette.com/colors
    /// </summary>
    public static class ColorHelper
    {
        public static Amber AmberSet { get; } = new Amber();
        public static Blue BlueSet { get; } = new Blue();
        public static BlueGrey BlueGreySet { get; } = new BlueGrey();
        public static Brown BrownSet { get; } = new Brown();
        public static Cyan CyanSet { get; } = new Cyan();
        public static DeepOrange DeepOrangeSet { get; } = new DeepOrange();
        public static DeepPurple DeepPurpleSet { get; } = new DeepPurple();
        public static Green GreenSet { get; } = new Green();
        public static Grey GreySet { get; } = new Grey();
        public static Indigo IndigoSet { get; } = new Indigo();
        public static LightBlue LightBlueSet { get; } = new LightBlue();
        public static LightGreen LightGreenSet { get; } = new LightGreen();
        public static Lime LimeSet { get; } = new Lime();
        public static Orange OrangeSet { get; } = new Orange();
        public static Pink PinkSet { get; } = new Pink();
        public static Purple PurpleSet { get; } = new Purple();
        public static Red RedSet { get; } = new Red();
        public static Teal TealSet { get; } = new Teal();
        public static Yellow YellowSet { get; } = new Yellow();

        public static SolidColorBrush BrushAlpha(this SolidColorBrush brush, byte alpha)
            => new SolidColorBrush(MColor.FromArgb(alpha, brush.Color.R, brush.Color.G, brush.Color.B));
        public static SolidColorBrush ToBrush(this DColor color)
           => new SolidColorBrush(color.ToMediaColor());
        public static SolidColorBrush ToBrush(this MColor color)
           => new SolidColorBrush(color);

        public static MColor ToMediaColor(this DColor color, byte? alpha = null)
            => MColor.FromArgb(alpha ?? color.A, color.R, color.G, color.B);
        public static DColor ToDrawColor(this MColor color, byte? alpha = null)
            => DColor.FromArgb(alpha ?? color.A, color.R, color.G, color.B);

        public static MColor FromArgb(int color)
        {
            byte[] argbarray = BitConverter.GetBytes(color).Reverse().ToArray();
            return MColor.FromArgb(argbarray[0], argbarray[1], argbarray[2], argbarray[3]);
        }

        public static MColor FromHSB(double H, double S, double B, byte? alpha = null)
        {
            if (H < 0)
                H += 360;
            else if (H >= 360)
                H -= 360;

            double r, g, b;

            if (B <= 0)
                r = g = b = 0;
            else if (S <= 0)
                r = g = b = B;
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;

                double pv = B * (1.0 - S);
                double qv = B * (1.0 - S * f);
                double tv = B * (1.0 - S * (1.0 - f));

                switch (i)
                {
                    case 0:
                        r = B;
                        g = tv;
                        b = pv;
                        break;
                    case 1:
                        r = qv;
                        g = B;
                        b = pv;
                        break;
                    case 2:
                        r = pv;
                        g = B;
                        b = tv;
                        break;
                    case 3:
                        r = pv;
                        g = qv;
                        b = B;
                        break;
                    case 4:
                        r = tv;
                        g = pv;
                        b = B;
                        break;
                    case 5:
                        r = B;
                        g = pv;
                        b = qv;
                        break;
                    case 6:
                        r = B;
                        g = tv;
                        b = pv;
                        break;
                    case -1:
                        r = B;
                        g = pv;
                        b = qv;
                        break;
                    default:
                        r = g = b = B;
                        break;
                }
            }

            return MColor.FromArgb(alpha ?? 255, Clamp((int)(r * 255)), Clamp((int)(g * 255)), Clamp((int)(b * 255)));
        }

        public static void ToHSB(this MColor color, out double H, out double S, out double B)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double cmax = Math.Max(r, Math.Max(g, b));
            double cmin = Math.Min(r, Math.Min(g, b)); 
            double diff = cmax - cmin;

            H = 0;
            B = cmax * 100;

            if (cmax == r)
                H = (60 * ((g - b) / diff) + 360) % 360;
            else if (cmax == g)
                H = (60 * ((b - r) / diff) + 120) % 360;
            else if (cmax == b)
                H = (60 * ((r - g) / diff) + 240) % 360;

            if (cmax == 0)
                S = 0;
            else
                S = (diff / cmax) * 100;

            S /= 100;
            B /= 100;

            if (double.IsNaN(H))
                H = 0;
        }

        private static byte Clamp(int i)
        {
            if (i < 0) 
                return 0;
            if (i > 255) 
                return 255;

            return (byte)i;
        }
    }
}
