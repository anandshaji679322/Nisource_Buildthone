using SmartBillAi.Api.Configuration;
using SmartBillAi.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers with JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Configure LLM Options
builder.Services.Configure<LlmOptions>(builder.Configuration.GetSection(LlmOptions.SectionName));

// Configure HttpClient with SSL bypass for internal enterprise/lab gateways
builder.Services.AddHttpClient<ILlmService, LlmService>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

// Configure HttpClient for Weather Service
builder.Services.AddHttpClient<IWeatherService, WeatherService>();

// Register Domain Services as Singletons to preserve state during demo interactions
builder.Services.AddSingleton<IBillAdvisorService, BillAdvisorService>();
builder.Services.AddSingleton<IBillingEstimatorService, BillingEstimatorService>();

// Configure CORS for Vite frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SmartBill AI API — Intelligent Billing & Usage Advisor", Version = "v1" });
});

var app = builder.Build();

// Enable Swagger in all environments for hackathon demo convenience
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartBill AI API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");

// Map Controllers
app.MapControllers();

// Health check endpoint
app.MapGet("/api/health", () => Results.Ok(new
{
    status = "Healthy",
    product = "SmartBill AI",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow
}));

// Root redirect to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
