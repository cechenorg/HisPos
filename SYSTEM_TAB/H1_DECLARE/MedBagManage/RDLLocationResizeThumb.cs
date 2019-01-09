using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace His_Pos.SYSTEM_TAB.H1_DECLARE.MedBagManage
{
    public class RdlLocationResizeThumb : Thumb
    {
        public RdlLocationResizeThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control designerItem = this.DataContext as Control;
            Canvas parent = designerItem.Parent as Canvas;
            double canvasWidth = parent.Width;
            double canvasHeight = parent.Height;
            if (designerItem != null)
            {
                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        designerItem.Height = ((designerItem.Height - deltaVertical) > canvasHeight) ? canvasHeight : designerItem.Height - deltaVertical;
                        break;

                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        double topLimit = (Canvas.GetTop(designerItem) + deltaVertical) < 0 ? 0 : (Canvas.GetTop(designerItem) + deltaVertical);
                        Canvas.SetTop(designerItem, topLimit);
                        designerItem.Height -= (topLimit == 0.0) ? 0 : (deltaVertical);
                        break;

                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        double leftLimit = (Canvas.GetLeft(designerItem) + deltaHorizontal) < 0 ? 0 : (Canvas.GetLeft(designerItem) + deltaHorizontal);
                        Canvas.SetLeft(designerItem, leftLimit);
                        designerItem.Width -= (leftLimit == 0.0) ? 0 : deltaHorizontal;
                        break;

                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        designerItem.Width = (designerItem.Width - deltaHorizontal > canvasWidth) ? canvasWidth : designerItem.Width - deltaHorizontal;
                        break;

                    default:
                        break;
                }
            }
            designerItem.Height = designerItem.Height < 10 ? 10 : designerItem.Height;
            designerItem.Width = designerItem.Width < 10 ? 10 : designerItem.Width;
            e.Handled = true;
        }
    }
}