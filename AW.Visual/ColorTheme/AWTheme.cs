using System.Windows.Media;

using MaterialDesignThemes.Wpf;

namespace AW.Visual.ColorTheme
{
    public static class AWTheme
    {
        public static IBaseTheme Light { get; } = new LightTheme();
        public static IBaseTheme Dark { get; } = new DarkTheme();
    }

    public class LightTheme : IBaseTheme
    {
        public Color ValidationErrorColor { get; } = (Color)ColorConverter.ConvertFromString("#f44336");
        public Color MaterialDesignBackground { get; } = (Color)ColorConverter.ConvertFromString("#FFE1E1E1");
        public Color MaterialDesignPaper { get; } = (Color)ColorConverter.ConvertFromString("#FFD1D1D1");
        public Color MaterialDesignCardBackground { get; } = (Color)ColorConverter.ConvertFromString("#FFE1E1E1");
        public Color MaterialDesignToolBarBackground { get; } = (Color)ColorConverter.ConvertFromString("#FFE1E1E1");
        public Color MaterialDesignBody { get; } = (Color)ColorConverter.ConvertFromString("#DD000000");
        public Color MaterialDesignBodyLight { get; } = (Color)ColorConverter.ConvertFromString("#89000000");
        public Color MaterialDesignColumnHeader { get; } = (Color)ColorConverter.ConvertFromString("#BC000000");
        public Color MaterialDesignCheckBoxOff { get; } = (Color)ColorConverter.ConvertFromString("#89000000");
        public Color MaterialDesignCheckBoxDisabled { get; } = (Color)ColorConverter.ConvertFromString("#FFBDBDBD");
        public Color MaterialDesignTextBoxBorder { get; } = (Color)ColorConverter.ConvertFromString("#89000000");
        public Color MaterialDesignDivider { get; } = (Color)ColorConverter.ConvertFromString("#1F000000");
        public Color MaterialDesignSelection { get; } = (Color)ColorConverter.ConvertFromString("#FFDeDeDe");
        public Color MaterialDesignFlatButtonClick { get; } = (Color)ColorConverter.ConvertFromString("#FFDeDeDe");
        public Color MaterialDesignFlatButtonRipple { get; } = (Color)ColorConverter.ConvertFromString("#FFB6B6B6");
        public Color MaterialDesignToolTipBackground { get; } = (Color)ColorConverter.ConvertFromString("#757575");
        public Color MaterialDesignChipBackground { get; } = (Color)ColorConverter.ConvertFromString("#12000000");
        public Color MaterialDesignSnackbarBackground { get; } = (Color)ColorConverter.ConvertFromString("#FF323232");
        public Color MaterialDesignSnackbarMouseOver { get; } = (Color)ColorConverter.ConvertFromString("#FF464642");
        public Color MaterialDesignSnackbarRipple { get; } = (Color)ColorConverter.ConvertFromString("#FFB6B6B6");
        public Color MaterialDesignTextFieldBoxBackground { get; } = (Color)ColorConverter.ConvertFromString("#0F000000");
        public Color MaterialDesignTextFieldBoxHoverBackground { get; } = (Color)ColorConverter.ConvertFromString("#14000000");
        public Color MaterialDesignTextFieldBoxDisabledBackground { get; } = (Color)ColorConverter.ConvertFromString("#08000000");
        public Color MaterialDesignTextAreaBorder { get; } = (Color)ColorConverter.ConvertFromString("#BC000000");
        public Color MaterialDesignTextAreaInactiveBorder { get; } = (Color)ColorConverter.ConvertFromString("#0F000000");
    }

    public class DarkTheme : IBaseTheme
    {
        public Color ValidationErrorColor { get; } = (Color)ColorConverter.ConvertFromString("#f44336");
        public Color MaterialDesignBackground { get; } = (Color)ColorConverter.ConvertFromString("#FF000000");
        public Color MaterialDesignPaper { get; } = (Color)ColorConverter.ConvertFromString("#FF303030");
        public Color MaterialDesignCardBackground { get; } = (Color)ColorConverter.ConvertFromString("#FF161616");
        public Color MaterialDesignToolBarBackground { get; } = (Color)ColorConverter.ConvertFromString("#FF212121");
        public Color MaterialDesignBody { get; } = (Color)ColorConverter.ConvertFromString("#DDFFFFFF");
        public Color MaterialDesignBodyLight { get; } = (Color)ColorConverter.ConvertFromString("#89FFFFFF");
        public Color MaterialDesignColumnHeader { get; } = (Color)ColorConverter.ConvertFromString("#BCFFFFFF");
        public Color MaterialDesignCheckBoxOff { get; } = (Color)ColorConverter.ConvertFromString("#89FFFFFF");
        public Color MaterialDesignCheckBoxDisabled { get; } = (Color)ColorConverter.ConvertFromString("#FF647076");
        public Color MaterialDesignTextBoxBorder { get; } = (Color)ColorConverter.ConvertFromString("#89FFFFFF");
        public Color MaterialDesignDivider { get; } = (Color)ColorConverter.ConvertFromString("#1FFFFFFF");
        public Color MaterialDesignSelection { get; } = (Color)ColorConverter.ConvertFromString("#757575");
        public Color MaterialDesignFlatButtonClick { get; } = (Color)ColorConverter.ConvertFromString("#19757575");
        public Color MaterialDesignFlatButtonRipple { get; } = (Color)ColorConverter.ConvertFromString("#FFB6B6B6");
        public Color MaterialDesignToolTipBackground { get; } = (Color)ColorConverter.ConvertFromString("#eeeeee");
        public Color MaterialDesignChipBackground { get; } = (Color)ColorConverter.ConvertFromString("#FF2E3C43");
        public Color MaterialDesignSnackbarBackground { get; } = (Color)ColorConverter.ConvertFromString("#FFCDCDCD");
        public Color MaterialDesignSnackbarMouseOver { get; } = (Color)ColorConverter.ConvertFromString("#FFB9B9BD");
        public Color MaterialDesignSnackbarRipple { get; } = (Color)ColorConverter.ConvertFromString("#FF494949");
        public Color MaterialDesignTextFieldBoxBackground { get; } = (Color)ColorConverter.ConvertFromString("#1AFFFFFF");
        public Color MaterialDesignTextFieldBoxHoverBackground { get; } = (Color)ColorConverter.ConvertFromString("#1FFFFFFF");
        public Color MaterialDesignTextFieldBoxDisabledBackground { get; } = (Color)ColorConverter.ConvertFromString("#0DFFFFFF");
        public Color MaterialDesignTextAreaBorder { get; } = (Color)ColorConverter.ConvertFromString("#BCFFFFFF");
        public Color MaterialDesignTextAreaInactiveBorder { get; } = (Color)ColorConverter.ConvertFromString("#1AFFFFFF");
    }
}
