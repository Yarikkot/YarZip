using System;
using System.IO;

namespace YarZip
{
    class Program
    {
        static string Decompress = "decompress";
        static string Compress = "compress";
        static int Main(string[] args)
        {
            try
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Input all parameters for work. Ex: compress/decompress pathFrom pathTo");
                    return 1;
                }
                if (args[0].ToLower() == Decompress)
                {
                    if (IsCorrectPathParams(args[1], args[2]))
                        new YarZipDecompress(args[1]).DecompressTo(args[2]);
                    else
                        return 1;
                }
                else if (args[0].ToLower() == Compress)
                {
                    if (IsCorrectPathParams(args[1], args[2]))
                        new YarZipCompress(args[1]).CompressTo(args[2]);
                    else
                        return 1;
                }
                else
                {
                    Console.WriteLine("First parameter must be decompress or compress");
                    return 1;
                }
            }
            catch (InvalidDataException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected error, show this message to your system administrator");
                Console.WriteLine(e);
            }
            return 0;
        }
        static bool IsCorrectPathParams(string sourcePath, string targetPath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                Console.WriteLine("Incorect source file name");
                return false;
            }
            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Console.WriteLine("Incorect result file name");
                return false;
            }
            return true;
        }
    }


}
