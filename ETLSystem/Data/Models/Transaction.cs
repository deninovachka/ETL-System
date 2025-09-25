namespace ETLSystem.Data.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; }

    }
}
