using System.Windows.Media;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.ColorTheme
{
    public static class AWTheme
    {
        public static IBaseTheme Light { get; set; } = new BaseTheme(false);
        public static IBaseTheme Dark { get; set; } = new BaseTheme(true);
    }

    public class BaseTheme : IBaseTheme
    {
        public BaseTheme(bool isDark)
        {
            MaterialDesignBackground = GetColor(isDark ? "#1C1D1E" : "#F0F1F2");
            MaterialDesignCardBackground = GetColor(isDark ? "#151617" : "#E5E6E7");
            MaterialDesignPaper = GetColor(isDark ? "#1D1E1F" : "#F1F2F3");

            MaterialDesignBody = GetColor(isDark ? "#DDFFFFFF" : "#DD000000");
            MaterialDesignBodyLight = GetColor(isDark ? "#DD000000" : "#DDFFFFFF");

            MaterialDesignSelection = GetColor(isDark ? "#AA444444" : "#88CCCCCC");
            MaterialDesignDivider = GetColor(isDark ? "#444444" : "#DD1C1D1E");
            
            MaterialDesignFlatButtonRipple = GetColor(isDark ? "#B6B6B6" : "#494949");

            ValidationErrorColor = GetColor("#F44336");
            MaterialDesignFlatButtonClick = GetColor(isDark ? "#AA333333" : "#198A8A8A");
            MaterialDesignToolBarBackground = GetColor(isDark ? "#212121" : "#DEDEDE");
            MaterialDesignColumnHeader = GetColor(isDark ? "#BCFFFFFF" : "#BC000000");
            MaterialDesignCheckBoxOff = GetColor(isDark ? "#7FFFFFFF" : "#7F000000");
            MaterialDesignCheckBoxDisabled = GetColor(isDark ? "#707070" : "#B9B9B9");
            MaterialDesignTextBoxBorder = GetColor(isDark ? "#7FFFFFFF" : "#7F000000");
            MaterialDesignToolTipBackground = GetColor(isDark ? "#EEEEEE" : "#111111");
            MaterialDesignChipBackground = GetColor(isDark ? "#2D3C46" : "#D1C3B9");
            MaterialDesignSnackbarBackground = GetColor(isDark ? "#CDCDCD" : "#323232");
            MaterialDesignSnackbarMouseOver = GetColor(isDark ? "#B9B9B9" : "#464646");
            MaterialDesignSnackbarRipple = GetColor(isDark ? "#494949" : "#B6B6B6");
            MaterialDesignTextFieldBoxBackground = GetColor(isDark ? "#1AFFFFFF" : "#1A000000");
            MaterialDesignTextFieldBoxHoverBackground = GetColor(isDark ? "#1FFFFFFF" : "#1F000000");
            MaterialDesignTextFieldBoxDisabledBackground = GetColor(isDark ? "#0DFFFFFF" : "#0D000000");
            MaterialDesignTextAreaBorder = GetColor(isDark ? "#BCFFFFFF" : "#BC000000");
            MaterialDesignTextAreaInactiveBorder = GetColor(isDark ? "#1AFFFFFF" : "#1A000000");

            MaterialDesignToolForeground = MaterialDesignDivider;
            MaterialDesignToolBackground = MaterialDesignBackground;
            MaterialDesignDataGridRowHoverBackground = MaterialDesignSelection;
        }

        public Color ValidationErrorColor { get; }
        public Color MaterialDesignBackground { get; }
        public Color MaterialDesignPaper { get; }
        public Color MaterialDesignCardBackground { get; }
        public Color MaterialDesignToolBarBackground { get; }
        public Color MaterialDesignBody { get; }
        public Color MaterialDesignBodyLight { get; }
        public Color MaterialDesignColumnHeader { get; }
        public Color MaterialDesignCheckBoxOff { get; }
        public Color MaterialDesignCheckBoxDisabled { get; }
        public Color MaterialDesignTextBoxBorder { get; }
        public Color MaterialDesignDivider { get; }
        public Color MaterialDesignSelection { get; }
        public Color MaterialDesignFlatButtonClick { get; }
        public Color MaterialDesignFlatButtonRipple { get; }
        public Color MaterialDesignToolTipBackground { get; }
        public Color MaterialDesignChipBackground { get; }
        public Color MaterialDesignSnackbarBackground { get; }
        public Color MaterialDesignSnackbarMouseOver { get; }
        public Color MaterialDesignSnackbarRipple { get; }
        public Color MaterialDesignTextFieldBoxBackground { get; }
        public Color MaterialDesignTextFieldBoxHoverBackground { get; }
        public Color MaterialDesignTextFieldBoxDisabledBackground { get; }
        public Color MaterialDesignTextAreaBorder { get; }
        public Color MaterialDesignTextAreaInactiveBorder { get; }

        public Color MaterialDesignToolForeground { get; }
        public Color MaterialDesignToolBackground { get; }
        public Color MaterialDesignDataGridRowHoverBackground { get; }

        private Color GetColor(string hex)
            => (Color)ColorConverter.ConvertFromString(hex);
    }
}