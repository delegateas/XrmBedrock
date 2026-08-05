using XrmBedrock.SharedContext;

namespace DataverseLogic.EconomyArea.TransactionCalculation;

internal sealed class TransactionCalculationFactory
{
    private readonly ctx_billinginterval interval;

    public TransactionCalculationFactory(ctx_billinginterval interval)
    {
        this.interval = interval;
    }

    public ITransactionCalculationService GetCalculationService()
    {
        return interval switch
        {
            ctx_billinginterval.Yearly => new YearlyTransactionCalculationService(),
            ctx_billinginterval.Monthly => new MonthlyTransactionCalculationService(),
            _ => throw new NotSupportedException($"Unknown interval {interval}"),
        };
    }
}
