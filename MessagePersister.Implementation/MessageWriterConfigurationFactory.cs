using MessagePersister.Interfaces;

namespace MessagePersister.Implementation
{
    public class MessageWriterConfigurationFactory : IMessageWriterFactory
    {
        public const string FileWriterName = "FileMessageWriter";
        public IMessageWriter Create()
        {
            IMessageWriter writer;
            var asyncPersisterConfig = (AsyncMessagePersisterConfiguration)System.Configuration.ConfigurationManager.GetSection("asyncPersisterGroup/asyncPersister");
            var writerName = asyncPersisterConfig.Writer.Name;
            switch (writerName)
            {
                case FileWriterName:
                    writer = new FileMessageWriter(asyncPersisterConfig.Writer.LogFolder, asyncPersisterConfig.Writer.LogFileName, asyncPersisterConfig.Writer.DateTimeFormat, asyncPersisterConfig.Writer.Extension);
                    break;
                    /*
                    Add any new writer/appender implementation here like Console Writer, Database Log Writer etc
                    Relevant configuration sections/elements should also be defined for that
                    */
                default:
                    writer = new FileMessageWriter(asyncPersisterConfig.Writer.LogFolder, asyncPersisterConfig.Writer.LogFileName, asyncPersisterConfig.Writer.DateTimeFormat, asyncPersisterConfig.Writer.Extension);
                    break;
            }
            return writer;
        }

      
    }
}
