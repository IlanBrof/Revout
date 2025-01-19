using System.Windows;

namespace Revout
{
    public partial class HoverBar : Window
    {
        public HoverBar()
        {
            InitializeComponent();
            Topmost = true; // Ensure the hover bar is always on top
            Visibility = Visibility.Hidden; // Start hidden
        }
    }
}

