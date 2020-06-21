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

        public static DColor FromArgb(int color)
        {
            byte[] argbarray = BitConverter.GetBytes(color).Reverse().ToArray();
            return DColor.FromArgb(argbarray[0], argbarray[1], argbarray[2], argbarray[3]);
        }

        public static SolidColorBrush ToBrush(this DColor color)
           => new SolidColorBrush(color.ToMediaColor());

        public static SolidColorBrush BrushAlpha(this SolidColorBrush brush, byte alpha)
            => new SolidColorBrush(MColor.FromArgb(alpha, brush.Color.R, brush.Color.G, brush.Color.B));

        public static MColor ToMediaColor(this DColor color)
            => MColor.FromArgb(color.A, color.R, color.G, color.B);

        public static DColor ToDrawColor(this MColor color)
            => DColor.FromArgb(color.A, color.R, color.G, color.B);

        public static MColor ToMediaColor(this DColor color, byte a)
            => MColor.FromArgb(a, color.R, color.G, color.B);
    }
}
