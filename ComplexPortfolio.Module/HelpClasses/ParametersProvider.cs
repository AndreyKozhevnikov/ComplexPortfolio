using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.HelpClasses {
    public class ParametersProvider : IParametersProvider {
        public virtual DateTime GetCurrentDate() {
            return DateTime.Today;
        }
    }
}
