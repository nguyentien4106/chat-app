using EzyChat.Application.DTOs.Common;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EzyChat.Application.Commands.Groups.CreateGroup;


public class CreateGroupHandler(
    IRepository<Group> groupRepository,
    IRepository<GroupMember> groupMemberRepository,
    IUserRepository userRepository
    ) : ICommandHandler<CreateGroupCommand, AppResponse<GroupDto>>
{
    public async Task<AppResponse<GroupDto>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        // Validate that the user exists
        var user = await userRepository.GetByIdAsync(request.CreatedById, cancellationToken: cancellationToken);
        if (user == null)
        {
            return AppResponse<GroupDto>.Fail($"User with ID {request.CreatedById} not found");
        }

        var group = new Group
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedById = request.CreatedById,
            CreatedAt = DateTime.UtcNow
        };

        var member = new GroupMember
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = request.CreatedById,
            JoinedAt = DateTime.UtcNow,
            IsAdmin = true
        };

        await groupRepository.AddAsync(group, cancellationToken);
        await groupMemberRepository.AddAsync(member, cancellationToken);

        var groupDto = new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            CreatedById = group.CreatedById,
            CreatedAt = group.CreatedAt,
            MemberCount = group.Members.Count
        };

        return AppResponse<GroupDto>.Success(groupDto);
    }
}