using System;

namespace WoTget.Core.Database
{
    public class DatabaseNotFoundException : Exception
    {
        public DatabaseNotFoundException(string message) : base(message) { }
    }

}
