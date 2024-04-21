using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPFCursach.Classes
{
    internal class FrameSingleton
    {
        // содержит Frame
        private static Frame _frame;

        public static void setFrame(Frame frame) 
        {
            _frame = frame;
        }

        public static Frame getFrame() { return _frame;}
    }
}
