using System.IO;
using System.Threading.Tasks;
using MsgApp.Services;

public class MockHttpClientService : HttpClient
{
    public byte[] FakeImageBytes { get; set; }

    public MockHttpClientService()
    {
        //Basisverzeichnis
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        // Pfad zur Bilddatei.
        string imagePath = Path.Combine(baseDir, "Assets", "offlinePlaceholder.png");

        FakeImageBytes = File.ReadAllBytes(imagePath);
    }

    public new Task<byte[]> GetByteArrayAsync(string url)
    {
        // wenn die URL ie dummy URL entspricht, liefere das Dummy-Bild zurück. b642b4217b34b1e8d3bd915fc65c4452
        if(url.EndsWith("https://www.gravatar.com/avatar/b642b4217b34b1e8d3bd915fc65c4452?d=mp"))
        {
            return Task.FromResult(FakeImageBytes);
        }
        // sonst 
        throw new Exception("Simulierter HTTP-Fehler");
    }
}
