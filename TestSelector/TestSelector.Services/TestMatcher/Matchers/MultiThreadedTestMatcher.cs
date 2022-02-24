using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestSelector.Services.SourceControl.Model;
using TestSelector.Services.TestMatcher.Model;

namespace TestSelector.Services.TestMatcher.Matchers
{
    public class MultiThreadedTestMatcher : BaseTestMatcher
    {
        private readonly int workers;
        private readonly ConcurrentQueue<Tuple<FileChange, List<CodeCoverage.Model.CodeCoverage>>> taskQueue;
        private readonly ConcurrentQueue<List<TestMatch>> resultQueue;

        public MultiThreadedTestMatcher(int workers = 5)
        {
            this.workers = workers;
            //This is just one queueing strategy; more sophisticated queueing strategies can be applied
            taskQueue = new ConcurrentQueue<Tuple<FileChange, List<CodeCoverage.Model.CodeCoverage>>>();
            //We forcibly iterate here through the results
            resultQueue = new ConcurrentQueue<List<TestMatch>>();
        }

        /// <summary>
        /// Implements a simple test matching procedure without any assumptions of the arguments.
        /// </summary>
        protected override IEnumerable<TestMatch> GetMatchesInternal(Dictionary<FileChange, List<CodeCoverage.Model.CodeCoverage>> comparisonTable)
        {
            foreach (var fileChange in comparisonTable.Keys)
            {
                taskQueue.Enqueue(Tuple.Create(fileChange, comparisonTable[fileChange]));
            }

            var threads = new List<Thread>();
            for (int i = 0; i < workers; i++)
            {
                var thread = new Thread(ProcessMatches);
                threads.Add(thread);
                thread.Start();
            }

            for (int i = 0; i < workers; i++)
            {
                threads[i].Join();
            }

            return resultQueue.SelectMany(x=>x);
        }

        public void ProcessMatches()
        {
            while (!taskQueue.IsEmpty)
            {
                if (!taskQueue.TryDequeue(out var entry)) 
                    continue;

                var matches = ExtractTestMatches(entry.Item2, entry.Item1).ToList();
                resultQueue.Enqueue(matches);
            }
        }
    }
}