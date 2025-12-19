using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Application.Exceptions;
using EzyChat.Application.Models;
using EzyChat.Domain.Entities;
using EzyChat.Domain.Repositories;
using Mapster;

namespace EzyChat.Application.Commands.QuickMessages.CreateQuickMessage;

public class CreateQuickMessageCommandHandler(
    IQuickMessageRepository quickMessageRepository
) : ICommandHandler<CreateQuickMessageCommand, AppResponse<QuickMessageDto>>
{
    public async Task<AppResponse<QuickMessageDto>> Handle(CreateQuickMessageCommand request, CancellationToken cancellationToken)
    {
        // Check if key already exists for this user
        var keyExists = await quickMessageRepository.KeyExistsAsync(request.Key, request.UserId, cancellationToken);
        if (keyExists)
        {
            throw new BadRequestException($"Quick message with key '{request.Key}' already exists");
        }

        var quickMessage = new QuickMessage
        {
            Content = request.Content,
            Key = request.Key,
            UserId = request.UserId
        };

        var created = await quickMessageRepository.AddAsync(quickMessage, cancellationToken);
        var dto = created.Adapt<QuickMessageDto>();

        return AppResponse<QuickMessageDto>.Success(dto);
    }
}
