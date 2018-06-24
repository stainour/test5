using System;

namespace BackupService.Integration.Implementation
{
    public class Backup : IBackup, IEquatable<Backup>
    {
        public Backup(int id, DateTime creationDate)
        {
            Id = id;
            CreationDate = creationDate;
        }

        public DateTime CreationDate { get; }
        public int Id { get; }

        public bool Equals(Backup other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return CreationDate.Equals(other.CreationDate) && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Backup)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (CreationDate.GetHashCode() * 397) ^ Id;
            }
        }
    }
}