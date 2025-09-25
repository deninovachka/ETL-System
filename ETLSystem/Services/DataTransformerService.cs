using ETLSystem.Data.Models;

namespace ETLSystem.Services
{
    public class DataTransformerService
    {
        public List<Transaction> TransformData(List<Transaction> transactions)
        {
            if (transactions == null || transactions.Count == 0)
            {
                Console.WriteLine("No transactions to transform.");
                return new List<Transaction>();
            }

            Console.WriteLine("Starting data transformation...");
            Console.WriteLine($"Input data: {transactions.Count} transactions");

            var filteredTransactions = FilterTransactionsUnder100(transactions);
            Console.WriteLine($"After filtering: {filteredTransactions.Count} transactions");

            var uniqueTransactions = RemoveDuplicateTransactions(filteredTransactions);
            Console.WriteLine($"After removing duplicates: {uniqueTransactions.Count} transactions");

            return uniqueTransactions;
        }
        private List<Transaction> FilterTransactionsUnder100(List<Transaction> transactions)
        {
            return transactions.Where(t => t.Amount >= 100).ToList();
        }

        private List<Transaction> RemoveDuplicateTransactions(List<Transaction> transactions)
        {
            var uniqueTransactions = new List<Transaction>();

            foreach (var transaction in transactions)
            {
                bool isDuplicate = uniqueTransactions.Any(existing =>
                    existing.CustomerName == transaction.CustomerName &&
                    existing.Amount == transaction.Amount &&
                    existing.TransactionDate == transaction.TransactionDate);

                if (!isDuplicate)
                {
                    uniqueTransactions.Add(transaction);
                }
            }

            return uniqueTransactions;
        }
    }
}
