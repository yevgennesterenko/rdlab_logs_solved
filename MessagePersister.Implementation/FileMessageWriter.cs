using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MessagePersister.Interfaces;

namespace MessagePersister.Implementation
{
    /// <summary>
    /// Writer implementation responsible for writing logs to file
    /// </summary>
    public class FileMessageWriter : IMessageWriter
    {    
        private string messageFolderBase;
        private string currentFolder;
        private string dateTimeFormat;
        private string extension;

        public FileMessageWriter(string messageFolderBase, string messageName, string dateTimeFormat, string extension)
        {
            this.messageFolderBase = messageFolderBase;
            this.currentFolder = String.Format("{0}\\{1}{2}", messageFolderBase, "Messages", DateTime.Now.ToString("yyyyMMdd"));
            this.dateTimeFormat = dateTimeFormat;
            this.extension = extension;
            this.CreateFolder(currentFolder);            
        }

        public void CreateFolder(string folder)
        {
            
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }


        public void Write(string line, IMessage message)
        {
            using (var writer = File.AppendText(String.Format("{0}\\{1}{2}{3}", this.currentFolder, message.Text, DateTime.Now.ToString(dateTimeFormat), extension)))
            {
                writer.AutoFlush = true;
                writer.Write("Timestamp".PadRight(25, ' ') + "\t" + "Data".PadRight(15, ' ') + "\t" + Environment.NewLine);
                writer.Write(line);
            }
        }
    }
}
