using IsoBase.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IsoBase.Extension
{
    public class ExcelExt
    {
        FileStream _filestrm;
        public ExcelExt(FileStream Filestrm)
        {
            _filestrm = Filestrm;
        }

        public EnrollPlanFileExcelDataVM ReadExcelEnrollPlan()
        {
            var evm = new EnrollPlanFileExcelDataVM();
            ExcelPackage package;
            try { package = new ExcelPackage(this._filestrm); }
            catch { throw new Exception("The file is not an valid Excel file. If the file is encrypted, please remove the password"); }

            if (package.Workbook.Worksheets.Count() < 3) throw new Exception("File must consist of 3 sheet (Plan,Coverage,Benefit)");

            // Sheet 1 Plan
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++) // baca mulai dari row 2
            {
                var pl = new PlanFileData
                {
                    PayorCode = worksheet.Cells[row, 2].Value.ToString(),
                    PlanID = worksheet.Cells[row, 3].Value.ToString(),
                    CorpCode = worksheet.Cells[row, 5].Value.ToString(),
                    EffectiveDate = worksheet.Cells[row, 6].Value.ToString(),
                    TerminationDate = worksheet.Cells[row, 7].Value.ToString(),
                    ActiveFlag = worksheet.Cells[row, 8].Value.ToString(),
                    ShortName = worksheet.Cells[row, 9].Value.ToString(),
                    LongName = worksheet.Cells[row, 10].Value.ToString(),
                    Remarks = worksheet.Cells[row, 12].Value.ToString(),
                    PolicyNo = worksheet.Cells[row, 13].Value.ToString(),
                    FrequencyCode = worksheet.Cells[row, 15].Value.ToString(),
                    LimitCode = worksheet.Cells[row, 16].Value.ToString(),
                    MaxValue = worksheet.Cells[row, 18].Value.ToString(),
                    FamilyLimit = worksheet.Cells[row, 20].Value.ToString(),
                    PrintText = worksheet.Cells[row, 22].Value.ToString()
                };
                evm.PlanData.Add(pl);
            }

            // Sheet 2 Coverage
            worksheet = package.Workbook.Worksheets[1];
            rowCount = worksheet.Dimension.Rows;
            for (int row = 2; row <= rowCount; row++)
            {
                var cl = new CoverageFileData
                {

                };
            }

                return evm;
        }
    }
}
