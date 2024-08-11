namespace Common
{
    [Serializable]
    public class TestsResults
    {
        public string Language => "C#";
        public List<TestResult> Results { get; } = [];

        public void Add(TestResult result) => Results.Add(result);
    }
}
