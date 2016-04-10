using System;
using System.Threading;
using MessagePersister.Implementation;
using MessagePersister.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace MessagePersister.UnitTests
{

    /// <summary>
    /// Those are true unit tests with every dependency mocked out
    /// Integration tests also should be implemented with real configuration from app.config and real writing to the file
    /// and checking the results
    /// </summary>
    public class MessagePersisterUnitTests 
    {

        [Test]
        public void GivenAsyncLog_WhenWriteMethodIsCalled_ThenCorrectStringWithTimeStampShouldBeWritten()
        {
            //arrange
            IMessagePersister persister;            
            IMessageWriterFactory mockMessageWriterFactory;
            var mockWriter = ConfigureStubsAndMocksForAsyncLog(out persister, "Test String", new DateTime(2015, 10, 1), MockDateDefaultFactory(), out mockMessageWriterFactory, 1);                       
            mockWriter.Expect(x => x.Write("2015-10-01 00:00:00:000\tTest String\t\r\n", null));

            //act
            persister.Persist(new Message(Guid.NewGuid(), "Test", "Test"));
            Thread.Sleep(100);
            persister.Stop();            
            //assert
            mockWriter.VerifyAllExpectations();
            mockMessageWriterFactory.VerifyAllExpectations();
        }


        [Test]
        public void GivenAsyncLog_WhenMidnightIsCrossed_ThenNewFileWithTimeStampShouldBeCreated()
        {
            //arrange
            IMessagePersister persister;
            var mockDateFactory = MockRepository.GenerateMock<IDateFactory>();
            var timeStamp = DateTime.Now.AddDays(-1) - DateTime.Now.Date + new TimeSpan(0, 0, 1);
            //setting date now 2 second before midnight of current date
            mockDateFactory.Stub(x => x.GetNow())
                 .WhenCalled(x => x.ReturnValue = DateTime.Now - timeStamp)    
                 .Return(DateTime.Now - timeStamp);
            IMessageWriterFactory mockMessageWriterFactory;
            int ExpectedNumberOfTimesCreateCalled = 2;
            ConfigureStubsAndMocksForAsyncLog(out persister, "Test String", new DateTime(2015, 10, 1), mockDateFactory, out mockMessageWriterFactory, ExpectedNumberOfTimesCreateCalled);          

            //act
            persister.Persist(new Message(Guid.NewGuid(), "Test","Test" ));
            Thread.Sleep(2000);  //midnight crossed
            persister.Persist(new Message(Guid.NewGuid(), "Test", "Test"));
            persister.Stop();
            Thread.Sleep(500);

            //assert
            //we are veryfing that mockMessageWriterFactory.Create has been called 2 times, once when persister has been constructed 
            // and once when new file has been created when midnight crossed
            mockMessageWriterFactory.VerifyAllExpectations();
        }



        [Test]
        public void GivenAsyncLog_WhenStopWithouthFlushIsCalled_ThenpersisterWillStopRightAway()
        {
            //arrange
            IMessagePersister persister;          
            IMessageWriterFactory mockMessageWriterFactory;
            int ExpectedNumberOfTimesWriteCalled = 1;
            var mockWriter = ConfigureStubsAndMocksForAsyncLog(out persister, "Test String", new DateTime(2015, 10, 1), MockDateDefaultFactory(), out mockMessageWriterFactory, 1);
            mockWriter.Expect(x => x.Write("2015-10-01 00:00:00:000\tTest String\t\r\n",null)).Repeat.Times(ExpectedNumberOfTimesWriteCalled);
            //act
            persister.Persist(new Message());
            Thread.Sleep(100);
            persister.Persist(new Message());
            persister.StopImmediately();
            Thread.Sleep(100);

            //assert
            //verifying that mockWriter has been called only once
            mockWriter.VerifyAllExpectations();
        }


        [Test]
        public void GivenAsyncLog_WhenStopWithFlushIsCalled_ThenpersisterWillLogOutstandingLines()
        {
            //arrange
            IMessagePersister persister;  
            IMessageWriterFactory mockMessageWriterFactory;
            int ExpectedNumberOfTimesWriteCalled = 2;
            var mockWriter = ConfigureStubsAndMocksForAsyncLog(out persister, "Test String", new DateTime(2015, 10, 1), MockDateDefaultFactory(), out mockMessageWriterFactory, 1);
            mockWriter.Expect(x => x.Write("2015-10-01 00:00:00:000\tTest String\t\r\n", null)).Repeat.Times(ExpectedNumberOfTimesWriteCalled);
            //act
            persister.Persist(new Message());
            Thread.Sleep(100);
            persister.Persist(new Message());
            persister.Stop();
            Thread.Sleep(100);

            //assert
            //verifying that mockWriter has been called twice
            mockWriter.VerifyAllExpectations();
        }


        private static IDateFactory MockDateDefaultFactory()
        {
            var mockDateFactory = MockRepository.GenerateMock<IDateFactory>();
            mockDateFactory.Stub(x => x.GetNow()).Return(DateTime.Now);
            return mockDateFactory;
        }

        private static IMessageWriter ConfigureStubsAndMocksForAsyncLog(out IMessagePersister persister,
                                                                    string testString, 
                                                                    DateTime logTimeStamp, 
                                                                    IDateFactory dateFactory,
                                                                    out IMessageWriterFactory mockMessageWriterFactory,
                                                                    int expecteNumberOfTimesCreateToCall)
        {
            mockMessageWriterFactory = MockRepository.GenerateMock<IMessageWriterFactory>();
            IMessageWriter mockWriter = MockRepository.GenerateMock<IMessageWriter>();
            IMessageFactory stubMessageFactory = MockRepository.GenerateStub<IMessageFactory>();
            IMessage stubMessage = MockRepository.GenerateStub<IMessage>();
            stubMessage.Stub(x => x.FormatMessage()).Return(testString);
            stubMessage.Timestamp = logTimeStamp;
            stubMessageFactory.Stub(x => x.GetMessage(Arg<string>.Is.Anything, Arg<DateTime>.Is.Anything))
                .Return(stubMessage);
            mockMessageWriterFactory.Expect(x => x.Create()).Return(mockWriter).Repeat.Times(expecteNumberOfTimesCreateToCall);
            persister = new AsyncMessagePersister(mockMessageWriterFactory, stubMessageFactory, dateFactory);
            return mockWriter;
        }

    }
}
