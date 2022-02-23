namespace TestSelector.Services.CodeCoverage.Xml
{
    public class XmlCoverageConfig : ICoverageConfig
    {
        public XmlCoverageConfig(string filepath)
        {
            Filepath = filepath;
        }

        public string Filepath { get; }
    }
}