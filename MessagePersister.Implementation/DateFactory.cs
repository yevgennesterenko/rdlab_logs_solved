using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessagePersister.Interfaces;

namespace MessagePersister.Implementation
{
    public class DateFactory : IDateFactory
    {
        public DateTime GetNow()
        {
            return DateTime.Now;
        }
    }
}
