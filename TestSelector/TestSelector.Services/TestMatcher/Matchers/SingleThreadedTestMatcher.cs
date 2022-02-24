using System.Collections.Generic;
using TestSelector.Services.SourceControl.Model;
using TestSelector.Services.TestMatcher.Model;

namespace TestSelector.Services.TestMatcher.Matchers
{
    public class SingleThreadedTestMatcher : BaseTestMatcher
    {
        /// <summary>
        /// Implements a simple test matching procedure without any assumptions of the arguments.
        /// </summary>
        protected override IEnumerable<TestMatch> GetMatchesInternal(Dictionary<FileChange, List<CodeCoverage.Model.CodeCoverage>> comparisonTable)
        {
            foreach (var fileChange in comparisonTable.Keys)
            {
                foreach (var testMatch in ExtractTestMatches(comparisonTable[fileChange], fileChange))
                {
                    yield return testMatch;
                }
            }
        }
    }
}