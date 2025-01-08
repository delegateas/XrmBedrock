using DataverseLogic.EconomyArea.TransactionCalculation.Dto;

namespace DataverseLogic.EconomyArea.TransactionCalculation;

internal interface ITransactionCalculationService
{
    IEnumerable<TransactionPeriod> CalculateTransactions(DateTime from, DateTime until);
}