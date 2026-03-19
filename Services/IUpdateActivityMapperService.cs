using PartsControlSystem.Models;
using PartsControlSystem.ViewModels;

namespace PartsControlSystem.Services
{
    public interface IUpdateActivityMapperService
    {
        MP2ToolingQuotationRequestApprovalVM MapQuotationRequest(ImportData data);
        MP2ToolingRequestOrderVM MapRequestOrder(ImportData data);
        MP2ToolingPoIssuanceVM MapPoIssuance(ImportData data);
        SQCDfmQcdApprovalVM MapDfmQcdApproval(ImportData data);
        MP2ToolingFabricationVM MapToolingFabrication(ImportData data);
        MP2ToolingTransferVM MapToolingTransfer(ImportData data);
        IQCKatakenSubmissionVM MapKatakenSubmission(ImportData data);
        IQCKatakenFinishVM MapKatakenFinish(ImportData data);
        DEEvaluationVM MapEvaluation(ImportData data);
        QASpecialEvaluationVM MapSpecialEvaluation(ImportData data);
        IQCTestRunVM MapTestRun(ImportData data);

    }
}
