using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace MergeWavHeader
{
    class Program
    {
        static int Diff(int a, int b)
        {
            if (a == 8 && b == 0 || a == 0 && b == 8)
                return 1;
            else return Math.Abs(a - b);
        }
        static bool IsDoubleChannel(string fileName)
        {
            byte[] bytes = File.ReadAllBytes(fileName);
            int secLen = 8;
            int secLenH = secLen * 2;

            int i = 0;
            int notSimiliarCnt = 0;
            int similiarCnt = 0;

            while (i < bytes.Length / secLen - 1 && notSimiliarCnt <= 2)
            {

                byte[] bsSec = bytes.Skip(i * secLen).Take(secLen).ToArray();
                int[] bsSecH = new int[16];
                for (int k = 0; k < 8; k++)
                {
                    bsSecH[k * 2] = bsSec[k] / secLenH;

                    bsSecH[k * 2 + 1] = bsSec[k] % secLenH;
                }
                Console.WriteLine(string.Join(" ", bsSecH.Take(secLen).Select(x => x.ToString("X"))));
                Console.WriteLine(string.Join(" ", bsSecH.Skip(secLen).Take(secLen).Select(x => x.ToString("X"))));
                bool allSame = true;
                bool all8Similiar = true;

                for (int j = 0; j < secLen; j++)
                {
                    Console.Write(j + " ");
                    if (bsSecH[j] != bsSecH[j + secLen])
                        allSame = false;
                    if (Diff(bsSecH[j], bsSecH[j + secLen]) > 3)
                    {
                        all8Similiar = false;
                        break;
                    }

                }
                if (all8Similiar)
                {
                    similiarCnt++;
                }
                else
                {
                    notSimiliarCnt++;
                }
                Console.WriteLine($"------第{i}个16段--相似={all8Similiar}--相同={allSame}-----");
                Console.WriteLine();
                i++;

                if (similiarCnt >= 2)
                    return true;

            }
            return false;
        }
        static void Main(string[] args)
        {
            //if (DateTime.Now > new DateTime(2020, 5, 30))
            //    return;
            Console.WriteLine("版本05290906");
            try
            {
                string outDir = "已合并";
                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);


                foreach (string f in Directory.GetFiles(Environment.CurrentDirectory, "*.wav"))
                {
                    string headerFile = "5.a";
                    string extSuffix = "-GG.wav";
                    Console.WriteLine(f);
                    if (IsDoubleChannel(f))
                    {
                        Console.WriteLine($"双声道");
                        headerFile = "6.a";
                        extSuffix = "-sh.wav";
                    }
                    else
                    {
                        Console.WriteLine($"单声道");
                    }

                    string filename = Path.GetFileName(f);
                    //bool isDouble = IsDoubleChannel(f);


                    string outFileName = Path.Combine(outDir, filename.ToLower().Replace(".wav", extSuffix));
                    using (var outputStream = File.Create(outFileName))
                    {
                        using (var inputStream = File.OpenRead(headerFile))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                        using (var inputStream = File.OpenRead(f))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                        Console.WriteLine($"已生成{outFileName}");
                    }
                    Console.WriteLine("===================================================");
                    Console.WriteLine();
                    Console.WriteLine();
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
