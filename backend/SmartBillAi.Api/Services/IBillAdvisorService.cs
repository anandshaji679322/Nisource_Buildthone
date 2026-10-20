using SmartBillAi.Api.Models;

namespace SmartBillAi.Api.Services;

public interface IBillAdvisorService
{
    CustomerProfileDto GetCustomerProfile(string accountNumber);
    CustomerDashboardSummaryDto GetDashboardSummary(string accountNumber, string? selectedPeriodId = null);
    BillComparisonResponse CompareBills(BillComparisonRequest request);
    List<EnergyRecommendationItem> GetPersonalizedRecommendations(string accountNumber, string periodId);
}
