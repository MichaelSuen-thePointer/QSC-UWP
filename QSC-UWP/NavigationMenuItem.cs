using System;
using Windows.UI.Xaml.Controls;

namespace QSC_UWP
{
    public class NavigationMenuItem
    {
        public string Label { get; set; }
        public Symbol Symbol { get; set; }

        public char SymbolAsChar => (char)this.Symbol;
        public Type DestPageType { get; set; }
        public object Arguments { get; set; }
    }
}