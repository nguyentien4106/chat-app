using ChatApp.Domain.Entities.Base;
using ChatApp.Domain.Enums;

namespace ChatApp.Domain.Entities;

public class GroupRequest : Entity<Guid>
{
    public Guid GroupId { get; set; }
    public Guid RequestedById { get; set; }
    public string? Description { get; set; }
    public GroupRequestTypes GroupRequestType { get; set; }
}