using ETLSystem.Data.Models;
using System.Globalization;

namespace ETLSystem.Services
{
    public class CsvReaderService
    {
        public List<Transaction> ReadTransactions(string filePath)
        {
            var transactions = new List<Transaction>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSV file not found: {filePath}");
            }

            try
            {
                var lines = File.ReadAllLines(filePath);

                if (lines.Length == 0)
                {
                    throw new Exception("CSV file is empty");
                }

                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var transaction = ParseTransactionLine(line, i + 1);
                        if (transaction != null)
                        {
                            transactions.Add(transaction);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing row {i + 1}: {line}. Грешка: {ex.Message}");

                    }
                }

                Console.WriteLine($"Successfully processed {transactions.Count} by {lines.Length - 1} rows");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error reading file: {ex.Message}", ex);
            }

            return transactions;
        }

        private Transaction ParseTransactionLine(string line, int lineNumber)
        {
            var columns = line.Split(',');

            if (columns.Length != 4)
            {
                Console.WriteLine($"Row {lineNumber}: Invalid number of columns. Expected: 4, Found: {columns.Length}");
                return null;
            }

            try
            {
                var transaction = new Transaction();

                if (!int.TryParse(columns[0].Trim(), out int transactionId))
                {
                    Console.WriteLine($"Row {lineNumber}: Invalid TransactionId: '{columns[0]}'");
                    return null;
                }
                transaction.TransactionId = transactionId;

                var customerName = columns[1].Trim();
                if (string.IsNullOrWhiteSpace(customerName))
                {
                    Console.WriteLine($"Row {lineNumber}: Empty customer name");
                    return null;
                }
                transaction.CustomerName = customerName;

                if (!decimal.TryParse(columns[2].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                {
                    Console.WriteLine($"Row {lineNumber}: Invalid sum: '{columns[2]}'");
                    return null;
                }
                transaction.Amount = amount;

                if (!TryParseDate(columns[3].Trim(), out DateTime transactionDate))
                {
                    Console.WriteLine($"Row {lineNumber}: Invalid date format: '{columns[3]}'");
                    return null;
                }
                transaction.TransactionDate = transactionDate;


                return transaction;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Row {lineNumber}: Unsuccessful transaction: {ex.Message}");
                return null;
            }
        }

        private bool TryParseDate(string dateString, out DateTime result)
        {
            string[] formats = {
                "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy",
                "yyyy-MM-dd", "yyyy-M-d", "yyyy-MM-d", "yyyy-M-dd",
                "MM.dd.yyyy", "M.d.yyyy", "MM.d.yyyy", "M.dd.yyyy",
                "dd-MM-yyyy", "d-M-yyyy", "dd-M-yyyy", "d-MM-yyyy",
                "yyyy/MM/dd", "yyyy/M/d", "yyyy/MM/d", "yyyy/M/dd"
            };

            return DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        }

        public void PrintStatistics(List<Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
            {
                Console.WriteLine("No information for transactions.");
                return;
            }

            Console.WriteLine($"Total number of transactions: {transactions.Count}");
            Console.WriteLine($"Total amount of all transactions: {transactions.Sum(t => t.Amount):C}");
            Console.WriteLine($"Largest transaction: {transactions.Max(t => t.Amount):C}");
        }
    }
}
