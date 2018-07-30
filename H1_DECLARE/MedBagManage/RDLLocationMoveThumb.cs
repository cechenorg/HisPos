﻿using System;
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
                Canvas parent = designerItem.Parent as Canvas;
                double canvasWidth = parent.Width;
                double canvasHeight = parent.Height;
                double left = Canvas.GetLeft(designerItem);
                double top = Canvas.GetTop(designerItem);

                double leftShiftPoint = (left + e.HorizontalChange < 0) ? 0 : left + e.HorizontalChange;
                double topShiftPoint = (top + e.VerticalChange < 0) ? 0 : top + e.VerticalChange;

                leftShiftPoint = (leftShiftPoint + designerItem.Width > canvasWidth) ? canvasWidth - designerItem.Width : leftShiftPoint;
                topShiftPoint = (topShiftPoint + designerItem.Height > canvasHeight) ? canvasHeight - designerItem.Height : topShiftPoint;

                Canvas.SetLeft(designerItem, leftShiftPoint);
                Canvas.SetTop(designerItem, topShiftPoint);
            }
        }
    }
}