using System;

namespace TestSelector.Services.SourceControl.Model
{
    public class LineChange : IEquatable<LineChange>
    {
        public LineChange(int? deletedStart, int? deletedCount, int? addedStart, int? addedCount)
        {
            if (deletedStart.HasValue)
            {
                DeletedStart = deletedStart;
                DeletedEnd = DeletedStart + deletedCount;
            }

            if (addedStart.HasValue)
            {
                AddedStart = addedStart;
                AddedEnd = AddedStart + addedCount;
            }
        }

        public int? DeletedStart { get; }
        public int? DeletedEnd { get; }
        public int? AddedStart { get; }
        public int? AddedEnd { get; }

        public bool Equals(LineChange other)
        {
            if (ReferenceEquals(null, other)) 
                return false;

            if (ReferenceEquals(this, other)) 
                return true;

            return DeletedStart == other.DeletedStart && DeletedEnd == other.DeletedEnd && AddedStart == other.AddedStart && AddedEnd == other.AddedEnd;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            
            if (ReferenceEquals(this, obj)) 
                return true;
            
            if (obj.GetType() != this.GetType()) 
                return false;

            return Equals((LineChange) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeletedStart, DeletedEnd, AddedStart, AddedEnd);
        }

        public override string ToString()
        {
            return $"Deleted: [{DeletedStart},{DeletedEnd}] Added: [{AddedStart},{AddedEnd}]";
        }
    }
}