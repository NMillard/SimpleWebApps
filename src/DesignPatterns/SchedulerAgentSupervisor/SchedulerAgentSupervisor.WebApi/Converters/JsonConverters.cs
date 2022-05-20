using System.Text.Json;
using System.Text.Json.Serialization;

namespace SchedulerAgentSupervisor.WebApi.Converters;

public class NullableDateOnlyConverter : JsonConverter<DateOnly?> {
    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        string? value = reader.GetString();
        return value is not null ? DateOnly.Parse(value) : null;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options) {
        if (value.HasValue) {
            writer.WriteStringValue(value.Value.ToString("yyyy-MM-dd"));
            return;
        }

        writer.WriteStringValue((string?)null);
    }
}

public class DateOnlyConverter : JsonConverter<DateOnly> {
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        string? value = reader.GetString();
        return value is not null ? DateOnly.Parse(value) : DateOnly.Parse("0000-01-01");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}

public class TimeOnlyConverter : JsonConverter<TimeOnly> {
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        string? value = reader.GetString();
        return TimeOnly.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToString("HH:mm:ss.fff"));
    }
}