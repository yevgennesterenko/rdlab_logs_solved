using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagePersister.Interfaces
{
    public interface IMessageWriterFactory
    {
        IMessageWriter Create();
    }
}
