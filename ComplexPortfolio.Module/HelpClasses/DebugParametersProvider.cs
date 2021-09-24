using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.HelpClasses {
    public class DebugParametersProvider: ParametersProvider {
        public DateTime CurrentDate{ get; set; }
        public override DateTime GetCurrentDate() {
            return CurrentDate;
        }
    }
}
