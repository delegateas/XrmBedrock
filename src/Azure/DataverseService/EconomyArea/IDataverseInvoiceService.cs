namespace DataverseService.EconomyArea;

public interface IDataverseInvoiceService
{
    Task CreateInvoices(DateTime invoiceUntil, Guid invoiceCollectionId);
}