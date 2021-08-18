using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace YarZip
{
    public class YarZipCompress : YarZip
    {
        public YarZipCompress(string fileToWork) : base(fileToWork)
        {
        }

        public void CompressTo(string resultPath)
        {
            File.Delete(resultPath);
            var startTime = DateTime.Now;

            new Thread(() => StartBuildResult(resultPath)).Start();

            for (int i = 0; i < blockCount; i++)
            {
                int tempI = i;
                var t = new Thread(() => ReadPart(tempI, resultPath));
                t.Start();
            }

            endWorkEvent.WaitOne();
            Console.WriteLine($"Compress done in {DateTime.Now - startTime}");
        }

        private void ReadPart(int partIndex, string resultPath)
        {
            sem.WaitOne();

            using (var sourceStream = File.OpenRead(fileToWork))
            using (var targetStream = File.Create(GetTempFileName(resultPath, partIndex)))
            using (var compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
            {
                var buffer = new byte[blockSize];
                sourceStream.Position = blockSize * partIndex;
                var readed = sourceStream.Read(buffer);
                compressionStream.Write(buffer, 0, readed);
            }

            lockers[partIndex].Set();

            sem.Release();
        }

    }
}
