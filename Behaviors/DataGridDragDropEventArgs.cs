using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace His_Pos.Behaviors
{
    public class DataGridDragDropEventArgs : EventArgs
    {
        public object Source { get; internal set; }
        public object Destination { get; internal set; }

        public object DroppedObject { get; internal set; }
        public object TargetObject { get; internal set; }

        public DataGridDragDropDirection Direction { get; internal set; }
        public DragDropEffects Effects { get; internal set; }
    }
}
