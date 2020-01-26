using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDCswitchserver.interfaceUI
{
    public class Sourceuser
    {
        private bool allowmirrorscreen;
        private double opacityofmirrorscreen;
        private string pathsavesnapshoot;

        public bool Allowmirrorscreen { get => allowmirrorscreen; set => allowmirrorscreen = value; }
        public double Opacityofmirrorscreen { get => opacityofmirrorscreen; set => opacityofmirrorscreen = value; }
        public string Pathsavesnapshoot { get => pathsavesnapshoot; set => pathsavesnapshoot = value; }
    }
}
