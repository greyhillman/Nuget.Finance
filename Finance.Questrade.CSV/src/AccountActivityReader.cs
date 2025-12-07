using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Finance.Questrade.CSV;

public class AccountActivityReader
{
    public async IAsyncEnumerable<AccountActivity> Read(TextReader reader)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            // Let the caller manage the reader
            LeaveOpen = true,
        };

        using (var csv = new CsvReader(reader, config))
        {
            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                var action = csv.GetField("Action");
                var activity = csv.GetField("Activity Type");
                var accountType = csv.GetField("Account Type");

                yield return new AccountActivity
                {
                    TransactionDate = csv.GetField<DateTime>("Transaction Date"),
                    SettlementDate = csv.GetField<DateTime>("Settlement Date"),
                    Action = action != null
                        ? ParseAction(action)
                        : null,
                    Symbol = csv.GetField<string?>("Symbol"),
                    Description = csv.GetField<string>("Description") ?? string.Empty,
                    Quantity = csv.GetField<decimal>("Quantity"),
                    Price = csv.GetField<decimal>("Price"),
                    Gross = csv.GetField<decimal>("Gross Amount"),
                    Commission = csv.GetField<decimal>("Commission"),
                    Net = csv.GetField<decimal>("Net Amount"),
                    Currency = csv.GetField("Currency"),
                    AccountNumber = csv.GetField("Account #"),
                    Activity = ParseActivityType(activity),
                    AccountType = ParseAccountType(accountType),
                };
            }
        }
    }

    static AccountAction? ParseAction(string value)
    {
        return value switch
        {
            "Buy" => AccountAction.Buy,
            "CON" => AccountAction.Contribute,
            "DEP" => AccountAction.Deposit,
            "WDR" => AccountAction.Withdraw,
            "EFT" => AccountAction.Withdraw, // Maybe something other than withdraw?
            "Sell" => AccountAction.Sell,
            "" => null,
            _ => throw new ArgumentException($"\"value\"", value),
        };
    }

    static ActivityType ParseActivityType(string value)
    {
        return value switch
        {
            "Dividends" => ActivityType.Dividends,
            "Trades" => ActivityType.Trades,
            "Deposits" => ActivityType.Deposits,
            "Withdrawals" => ActivityType.Withdrawals,
            _ => throw new NotSupportedException(value),
        };
    }

    static AccountType ParseAccountType(string value)
    {
        return value switch
        {
            "Individual RRSP" => AccountType.IndividualRRSP,
            "Individual TFSA" => AccountType.IndividualTFSA,
            "Individual margin" => AccountType.IndividualMargin,
            "Individual FHSA" => AccountType.IndividualFHSA,
            _ => throw new NotSupportedException(value),
        };
    }
}
