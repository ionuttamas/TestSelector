using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestSelector.Services.SourceControl.Model;
using TestSelector.Services.TestMatcher.Matchers;
using TestSelector.Services.TestMatcher.Model;
using TestSelector.Services.UnitTests.Matchers.Utils;

namespace TestSelector.Services.UnitTests.Matchers
{
    [TestFixture]
    public class TestMatcherPerfTests
    {
        private SingleThreadedTestMatcher singleThreadedMatcher;
        private MultiThreadedTestMatcher multiThreadedMatcher;

        [SetUp]
        public void SetUp()
        {
            singleThreadedMatcher = new SingleThreadedTestMatcher();
            multiThreadedMatcher = new MultiThreadedTestMatcher(5);
        }

        [TearDown]
        public void TearDown()
        {
            singleThreadedMatcher = null;
            multiThreadedMatcher = null;
        }

        [TestCaseSource(nameof(TestMatchData))]
        public void TestMatchers_FindsTestMatchesCorrectly(List<CodeCoverage.Model.CodeCoverage> codeCoverages, List<FileChange> fileChanges)
        {
            List<TestMatch> singleThreadedMatches = null;
            List<TestMatch> multiThreadedMatches = null;

            Action getSingleThreadedMatches = () => singleThreadedMatches = singleThreadedMatcher.GetMatches(codeCoverages, fileChanges).ToList();
            Action getMultiThreadedMatches = () => multiThreadedMatches = multiThreadedMatcher.GetMatches(codeCoverages, fileChanges).ToList();

            var singleThreadedDuration = PerfUtils.Time(getSingleThreadedMatches);
            var multiThreadedDuration = PerfUtils.Time(getMultiThreadedMatches);

            Console.WriteLine($"SingleThreaded duration: {singleThreadedDuration}");
            Console.WriteLine($"MultiThreaded duration: {multiThreadedDuration}");

            Assert.IsNotNull(singleThreadedMatches);
            Assert.IsNotNull(multiThreadedMatches);

            Assert.True(singleThreadedMatches.Count==multiThreadedMatches.Count);
            Assert.True(singleThreadedMatches.SelectMany(x => x.MatchingTestIds).Count() == multiThreadedMatches.SelectMany(x=>x.MatchingTestIds).Count());
        }

        #region Test Cases

        public static IEnumerable<TestCaseData> TestMatchData => MatcherDataGeneratorUtils.GenerateTestMatchData(1, 10000, 50000, 50000); 

        #endregion
    }
}