using Swazor.Extensions;
using Swazor.SwaggerUi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwazor(options =>
{
    options.DescriptionsPath = "Descriptions";
    options.WarnOnMissingTemplate = true;
    options.ValidateTemplatesOnStartup = true;
});

builder.Services.AddOpenApi(options =>
{
    options.AddSwazorDescriptions();
});

var app = builder.Build();

app.MapOpenApi();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Swazor Sample API");
    options.UseSwazorStyles();
});

app.MapControllers();

app.Run();