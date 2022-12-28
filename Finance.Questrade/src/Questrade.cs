using System;

namespace Finance.Questrade
{
    public struct AccountActivity
    {
        public DateTime TransactionDate { get; set; }
        public DateTime SettlementDate { get; set; }
        public AccountAction? Action { get; set; }
        public string? Symbol { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Gross { get; set; }
        public decimal Commission { get; set; }
        public decimal Net { get; set; }
        public string Currency { get; set; }
        public string AccountNumber { get; set; }
        public ActivityType Activity { get; set; }
        public AccountType AccountType { get; set; }
    }

    public enum AccountType
    {
        IndividualRRSP,
        IndividualTFSA,
        IndividualMargin,
    }

    public enum ActivityType
    {
        Dividends,
        Trades,
        Deposits,
    }

    public enum AccountAction
    {
        Buy,
        Contribute,
        Deposit,
    }
}
