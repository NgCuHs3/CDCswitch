using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDCswitchserver.switcher
{
    class Finger
    {
        public double X;
        public double Y;
        public StateFinger State;
        public int FingerID;
    }

    public enum StateFinger
    {
        UP,
        DOWN
    }
}
