using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CopyAsync
{
    class Context
    {

    }
    public class Copy
    {
        const int BUFFSIZE = 1000;
        byte[][] buf;
        FileStream From, To;

        AutoResetEvent schranke = new AutoResetEvent(true);
        ManualResetEvent CopyFinished = new ManualResetEvent(false);

        public Copy()
        {
            buf = new byte[2][];
            buf[0] = new byte[BUFFSIZE];
            buf[1] = new byte[BUFFSIZE];
        }

        public void ReadFinished(IAsyncResult ar)
        {
            int FinishedBufIdx = (int)ar.AsyncState;
            int NumberRead = From.EndRead(ar);

            if ( NumberRead != 0 )
            {
                int ReadBufIdx = FinishedBufIdx ^ 1;

                schranke.WaitOne();
                From.BeginRead(buf[ReadBufIdx], 0, BUFFSIZE, ReadFinished, ReadBufIdx);

                //
                // DO SOMETHING! DO SOMETHING! (with the buf[FinishedBufIdx])
                // meanwhile the read into buf[ReadBufIdx] is performed
                // 

                // write out the buffer sync
                To.Write(buf[FinishedBufIdx], 0, NumberRead);
                schranke.Set();
            }
            else
            {
                schranke.WaitOne();
                CopyFinished.Set();
            }
        }

        public void Run(string Filename1, string Filename2)
        {
            try
            {
                From = new FileStream(Filename1, FileMode.Open, FileAccess.Read, FileShare.Read, 12, FileOptions.SequentialScan );
                To = new FileStream(Filename2, FileMode.CreateNew);

                From.BeginRead(buf[0], 0, BUFFSIZE, ReadFinished, 0);
                CopyFinished.WaitOne();
            }
            finally
            {
                To.Close();
                From.Close();
            }

        }
    }
}
