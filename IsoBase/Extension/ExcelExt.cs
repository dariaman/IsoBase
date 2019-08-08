using EPPlus.DataExtractor;
using IsoBase.Models;
using IsoBase.ViewModels;
using System.Collections.Generic;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using IsoBase.Data;
using EFCore.BulkExtensions;
using System.Threading.Tasks;

namespace IsoBase.Extension
{
    public class ExcelExt
    {
        FileStream _filestrm;
        int _clientID;

        public ExcelExt(FileStream Filestrm, int ClientID)
        {
            _filestrm = Filestrm;
            _clientID = ClientID;
        }

        public EnrollPlanFileExcelDataVM ReadExcelEnrollPlan( ref EnrollPlanFileExcelDataVM evm)
        {
            //var evm = new EnrollPlanFileExcelDataVM();
            ExcelPackage package;
            try { package = new ExcelPackage(this._filestrm); }
            catch { throw new Exception("The file is not an valid Excel file. If the file is encrypted, please remove the password"); }

            if (package.Workbook.Worksheets.Count() < 3) throw new Exception("File must consist of 3 sheet (Plan,Coverage,Benefit)");

            var plans = new List<PlanUploadModel>(evm.ClientID);
            var covs = new List<CoverageUploadModel>(evm.ClientID);
            var benfs = new List<BenefitUploadModel>(evm.ClientID);
            ExcelWorksheet ws0 = package.Workbook.Worksheets[0];
            try
            {
                // Sheet 1 Plan
                plans = ws0
                    .Extract<PlanUploadModel>()
                    .WithProperty(p => p.PayorCode, "A")
                    .WithProperty(p => p.PlanId, "B")
                    .WithProperty(p => p.CorpCode, "D")
                    .WithProperty(p => p.EffectiveDate, "E")
                    .WithProperty(p => p.TerminationDate, "F")
                    .WithProperty(p => p.ActiveFlag, "G")
                    .WithProperty(p => p.ShortName, "H")
                    .WithProperty(p => p.LongName, "I")
                    .WithProperty(p => p.Remarks, "K")
                    .WithProperty(p => p.PolicyNo, "L")
                    .WithProperty(p => p.FrequencyCode, "N")
                    .WithProperty(p => p.LimitCode, "O")
                    .WithProperty(p => p.MaxValue, "Q")
                    .WithProperty(p => p.FamilyMaxValue, "S")
                    .WithProperty(p => p.PrintText, "U")
                    .GetData(2, ws0.Dimension.Rows)
                    .ToList();
                ws0.Dispose();
                plans.RemoveAll(x => x == null);
                plans.RemoveAll(x => string.IsNullOrWhiteSpace(x.PayorCode));
                evm.PlanUploadModel = plans;

                // Sheet 2 Coverage
                ws0 = package.Workbook.Worksheets[1];
                covs = ws0
                    .Extract<CoverageUploadModel>()
                    .WithProperty(p => p.PlanId, "A")
                    .WithProperty(p => p.CorpCode, "B")
                    .WithProperty(p => p.CoverageCode, "C")
                    .WithProperty(p => p.ActiveFlag, "D")
                    .WithProperty(p => p.ClientCoverageCode, "E")
                    .WithProperty(p => p.LimitCode, "G")
                    .WithProperty(p => p.FrequencyCode, "H")
                    .WithProperty(p => p.MinValue, "I")
                    .WithProperty(p => p.MaxValue, "J")
                    .WithProperty(p => p.FamilyValue, "K")
                    .GetData(2, ws0.Dimension.Rows)
                    .ToList();
                ws0.Dispose();
                covs.RemoveAll(x => x == null);
                covs.RemoveAll(x => string.IsNullOrWhiteSpace(x.PlanId));
                evm.CoverageUploadModel = covs;

                // Sheet 3 Benefit
                ws0 = package.Workbook.Worksheets[2];
                benfs = ws0
                    .Extract<BenefitUploadModel>()
                    .WithProperty(p => p.PlanId, "A")
                    .WithProperty(p => p.CorpCode, "B")
                    .WithProperty(p => p.CoverageCode, "C")
                    .WithProperty(p => p.BenefitCode, "D")
                    .WithProperty(p => p.ActiveFlag, "E")
                    .WithProperty(p => p.ConditionDescription, "F")
                    .WithProperty(p => p.LOA_Description, "G")
                    .WithProperty(p => p.ClientBenefitcode, "H")
                    .WithProperty(p => p.MaxValue, "J")
                    .WithProperty(p => p.LimitCode, "K")
                    .WithProperty(p => p.FrequencyCode, "L")
                    .WithProperty(p => p.MultipleCondition, "P")
                    .GetData(2, ws0.Dimension.Rows)
                    .ToList();
                benfs.RemoveAll(x => x == null);
                benfs.RemoveAll(x => string.IsNullOrWhiteSpace(x.PlanId));
                evm.BenefitUploadModel = benfs;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Read Excel Sheet : " + ex.Message);
            }
            finally
            {
                ws0.Dispose();
                package.Dispose();
                _filestrm.Dispose();
            }
            return evm;
        }
    }
}
