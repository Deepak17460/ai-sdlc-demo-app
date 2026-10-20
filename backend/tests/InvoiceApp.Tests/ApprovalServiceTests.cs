using InvoiceApp.Domain;
using Xunit;

namespace InvoiceApp.Tests;

public class ApprovalServiceTests
{
    private static Invoice InvoiceWith(string id, InvoiceStatus status, decimal amount = 100m) =>
        new() { Id = id, CustomerName = "Test School", Amount = amount, Status = status };

    private static ApprovalService ServiceFor(params Invoice[] invoices) =>
        new(new FakeInvoiceRepository(invoices));

    [Fact]
    public void Submit_MovesDraftToPendingApproval()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.Draft);

        var result = ServiceFor(invoice).Submit("INV-1");

        Assert.Equal(InvoiceStatus.PendingApproval, result.Status);
    }

    [Fact]
    public void Submit_RejectsInvoiceThatIsNotADraft()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.Approved));

        Assert.Throws<InvalidOperationException>(() => service.Submit("INV-1"));
    }

    [Fact]
    public void Approve_SetsStatusApproverAndTimestamp()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval, amount: 100m));

        var result = service.Approve("INV-1", "demo.manager", "Manager");

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("demo.manager", result.DecidedBy);
        Assert.NotNull(result.DecidedAt);
    }

    [Fact]
    public void Approve_RejectsInvoiceThatIsStillADraft()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.Draft));

        Assert.Throws<InvalidOperationException>(() =>
            service.Approve("INV-1", "demo.manager", "Manager"));
    }

    [Fact]
    public void Approve_ThrowsWhenInvoiceDoesNotExist()
    {
        var service = ServiceFor();

        Assert.Throws<KeyNotFoundException>(() =>
            service.Approve("INV-404", "demo.manager", "Manager"));
    }

    [Fact]
    public void Reject_RecordsReasonAndApprover()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval));

        var result = service.Reject("INV-1", "demo.manager", "Duplicate invoice");

        Assert.Equal(InvoiceStatus.Rejected, result.Status);
        Assert.Equal("Duplicate invoice", result.DecisionNote);
    }

    [Fact]
    public void Reject_RequiresAReason()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval));

        Assert.Throws<ArgumentException>(() => service.Reject("INV-1", "demo.manager", "  "));
    }

    [Fact]
    public void Approve_Manager_AllowsAmountAtLimit()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval, ApprovalService.ManagerLimit));

        var result = service.Approve("INV-1", "demo.manager", "Manager");

        Assert.Equal(InvoiceStatus.Approved, result.Status);
    }

    [Fact]
    public void Approve_Manager_RejectsAboveLimit_WithoutChangingInvoice()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, ApprovalService.ManagerLimit + 0.01m);
        var service = ServiceFor(invoice);

        var error = Assert.Throws<ArgumentException>(() =>
            service.Approve("INV-1", "demo.manager", "Manager"));

        Assert.Contains("50,000", error.Message);
        Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
        Assert.Null(invoice.DecidedBy);
        Assert.Null(invoice.DecidedAt);
        Assert.Null(invoice.DecisionNote);
    }

    [Fact]
    public void Approve_Director_AllowsAmountAtLimit()
    {
        var service = ServiceFor(InvoiceWith("INV-1", InvoiceStatus.PendingApproval, ApprovalService.DirectorLimit));

        var result = service.Approve("INV-1", "demo.director", "Director");

        Assert.Equal(InvoiceStatus.Approved, result.Status);
    }

    [Fact]
    public void Approve_Director_AboveSoloLimit_RecordsFirstApprovalOnly()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, ApprovalService.DirectorLimit + 0.01m);
        var service = ServiceFor(invoice);

        var result = service.Approve("INV-1", "demo.director", "Director");

        Assert.Equal(InvoiceStatus.PendingApproval, result.Status);
        Assert.Equal("demo.director", result.FirstApprovedBy);
        Assert.NotNull(result.FirstApprovedAt);
        Assert.Null(result.DecidedBy);
    }

    [Fact]
    public void Approve_LegacyApproverRole_UsesManagerLimit()
    {
        var invoice = InvoiceWith("INV-1", InvoiceStatus.PendingApproval, ApprovalService.ManagerLimit + 1m);
        var service = ServiceFor(invoice);

        Assert.Throws<ArgumentException>(() =>
            service.Approve("INV-1", "demo.approver", "Approver"));
        Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
    }

    [Fact]
    public void Approve_HighValue_SecondDirector_CompletesApproval()
    {
        var invoice = InvoiceWith("INV-HV", InvoiceStatus.PendingApproval, 600_000m);
        var service = ServiceFor(invoice);

        service.Approve("INV-HV", "demo.manager", "Manager");
        var result = service.Approve("INV-HV", "demo.director", "Director");

        Assert.Equal(InvoiceStatus.Approved, result.Status);
        Assert.Equal("demo.manager", result.FirstApprovedBy);
        Assert.Equal("demo.director", result.DecidedBy);
    }

    [Fact]
    public void Approve_HighValue_SameUserCannotBeBothApprovers_WithoutChangingInvoice()
    {
        var invoice = InvoiceWith("INV-HV", InvoiceStatus.PendingApproval, 600_000m);
        var service = ServiceFor(invoice);
        service.Approve("INV-HV", "demo.director", "Director");

        var error = Assert.Throws<ArgumentException>(() =>
            service.Approve("INV-HV", "demo.director", "Director"));

        Assert.Contains("same user", error.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
        Assert.Equal("demo.director", invoice.FirstApprovedBy);
        Assert.Null(invoice.DecidedBy);
        Assert.Null(invoice.DecidedAt);
    }

    [Fact]
    public void Approve_HighValue_SecondApproverMustBeDirector_WithoutChangingInvoice()
    {
        var invoice = InvoiceWith("INV-HV", InvoiceStatus.PendingApproval, 600_000m);
        var service = ServiceFor(invoice);
        service.Approve("INV-HV", "demo.director", "Director");

        var error = Assert.Throws<ArgumentException>(() =>
            service.Approve("INV-HV", "demo.manager", "Manager"));

        Assert.Contains("second approver", error.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(InvoiceStatus.PendingApproval, invoice.Status);
        Assert.Equal("demo.director", invoice.FirstApprovedBy);
        Assert.Null(invoice.DecidedBy);
    }
}
