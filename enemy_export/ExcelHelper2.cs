using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
// using OfficeOpenXml;

namespace enemy_export
{
    class ExcelHelper2
    {
        /*
        private ExcelPackage package;
        private ExcelWorksheet worksheet;
        private int index;

        public ExcelHelper2()
        {
            package = new ExcelPackage();
            worksheet = package.Workbook.Worksheets.Add("Sheet1");
            index = 1;
        }

        public void addLine(IEnumerable<object> list)
        {
            IEnumerator<object> iEnumerator = list.GetEnumerator();
            int i = 1;
            while (iEnumerator.MoveNext())
            {
                worksheet.Cells[index, i++].Value = iEnumerator.Current;
            }
            iEnumerator.Dispose();
            index++;
        }

        public void save(string filename)
        {
            File.Delete(filename);
            package.SaveAs(new FileInfo(filename));
        }
        */
    }
}
