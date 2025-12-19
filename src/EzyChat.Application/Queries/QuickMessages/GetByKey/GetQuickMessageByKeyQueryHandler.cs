using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Application.Models;
using EzyChat.Domain.Exceptions;
using EzyChat.Domain.Repositories;
using Mapster;

namespace EzyChat.Application.Queries.QuickMessages;

public class GetQuickMessageByKeyQueryHandler(
    IQuickMessageRepository quickMessageRepository
) : IQueryHandler<GetQuickMessageByKeyQuery, AppResponse<QuickMessageDto>>
{
    public async Task<AppResponse<QuickMessageDto>> Handle(GetQuickMessageByKeyQuery request, CancellationToken cancellationToken)
    {
        var quickMessage = await quickMessageRepository.GetByKeyAsync(request.Key, request.UserId, cancellationToken);
        
        if (quickMessage == null)
        {
            throw new NotFoundException($"Quick message with key '{request.Key}' not found");
        }

        var dto = quickMessage.Adapt<QuickMessageDto>();

        return AppResponse<QuickMessageDto>.Success(dto);
    }
}
