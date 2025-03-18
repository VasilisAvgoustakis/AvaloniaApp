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

    // Konstruktor-Injection: Der DI-Container wird
    // hier einen ILogger<JsonMessageLoader> übergeben.
    public JsonMessageLoader(ILogger<JsonMessageLoader> logger)
    {
        _logger = logger;
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

            return messages ?? new List<Message>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Fehler beim Laden von {path}");
            // Im Fehlerfall eine leere Liste zurückgeben
            return new List<Message>();
        }
    }


    


}