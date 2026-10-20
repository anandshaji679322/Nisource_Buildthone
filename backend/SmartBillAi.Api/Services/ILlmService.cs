using SmartBillAi.Api.Models;

namespace SmartBillAi.Api.Services;

public interface ILlmService
{
    Task<ExplainBillResponse> ExplainBillAsync(BillPeriodRecord currentBill, BillPeriodRecord previousBill, CustomerProfileDto profile, CancellationToken cancellationToken = default);
}
