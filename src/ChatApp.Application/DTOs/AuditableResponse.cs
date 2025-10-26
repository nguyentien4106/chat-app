namespace ChatApp.Application.DTOs;

public record AuditableResponse(
    bool IsActive,
    DateTime Created,
    string? CreatedBy = null,
    DateTime? LastModified = null,
    string? LastModifiedBy = null,
    DateTime? Deleted = null,
    string? DeletedBy = null
);