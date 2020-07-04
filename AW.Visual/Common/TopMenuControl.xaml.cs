using System.Collections.Generic;
using System.Windows.Controls;

namespace AW.Visual.Common
{
    public partial class TopMenuControl : UserControl
    {
        public TopMenuControl() => InitializeComponent();
    }

    public class TopMenuContext
    {
        public TopMenuContext(IEnumerable<IContextMenuAction> items)
        {
            Items = items;

            foreach (IContextMenuAction item in Items)
            {
                item.SubContextMenuType = SubContextMenuType.Bottom;
                item.SeparatorStyle = SeparatorStyle.None;
            }
        }

        public IEnumerable<IContextMenuAction> Items { get; }
    }
}
