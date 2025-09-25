using ETLSystem.Data;
using ETLSystem.Data.Models;
using ETLSystem.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ETLSystem
{
    internal class Program
    {

        private static List<Transaction> _cleanTransactions = new List<Transaction>();
        public static ApplicationDBContext db;
        static async Task Main(string[] args)
        {
            db = new ApplicationDBContext();

            await db.Database.EnsureCreatedAsync();

            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            var csvReader = new CsvReaderService();
            var transformer = new DataTransformerService();

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("===== Menu =====");
                Console.WriteLine("1) Import a CSV file");
                Console.WriteLine("2) Show total number of transactions");
                Console.WriteLine("3) Show the amount of all transactions");
                Console.WriteLine("4) Show largest transaction");
                Console.WriteLine("0) Exit");
                Console.Write("Selected: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            HandleImport(csvReader, transformer);
                            break;

                        case "2":
                            if (!RequireData()) break;
                            var count = await db.Transactions.CountAsync();
                            Console.WriteLine("Total number of transactions: " + count);
                            break;

                        case "3":
                            if (!RequireData()) break;
                            var sum = await db.Transactions.SumAsync(t => t.Amount);
                            Console.WriteLine("Total of all transactions: " + sum);
                            break;

                        case "4":
                            if (!RequireData()) break;
                            var maxTx = await db.Transactions
                                .OrderByDescending(t => t.Amount)
                                .FirstOrDefaultAsync();

                            Console.WriteLine("Largest transaction:");
                            Console.WriteLine("  Id: " + maxTx.TransactionId);
                            Console.WriteLine("  Customer: " + maxTx.CustomerName);
                            Console.WriteLine("  Amount: " + maxTx.Amount);
                            Console.WriteLine("  Date: " + maxTx.TransactionDate.ToString("yyyy-MM-dd"));
                            break;

                        case "0":
                            return;

                        default:
                            Console.WriteLine("Invalid selection.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    if (ex.InnerException != null)
                        Console.WriteLine("Detail: " + ex.InnerException.Message);
                }
            }
        }

        static async Task HandleImport(CsvReaderService csvReader, DataTransformerService transformer)
        {
            string csvFilePath = FindCsvFile();
            if (csvFilePath == null)
            {
                Console.WriteLine("transactions.csv file not found!");
                return;
            }

            List<Transaction> rawTransactions = csvReader.ReadTransactions(csvFilePath);
            Console.WriteLine("Retrieved records: " + rawTransactions.Count);


            _cleanTransactions = transformer.TransformData(rawTransactions);
            Console.WriteLine("Valid after cleaning: " + _cleanTransactions.Count);


            if (_cleanTransactions.Count > 0)
            {
                string dir = Path.GetDirectoryName(csvFilePath);
                if (string.IsNullOrEmpty(dir)) dir = Directory.GetCurrentDirectory();
                string cleanDataPath = Path.Combine(dir, "clean_transactions.csv");
                ExportCleanData(_cleanTransactions, cleanDataPath);
                await UpsertTransactionsAsync(_cleanTransactions, db);
            }
        }

        static bool RequireData()
        {
            if (_cleanTransactions == null || _cleanTransactions.Count == 0)
            {
                Console.WriteLine("No data loaded. First select '1) Import CSV file'.");
                return false;
            }
            return true;
        }

        static string FindCsvFile()
        {
            string currentDir = Directory.GetCurrentDirectory();
            string[] possiblePaths = new string[]
            {
                "transactions.csv",
                Path.Combine(currentDir, "transactions.csv"),
                Path.Combine(currentDir, "..", "..", "..", "transactions.csv")
            };

            for (int i = 0; i < possiblePaths.Length; i++)
            {
                string fullPath = Path.GetFullPath(possiblePaths[i]);
                if (File.Exists(fullPath))
                {
                    Console.WriteLine("CSV file found.");
                    return fullPath;
                }
            }

            Console.WriteLine("'transactions.csv' not found in known paths.");
            return null;
        }

        static void ExportCleanData(List<Transaction> transactions, string outputPath)
        {
            if (transactions == null || transactions.Count == 0)
            {
                Console.WriteLine("There is no data to export.");
                return;
            }

            var csvLines = new List<string>();
            csvLines.Add("TransactionId,CustomerName,Amount,TransactionDate");
            foreach (var t in transactions)
            {
                csvLines.Add(string.Format(
                    "{0},{1},{2},{3}",
                    t.TransactionId,
                    t.CustomerName,
                    t.Amount,
                    t.TransactionDate.ToString("yyyy-MM-dd")));
            }

            File.WriteAllLines(outputPath, csvLines);
            Console.WriteLine("Exported " + transactions.Count + " : " + Path.GetFullPath(outputPath));
        }

        public static async Task<int> UpsertTransactionsAsync(List<Transaction> items, ApplicationDBContext db)
        {

            if (items == null || items.Count == 0)
                return 0;

            var ids = items.Select(i => i.TransactionId).ToList();

            var existing = await db.Transactions
                .Where(t => ids.Contains(t.TransactionId))
                .ToListAsync();

            var existingById = existing.ToDictionary(t => t.TransactionId);

            foreach (var item in items)
            {
                Transaction current;
                if (existingById.TryGetValue(item.TransactionId, out current))
                {
                    current.CustomerName = item.CustomerName;
                    current.Amount = item.Amount;
                    current.TransactionDate = item.TransactionDate;


                    db.Entry(current).State = EntityState.Modified;
                }
                else
                {
                    await db.Transactions.AddAsync(item);
                }
            }

            return await db.SaveChangesAsync();
        }
    }
}


