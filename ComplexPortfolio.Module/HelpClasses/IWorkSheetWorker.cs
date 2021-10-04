using DevExpress.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.HelpClasses {
    public interface IWorkSheetWorker {
        void SetCellValue(int rowNumber, int columnNumber, CellValue value);
        Worksheet CurrentWorkSheet { get; set; }
    }
}
