
using System.Security.Claims;
using ChatApp.Api.Controllers.Base;
using ChatApp.Api.Models.Requests;
using ChatApp.Application.Commands.Groups.AddUserToGroup;
using ChatApp.Application.Commands.Groups.CreateGroup;
using ChatApp.Application.Commands.Groups.DeleteGroup;
using ChatApp.Application.Commands.Groups.GenerateLink;
using ChatApp.Application.Commands.Groups.JoinGroup;
using ChatApp.Application.Commands.Groups.LeaveGroup;
using ChatApp.Application.Commands.Groups.RemoveMember;
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Models;
using ChatApp.Application.Queries.Groups.GetGroupInfo;
using ChatApp.Application.Queries.Groups.GetUserGroups;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace ChatApp.Api.Controllers;


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