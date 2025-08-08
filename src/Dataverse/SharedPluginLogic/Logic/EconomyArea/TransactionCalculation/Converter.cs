using DataverseLogic.EconomyArea.TransactionCalculation.Dto;
using XrmBedrock.SharedContext;

namespace DataverseLogic.EconomyArea.TransactionCalculation;

internal static class Converter
{
    public static Interval ToDomain(this demo_billinginterval billinginterval) => billinginterval switch
    {
        demo_billinginterval.Monthly => Interval.Monthly,
        demo_billinginterval.Yearly => Interval.Yearly,
        _ => throw new NotImplementedException($"{billinginterval}"),
    };
}