namespace TestSelector.Services.CodeCoverage.Model
{
    public class CodeRange
    {
        public CodeRange(string filepath, int from, int to)
        {
            Filepath = filepath;
            From = from;
            To = to;
        }

        public string Filepath { get; }
        public int From { get; }
        public int To { get; }
    }
}