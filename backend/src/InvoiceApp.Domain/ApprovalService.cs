namespace InvoiceApp.Domain;

/// <summary>
/// Moves an invoice through its lifecycle: Draft -> PendingApproval -> Approved or Rejected.
/// Enforces role-based solo approval limits (Manager / Director).
/// </summary>
public class ApprovalService
{
    public const decimal ManagerLimit = 50_000m;
    public const decimal DirectorLimit = 500_000m;

    private readonly IInvoiceRepository _repository;

    public ApprovalService(IInvoiceRepository repository)
    {
        _repository = repository;
    }

    public Invoice Submit(string invoiceId)
    {
        var invoice = Find(invoiceId);

        if (invoice.Status != InvoiceStatus.Draft)
        {
            throw new InvalidOperationException(
                $"Invoice {invoiceId} is {invoice.Status}; only drafts can be submitted.");
        }

        invoice.Status = InvoiceStatus.PendingApproval;
        _repository.Update(invoice);
        return invoice;
    }

    public Invoice Approve(string invoiceId, string approver, string role)
    {
        var invoice = Find(invoiceId);
        EnsurePending(invoice);

        var authority = ResolveAuthority(role);
        EnsureWithinSoloLimit(invoice, authority);

        invoice.Status = InvoiceStatus.Approved;
        invoice.DecidedBy = approver;
        invoice.DecidedAt = DateTime.UtcNow;
        _repository.Update(invoice);
        return invoice;
    }

    public Invoice Reject(string invoiceId, string approver, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("A reason is required to reject an invoice.", nameof(reason));
        }

        var invoice = Find(invoiceId);
        EnsurePending(invoice);

        invoice.Status = InvoiceStatus.Rejected;
        invoice.DecidedBy = approver;
        invoice.DecidedAt = DateTime.UtcNow;
        invoice.DecisionNote = reason;
        _repository.Update(invoice);
        return invoice;
    }

    private static void EnsureWithinSoloLimit(Invoice invoice, ApprovalAuthority authority)
    {
        var limit = authority switch
        {
            ApprovalAuthority.Manager => ManagerLimit,
            ApprovalAuthority.Director => DirectorLimit,
            _ => 0m
        };

        if (invoice.Amount > limit)
        {
            throw new ArgumentException(
                $"A {authority} may not approve invoices above {limit:N0}. Invoice {invoice.Id} is for {invoice.Amount:N2}.");
        }
    }

    private static ApprovalAuthority ResolveAuthority(string role)
    {
        return role?.Trim() switch
        {
            "Manager" => ApprovalAuthority.Manager,
            "Approver" => ApprovalAuthority.Manager,
            "Director" => ApprovalAuthority.Director,
            "Admin" => ApprovalAuthority.Director,
            _ => throw new ArgumentException($"Unknown approval role '{role}'.")
        };
    }

    private Invoice Find(string invoiceId) =>
        _repository.GetById(invoiceId)
        ?? throw new KeyNotFoundException($"Invoice {invoiceId} was not found.");

    private static void EnsurePending(Invoice invoice)
    {
        if (invoice.Status != InvoiceStatus.PendingApproval)
        {
            throw new InvalidOperationException(
                $"Invoice {invoice.Id} is {invoice.Status}; only pending invoices can be decided.");
        }
    }

    private enum ApprovalAuthority
    {
        Manager,
        Director
    }
}
