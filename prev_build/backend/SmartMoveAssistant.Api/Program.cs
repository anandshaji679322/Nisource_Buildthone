using SmartMoveAssistant.Api.Configuration;
using SmartMoveAssistant.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// Configure LLM Options
builder.Services.Configure<LlmOptions>(builder.Configuration.GetSection(LlmOptions.SectionName));

// Configure HttpClient with SSL bypass for internal enterprise/lab gateways
builder.Services.AddHttpClient<ILlmService, LlmService>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

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
    c.SwaggerDoc("v1", new() { Title = "Smart Move Assistant API", Version = "v1" });
});

var app = builder.Build();

// Enable Swagger in all environments for hackathon demo convenience
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Move Assistant v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("AllowAll");

// Map Controllers
app.MapControllers();

// Root redirect to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
