using System.Collections.Generic;
using TestSelector.Services.CodeCoverage;
using TestSelector.Services.CodeCoverage.Model;
using TestSelector.Services.CodeCoverage.Xml;
using TestSelector.Services.SourceControl;
using TestSelector.Services.SourceControl.Git;
using TestSelector.Services.SourceControl.Model;
using TestSelector.Services.TestMatcher;
using TestSelector.Services.TestMatcher.Matchers;
using TestSelector.Services.TestMatcher.Model;

namespace TestSelector.Client.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            ICoverageService coverageService = new XmlCoverageService();
            ICodeChangeService codeChangeService = new GitCodeChangeService();
            ITestMatcher testMatcher = new MultiThreadedTestMatcher();
            List<CodeCoverage> codeCoverage = coverageService.GetCodeCoverage(new XmlCoverageConfig("..."));
            List<FileChange> fileChanges = codeChangeService.GetFileChanges(new GitCodeDelta("from_commit_hash", "to_commit_hash"));

            IEnumerable<TestMatch> testMatches = testMatcher.GetMatches(codeCoverage, fileChanges);
            System.Console.Read();
        } 
    }
}
