using EzyChat.Application.DTOs.QuickMessages;

namespace EzyChat.Application.Queries.QuickMessages;

public class GetUserQuickMessagesQueryHandler(
    IRepository<QuickMessage> quickMessageRepository
) : IQueryHandler<GetUserQuickMessagesQuery, AppResponse<List<QuickMessageDto>>>
{
    public async Task<AppResponse<List<QuickMessageDto>>> Handle(GetUserQuickMessagesQuery request, CancellationToken cancellationToken)
    {
        var quickMessages = await quickMessageRepository.GetAllAsync(
            qm => qm.UserId == request.UserId,
            cancellationToken: cancellationToken
        );
        var dtos = quickMessages.Adapt<List<QuickMessageDto>>();

        return AppResponse<List<QuickMessageDto>>.Success(dtos);
    }
}
