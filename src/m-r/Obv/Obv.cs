namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // ON-BALANCE VOLUME
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<ObvResult> GetObv<TQuote>(
        this IEnumerable<TQuote> quotes,
        int? smaPeriods = null)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ToQuoteD();

        // check parameter arguments
        ValidateObv(smaPeriods);

        // initialize
        List<ObvResult> results = new(quotesList.Count);

        double? prevClose = null;
        double? obv = 0;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            QuoteD q = quotesList[i];

            if (prevClose == null || q.Close == prevClose)
            {
                // no change to OBV
            }
            else if (q.Close > prevClose)
            {
                obv += q.Volume;
            }
            else if (q.Close < prevClose)
            {
                obv -= q.Volume;
            }

            ObvResult result = new()
            {
                Date = q.Date,
                Obv = obv
            };
            results.Add(result);

            prevClose = q.Close;

            // optional SMA
            if (smaPeriods != null && i + 1 > smaPeriods)
            {
                double? sumSma = 0;
                for (int p = i + 1 - (int)smaPeriods; p <= i; p++)
                {
                    sumSma += results[p].Obv;
                }

                result.ObvSma = sumSma / smaPeriods;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateObv(
        int? smaPeriods)
    {
        // check parameter arguments
        if (smaPeriods is not null and <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for OBV.");
        }
    }
}
