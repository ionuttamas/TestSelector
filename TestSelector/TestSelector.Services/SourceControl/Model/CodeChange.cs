using System;

namespace TestSelector.Services.SourceControl.Model
{
    public class CodeChange: IEquatable<CodeChange>
    {
        public CodeChange(string filepath, LineChange lineChange)
        {
            Filepath = filepath;
            LineChange = lineChange;
        }

        public string Filepath { get; }
        public LineChange LineChange { get; }


        public bool Equals(CodeChange other)
        {
            if (ReferenceEquals(null, other)) 
                return false;

            if (ReferenceEquals(this, other)) 
                return true;

            return Filepath == other.Filepath && Equals(LineChange, other.LineChange);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;

            if (ReferenceEquals(this, obj)) 
                return true;

            if (obj.GetType() != GetType()) 
                return false;
            
            return Equals((CodeChange) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Filepath, LineChange);
        }
    }
}