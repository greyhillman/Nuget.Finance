using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Finance.RBC
{
    public record Statement
    {
        public string AccountType { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string ChequeNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SecondaryDescription { get; set; } = string.Empty;
        public decimal? CAD { get; set; } = null;
        public decimal? USD { get; set; } = null;

        public Statement(DateTime date)
        {
            TransactionDate = date;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("RBC Transaction");
            builder.AppendLine($"Account Type: {AccountType}");
            builder.AppendLine($"Account Number: {AccountNumber}");
            builder.AppendLine($"Transaction Date: {TransactionDate}");
            builder.AppendLine($"Cheque Number: {ChequeNumber}");
            builder.AppendLine($"Description: {Description}");
            builder.AppendLine($"Description 2: {SecondaryDescription}");
            builder.AppendLine($"CAD: {CAD}");
            builder.AppendLine($"USD: {USD}");

            return builder.ToString();
        }
    }
}