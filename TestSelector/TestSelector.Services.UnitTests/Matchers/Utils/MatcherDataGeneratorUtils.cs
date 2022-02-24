using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TestSelector.Services.CodeCoverage.Model;
using TestSelector.Services.SourceControl.Model;

namespace TestSelector.Services.UnitTests.Matchers.Utils
{
    public static class MatcherDataGeneratorUtils
    {
        private static readonly Random Random;

        static MatcherDataGeneratorUtils()
        {
            Random = new Random(1);
        }

        public static IEnumerable<TestCaseData> GenerateTestMatchData(int testCases, int filesCount, int codeCoveragesCount, int changesCount)
        {
            var filepaths = Enumerable.Range(0, filesCount).Select(x => Guid.NewGuid().ToString()).ToList();
            var averageCoveragesCount = 10;//1;//
            var averageCoverageSpan = 10;

            var averageChangesCount = 20;//1;//
            var averageChangeSpan = 100;

            for (int i = 0; i < testCases; i++)
            {
                var codeCoverages = new List<CodeCoverage.Model.CodeCoverage>();
                var fileChanges = new List<FileChange>();

                for (int j = 0; j < codeCoveragesCount; j++)
                {
                    codeCoverages.Add(GenerageCodeCoverage(filepaths[Random.Next(filesCount)], averageCoveragesCount, averageCoverageSpan));
                }

                for (int j = 0; j < changesCount; j++)
                {
                    var filepath = filepaths[Random.Next(filesCount)];

                    if (fileChanges.All(x => x.Filepath == filepath))
                        continue;

                    var fileChange = GenerageFileChange(filepath, averageChangesCount, averageChangeSpan);
                    fileChanges.Add(fileChange);
                }

                yield return new TestCaseData(codeCoverages, fileChanges);
            }
        }

        public static CodeCoverage.Model.CodeCoverage GenerageCodeCoverage(string filepath, int averageCoveragesCount, int averageCoverageSpan)
        {
            var coverage = new CodeCoverage.Model.CodeCoverage(Guid.NewGuid().ToString());
            var stdDev = 0;
            var lastChangedLine = 10;

            for (int i = 0; i < averageCoveragesCount + Random.Next(stdDev); i++)
            {
                var from = Random.Next(lastChangedLine + 1);
                var to = Random.Next(from, from + Random.Next(averageCoverageSpan));
                lastChangedLine = to;
                coverage.Ranges.Add(new CodeRange(filepath, from, to));
            }

            return coverage;
        }

        public static FileChange GenerageFileChange(string filepath, int averageChangesCount, int averageChangeSpan)
        {
            var change = new FileChange(filepath);
            var stdDev = 0;
            var lastChangedLine = 10;

            for (int i = 0; i < averageChangesCount + Random.Next(stdDev); i++)
            {
                var hasAdditions = Random.Next() % 2 == 0;
                var hasDeletions = Random.Next() % 2 == 0;
                var deletedStart = hasDeletions ? Random.Next(lastChangedLine + 1) : (int?)null;
                var deletedCount = hasDeletions ? Random.Next(averageChangeSpan) : (int?)null;
                var addedStart = hasAdditions ? Random.Next(lastChangedLine + 1) : (int?)null;
                var addedCount = hasAdditions ? Random.Next(averageChangeSpan) : (int?)null;

                if (hasAdditions || hasDeletions)
                {
                    lastChangedLine = Math.Max(hasDeletions ? deletedStart.Value + deletedCount.Value : 0, hasAdditions ? addedStart.Value + addedCount.Value : 0);
                    change.LineChanges.Add(new LineChange(deletedStart, deletedCount, addedStart, addedCount));
                }
            }

            return change;
        }
    }
}
