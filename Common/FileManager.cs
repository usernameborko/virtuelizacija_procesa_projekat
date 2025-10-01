using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class FileManager : IDisposable
    {
        private StreamReader streamReader;
        private FileStream fileStream;
        private bool disposed = false;

        public FileManager(string filePath)
        {
            try
            {
                fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                streamReader = new StreamReader(fileStream);
            } catch(Exception ex)
            {
                throw new Exception($"Error opening file {filePath}: {ex.Message}");
            }
        }

        public string ReadLine()
        {
            if (disposed)
            {
                throw new ObjectDisposedException("FileManager");
            }

            return streamReader?.ReadLine();
        }

        public bool EndOfStream
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("FileManager");
                }

                return streamReader?.EndOfStream ?? true;
            }
        }

        ~FileManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    streamReader?.Dispose();
                    fileStream?.Dispose();
                }

                disposed = true;
            }
        }
    }
}
