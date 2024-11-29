using Microsoft.Extensions.Logging;
using Moq;
using PieHandlerService.Core.Interfaces;
using PieHandlerService.Core.Models;
using PieHandlerService.Infrastructure.Services.Storage.Service;

namespace PieHandlerService.Infrastructure.Test.Storage;

public class VbfStorageHandlerTest
{
    private readonly VbfStorageHandler _vbfStorageHandler;

    private readonly Mock<IMetricsService> _metricsService;
    private readonly Mock<IProblemDetailsManager> _problemDetailsManager;
    private readonly Mock<ILogger<VbfStorageHandler>> _loggerMock;

    public VbfStorageHandlerTest()
    {
        _metricsService = new Mock<IMetricsService>();
        _problemDetailsManager = new Mock<IProblemDetailsManager>();
        _loggerMock = new Mock<ILogger<VbfStorageHandler>>();
        _vbfStorageHandler = new VbfStorageHandler(_loggerMock.Object, _problemDetailsManager.Object, _metricsService.Object);
    }


    [Fact]
    public async Task Get_FetchVbfFileStatus_Test()
    {
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASPIEVbfLocation, "software");
        Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.NASEndOfLineSubDirectory, "endofline");
        var fileMetadataList = new List<FileMetaData>();
        fileMetadataList.Add(new FileMetaData("sample", 12312313));
        var fileCheckIntegrity = _vbfStorageHandler.FetchVbfFileStatus("14141414", VbfCategory.STATIC, fileMetadataList, SIIGOrderType.SIIGOrderEndOfLine);
        Assert.NotNull(fileCheckIntegrity);
        Assert.True(fileCheckIntegrity.Count() > 0);
    }
}