using Microsoft.AspNetCore.Mvc;
using SmartBillAi.Api.Models;
using SmartBillAi.Api.Services;

namespace SmartBillAi.Api.Controllers;

[ApiController]
[Route("api/estimator")]
public class BillingEstimatorController : ControllerBase
{
    private readonly IBillingEstimatorService _estimatorService;
    private readonly ILogger<BillingEstimatorController> _logger;

    public BillingEstimatorController(IBillingEstimatorService estimatorService, ILogger<BillingEstimatorController> logger)
    {
        _estimatorService = estimatorService;
        _logger = logger;
    }

    [HttpGet("dashboard")]
    public ActionResult<EstimatorDashboardDto> GetDashboard()
    {
        var dashboard = _estimatorService.GetDashboard();
        return Ok(dashboard);
    }

    [HttpGet("meter/{customerId}")]
    public ActionResult<MeterEstimateDetailDto> GetMeterDetail(string customerId)
    {
        var detail = _estimatorService.GetMeterDetail(customerId);
        return Ok(detail);
    }

    [HttpPost("review")]
    public ActionResult<ReviewActionResponse> UpdateReviewStatus([FromBody] ReviewActionRequest request)
    {
        var result = _estimatorService.UpdateReviewStatus(request);
        return Ok(result);
    }

    [HttpGet("backtest")]
    public ActionResult<BacktestEvaluationResultDto> RunBacktest()
    {
        var backtest = _estimatorService.RunBacktest();
        return Ok(backtest);
    }
}
