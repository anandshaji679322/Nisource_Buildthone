using SmartBillAi.Api.Models;

namespace SmartBillAi.Api.Services;

public interface IBillingEstimatorService
{
    EstimatorDashboardDto GetDashboard();
    MeterEstimateDetailDto GetMeterDetail(string customerId);
    ReviewActionResponse UpdateReviewStatus(ReviewActionRequest request);
    BacktestEvaluationResultDto RunBacktest();
}
