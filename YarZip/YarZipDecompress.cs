using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace YarZip
{
    public class YarZipDecompress : YarZip
    {
        public YarZipDecompress(string fileToWork) : base(fileToWork)
        {
        }

        public void DecompressTo(string resultPath)
        {
            File.Delete(resultPath);
            var startTime = DateTime.Now;
            new Thread(() => StartBuildResult(resultPath)).Start();

            using (var sourceStream = File.OpenRead(fileToWork))
            using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                for (int i = 0; i < blockCount; i++)
                {

                    var buffer = new byte[blockSize];
                    var readed = decompressionStream.Read(buffer, 0, buffer.Length);
                    using (var targetStream = File.Create(GetTempFileName(resultPath, i)))
                        targetStream.Write(buffer, 0, readed);
                    lockers[i].Set();
                }

            endWorkEvent.WaitOne();
            Console.WriteLine($"Decompress done in {DateTime.Now - startTime}");
        }
    }
}
