using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Application.Models;
using EzyChat.Domain.Repositories;
using Mapster;

namespace EzyChat.Application.Queries.QuickMessages;

public class GetUserQuickMessagesQueryHandler(
    IQuickMessageRepository quickMessageRepository
) : IQueryHandler<GetUserQuickMessagesQuery, AppResponse<List<QuickMessageDto>>>
{
    public async Task<AppResponse<List<QuickMessageDto>>> Handle(GetUserQuickMessagesQuery request, CancellationToken cancellationToken)
    {
        var quickMessages = await quickMessageRepository.GetUserQuickMessagesAsync(request.UserId, cancellationToken);
        var dtos = quickMessages.Adapt<List<QuickMessageDto>>();

        return AppResponse<List<QuickMessageDto>>.Success(dtos);
    }
}
