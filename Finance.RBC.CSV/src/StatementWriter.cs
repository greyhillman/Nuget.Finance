using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Finance.RBC;

namespace Finance.RBC.CSV
{
    public class StatementWriter
    {
        public async Task Write(TextWriter writer, IAsyncEnumerable<Statement> transactions)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Let caller handle the writer/stream
                LeaveOpen = true,
            };

            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteField("Account Type");
                csv.WriteField("Account Number");
                csv.WriteField("Transaction Date");
                csv.WriteField("Cheque Number");
                csv.WriteField("Description 1");
                csv.WriteField("Description 2");
                csv.WriteField("CAD$");
                csv.WriteField("USD$");

                await csv.NextRecordAsync();

                await foreach (var transaction in transactions)
                {
                    csv.WriteField(transaction.AccountType);
                    csv.WriteField(transaction.AccountNumber);
                    csv.WriteField(transaction.TransactionDate);
                    csv.WriteField(transaction.ChequeNumber);
                    csv.WriteField(transaction.Description);
                    csv.WriteField(transaction.SecondaryDescription);
                    csv.WriteField(transaction.CAD);
                    csv.WriteField(transaction.USD);

                    await csv.NextRecordAsync();
                }
            }
        }
    }
}
