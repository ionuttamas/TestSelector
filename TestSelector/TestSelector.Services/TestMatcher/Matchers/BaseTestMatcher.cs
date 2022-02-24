using System;
using System.Collections.Generic;
using System.Linq;
using TestSelector.Services.CodeCoverage.Model;
using TestSelector.Services.SourceControl.Model;
using TestSelector.Services.TestMatcher.Model;

namespace TestSelector.Services.TestMatcher.Matchers
{
    public abstract class BaseTestMatcher : ITestMatcher
    {
        public IEnumerable<TestMatch> GetMatches(List<CodeCoverage.Model.CodeCoverage> codeCoverages, List<FileChange> fileChanges)
        {
            if (fileChanges == null)
                throw new ArgumentNullException(nameof(fileChanges));

            if (codeCoverages == null)
                throw new ArgumentNullException(nameof(codeCoverages));

            if (!fileChanges.Any())
                return new List<TestMatch>();

            if (!codeCoverages.Any())
                return new List<TestMatch>();

            var changedFiles = new HashSet<string>(fileChanges.Select(x => x.Filepath));
            var fileToCoverages = new Dictionary<FileChange, List<CodeCoverage.Model.CodeCoverage>>();

            foreach (var fileChange in fileChanges)
            {
                foreach (var codeCoverage in codeCoverages)
                {
                    foreach (var codeRange in codeCoverage.Ranges)
                    {
                        if(!changedFiles.Contains(codeRange.Filepath))
                            continue;

                        if (fileChange.Filepath!=codeRange.Filepath)
                            continue;

                        if (!fileToCoverages.ContainsKey(fileChange))
                        {
                            fileToCoverages[fileChange] = new List<CodeCoverage.Model.CodeCoverage>();
                        }

                        fileToCoverages[fileChange].Add(codeCoverage);
                    }
                }
            }

            return GetMatchesInternal(fileToCoverages);
        }

        protected IEnumerable<TestMatch> ExtractTestMatches(List<CodeCoverage.Model.CodeCoverage> coverages, FileChange change)
        {
            var intervalTree = new IntervalTree.IntervalTree<string>();

            //For now we pick at random, but we can check based on the imbalance data (N code ranges vs. M line changes) and create the interval tree for the biggest in O(min(N,M) x log(min(N,M)) time and then check for each range in the bigger set;
            //Total average time would be O(min(N,M) x log(min(N,M) x max(N,M))
            foreach (var coverage in coverages)
            {
                foreach (CodeRange range in coverage.Ranges)
                {
                    intervalTree.InsertInterval(range.From, range.To, coverage.TestId);
                }
            }

            foreach (var lineChange in change.LineChanges)
            {
                if (lineChange.AddedStart.HasValue)
                {
                    var matchingTestIds = intervalTree.GetOverlaps(lineChange.AddedStart.Value, lineChange.AddedEnd.Value);
                    if (matchingTestIds.Any())
                    {
                        yield return new TestMatch(new CodeChange(change.Filepath, lineChange), matchingTestIds);
                    }
                }

                if (lineChange.DeletedStart.HasValue)
                {
                     var matchingTestIds = intervalTree.GetOverlaps(lineChange.DeletedStart.Value, lineChange.DeletedEnd.Value);

                    if (matchingTestIds.Any())
                    {
                        yield return new TestMatch(new CodeChange(change.Filepath, lineChange), matchingTestIds);
                    }
                }
            }
        }

        protected abstract IEnumerable<TestMatch> GetMatchesInternal(Dictionary<FileChange, List<CodeCoverage.Model.CodeCoverage>> comparisonTable);
    }
}