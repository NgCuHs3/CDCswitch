using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDCswitchserver.interfaceUI
{
    class FunctionkeyModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Key;
        }
    }
}
