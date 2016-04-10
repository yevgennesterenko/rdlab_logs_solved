using System.Configuration;

namespace MessagePersister.Implementation
{
    public class AsyncMessagePersisterConfiguration : ConfigurationSection
    {
        // Create a "writer" element.
        [ConfigurationProperty("writer")]
        public WriterConfiguration Writer
        {
            get { return (WriterConfiguration) this["writer"]; }
            set { this["writer"] = value; }
        }
    }
}
