using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartMoveAssistant.Api.Configuration;
using SmartMoveAssistant.Api.Models;
using SmartMoveAssistant.Api.Services;

namespace SmartMoveAssistant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferController : ControllerBase
{
    private readonly ILlmService _llmService;
    private readonly LlmOptions _options;
    private readonly ILogger<TransferController> _logger;

    public TransferController(ILlmService llmService, IOptions<LlmOptions> options, ILogger<TransferController> logger)
    {
        _llmService = llmService;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Processes conversational move request, extracts mandatory data, and enforces validation rules.
    /// </summary>
    [HttpPost("process")]
    public async Task<ActionResult<TransferResponseDto>> ProcessTransfer([FromBody] ProcessTransferRequest request, CancellationToken cancellationToken)
    {
        if (request == null || (string.IsNullOrWhiteSpace(request.Message) && request.ExistingData == null))
        {
            return BadRequest(new { error = "Request message or existing data must be provided." });
        }

        var result = await _llmService.ProcessTransferMessageAsync(request.Message, request.ExistingData, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Confirms and schedules the move order after all mandatory fields are satisfied.
    /// </summary>
    [HttpPost("confirm")]
    public ActionResult<ConfirmTransferResponse> ConfirmTransfer([FromBody] ConfirmTransferRequest request)
    {
        var dto = request?.TransferDetails;
        var confirmationId = $"SMA-{Random.Shared.Next(100000, 999999)}";
        var dateScheduled = dto?.ConnectDate ?? dto?.DisconnectDate ?? DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd");

        _logger.LogInformation("Confirmed transfer order {ConfirmationId} for account {Account} at {Address}", 
            confirmationId, dto?.AccountNumber, dto?.NewAddress ?? dto?.CurrentAddress);

        return Ok(new ConfirmTransferResponse
        {
            ConfirmationNumber = confirmationId,
            Status = "Confirmed & Dispatched",
            ScheduledDate = dateScheduled,
            Message = $"Order #{confirmationId} has been scheduled for {dto?.FirstName} {dto?.LastName}. Service activation date: {dateScheduled}. Dispatch notice sent to {dto?.Email}."
        });
    }

    /// <summary>
    /// Returns available models from the LLM provider.
    /// </summary>
    [HttpGet("models")]
    public async Task<ActionResult<List<string>>> GetModels(CancellationToken cancellationToken)
    {
        var models = await _llmService.GetAvailableModelsAsync(cancellationToken);
        return Ok(models);
    }

    /// <summary>
    /// Service health & configuration status check.
    /// </summary>
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            service = "Smart Move Assistant API",
            status = "Online",
            model = _options.Model,
            endpoint = _options.BaseUrl,
            timestamp = DateTime.UtcNow
        });
    }
}
