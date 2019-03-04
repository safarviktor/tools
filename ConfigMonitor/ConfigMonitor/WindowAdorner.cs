using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ConfigMonitor
{
    public class WindowAdorner : Adorner
    {
        public WindowAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(Brushes.Transparent, new Pen(), new Rect(0, 0, 100, 100));
        }
    }
}