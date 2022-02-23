using System.Collections.Generic;
using TestSelector.Services.SourceControl.Model;

namespace TestSelector.Services.TestMatcher.Model
{
    public class TestMatch
    {
        public TestMatch(CodeChange codeChange, IEnumerable<string> matchingTestIds)
        {
            CodeChange = codeChange;
            MatchingTestIds = matchingTestIds;
        }

        public CodeChange CodeChange { get; }
        public IEnumerable<string> MatchingTestIds { get; }
    }
}
