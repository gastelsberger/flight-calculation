using AppServices;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqliteDbContext<ApplicationDataContext>("database");
builder.Services.AddOpenApi();

// Register business logic services
builder.Services.AddScoped<IConfigurationOptimizer, ConfigurationOptimizer>();
builder.Services.AddScoped<IScheduleValidator, ScheduleValidator>();
builder.Services.AddScoped<IBookingCurveAnalyzer, BookingCurveAnalyzer>();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.MapOpenApi();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
app.UseHttpsRedirection();

// Map flight planning endpoints
app.MapFlightPlanningEndpoints();

app.Run();
