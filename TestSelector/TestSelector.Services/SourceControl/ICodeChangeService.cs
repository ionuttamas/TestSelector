using System.Collections.Generic;
using TestSelector.Services.SourceControl.Model;

namespace TestSelector.Services.SourceControl
{
    public interface ICodeChangeService
    {
        public List<FileChange> GetCodeChanges(ICodeDelta codeDelta);
    }
}