using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var app = builder.Build();

app.MapGet("/", async (HttpContext ctx) =>
{
    var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

    try
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO visits(ip) VALUES (@ip)";
        cmd.Parameters.AddWithValue("ip", ip);
        await cmd.ExecuteNonQueryAsync();
    }
    catch (Exception ex)
    {
        var logLine = $"{DateTime.UtcNow:u} INSERT ERROR: {ex.Message}{Environment.NewLine}";
        Directory.CreateDirectory("logs");
        await File.AppendAllTextAsync("logs/errors.log", logLine);
    }

    return Results.Content($"<h1>Your IP is: {ip}</h1>", "text/html");
});

app.MapGet("/stats", async () =>
{
    long count = 0;
    try
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM visits";
        count = (long)await cmd.ExecuteScalarAsync();
    }
    catch (Exception ex)
    {
        var logLine = $"{DateTime.UtcNow:u} STATS ERROR: {ex.Message}{Environment.NewLine}";
        Directory.CreateDirectory("logs");
        await File.AppendAllTextAsync("logs/errors.log", logLine);
    }
    return Results.Json(new { visits = count });
});

app.Run("http://0.0.0.0:8000");
