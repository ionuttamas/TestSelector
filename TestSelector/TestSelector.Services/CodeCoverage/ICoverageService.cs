using System.Collections.Generic;
using TestSelector.Services.SourceControl;
using TestSelector.Services.SourceControl.Git;

namespace TestSelector.Services.CodeCoverage
{
    public interface ICoverageService
    {
        public List<Model.CodeCoverage> GetCodeCoverage(ICoverageConfig config);
    }
}