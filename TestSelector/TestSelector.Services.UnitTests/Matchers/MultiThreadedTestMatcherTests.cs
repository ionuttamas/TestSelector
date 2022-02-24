using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestSelector.Services.SourceControl.Model;
using TestSelector.Services.TestMatcher.IntervalTree;
using TestSelector.Services.TestMatcher.Matchers;
using TestSelector.Services.TestMatcher.Model;
using TestSelector.Services.UnitTests.Matchers.Utils;

namespace TestSelector.Services.UnitTests.Matchers
{
    [TestFixture]
    public class MultiThreadedTestMatcherTests
    {
        private MultiThreadedTestMatcher matcher;

        [SetUp]
        public void SetUp()
        {
            matcher = new MultiThreadedTestMatcher(5);
        }

        [TearDown]
        public void TearDown()
        {
            matcher = null;
        }

        [TestCaseSource(nameof(TestMatchDataSmall))]
        [TestCaseSource(nameof(TestMatchDataMedium))]
        //[TestCaseSource(nameof(TestMatchDataLarge))]
        public void MultiThreadedTestMatcher_FindsTestMatchesCorrectly(List<CodeCoverage.Model.CodeCoverage> codeCoverages, List<FileChange> fileChanges)
        { 
            var matches = matcher.GetMatches(codeCoverages, fileChanges).ToList();

            foreach (TestMatch testMatch in matches)
            {
                var lineChange = testMatch.CodeChange.LineChange;
                var filepath = testMatch.CodeChange.Filepath;
                var addedMatchingCount = codeCoverages
                    .Where(x => lineChange.AddedStart.HasValue && x.Ranges.Any(r => r.Filepath == filepath && IntervalTree<string>.Overlaps(r.From, r.To, lineChange.AddedStart.Value, lineChange.AddedEnd.Value)))
                    .Select(x=>x.TestId)
                    .Count();
                var deletedMatchingCount = codeCoverages
                    .Where(x => lineChange.DeletedStart.HasValue && x.Ranges.Any(r => r.Filepath == testMatch.CodeChange.Filepath && IntervalTree<string>.Overlaps(r.From, r.To, lineChange.DeletedStart.Value, lineChange.DeletedEnd.Value)))
                    .Select(x => x.TestId)
                    .Count();
                
                Assert.AreEqual(addedMatchingCount+ deletedMatchingCount, testMatch.MatchingTestIds.Count());
            }
        }

        #region Test Cases

        public static IEnumerable<TestCaseData> TestMatchDataSmall => MatcherDataGeneratorUtils.GenerateTestMatchData(10, 100, 100, 100);
        public static IEnumerable<TestCaseData> TestMatchDataMedium => MatcherDataGeneratorUtils.GenerateTestMatchData(100, 1000, 1000, 1000);
        public static IEnumerable<TestCaseData> TestMatchDataLarge => MatcherDataGeneratorUtils.GenerateTestMatchData(1000, 10000, 500, 500); 

        #endregion
    }
}