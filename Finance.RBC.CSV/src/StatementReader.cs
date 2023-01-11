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
    public class StatementReader
    {
        public async IAsyncEnumerable<Statement> Read(TextReader reader)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                // Let the caller manage the reader/stream
                LeaveOpen = true,
            };
            using (var csv = new CsvReader(reader, config))
            {
                await csv.ReadAsync();
                csv.ReadHeader();

                while (await csv.ReadAsync())
                {
                    var date = csv.GetField<DateTime>("Transaction Date");

                    var accountType = csv.GetField("Account Type");
                    var accountNumber = csv.GetField("Account Number");
                    var chequeNumber = csv.GetField("Cheque Number");
                    var description = csv.GetField("Description 1");
                    var secondaryDescription = csv.GetField("Description 2");

                    var amount = new Amount();
                    for (var column = 6; column < csv.HeaderRecord!.Length; column++)
                    {
                        var commodity = csv.HeaderRecord![column];
                        var quantity = csv.GetField<decimal?>(column) ?? 0;

                        amount.Add(commodity, quantity);
                    }

                    yield return new Statement(date)
                    {
                        AccountType = accountType?.Trim() ?? string.Empty,
                        AccountNumber = accountNumber?.Trim() ?? string.Empty,
                        ChequeNumber = chequeNumber?.Trim() ?? string.Empty,
                        Description = description?.Trim() ?? string.Empty,
                        SecondaryDescription = secondaryDescription?.Trim() ?? string.Empty,
                        Amount = amount,
                    };
                }
            }
        }
    }
}
