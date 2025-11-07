
using EzyChat.Api.Controllers.Base;
using EzyChat.Application.Commands.Groups.AddUserToGroup;
using EzyChat.Application.Commands.Groups.CreateGroup;
using EzyChat.Application.Commands.Groups.DeleteGroup;
using EzyChat.Application.Commands.Groups.GenerateLink;
using EzyChat.Application.Commands.Groups.JoinGroup;
using EzyChat.Application.Commands.Groups.LeaveGroup;
using EzyChat.Application.Commands.Groups.RemoveMember;
using EzyChat.Application.DTOs.Common;
using EzyChat.Application.DTOs.Groups;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Models;
using EzyChat.Application.Queries.Groups.GetGroupInfo;
using EzyChat.Application.Queries.Groups.GetGroupMessages;
using EzyChat.Application.Queries.Groups.GetUserGroups;
using Microsoft.AspNetCore.Authorization;

namespace EzyChat.Api.Controllers;

// API/Controllers/GroupsController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController(IMediator mediator) : AuthenticatedControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AppResponse<PagedResult<GroupDto>>>> GetUserGroups([FromQuery] PaginationRequest request)
    {
        var query = new GetUserGroupsQuery
        {
            UserId = CurrentUserId,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortOrder = request.SortOrder
        };
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{groupId}/messages")]
    public async Task<ActionResult<AppResponse<PagedResult<MessageDto>>>> GetGroupMessages(Guid groupId, [FromQuery] DateTime? beforeDateTime)
    {
        var query = new GetGroupMessagesQuery
        {
            GroupId = groupId,
            UserId = CurrentUserId,
            BeforeDateTime = beforeDateTime ?? DateTime.Now
        };
        var response = await mediator.Send(query);
        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<AppResponse<GroupDto>>> CreateGroup([FromBody] CreateGroupRequest request)
    {
        var userId = CurrentUserId;
        var command = new CreateGroupCommand
        {
            Name = request.Name,
            Description = request.Description,
            CreatedById = userId
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("{groupId}/members")]
    public async Task<ActionResult<AppResponse<Unit>>> AddMember(Guid groupId, [FromBody] AddMemberRequest request)
    {
        var command = new AddMemberToGroupCommand
        {
            GroupId = groupId,
            UserName = request.UserName,
            AddedById = CurrentUserId
        };

        return Ok(await mediator.Send(command));
    }

    [HttpPost("{groupId}/invite")]
    public async Task<ActionResult<AppResponse<InviteLinkResponse>>> GenerateInviteLink(Guid groupId)
    {
        var userId = CurrentUserId;
        var command = new GenerateLinkCommand()
        {
            GroupId = groupId,
            RequestedById = userId
        };

        var response = await mediator.Send(command);
        
        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        var inviteLinkResponse = AppResponse<InviteLinkResponse>.Success(new InviteLinkResponse { InviteCode = response.Data });
        return Ok(inviteLinkResponse);
    }

    [HttpPost("join/{inviteCode}")]
    public async Task<ActionResult<AppResponse<Unit>>> JoinByInvite(string inviteCode)
    {
        var userId = CurrentUserId;
        var command = new JoinGroupByInviteCommand
        {
            InviteCode = inviteCode,
            UserId = userId
        };

        return Ok(await mediator.Send(command));
    }

    [HttpGet("{groupId}/info")]
    public async Task<ActionResult<AppResponse<GroupDto>>> GetGroupInfo(Guid groupId)
    {
        var userId = CurrentUserId;
        var query = new GetGroupInfoQuery
        {
            GroupId = groupId,
            UserId = userId
        };

        return Ok(await mediator.Send(query));
    }
    
    [HttpDelete("{groupId}/members")]
    public async Task<ActionResult<AppResponse<Unit>>> RemoveMember(Guid groupId, [FromBody] RemoveMemberRequest request)
    {
        var command = new RemoveMemberFromGroupCommand
        {
            GroupId = groupId,
            UserId = request.UserId,
            RemovedById = CurrentUserId
        };

        return Ok(await mediator.Send(command));
    }

    [HttpPost("{groupId}/leave")]
    public async Task<ActionResult<AppResponse<Unit>>> LeaveGroup(Guid groupId)
    {
        var userId = CurrentUserId;
        var command = new LeaveGroupCommand
        {
            GroupId = groupId,
            UserId = userId
        };

        return Ok(await mediator.Send(command));
    }

    [HttpDelete("{groupId}")]
    public async Task<ActionResult<AppResponse<Unit>>> DeleteGroup(Guid groupId)
    {
        var userId = CurrentUserId;
        var command = new DeleteGroupCommand
        {
            GroupId = groupId,
            UserId = userId
        };

        return Ok(await mediator.Send(command));
    }
}