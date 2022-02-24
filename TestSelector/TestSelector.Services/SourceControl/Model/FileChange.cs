using System.Collections.Generic;

namespace TestSelector.Services.SourceControl.Model
{
    public class FileChange
    {
        public FileChange()
        {
            LineChanges = new List<LineChange>();
        }

        public FileChange(string filepath): this()
        {
            Filepath = filepath;
        }

        public string Filepath { get; }
        public List<LineChange> LineChanges { get; set; }

        public override string ToString()
        {
            return Filepath;
        }
    }
}