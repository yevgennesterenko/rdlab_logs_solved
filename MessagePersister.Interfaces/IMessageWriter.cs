using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagePersister.Interfaces
{
    public interface IMessageWriter
    {
        void Write(string s, IMessage message);
        void CreateFolder(string folder);
    }
}
