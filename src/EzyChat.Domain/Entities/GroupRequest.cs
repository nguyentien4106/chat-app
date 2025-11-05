using EzyChat.Domain.Entities.Base;
using EzyChat.Domain.Enums;

namespace EzyChat.Domain.Entities;

public class GroupRequest : Entity<Guid>
{
    public Guid GroupId { get; set; }
    public Guid RequestedById { get; set; }
    public string? Description { get; set; }
    public GroupRequestTypes GroupRequestType { get; set; }
}