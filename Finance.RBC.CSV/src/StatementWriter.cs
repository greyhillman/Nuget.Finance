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
        public async Task Write(TextWriter writer, Statement[] statements)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Let caller handle the writer/stream
                LeaveOpen = true,
            };

            var commodities = new HashSet<string>();
            foreach (var statement in statements)
            {
                foreach (var commodity in statement.Amount.Commodities)
                {
                    commodities.Add(commodity);
                }
            }

            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteField("Account Type");
                csv.WriteField("Account Number");
                csv.WriteField("Transaction Date");
                csv.WriteField("Cheque Number");
                csv.WriteField("Description 1");
                csv.WriteField("Description 2");

                foreach (var commodity in commodities)
                {
                    csv.WriteField(commodity);
                }

                await csv.NextRecordAsync();

                foreach (var statement in statements)
                {
                    csv.WriteField(statement.AccountType);
                    csv.WriteField(statement.AccountNumber);
                    csv.WriteField(statement.TransactionDate);
                    csv.WriteField(statement.ChequeNumber);
                    csv.WriteField(statement.Description);
                    csv.WriteField(statement.SecondaryDescription);

                    foreach (var commodity in commodities)
                    {
                        var quantity = statement.Amount[commodity];

                        csv.WriteField(quantity);
                    }

                    await csv.NextRecordAsync();
                }
            }
        }
    }
}
