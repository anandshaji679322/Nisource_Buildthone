namespace SmartBillAi.Api.Configuration;

public class LlmOptions
{
    public const string SectionName = "LlmSettings";

    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://genailab.tcs.in";
    public string Model { get; set; } = "azure/genailab-maas-gpt-4o-mini";
    public double Temperature { get; set; } = 0.2;
    public int TimeoutSeconds { get; set; } = 25;
}
