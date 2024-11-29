using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Dynamic;

namespace PieHandlerService.Api.Controller;

[ApiVersion("1.0")]
[Route("api/healthcheck")]
[ApiController]
public sealed class HealthCheckController(IConfiguration configuration, IWebHostEnvironment env) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly IWebHostEnvironment _environment = env ?? throw new ArgumentNullException(nameof(env));

    [HttpGet]
    public async Task<ActionResult<string>> Get()
    {
        var response = new
        {
            Metadata = GetMetadata()
        };

        return await Task.FromResult(JsonConvert.SerializeObject(response, new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        }));
    }

    private dynamic GetMetadata()
    {
        dynamic metadata = new ExpandoObject();
        metadata.AspDotNetCoreVersion = _configuration[Core.Constants.Settings.KeyAspNetCoreVersion] ?? "";
        metadata.AspNetCoreEnvironment = _configuration[Core.Constants.Settings.KeyAspNetCoreEnvironment] ?? "";
        metadata.DotNetRunningInContainer = _configuration[Core.Constants.Settings.KeyDotNetRunningInContainer] ?? "";
        if (_environment.IsDevelopment())
        {
            metadata.BranchName = _configuration[Core.Constants.Settings.KeyBranchName] ?? "";
        }
        metadata.ContainerImageName = _configuration[Core.Constants.Settings.KeyContainerImageName] ?? "";
        metadata.PodName = _configuration[Core.Constants.Settings.KeyPodName] ?? "";
        metadata.SemanticVersion = _configuration[Core.Constants.Settings.SemanticVersion] ?? "";
        metadata.ServiceName = Core.Constants.Settings.NamePieHandlerService;
        metadata.ExecutionEnvironment = _configuration[Constants.EnvironmentVariables.ExecutionEnvironmentIdentifier] ?? string.Empty;

        return metadata;
    }
}