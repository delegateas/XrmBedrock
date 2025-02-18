using DataverseLogic.EconomyArea.TransactionCalculation.Dto;

namespace DataverseLogic.EconomyArea.TransactionCalculation;

internal sealed class YearlyTransactionCalculationService : ITransactionCalculationService
{
    public IEnumerable<TransactionPeriod> CalculateTransactions(DateTime from, DateTime until)
    {
        var startYear =
            from.Month >= 7
            ? from.Year + 1
            : from.Year;

        var endYear =
            until.Month <= 6
            ? until.Year - 1
            : until.Year;

        for (var year = startYear; year <= endYear; year++)
        {
            yield return new TransactionPeriod(
                new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(year, 12, 31, 0, 0, 0, DateTimeKind.Utc));
        }
    }
}