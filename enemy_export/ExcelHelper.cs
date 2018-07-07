using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace enemy_export
{
    class ExcelHelper
    {
        Microsoft.Office.Interop.Excel.Application oXL;
        Microsoft.Office.Interop.Excel._Workbook oWB;
        Microsoft.Office.Interop.Excel._Worksheet oSheet;
        Microsoft.Office.Interop.Excel.Range oRng;
        private int index;

        public ExcelHelper()
        {
            oXL = new Microsoft.Office.Interop.Excel.Application();
            oXL.Visible = false;
            oWB = oXL.Workbooks.Add();
            oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            index = 1;
        }

        public void addLine(IEnumerable<object> list)
        {
            IEnumerator<object> iEnumerator = list.GetEnumerator();
            int i = 1;
            while (iEnumerator.MoveNext())
            {
                oSheet.Cells[index, i++].Value = iEnumerator.Current;
            }
            iEnumerator.Dispose();
            index++;
        }

        public void save(string filename)
        {
            oRng = oSheet.get_Range("A1", "N1");
            oRng.EntireColumn.AutoFit();

            File.Delete(filename);
            // package.SaveAs(new FileInfo(filename));
            oXL.Visible = false;
            oXL.UserControl = false;
            oWB.SaveAs(filename, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            oWB.Close();
        }

    }
}
