using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Application.Exceptions;
using EzyChat.Application.Models;
using EzyChat.Domain.Exceptions;
using EzyChat.Domain.Repositories;
using Mapster;

namespace EzyChat.Application.Commands.QuickMessages.UpdateQuickMessage;

public class UpdateQuickMessageCommandHandler(
    IQuickMessageRepository quickMessageRepository
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
            throw new BadRequestException("You do not have permission to update this quick message");
        }

        // Check if new key conflicts with another quick message
        if (quickMessage.Key != request.Key)
        {
            var keyExists = await quickMessageRepository.KeyExistsAsync(request.Key, request.UserId, cancellationToken);
            if (keyExists)
            {
                throw new BadRequestException($"Quick message with key '{request.Key}' already exists");
            }
        }

        quickMessage.Content = request.Content;
        quickMessage.Key = request.Key;

        await quickMessageRepository.UpdateAsync(quickMessage, cancellationToken);
        var dto = quickMessage.Adapt<QuickMessageDto>();

        return AppResponse<QuickMessageDto>.Success(dto);
    }
}
