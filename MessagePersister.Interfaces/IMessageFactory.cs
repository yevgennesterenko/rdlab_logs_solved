using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagePersister.Interfaces
{
    public interface IMessageFactory
    {
        IMessage GetMessage(string text, DateTime dateTime);
    }
}
