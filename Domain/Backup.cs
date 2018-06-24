using System;

namespace Domain
{
    public readonly struct Backup
    {
        public Backup(DateTime dateTime)
        {
            CreationDate = dateTime;
        }

        public DateTime CreationDate { get; }
    }
}