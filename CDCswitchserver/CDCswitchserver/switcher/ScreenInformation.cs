using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDCswitchserver.switcher
{
    public class ScreenInformation
    {
        public int Height;
        public int Width;
        public ScreenOrentation ScreenOrentation;
    }

    public enum ScreenOrentation
    {
        LEFT_HORIZONTAL,
        RIGHT_HORIZONTAL,
        MIDDLE_VERTICAL
    }
}
