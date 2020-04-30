using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EscapeFromTheWoods.Models;
using Syroot.Windows.IO;

namespace EscapeFromTheWoods.Func
{
    class Output
    {
        private static SqlConnection GetConnection()
        {
            string connectionString = @"Data Source=DESKTOP-CQ5M5QL\SQLEXPRESS;Initial Catalog=Excersises;Integrated Security=True";
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }
        public static async Task MaakTxtOutput(List<Bos> output)
        {
            KnownFolder documents = new KnownFolder(KnownFolderType.Documents);
            Directory.CreateDirectory(documents.Path + @"\OefThread" + @"\Txts");
            foreach (var bos in output)
            {
                if (bos.ZijnApenKlaarMetSpringen == true)
                {
                    await MaakBosTxt(bos);
                }

            }
        }

        private static async Task MaakBosTxt(Bos output)
        {
            KnownFolder docs = new KnownFolder(KnownFolderType.Documents);
            int meesteSprongen = output.Apen.OrderByDescending(s => s.VorigeBomen.Count).First().VorigeBomen.Count;
            string path = docs.Path + @"\OefThread" + @"\Txts";
            Console.WriteLine("begin bos {0} txt", output.ID);
            using (StreamWriter sw = new StreamWriter(path + @$"\SprongenBos_{output.ID}.txt"))
            {
                for (int i = 0; i < meesteSprongen; i++)
                {
                    for (int j = 0; j < output.Apen.Count; j++)
                    {
                        if (output.Apen[j].VorigeBomen.ContainsKey(i))
                        {
                            await sw.WriteAsync($"{output.Apen[j].Naam} is in tree {output.Apen[j].VorigeBomen[i].ID}" +
                                $" at ({output.Apen[j].VorigeBomen[i].X + 1}, {output.Apen[j].VorigeBomen[i].Y + 1})\n");
                        }
                    }
                }
            }
            Console.WriteLine("einde bos  {0} txt", output.ID);

        }

        public static async Task MaakBmOutput(List<Bos> output)
        {
            KnownFolder documents = new KnownFolder(KnownFolderType.Documents);
            Directory.CreateDirectory(documents.Path + @"\OefThread" + @"\Bitmaps");
            foreach (var bos in output)
            {
                if (bos.ZijnApenKlaarMetSpringen == true)
                {
                    await MaakBosBm(bos);
                }

            }
        }

        private static async Task MaakBosBm(Bos output)
        {
            Console.WriteLine("begin bos {0} bm", output.ID);
            await Task.Run(() => output.SaveBm());
            Console.WriteLine("einde bos  {0} bm", output.ID);
        }

        public static async Task MaakDBOutput(List<Bos> output)
        {

            Dictionary<string, DataTable> tables = new Dictionary<string, DataTable>();

            DataTable wood = new DataTable();
            wood.Columns.Add("recordId", typeof(int));
            wood.Columns.Add("woodID", typeof(int));
            wood.Columns.Add("treeID", typeof(int));
            wood.Columns.Add("x", typeof(int));
            wood.Columns.Add("y", typeof(int));

            DataTable monkey = new DataTable();
            monkey.Columns.Add("recordId", typeof(int));
            monkey.Columns.Add("monkeyID", typeof(int));
            monkey.Columns.Add("monkeyName", typeof(string));
            monkey.Columns.Add("woodID", typeof(int));
            monkey.Columns.Add("seqnr", typeof(int));
            monkey.Columns.Add("treeID", typeof(int));
            monkey.Columns.Add("x", typeof(int));
            monkey.Columns.Add("y", typeof(int));

            DataTable logs = new DataTable();
            logs.Columns.Add("Id", typeof(int));
            logs.Columns.Add("woodID", typeof(int));
            logs.Columns.Add("monkeyID", typeof(int));
            logs.Columns.Add("message", typeof(string));
            int woodRecordCounter = 0;
            int mokeyRecordCounter = 0;
            int LogsCounter = 0;
            foreach (var bos in output)
            {
                foreach (var boom in bos.Bomen)
                {
                    wood.Rows.Add(woodRecordCounter, bos.ID, boom.ID, boom.X, boom.Y);
                    woodRecordCounter++;
                }

                foreach (var aap in bos.Apen)
                {
                    foreach (var pair in aap.VorigeBomen)
                    {
                        monkey.Rows.Add(mokeyRecordCounter, aap.ID, aap.Naam, bos.ID, pair.Key, pair.Value.ID, pair.Value.X, pair.Value.Y);
                        mokeyRecordCounter++;
                        string message = $"{aap.Naam} is now at tree {pair.Value.ID} at location ({pair.Value.X},{pair.Value.Y})";
                        logs.Rows.Add(LogsCounter, bos.ID, aap.ID, message);
                        LogsCounter++;
                    }
                }
            }


            tables.Add("WoodRecords", wood);
            tables.Add("MonkeyRecords", monkey);
            tables.Add("Logs", logs);
            await InsertDbOutput(tables);
        }

        private static async Task InsertDbOutput(Dictionary<string, DataTable> tablesToInsert)
        {

            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                using (SqlBulkCopy sqlBulk = new SqlBulkCopy(connection))
                {
                    foreach (var datatable in tablesToInsert)
                    {
                        await Task.Run(() =>
                        {
                            Console.WriteLine("Inserting: " + datatable.Key);
                            sqlBulk.BulkCopyTimeout = 0;
                            sqlBulk.DestinationTableName = datatable.Key;
                            sqlBulk.WriteToServer(datatable.Value);
                            Console.WriteLine(datatable.Key + " is in database toegevoegd");
                        });
                    }
                }
            }
        }
    }
}
