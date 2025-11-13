using DataverseLogic.EconomyArea.TransactionCalculation.Dto;

namespace DataverseLogic.EconomyArea.TransactionCalculation;

internal sealed class MonthlyTransactionCalculationService : ITransactionCalculationService
{
    public IEnumerable<TransactionPeriod> CalculateTransactions(DateTime from, DateTime until)
    {
        var startMonth =
            from.Day >= 15
            ? from.Month + 1
            : from.Month;
        var endMonth =
            until.Day < 15
            ? until.Month - 1
            : until.Month;
        var startYear = from.Year;
        var endYear = until.Year;

        for (var year = startYear; year <= endYear; year++)
        {
            var monthStart = year == startYear ? startMonth : 1;
            var monthEnd = year == endYear ? endMonth : 12;

            for (var month = monthStart; month <= monthEnd; month++)
            {
                var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
                var endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0, DateTimeKind.Utc);

                yield return new TransactionPeriod(startDate, endDate);
            }
        }
    }
}