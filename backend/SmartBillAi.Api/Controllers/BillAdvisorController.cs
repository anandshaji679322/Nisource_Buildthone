using Microsoft.AspNetCore.Mvc;
using SmartBillAi.Api.Models;
using SmartBillAi.Api.Services;

namespace SmartBillAi.Api.Controllers;

[ApiController]
[Route("api/bill")]
public class BillAdvisorController : ControllerBase
{
    private readonly IBillAdvisorService _advisorService;
    private readonly ILlmService _llmService;
    private readonly ILogger<BillAdvisorController> _logger;

    public BillAdvisorController(IBillAdvisorService advisorService, ILlmService llmService, ILogger<BillAdvisorController> logger)
    {
        _advisorService = advisorService;
        _llmService = llmService;
        _logger = logger;
    }

    [HttpGet("customer/{accountNumber}")]
    public ActionResult<CustomerProfileDto> GetCustomerProfile(string accountNumber)
    {
        var profile = _advisorService.GetCustomerProfile(accountNumber);
        return Ok(profile);
    }

    [HttpGet("summary")]
    public ActionResult<CustomerDashboardSummaryDto> GetDashboardSummary([FromQuery] string accountNumber = "ACT-7492-EAST", [FromQuery] string? periodId = null)
    {
        var summary = _advisorService.GetDashboardSummary(accountNumber, periodId);
        return Ok(summary);
    }

    [HttpPost("explain")]
    public async Task<ActionResult<ExplainBillResponse>> ExplainBill([FromBody] ExplainBillRequest request, CancellationToken cancellationToken)
    {
        var profile = _advisorService.GetCustomerProfile(request.AccountNumber);
        var currentBill = profile.BillingHistory.FirstOrDefault(x => x.PeriodId == request.SelectedPeriodId)
            ?? profile.BillingHistory.First();

        var previousBill = !string.IsNullOrEmpty(request.BaselinePeriodId)
            ? profile.BillingHistory.FirstOrDefault(x => x.PeriodId == request.BaselinePeriodId)
            : null;

        if (previousBill == null)
        {
            var idx = profile.BillingHistory.IndexOf(currentBill);
            previousBill = (idx + 1 < profile.BillingHistory.Count) ? profile.BillingHistory[idx + 1] : currentBill;
        }

        var explanation = await _llmService.ExplainBillAsync(currentBill, previousBill, profile, cancellationToken);
        return Ok(explanation);
    }

    [HttpPost("compare")]
    public ActionResult<BillComparisonResponse> CompareBills([FromBody] BillComparisonRequest request)
    {
        var result = _advisorService.CompareBills(request);
        return Ok(result);
    }

    [HttpGet("recommendations")]
    public ActionResult<List<EnergyRecommendationItem>> GetRecommendations([FromQuery] string accountNumber = "ACT-7492-EAST", [FromQuery] string periodId = "2026-07")
    {
        var recommendations = _advisorService.GetPersonalizedRecommendations(accountNumber, periodId);
        return Ok(recommendations);
    }
}
