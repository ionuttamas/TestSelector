using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TestSelector.Services.SourceControl.Model;

namespace TestSelector.Services.SourceControl.Git
{
    public class GitCodeChangeService : ICodeChangeService
    {
        private const string GIT_DIFF_COMMAND_FORMAT = "git diff -U0 {0} {1} | grep -e \"@@\" -e \"--- a\" ";
        private const string FILE_START_MARKER = "---a/";
        private const string ADDED_MARKER = "+";
        private const string DELETED_MARKER = "-";

        public List<FileChange> GetCodeChanges(ICodeDelta codeDelta)
        {
            if (!(codeDelta is GitCodeDelta))
                throw new ArgumentException("Code delta argument is supported");

            var delta = (GitCodeDelta)codeDelta;

            string gitDiff = ExtractGitDiff(delta);

            if (string.IsNullOrWhiteSpace(gitDiff))
                return new List<FileChange>();

            List<FileChange> fileChanges = GetFileChanges(gitDiff);

            return fileChanges;
        }

        private string ExtractGitDiff(GitCodeDelta delta)
        {
            var command = string.Format(GIT_DIFF_COMMAND_FORMAT, delta.ToCommit, delta.FromCommit);
            var process = new Process
            {
                StartInfo =
                {
                    FileName = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output;
        }

        private List<FileChange> GetFileChanges(string gitDiff)
        {
            var lines = gitDiff.Split(Environment.NewLine);
            var fileChanges = new List<FileChange>();

            var index = 0;

            while (index<lines.Length)
            {
                string currentLine = lines[index];

                if (!ContainsFilePath(currentLine))
                    throw new ArgumentException($"Line {currentLine} does not start with file start marker");

                string filepath = lines[index].Substring(FILE_START_MARKER.Length);
                var fileChange = new FileChange(filepath);

                index++;

                while (!ContainsFilePath(lines[index]))
                {
                    LineChange codeChange = GetCodeChange(lines[index]);
                    fileChange.LineChanges.Add(codeChange);
                    index++;
                }

                fileChanges.Add(fileChange);
            }

            return fileChanges;
        }

        private bool ContainsFilePath(string line)
        {
            return line.StartsWith(FILE_START_MARKER);
        }

        private LineChange GetCodeChange(string line)
        {
            var match = Regex.Match(line, @"@@ : (.+?) @@").Groups[1];

            if(!match.Success)
                throw new ArgumentException($"Line {line} did not contain changes information");
            

            int? deletedStart = null;
            int? deletedCount = null;
            int? addedStart = null;
            int? addedCount = null;
            var tokens = match.Value.Split(' ');

            foreach (string token in tokens)
            {
                if (token.StartsWith(ADDED_MARKER))
                {
                    var addedTokens = tokens[0].Substring(1).Split(',');
                    addedStart = int.Parse(addedTokens[0]);
                    addedCount = int.Parse(addedTokens[1]);
                }
                else if (token.StartsWith(DELETED_MARKER))
                {
                    var deletedTokens = tokens[0].Substring(1).Split(',');
                    deletedStart = int.Parse(deletedTokens[0]);
                    deletedCount = int.Parse(deletedTokens[1]);
                }
            }

            return new LineChange(deletedStart, deletedCount, addedStart, addedCount);
        }

    }
}