using Newtonsoft.Json;

namespace Common;

internal class TestResultToFileSaver
{
    readonly JsonSerializer _serializer = new JsonSerializer();

    public void SaveTestResultAsFile(string fileName, TestResult testResult)
    {
        using StreamWriter sw = new StreamWriter($"{fileName}.txt");
        using JsonWriter writer = new JsonTextWriter(sw);
        _serializer.Serialize(writer, testResult);
    }

}