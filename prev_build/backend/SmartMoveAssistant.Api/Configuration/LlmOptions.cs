namespace SmartMoveAssistant.Api.Configuration;

public class LlmOptions
{
    public const string SectionName = "LlmSettings";

    public string ApiKey { get; set; } = "sk-w0AxDs1W5AFg07WGpRTYPw";
    public string BaseUrl { get; set; } = "https://genailab.tcs.in";
    public string Model { get; set; } = "azure/genailab-maas-gpt-4o-mini";
    public double Temperature { get; set; } = 0.1;
    public int TimeoutSeconds { get; set; } = 30;
}
