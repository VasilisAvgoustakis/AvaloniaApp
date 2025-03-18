using System.Text.Json;
using Microsoft.Extensions.Logging;
using MsgApp.Models;
using System.Collections.Generic;
using System.IO;
using System;
using MsgApp.Services;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using System.Net.Http;

// Klasse zum Laden der Nachrichten aus der JSON  
public class JsonMessageLoader
{
    private readonly ILogger<JsonMessageLoader> _logger;

    private readonly HttpClientService _httpClient;
    private readonly GravatarService _gravatarService;

    // Konstruktor-Injection: Der DI-Container wird
    // hier einen ILogger<JsonMessageLoader> übergeben.
    public JsonMessageLoader(ILogger<JsonMessageLoader> logger, GravatarService gravatarService, HttpClientService httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _gravatarService = gravatarService;
    }

    public List<Message> LoadMessagesFromJson(string path)
    {
        try
        {
            _logger.LogInformation($"Lade JSON-Datei: {path}");

            string jsonString = File.ReadAllText(path);
            // aus dem JSON-Text C# Objekte machen
            var messages = JsonSerializer.Deserialize<List<Message>>(jsonString);

            int msgCount = messages?.Count ?? 0;

            // berechne Gravatar Urls und setze AvatarUrl property of Messages
            if (messages is not null) // Weg mit der CS8602 Warning
            {
                foreach (var msg in messages)
                {
                    // Bewusst Fire-and-Forget: Avatare laden im Hintergrund
                    var _ = LoadAvatarAsync(msg);
                }
            }


            return messages ?? new List<Message>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Fehler beim Laden von {path}");
            // Im Fehlerfall eine leere Liste zurückgeben
            return new List<Message>();
        }
    }


    public async Task LoadAvatarAsync(Message msg)
    {
        try
        {
            if (string.IsNullOrEmpty(msg.SenderEmail)) return;

            // Gravatar Url bauen
            string url = _gravatarService.GetGravatarUrl(msg.SenderEmail);

            // Bytes herunterladen
            if (_httpClient.Client is not null)
            {
                var bytes = await _httpClient.Client.GetByteArrayAsync(url);

                // image aus Bytes bauen
                using var ms = new MemoryStream(bytes);
                var bmp = new Bitmap(ms);

                // Property of Message Objekt setzen
                msg.AvatarBitmap = bmp;
            }
        }
        catch (Exception ex)
        {
            // Falls auch die Gravatar Platzhalter versagt
            msg.AvatarBitmap = new Bitmap("Assets/offlinePlaceholder.png");

            _logger.LogError(ex, "Fehler beim Laden des Avatars für {Email}", msg.SenderEmail);
        }

    }


}