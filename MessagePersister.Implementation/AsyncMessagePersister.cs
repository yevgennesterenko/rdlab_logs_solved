using System.Collections.Concurrent;
using MessagePersister.Interfaces;



namespace MessagePersister.Implementation
{
    using System;
    using System.Text;
    using System.Threading;

    public class AsyncMessagePersister : IMessagePersister
    {
        private Thread runThread;
        private ConcurrentQueue<IMessage> messagesQueue = new ConcurrentQueue<IMessage>();

        private bool exit;
        private DateTime currentDate = DateTime.Now;
        private bool quitWithFlush = false;

        private IMessageWriterFactory messageWriterFactory;
        private IMessageFactory messageFactory;
        private IDateFactory dateFactory;
        private IMessageWriter writer;

        //TODO That is now ready to be used with some IoC container like Castle or Unity
        //TODO instead of hardcoding MessageFactory here it can be extended to configure different LogLines from configuration
        //TODO e.g. from app.config as it is the case with IMessageWriter or Unity config
        public AsyncMessagePersister(): this(new MessageWriterConfigurationFactory(), new MessageFactory(), new DateFactory())
        {        
        }


        //TODO That is not ready to be used with some IoC container like Castle or Unity
        public AsyncMessagePersister(IMessageWriterFactory factory, IMessageFactory messageFactory,  IDateFactory dateFactory)
        {
            this.messageWriterFactory = factory;
            this.dateFactory = dateFactory;
            this.messageFactory = messageFactory;
            this.writer = CreateMessageWriter(factory);            

            if (this.writer != null)
            {
                this.runThread = new Thread(this.Processing);
                this.runThread.Start();
            }
            else
            {
                //TODO instead of Console.Writeline it could be some other persister like log4net log
                Console.WriteLine("Cannot initialize Async Log!");
            }
        }

        private IMessageWriter CreateMessageWriter(IMessageWriterFactory factory)
        {
            IMessageWriter writer = null;
            try
            {
                //TODO Currently this implementation is limited by one writer for the log e.g. FileMessageWriter or ConsoleWriter,etc
                //TODO It is possible to introduce concept of appenders instead of writers. E.g. to define more than one "appender"
                //TODO in application config, initialize collection of appenders and than call IAppedner.Persist() for every appender in collection
                writer = factory.Create();                
            }
            catch (Exception ex)
            {
                //Catch exception per this requirment: If an error occur the calling application must not be put down.
                // It is more important that the application continues to  run than lines not being written to log
                Console.Write("Error creating MessagePersister: {0}", ex.Message);
                //throw new ApplicationException(String.Format("Error creating MessagePersister: {0}", ex.Message));
            }            
            return writer;
        }
     

        private void Processing()
        {
            try
            {
                while (!this.exit)
                {
                    IMessage message;
                    if (this.messagesQueue.TryDequeue(out message))
                    {
                        ProcessMessage(message);
                    }
                    if (this.quitWithFlush == true && this.messagesQueue.Count == 0)
                    {
                        this.exit = true;
                    }
                    //TODO that timeout can be decreased to 10 millseconds or less to allow faster processing      
                    Thread.Sleep(50);
                }
            }
            //in case any exception we will write error to console and gracefully exit
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error in AsynLog: {0} {1}", ex.Message, ex.StackTrace));
            }
        }

        private void ProcessMessage(IMessage message)
        {            
            if (!this.exit || this.quitWithFlush)
            {                

                StringBuilder stringBuilder = new StringBuilder();

                if ((this.dateFactory.GetNow().Day - currentDate.Day) != 0)
                {
                    CreateNewMessageFolder(stringBuilder);                   
                }

                PrepareMessage(stringBuilder, message);

                this.writer.Write(stringBuilder.ToString(), message);
            }            
        }

        private static void PrepareMessage(StringBuilder stringBuilder, IMessage message)
        {
            stringBuilder.Append(message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            stringBuilder.Append("\t");
            stringBuilder.Append(message.FormatMessage());
            stringBuilder.Append("\t");

            stringBuilder.Append(Environment.NewLine);
        }

        private void CreateNewMessageFolder(StringBuilder stringBuilder)
        {
            this.currentDate = DateTime.Now;
            this.writer = this.messageWriterFactory.Create();
            stringBuilder.Append(Environment.NewLine);
           // this.writer.CreateFolder();
        }      

        public void StopImmediately()
        {
            this.exit = true;
        }

        public void Stop()
        {
            this.quitWithFlush = true;
        }

        public void Persist(IMessage message)
        {
            message.Timestamp = DateTime.Now;
            this.messagesQueue.Enqueue( message );
        }
    }

}