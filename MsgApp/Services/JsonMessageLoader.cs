using System.Text.Json;
using Microsoft.Extensions.Logging;
using MsgApp.Models;
using System.Collections.Generic;
using System.IO;
using System;


// Klasse zum Laden der Nachrichten aus der JSON  
public class JsonMessageLoader
{
    private readonly ILogger<JsonMessageLoader> _logger;

    public JsonMessageLoader(ILogger<JsonMessageLoader> logger)
    {
        _logger = logger;
    }

    public List<Message> LoadMessagesFromJson(string path)
    {
        try
        {
            _logger.LogInformation($"JsonMessageLoader: Lade JSON-Datei: {path}");

            string jsonString = File.ReadAllText(path);
            // aus dem JSON-Text C# Objekte machen
            var messages = JsonSerializer.Deserialize<List<Message>>(jsonString);

            int msgCount = messages?.Count ?? 0;
            _logger.LogInformation($"JsonMessageLoader: Nachrichten erfolgreich geladen. Anzahl: {msgCount}");
            return messages ?? new List<Message>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"JsonMessageLoader: Fehler beim Laden von {path}");
            // Im Fehlerfall eine leere Liste zurückgeben
            return new List<Message>();
        }
    }
}