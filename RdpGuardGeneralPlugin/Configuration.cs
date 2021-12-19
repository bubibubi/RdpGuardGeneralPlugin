using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RdpGuardGeneralPlugin
{
    public class Configuration
    {
        public string Name { get; set; }

        public string Protocol { get; set; }

        public string Directory { get; set; }

        public string FileMask { get; set; }

        public string RegularExpression { get; set; }
    }
}
