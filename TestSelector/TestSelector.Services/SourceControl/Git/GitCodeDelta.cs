namespace TestSelector.Services.SourceControl.Git
{
    public class GitCodeDelta: ICodeDelta
    {
        public string FromCommit { get; }
        public string ToCommit { get; }

        public GitCodeDelta(string fromCommit, string toCommit)
        {
            FromCommit = fromCommit;
            ToCommit = toCommit;
        }
    }
}
