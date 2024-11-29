using Microsoft.Extensions.Logging;
using Moq;
using PieHandlerService.Infrastructure.Services.Pie.Interfaces;
using PieHandlerService.Infrastructure.Services.Storage.Service;

namespace PieHandlerService.Infrastructure.Test.Storage;

public class NASStorageOperationTest
{
    private readonly Mock<ILogger<NasStorageOperation>> _loggerMock;
    private readonly NasStorageOperation _nasStorageOperation;
    private readonly Mock<IProblemDetailHandler> _problemDetailsHandler;

    private readonly CancellationToken _cts;

    public NASStorageOperationTest() {
        _cts = new CancellationToken();
        _problemDetailsHandler = new Mock<IProblemDetailHandler>();
        _loggerMock = new Mock<ILogger<NasStorageOperation>>();
        _nasStorageOperation = new NasStorageOperation(_loggerMock.Object, _problemDetailsHandler.Object);
    }

    [Fact]
    public async void TestCheckFileExists() {
        var response = await _nasStorageOperation.CheckFileExists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), It.IsAny<string>(), _cts);
        Assert.NotNull(response);
        Assert.True(!response?.IsAvailable);
    }

    [Fact]
    public async void TestDeleteFile()
    {
        var response = await _nasStorageOperation.DeleteFile(It.IsAny<string>(), Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), _cts);
        Assert.True(!response);
    }

    [Fact]
    public async void TestFetchFileContent()
    {
        var response = await _nasStorageOperation.FetchFileContent(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), It.IsAny<string>());
        Assert.NotNull(response);
        Assert.True(string.IsNullOrEmpty(response.Content));
        Assert.True(!response.IsAvailable);
    }

    [Fact]
    public async void TestSearchFilesByName()
    {
        var response = await _nasStorageOperation.SearchFilesByName(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "test");
        Assert.NotNull(response);
        Assert.True(response.Count()==0);
    }

    [Fact]
    public async void TestSaveFile()
    {
        var response = await _nasStorageOperation.SaveFile("test", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "sample");
        Assert.NotNull(response);
        Assert.False(!response.IsAvailable);
    }

}