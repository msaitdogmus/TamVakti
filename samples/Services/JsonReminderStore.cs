using System.Text.Json;
using Microsoft.Maui.Storage;
using TamVakti.Sample.Models;

namespace TamVakti.Sample.Services;

public sealed class JsonReminderStore : IReminderStore
{
    private const string FileName = "reminders.json";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly SemaphoreSlim gate = new(1, 1);

    private static string StatePath => Path.Combine(FileSystem.AppDataDirectory, FileName);

    public async Task<IReadOnlyList<Reminder>> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(StatePath))
            {
                return [];
            }

            await using var stream = File.OpenRead(StatePath);
            return await JsonSerializer.DeserializeAsync<List<Reminder>>(
                       stream,
                       JsonOptions,
                       cancellationToken)
                   ?? [];
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task SaveAsync(
        IEnumerable<Reminder> reminders,
        CancellationToken cancellationToken = default)
    {
        await gate.WaitAsync(cancellationToken);
        try
        {
            var directory = Path.GetDirectoryName(StatePath)!;
            var tempPath = StatePath + ".tmp";

            Directory.CreateDirectory(directory);

            await using (var stream = File.Create(tempPath))
            {
                await JsonSerializer.SerializeAsync(
                    stream,
                    reminders.ToList(),
                    JsonOptions,
                    cancellationToken);
            }

            File.Move(tempPath, StatePath, true);
        }
        finally
        {
            gate.Release();
        }
    }
}
