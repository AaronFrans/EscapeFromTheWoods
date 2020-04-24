using EscapeFromTheWoods.Models;
using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Func
{
    class Controller
    {
        public static void Run()
        {
            List<Task> tasks = new List<Task>();
            List<Bos> bossen = new List<Bos>();
            bossen.Add(new Bos(0, 20, 20, 2, 350));
            bossen.Add(new Bos(1, 22, 22, 2, 400));
            //bossen.Add(new Bos(2, 30, 10, 4, 250));
            //bossen.Add(new Bos(3, 25, 30, 5, 700));
            foreach (var bos in bossen)
            {
                tasks.Add(Task.Run(() => bos.LaatApenSpringen()));
            }
            Task.WaitAll(tasks.ToArray());
            Task outputTask = makeOutput(bossen);
            outputTask.Wait();
            
        }
        private static async Task makeOutput(List<Bos> output)
        {
            KnownFolder documents = new KnownFolder(KnownFolderType.Documents);
            Directory.CreateDirectory(documents.Path + @"\OefThread");
            Task task1 = Output.MaakTxtOutput(output);
            Task task2 = Output.MaakBmOutput(output);
            Task task3 = Output.MaakDBOutput(output);

            Task.WaitAll(task1, task2,task3);
        }
    }
}
