using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Finance.Yahoo.CSV;

public record StockPrice
{
    public DateTime Date { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal AdjustedClose { get; set; }
    public decimal Volume { get; set; }

    public StockPrice(DateTime date)
    {
        Date = date;
    }
}

public class StockPriceReader
{
    public async IAsyncEnumerable<StockPrice> Read(TextReader reader)
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
                var date = csv.GetField<DateTime>("Date");

                var open = csv.GetField<decimal>("Open");
                var high = csv.GetField<decimal>("High");
                var low = csv.GetField<decimal>("Low");
                var close = csv.GetField<decimal>("Close");
                var adjustedClose = csv.GetField<decimal>("Adj Close");
                var volume = csv.GetField<decimal>("Volume");

                yield return new StockPrice(date)
                {
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close,
                    AdjustedClose = adjustedClose,
                    Volume = volume,
                };
            }
        }
    }
}
