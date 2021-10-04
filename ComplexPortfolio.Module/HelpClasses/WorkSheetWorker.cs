using DevExpress.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPortfolio.Module.HelpClasses {
    public class WorkSheetWorker : IWorkSheetWorker {
        public Worksheet CurrentWorkSheet { get; set; }

        public void SetCellValue(int rowNumber, int columnNumber, CellValue value) {
            CurrentWorkSheet.Cells[rowNumber, columnNumber].Value = value;
        }
    }
}
