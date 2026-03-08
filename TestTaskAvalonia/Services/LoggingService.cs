using System;
using System.IO;

namespace TestTaskAvalonia.Services;

public class LoggingService
{
    private readonly string _logFilePath;

    public LoggingService(string? logFilePath = null)
    {
        _logFilePath = logFilePath ?? "./log.txt";
    }

    public void Log(string message)
    {
        var line = $"{DateTime.Now:u} {message}";
        File.AppendAllLines(_logFilePath, new[] { line });
    }

    public void LogError(Exception message)
    {
        Log("Error: " + message);
    }
}