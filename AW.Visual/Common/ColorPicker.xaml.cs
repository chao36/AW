using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

using AW.Visual.Behaviors;

using MColor = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace AW.Visual.Common
{
    public partial class ColorPicker : BaseControl
    {
        private ColorPickerContext Context => Container.DataContext as ColorPickerContext;

        public MColor CurrentColor
        {
            get => Context.CurrentColor;
            set
            {
                Context.CurrentColor = value;
                if (IsLoaded)
                    Context.UpdatePointPosition();
            }
        }

        public ColorPicker()
        {
            InitializeComponent();

            VisualHelper.ExitOnEnter(R);
            VisualHelper.ExitOnEnter(G);
            VisualHelper.ExitOnEnter(B);
            VisualHelper.ExitOnEnter(Hex);

            VisualHelper.LimitInput(R, LimitType.Int);
            VisualHelper.LimitInput(G, LimitType.Int);
            VisualHelper.LimitInput(B, LimitType.Int);

            Container.DataContext = new ColorPickerContext
            {
                ColorsDrag = new DragContext()
                {
                    OnUpdate = ColorsUpdate,
                    Bounds = new Thickness(0, 0, 200, 200),
                    CanYChange = true,
                    CanXChange = true
                },
                HueDrag = new DragContext()
                {
                    OnUpdate = HueColorsUpdate,
                    Bounds = new Thickness(0, 0, 0, 200),
                    CanYChange = true,
                    CanXChange = false
                }
            };

            VisualHelper.LeftDown(Colors, e =>
            {
                Point point = e.GetPosition(Colors.Parent as FrameworkElement);

                Context?.ColorsDrag.SetPositionAndCapture(point.X, point.Y);
            });
            VisualHelper.LeftDown(HueColors, e =>
            {
                Point point = e.GetPosition(HueColors.Parent as FrameworkElement);

                Context?.HueDrag.SetPositionAndCapture(0, point.Y);
            });
        }

        protected override void OnLoaded()
        {
            Context.UpdatePointPosition();
        }

        private void ColorsUpdate(double x, double y)
        {
            double H = 1.8 * Context.HueDrag.Y;
            double S = x / 200;
            double B = (100 - y / 2) / 100;

            Context.CurrentColor = ColorHelper.MediaColorFromHSB(H, S, B);
        }

        private void HueColorsUpdate(double x, double y)
        {
            Context.HueColor = ColorHelper.MediaColorFromHSB(1.8 * y, 1, 1);
            ColorsUpdate(Context.ColorsDrag.X, Context.ColorsDrag.Y);
        }
    }

    public class ColorPickerContext : BaseContext
    {
        public DragContext ColorsDrag { get; set; }
        public DragContext HueDrag { get; set; }

        private MColor currentColor;
        public MColor CurrentColor
        {
            get => currentColor;
            set
            {
                currentColor = value;

                NotifyAll();
            }
        }

        private MColor hueColor = Colors.Red;
        public MColor HueColor
        {
            get => hueColor;
            set
            {
                hueColor = value;

                NotifyAll();
            }
        }

        public object R
        {
            get => CurrentColor.R;
            set
            {
                if (byte.TryParse(value.ToString(), out byte r))
                    currentColor.R = r;

                NotifyAll();
                UpdatePointPosition();
            }
        }

        public object G
        {
            get => CurrentColor.G;
            set
            {
                if (byte.TryParse(value.ToString(), out byte g))
                    currentColor.G = g;

                NotifyAll();
                UpdatePointPosition();
            }
        }

        public object B
        {
            get => CurrentColor.B;
            set
            {
                if (byte.TryParse(value.ToString(), out byte b))
                    currentColor.B = b;

                NotifyAll();
                UpdatePointPosition();
            }
        }

        public string Hex
        {
            get => ColorTranslator.ToHtml(CurrentColor.ToDrawColor()).Replace("#", "");
            set
            {
                try
                {
                    currentColor = ColorTranslator.FromHtml(value).ToMediaColor();
                }
                catch { }

                NotifyAll();
                UpdatePointPosition();
            }
        }

        private void NotifyAll()
        {
            Notify(nameof(HueColor));
            Notify(nameof(CurrentColor));
            Notify(nameof(R));
            Notify(nameof(G));
            Notify(nameof(B));
            Notify(nameof(Hex));
        }

        public void UpdatePointPosition()
        {
            CurrentColor.ToHSB(out double H, out double S, out double B);
            HueColor = ColorHelper.MediaColorFromHSB(H, 1, 1);
            Notify(nameof(HueColor));

            HueDrag.Y = H / 1.8;
            HueDrag.SetPosition(0, HueDrag.Y);

            ColorsDrag.X = S * 200;
            ColorsDrag.Y = 200 - 200 * B;
            ColorsDrag.SetPosition(ColorsDrag.X, ColorsDrag.Y);
        }
    }

    public class DragContext : IDragElementContext
    {
        public Action OnStart { get; set; }
        public Action<double, double> OnUpdate { get; set; }
        public Action OnEnd { get; set; }

        public Action<double, double> SetPosition { get; set; }
        public Action<double, double> SetPositionAndUpdate { get; set; }
        public Action<double, double> SetPositionAndCapture { get; set; }

        public Thickness? Bounds { get; set; }
        public bool CanXChange { get; set; }
        public bool CanYChange { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public void OnStartUpdatePosition()
            => OnStart?.Invoke();
        public void OnUpdatePosition(double x, double y)
        {
            X = x;
            Y = y;
            OnUpdate?.Invoke(x, y);
        }
        public void OnEndUpdatePosition()
            => OnEnd?.Invoke();

    }
}
