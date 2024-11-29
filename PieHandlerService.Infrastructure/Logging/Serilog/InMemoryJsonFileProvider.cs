using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace PieHandlerService.Infrastructure.Logging.Serilog;

internal class InMemoryJsonFileProvider : IFileProvider
{
    private class InMemoryFile : IFileInfo
    {
        private readonly byte[] _data;
        public InMemoryFile(string json) => _data = Encoding.UTF8.GetBytes(json);
        public InMemoryFile(byte[] jsonBytes) => _data = jsonBytes;
        public Stream CreateReadStream() => new MemoryStream(_data);
        public bool Exists { get; } = true;
        public long Length => _data.Length;
        public string PhysicalPath { get; } = string.Empty;
        public string Name { get; } = string.Empty;
        public DateTimeOffset LastModified { get; } = DateTimeOffset.UtcNow;
        public bool IsDirectory { get; } = false;
    }

    private readonly IFileInfo _fileInfo;
    public InMemoryJsonFileProvider(string json) => _fileInfo = new InMemoryFile(json);
    public InMemoryJsonFileProvider(byte[] jsonBytes) => _fileInfo = new InMemoryFile(jsonBytes);
    public IFileInfo GetFileInfo(string _) => _fileInfo;
    public IDirectoryContents GetDirectoryContents(string _) =>  throw new NotImplementedException();
    public IChangeToken Watch(string _) => NullChangeToken.Singleton;
}
