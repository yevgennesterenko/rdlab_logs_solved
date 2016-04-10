using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagePersister.Interfaces
{
    public interface IMessage
    {
        DateTime Timestamp { get; set; }
        string FormatMessage();
        string Text { get; set; }

        string Name { get;  }
    }
}
