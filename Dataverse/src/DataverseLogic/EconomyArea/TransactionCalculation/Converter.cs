using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataverseLogic.EconomyArea.TransactionCalculation.Dto;
using XrmBedrock.SharedContext;

namespace DataverseLogic.EconomyArea.TransactionCalculation;

internal static class Converter
{
    public static Interval ToDomain(this mgs_billinginterval billinginterval) => billinginterval switch
    {
        mgs_billinginterval.Monthly => Interval.Monthly,
        mgs_billinginterval.Yearly => Interval.Yearly,
        _ => throw new NotImplementedException($"{billinginterval}"),
    };
}