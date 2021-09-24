using ComplexPortfolio.Module.BusinessObjects;
using ComplexPortfolio.Module.HelpClasses;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.Controllers {
    public class CalculatePositionController : ObjectViewController<DetailView, Position> {
        public CalculatePositionController() {
            var calculateAction = new SimpleAction(this, "Calculate", PredefinedCategory.Edit);
            calculateAction.Execute += AddDayDataAction_Execute; ;
        }

        private void AddDayDataAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            CalculatePosition(this.ViewCurrentObject, this.ObjectSpace, new ParametersProvider());
        }

        public void CalculatePosition(Position viewCurrentObject, IObjectSpace objectSpace, IParametersProvider parametersProvider) {

        }
    }
}
