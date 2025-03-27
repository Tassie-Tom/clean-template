namespace Api.SharedKernel;

public abstract class AuditableEntity : Entity
{
    protected AuditableEntity(Guid id) : base(id)
    {
    }

    // Parameter-less constructor for EF Core
    protected AuditableEntity() : base()
    {
    }

    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime CreatedOnUtc { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedOnUtc { get; private set; }

    // For soft delete functionality
    public bool IsDeleted { get; private set; }
    public string? DeletedBy { get; private set; }
    public DateTime? DeletedOnUtc { get; private set; }

    public void SetCreatedBy(string createdBy, DateTime createdOnUtc)
    {
        CreatedBy = createdBy;
        CreatedOnUtc = createdOnUtc;
    }

    public void SetModifiedBy(string modifiedBy, DateTime modifiedOnUtc)
    {
        LastModifiedBy = modifiedBy;
        LastModifiedOnUtc = modifiedOnUtc;
    }

    public void SetDeleted(string deletedBy, DateTime deletedOnUtc)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedOnUtc = deletedOnUtc;
    }
}