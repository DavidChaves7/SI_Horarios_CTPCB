using NJsonSchema;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class Validators
{
    public static string ValidateObjectFields<T>(this T parent)
    {
        var schema = JsonSchema.FromType(typeof(T));
        var schemaErrors = schema.Validate(JsonSerializer.Serialize(parent,new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
        }));
        if (schemaErrors.Count() > 0)
        {
            var errorString = "Debe de completar los campos requeridos:\n";
            foreach (var error in schemaErrors)
            {
                errorString += $"{error.Property}";
            }
            return errorString;
        }
        else
        {
            return "";
        }
    }
}
