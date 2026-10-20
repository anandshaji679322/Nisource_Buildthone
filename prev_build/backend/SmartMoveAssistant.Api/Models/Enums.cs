using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartMoveAssistant.Api.Models;

[JsonConverter(typeof(FlexibleActionIntentConverter))]
public enum ActionIntent
{
    Move,
    Stop,
    Start
}

public class FlexibleActionIntentConverter : JsonConverter<ActionIntent>
{
    public override ActionIntent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString()?.Trim().ToLowerInvariant() ?? "";
            if (str.Contains("stop") || str.Contains("disconnect") || str.Contains("cancel") || str.Contains("terminate"))
            {
                return ActionIntent.Stop;
            }
            if (str.Contains("start") || str.Contains("connect") || str.Contains("new"))
            {
                return ActionIntent.Start;
            }
            return ActionIntent.Move;
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            var intVal = reader.GetInt32();
            if (Enum.IsDefined(typeof(ActionIntent), intVal))
            {
                return (ActionIntent)intVal;
            }
        }

        return ActionIntent.Move;
    }

    public override void Write(Utf8JsonWriter writer, ActionIntent value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
