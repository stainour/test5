using System;

namespace BackupService.Integration
{
    public class OperationException : ApplicationException
    {
        public OperationException(string message, Exception exception) : base(message, exception)
        {
        }
    }
}