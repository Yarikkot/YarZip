using System;
using System.IO;
using System.Threading;

namespace YarZip
{
    public abstract class YarZip
    {
        readonly protected long blockSize = 1 * 1024 * 1024;

        readonly protected int blockCount;
        readonly protected ManualResetEvent endWorkEvent = new ManualResetEvent(false);
        readonly protected ManualResetEvent[] lockers;
        readonly protected string fileToWork;
        readonly protected Semaphore sem = new Semaphore(Environment.ProcessorCount, Environment.ProcessorCount);

        protected YarZip(string fileToWork)
        {
            blockCount = (int)Math.Ceiling((double)new FileInfo(fileToWork).Length / blockSize);
            this.fileToWork = fileToWork;
            lockers = new ManualResetEvent[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                lockers[i] = new ManualResetEvent(false);
            }
        }

        protected void StartBuildResult(string resultPath)
        {
            for (int i = 0; i < blockCount; i++)
            {
                lockers[i].WaitOne();
                var bytes = File.ReadAllBytes(GetTempFileName(resultPath, i));
                using (FileStream targetStream = new FileStream(resultPath, FileMode.Append))
                    targetStream.Write(bytes);

                File.Delete($"{resultPath}_{i}");
                Console.CursorLeft = 0;
                Console.Write($"{i} from {blockCount - 1}");
            }
            endWorkEvent.Set();
            Console.WriteLine();
        }
        protected string GetTempFileName(string path, int index)
        {
            return $"{path}_{index}";
        }
    }
}
