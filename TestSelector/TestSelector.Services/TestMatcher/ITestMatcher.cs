using System.Collections.Generic;
using TestSelector.Services.SourceControl.Model;
using TestSelector.Services.TestMatcher.Model;

namespace TestSelector.Services.TestMatcher
{
    public interface ITestMatcher
    {
        public IEnumerable<TestMatch> GetMatches(List<CodeCoverage.Model.CodeCoverage> codeCoverages, List<FileChange> fileChanges);
    }
}