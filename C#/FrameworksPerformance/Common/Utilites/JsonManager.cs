using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Common.Utilites;

public class JsonManager
{
    readonly JsonSerializer _serializer = JsonSerializer.Create(
        new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        });

    public void SaveTestResultAsFile(string fileName, object testResult)
    {
        using StreamWriter sw = new StreamWriter($"{fileName}.txt");
        using JsonWriter writer = new JsonTextWriter(sw);
        _serializer.Serialize(writer, testResult);
    }

    public string ConvertToJson(object testResult)
    {
        using var writer = new StringWriter();
        writer.NewLine = "\r\n"; ;
        _serializer.Serialize(writer, testResult);
        return writer.ToString();
    }
}