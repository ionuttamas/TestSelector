using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestSelector.Services.TestMatcher.IntervalTree;

namespace TestSelector.Services.UnitTests
{
    [TestFixture]
    public class IntervalTreeTests
    {
        private IntervalTree<string> intervalTree;
        private static readonly Random random;

        static IntervalTreeTests()
        {
            random = new Random(1);
        } 

        [SetUp]
        public void SetUp()
        {
            intervalTree = new IntervalTree<string>();
        }

        [TearDown]
        public void TearDown()
        {
            intervalTree = null;
        }

        [TestCaseSource(nameof(IntervalsSmall))]
        //[TestCaseSource(nameof(IntervalsMedium))]
        //[TestCaseSource(nameof(IntervalsLarge))]
        public void IntervalTree_AfterIntervalInserts_FindsOverlapsCorrectly(List<Tuple<int, int, string>> intervals)
        {
            var testCases = 20;

            foreach (var interval in intervals)
            {
                intervalTree.InsertInterval(interval.Item1, interval.Item2, interval.Item3);
            }

            var max = intervals.Max(x => x.Item2);
            var overlaps = intervalTree.GetOverlaps(max, max);
            Assert.Greater(overlaps.Count(), 0);

            for (int i = 0; i < testCases; i++)
            {
                var randomInterval = intervals[random.Next(0, intervals.Count)];

                if(randomInterval.Item1>= randomInterval.Item2-2)
                    continue;

                //var testInterval = Tuple.Create(randomInterval.Item1 + 1, randomInterval.Item2 - 1);
                var testInterval = Tuple.Create(47, 86);
                var expectedIds = intervals
                    .Where(x => Math.Max(x.Item1, testInterval.Item1) <= Math.Min(x.Item2, testInterval.Item2))
                    .Select(x => x.Item3)
                    .ToList();
                var actualIds = intervalTree.GetOverlaps(testInterval.Item1, testInterval.Item2).ToList();

                Assert.AreEqual(expectedIds.Count, actualIds.Count);
                Assert.True(expectedIds.TrueForAll(x=>actualIds.Contains(x)));
                Assert.True(actualIds.TrueForAll(x=> expectedIds.Contains(x)));
            }
        }

        #region Test Cases

        public static IEnumerable<TestCaseData> IntervalsSmall => GenerateIntervals(5, 5, 100);
        public static IEnumerable<TestCaseData> IntervalsMedium => GenerateIntervals(100, 1000, 1000);
        public static IEnumerable<TestCaseData> IntervalsLarge => GenerateIntervals(1000, 10000, 10000);

        public static IEnumerable<TestCaseData> GenerateIntervals(int listsCount, int intervalsCount, int maxValue)
        {
            for (int i = 0; i< listsCount; i++)
            {
                var intervals = new List<Tuple<int, int, string>>();

                for (int j = 0; j < intervalsCount; j++)
                {
                    var low = random.Next(maxValue-1);
                    var high = random.Next(low, maxValue);
                    var id = Guid.NewGuid().ToString();
                    var tuple = Tuple.Create(low, high, id);
                    intervals.Add(tuple);
                }

                yield return new TestCaseData(intervals);
            }
        }

        #endregion
    }
}