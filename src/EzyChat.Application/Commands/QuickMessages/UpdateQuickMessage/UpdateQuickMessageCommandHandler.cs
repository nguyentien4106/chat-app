using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Application.Exceptions;
using EzyChat.Application.Models;
using EzyChat.Domain.Exceptions;
using EzyChat.Domain.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace EzyChat.Application.Commands.QuickMessages.UpdateQuickMessage;

public class UpdateQuickMessageCommandHandler(
    IRepository<QuickMessage> quickMessageRepository
) : ICommandHandler<UpdateQuickMessageCommand, AppResponse<QuickMessageDto>>
{
    public async Task<AppResponse<QuickMessageDto>> Handle(UpdateQuickMessageCommand request, CancellationToken cancellationToken)
    {
        var quickMessage = await quickMessageRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        
        if (quickMessage == null)
        {
            throw new NotFoundException("Quick message not found");
        }

        // Verify ownership
        if (quickMessage.UserId != request.UserId)
        {
            return AppResponse<QuickMessageDto>.Error("You do not have permission to update this quick message");
        }

        // Check if new key conflicts with another quick message
        if (quickMessage.Key != request.Key)
        {
            var keyExists = await quickMessageRepository.GetQuery()
                                    .AsNoTracking()
                                    .AnyAsync(qm => qm.Key == request.Key && qm.UserId == request.UserId, cancellationToken);
            if (keyExists)
            {
                return AppResponse<QuickMessageDto>.Error($"Quick message with key '{request.Key}' already exists");
            }
        }

        quickMessage.Content = request.Content;
        quickMessage.Key = request.Key;

        await quickMessageRepository.UpdateAsync(quickMessage, cancellationToken);
        var dto = quickMessage.Adapt<QuickMessageDto>();

        return AppResponse<QuickMessageDto>.Success(dto);
    }
}
