using PartsControlSystem.Models;
using PartsControlSystem.ViewModels;

namespace PartsControlSystem.Services
{
    public class UpdateActivityMapperService : IUpdateActivityMapperService
    {
        public MP2ToolingQuotationRequestApprovalVM MapQuotationRequest(ImportData data)
        {
            return new MP2ToolingQuotationRequestApprovalVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }

        public MP2ToolingRequestOrderVM MapRequestOrder(ImportData data)
        {
            return new MP2ToolingRequestOrderVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }

        public MP2ToolingPoIssuanceVM MapPoIssuance(ImportData data)
        {
            return new MP2ToolingPoIssuanceVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }

        public SQCDfmQcdApprovalVM MapDfmQcdApproval(ImportData data) 
        {
            return new SQCDfmQcdApprovalVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }

        public MP2ToolingFabricationVM MapToolingFabrication(ImportData data)
        {
            return new MP2ToolingFabricationVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }

        public MP2ToolingTransferVM MapToolingTransfer(ImportData data)
        {
            return new MP2ToolingTransferVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }
        
        public IQCKatakenSubmissionVM MapKatakenSubmission(ImportData data)
        {
            return new IQCKatakenSubmissionVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }

        public IQCKatakenFinishVM MapKatakenFinish(ImportData data)
        {
            return new IQCKatakenFinishVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }

        public DEEvaluationVM MapEvaluation(ImportData data)
        {
            return new DEEvaluationVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }

        public QASpecialEvaluationVM MapSpecialEvaluation(ImportData data)
        {
            return new QASpecialEvaluationVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }

        public IQCTestRunVM MapTestRun(ImportData data)
        {
            return new IQCTestRunVM
            {
                ImportDataId = data.Id,
                ControlNo = data.ControlNo,
                Section = data.Section,
                ToolingType = data.ToolingType,
                ToolingCategory = data.ToolingCategory,
                Model = data.Model,
                PartCode = data.ChildPartcode,
                PartName = data.PartName,
                ToolingManagement = data.ToolingManagement,
                Supplier = data.Supplier,
                BiphMoldNo = data.BIPHMoldNo,
                SupplierMoldNo = data.SupplierMoldNo,
                MoldMaker = data.MoldMaker
            };
        }

    }
}
