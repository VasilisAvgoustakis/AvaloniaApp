using System.Text;
using System.Threading.Tasks;
using MsgApp.Services;

public class MockHttpClientService : HttpClientService
{
    // "Fake"-Antwortinhalt einfaches Dummy-Bild.
    public byte[] FakeImageBytes { get; set; } = Encoding.UTF8.GetBytes("Assets/offlinePlaceholder.png");

    public Task<byte[]> GetByteArrayAsync(string url)
    {
        // Optional: Je nach URL den Fake-Inhalt variieren
        return Task.FromResult(FakeImageBytes);
    }
}