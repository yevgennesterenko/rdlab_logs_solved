using MessagePersister.Interfaces;

namespace MessagePersister.Implementation
{
    using System;
    using System.Text;

    /// <summary>
    /// This is the object that the diff. persisters (filepersister, consolepersister etc.) will operate on. The LineText() method will be called to get the text (formatted) to log
    /// </summary>
    public class Message : IMessage
    {
        #region Private Fields
        private Guid guid;
        private string name;
        private string payload;


        #endregion

        #region Constructors

        public Message()
        {
            this.Text = "";
        }

        public string Name
        {
            get { return this.name; }
        }

        #endregion

        #region Public Methods

        public Message(Guid guid, string name, string payload)
        {
            this.guid = guid;
            this.name = name;
            this.payload = payload;
        }


        /// <summary>
        /// Return a formatted message
        /// </summary>
        /// <returns></returns>
        public virtual string FormatMessage()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(this.guid);
            sb.Append(". ");

            if (!string.IsNullOrEmpty(this.name))
            {
                sb.Append(this.name);
                sb.Append(". ");
            }

            if (!string.IsNullOrEmpty(payload))
            {
                sb.Append(this.payload);
                sb.Append(". ");
            }

            sb.Append(this.CreateMessageFooter());

            return sb.ToString();
        }


        public virtual string CreateMessageFooter()
        {
            return "";
        }

        public virtual string CreateLineText()
        {
            return "";
        }


        #endregion

        #region Properties

        /// <summary>
        /// The text to be display in logline
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The Timestamp is initialized when the log is added. Th
        /// </summary>
        public virtual DateTime Timestamp { get; set; }
  

        #endregion
    }
}