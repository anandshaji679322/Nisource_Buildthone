using SmartMoveAssistant.Api.Models;

namespace SmartMoveAssistant.Api.Services;

public interface ILlmService
{
    Task<TransferResponseDto> ProcessTransferMessageAsync(string message, TransferResponseDto? existingData = null, CancellationToken cancellationToken = default);
    Task<List<string>> GetAvailableModelsAsync(CancellationToken cancellationToken = default);
}
