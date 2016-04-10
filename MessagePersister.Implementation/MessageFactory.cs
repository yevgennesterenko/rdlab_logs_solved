using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessagePersister.Interfaces;

namespace MessagePersister.Implementation
{
    public class MessageFactory: IMessageFactory
    {
        public IMessage GetMessage(string text, DateTime dateTime)
        {
            return new Message() {Text = text, Timestamp = dateTime};
        }
    }
}
