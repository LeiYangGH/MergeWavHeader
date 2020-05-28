using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeWavHeader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (DateTime.Now > new DateTime(2020, 5, 30))
                return;
            try
            {
                string outDir = "已合并";
                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);
                string headerFile = "5.a";
                foreach (string f in Directory.GetFiles(Environment.CurrentDirectory, "*.wav"))
                {
                    Console.WriteLine(f);
                    string filename = Path.GetFileName(f);
                    string outFileName = Path.Combine(outDir, filename.Replace(".wav", "-GG.wav"));
                    using (var outputStream = File.Create(outFileName))
                    {
                        using (var inputStream = File.OpenRead(headerFile))
                        {
                            // Buffer size can be passed as the second argument.
                            inputStream.CopyTo(outputStream);
                        }
                        using (var inputStream = File.OpenRead(f))
                        {
                            // Buffer size can be passed as the second argument.
                            inputStream.CopyTo(outputStream);
                        }
                        Console.WriteLine($"已生成{outFileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("按任意键退出");

            Console.ReadKey();
        }
    }
}
