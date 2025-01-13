using DataverseLogic.EconomyArea.TransactionCalculation.Dto;

namespace DataverseLogic.EconomyArea.TransactionCalculation;

internal sealed class TransactionCalculationFactory
{
    private readonly Interval interval;

    public TransactionCalculationFactory(Interval interval)
    {
        this.interval = interval;
    }

    public ITransactionCalculationService Create()
    {
        return interval switch
        {
            Interval.Yearly => new YearlyTransactionCalculationService(),
            Interval.Monthly => new MonthlyTransactionCalculationService(),
            _ => throw new NotSupportedException($"Unknown interval {interval}"),
        };
    }
}