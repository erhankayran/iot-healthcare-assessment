using System.Globalization;
using Simulator.Application.Abstractions;
using Simulator.Domain.Models;
using Simulator.Domain.Validation;

namespace Simulator.Application.Services;

public sealed class CsvTelemetryParser : ICsvTelemetryParser
{
    public IReadOnlyList<VitalTelemetry> Parse(string csvContent)
    {
        if (string.IsNullOrWhiteSpace(csvContent))
        {
            throw new InvalidOperationException("CSV content is empty.");
        }

        var lines = csvContent
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (lines.Length == 0)
        {
            throw new InvalidOperationException("CSV content is empty.");
        }

        var startIndex = 0;
        var heartRateIndex = 0;
        var spO2Index = 1;
        var timestampIndex = -1;

        var headerColumns = SplitCsvLine(lines[0]);
        if (IsHeaderRow(headerColumns))
        {
            heartRateIndex = FindColumnIndex(headerColumns, "HeartRate", "heart_rate", "hr");
            spO2Index = FindColumnIndex(headerColumns, "SpO2", "spo2", "Sp02");
            timestampIndex = FindOptionalColumnIndex(headerColumns, "Timestamp", "timestamp", "time");
            startIndex = 1;
        }

        var readings = new List<VitalTelemetry>();

        for (var i = startIndex; i < lines.Length; i++)
        {
            var columns = SplitCsvLine(lines[i]);
            if (columns.Length == 0)
            {
                continue;
            }

            if (!int.TryParse(columns[heartRateIndex], NumberStyles.Integer, CultureInfo.InvariantCulture, out var heartRate))
            {
                throw new InvalidOperationException($"Invalid heart rate on line {i + 1}.");
            }

            if (!int.TryParse(columns[spO2Index], NumberStyles.Integer, CultureInfo.InvariantCulture, out var spO2))
            {
                throw new InvalidOperationException($"Invalid SpO2 on line {i + 1}.");
            }

            DateTimeOffset timestamp = DateTimeOffset.UtcNow;
            if (timestampIndex >= 0 &&
                timestampIndex < columns.Length &&
                DateTimeOffset.TryParse(columns[timestampIndex], CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsedTimestamp))
            {
                timestamp = parsedTimestamp;
            }

            var telemetry = new VitalTelemetry(heartRate, spO2, timestamp);
            if (!VitalTelemetryValidator.TryValidate(telemetry, out var errorMessage))
            {
                throw new InvalidOperationException($"Line {i + 1}: {errorMessage}");
            }

            readings.Add(telemetry);
        }

        if (readings.Count == 0)
        {
            throw new InvalidOperationException("CSV file does not contain any telemetry rows.");
        }

        return readings;
    }

    private static bool IsHeaderRow(IReadOnlyList<string> columns)
    {
        return columns.Any(column =>
            column.Contains("Heart", StringComparison.OrdinalIgnoreCase) ||
            column.Contains("SpO", StringComparison.OrdinalIgnoreCase) ||
            column.Equals("hr", StringComparison.OrdinalIgnoreCase));
    }

    private static int FindColumnIndex(IReadOnlyList<string> columns, params string[] candidates)
    {
        for (var i = 0; i < columns.Count; i++)
        {
            if (candidates.Any(candidate =>
                    columns[i].Equals(candidate, StringComparison.OrdinalIgnoreCase)))
            {
                return i;
            }
        }

        throw new InvalidOperationException(
            $"CSV header must include one of: {string.Join(", ", candidates)}.");
    }

    private static int FindOptionalColumnIndex(IReadOnlyList<string> columns, params string[] candidates)
    {
        for (var i = 0; i < columns.Count; i++)
        {
            if (candidates.Any(candidate =>
                    columns[i].Equals(candidate, StringComparison.OrdinalIgnoreCase)))
            {
                return i;
            }
        }

        return -1;
    }

    private static string[] SplitCsvLine(string line)
    {
        return line.Split(',', StringSplitOptions.TrimEntries);
    }
}
