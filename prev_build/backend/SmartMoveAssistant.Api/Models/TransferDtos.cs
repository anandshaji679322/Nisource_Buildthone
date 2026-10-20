namespace SmartMoveAssistant.Api.Models;

public class ProcessTransferRequest
{
    public string Message { get; set; } = string.Empty;
    public TransferResponseDto? ExistingData { get; set; }
}

public class TransferResponseDto
{
    // Intent
    public ActionIntent ActionIntent { get; set; } = ActionIntent.Move;

    // Utility Account
    public string AccountNumber { get; set; } = string.Empty;

    // Customer Identity (Mandatory for all intents)
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Ssn { get; set; } = string.Empty;
    public string Dob { get; set; } = string.Empty;

    // Contact Information (Mandatory for all intents)
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Addresses
    public string CurrentAddress { get; set; } = string.Empty;
    public string NewAddress { get; set; } = string.Empty;
    public string ForwardingAddress { get; set; } = string.Empty;

    // Dates
    public string DisconnectDate { get; set; } = string.Empty;
    public string ConnectDate { get; set; } = string.Empty;

    // Calculations & Validation
    public int OverlapDays { get; set; } = 0;
    public bool IsComplete { get; set; } = false;
    public List<string> MissingFields { get; set; } = new();

    // AI Messaging
    public string AiExplanation { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; } = 0.98;
}

public class ConfirmTransferRequest
{
    public TransferResponseDto? TransferDetails { get; set; }
    public string? CustomerNotes { get; set; }
}

public class ConfirmTransferResponse
{
    public string ConfirmationNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Confirmed";
    public string ScheduledDate { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
