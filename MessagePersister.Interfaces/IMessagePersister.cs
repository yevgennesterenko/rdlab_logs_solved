using System.Linq;
using System.Threading.Tasks;

namespace MessagePersister.Interfaces
{
    public interface IMessagePersister
    {
        /// <summary>
        /// Stop the persisting. If any outstadning messages these will not be persisted
        /// </summary>
        void StopImmediately();

        /// <summary>
        /// Stop gracefully. The call will not return until all all messages will be persisted.
        /// </summary>
        void Stop();

        /// <summary>
        /// Persist a message to the storage.
        /// </summary>
        /// <param name="message">The message persisted to storage</param>
        void Persist(IMessage message);
    }
}
