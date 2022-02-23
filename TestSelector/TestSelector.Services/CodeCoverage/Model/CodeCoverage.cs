using System.Collections.Generic;

namespace TestSelector.Services.CodeCoverage.Model
{
    public class CodeCoverage
    {
        public CodeCoverage(string testIdentifier)
        {
            TestId = testIdentifier;
            Ranges = new List<CodeRange>();
        }

        public string TestId { get; }
        public List<CodeRange> Ranges { get; }
    }
}