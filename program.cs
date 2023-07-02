using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using Microsoft.Win32;
using XZNX5;
using System.Diagnostics;

static string GenerateRandomString(int length)
{
    const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var random = new Random();
    var result = new char[length];

    for (int i = 0; i < length; i++)
    {
        result[i] = chars[random.Next(chars.Length)];
    }

    return new string(result);
}

Console.Title = "Farlight Bypass - By ! XZNX 5#5555";

string welcomeMessage1 = "Welcome! This is a tool to inject DLLs.";
string welcomeMessage2 = "Enjoy N1GG3RS!";

Console.ForegroundColor = ConsoleColor.Cyan;
Console.SetCursorPosition((Console.WindowWidth - welcomeMessage1.Length) / 2, Console.CursorTop);
Console.WriteLine(welcomeMessage1);
System.Threading.Thread.Sleep(2000);

Console.ForegroundColor = ConsoleColor.Green;
Console.SetCursorPosition((Console.WindowWidth - welcomeMessage2.Length) / 2, Console.CursorTop);
Console.WriteLine(welcomeMessage2);
System.Threading.Thread.Sleep(3000);


var steamKeyPath = Environment.Is64BitProcess
    ? "SOFTWARE\\Wow6432Node\\Valve\\Steam"
    : "SOFTWARE\\Valve\\Steam";

var pathToSettings = string.Empty;

using var steamKey = Registry.LocalMachine.OpenSubKey(steamKeyPath);
if (steamKey is not null)
{
    var steamPath = steamKey.GetValue("InstallPath") as string;
    if (Directory.Exists(steamPath))
    {
        var libraryFile = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
        if (File.Exists(libraryFile))
        {
            var lines = File.ReadAllLines(libraryFile)
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line => line.Trim())
                .ToList();

            var libraries = new List<string>();
            lines.ForEach(line =>
            {
                line = line.Replace("\"", string.Empty);
                if (!line.StartsWith("path")) return;
                line = line.Remove(0, 4).Replace("\\\\", "\\").Trim();
                if (Directory.Exists(line)) libraries.Add(line);
            });

            libraries.ForEach(library =>
            {
                var settingsPath = Path.Combine(library,
                    "steamapps", "common", "Farlight 84",
                    "EasyAntiCheat", "settings.json"
                );

                if (File.Exists(settingsPath)) pathToSettings = library;
            });
        }
    }
}

if (!Directory.Exists(pathToSettings))
{
    Console.WriteLine("could not find Farlight 84 directory, enter manually.");
    Console.Write("> ");
    var input = Console.ReadLine();
    if (!Directory.Exists(input))
    {
        Console.WriteLine("\"{0}\" Is not valid Directory!", input);
        Console.WriteLine("Exiting in...");
        Console.ReadLine();
        System.Threading.Thread.Sleep(3000);
        Environment.Exit(1);
    }
}

var settingsFile = Path.Combine(pathToSettings,
    "steamapps", "common", "Farlight 84",
    "EasyAntiCheat", "settings.json"
);

if (!File.Exists(settingsFile))
{
    Console.WriteLine("\"{0}\" the game installation is wrong", pathToSettings);
    Console.WriteLine("Exiting in...");
    Console.ReadLine();
    System.Threading.Thread.Sleep(3000);
    Environment.Exit(1);
}

var backupFile = Path.Combine(pathToSettings,
    "steamapps", "common", "Farlight 84",
   "EasyAntiCheat", "settings2.json"
);

if (File.Exists(backupFile)) File.Delete(backupFile);
File.Copy(settingsFile, backupFile);


var originalContent = File.ReadAllText(settingsFile, Encoding.UTF8);
var originalSettings = JsonSerializer.Deserialize<Settings>(originalContent);

var patchedSettings = originalSettings! with
{
    ProductId = GenerateRandomString(15),
    SandboxId = GenerateRandomString(15),
    DeploymentId = GenerateRandomString(15)
};

var options = new JsonSerializerOptions
{
    WriteIndented = true,
    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
};
var patchedContent = JsonSerializer.Serialize(patchedSettings, options);
File.WriteAllText(settingsFile, patchedContent);

Console.Clear();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("waiting for the N1GG3R to open the game:");

while (Process.GetProcessesByName("start_protected_game").Length <= 0)
{
    System.Threading.Thread.Sleep(1000);
}

Console.Clear();
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Bypass successful! now suck my dick bitch.");

while (Process.GetProcessesByName("SolarlandClient-Win64-Shipping").Length <= 0)
{
    System.Threading.Thread.Sleep(2000);
}
if (File.Exists(settingsFile)) File.Delete(settingsFile);
File.Copy(backupFile, settingsFile);
System.Threading.Thread.Sleep(5000);
