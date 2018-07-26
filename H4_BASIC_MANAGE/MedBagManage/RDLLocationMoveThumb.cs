using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace His_Pos.H4_BASIC_MANAGE.MedBagManage
{
    public class RDLLocationMoveThumb : Thumb
    {
        public RDLLocationMoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control designerItem = DataContext as Control;
            if (designerItem != null)
            {
                double left = Canvas.GetLeft(designerItem);
                double top = Canvas.GetTop(designerItem);

                double leftShiftPoint = (left + e.HorizontalChange < 20) ? 20 : left + e.HorizontalChange;
                double topShiftPoint = (top + e.VerticalChange < 20) ? 20 : top + e.VerticalChange;

                leftShiftPoint = (leftShiftPoint + designerItem.Width > 1327) ? 1327 - designerItem.Width : leftShiftPoint;
                topShiftPoint = (topShiftPoint + designerItem.Height > 750) ? 750 - designerItem.Height : topShiftPoint;

                Canvas.SetLeft(designerItem, leftShiftPoint);
                Canvas.SetTop(designerItem, topShiftPoint);
            }
        }
    }
}
