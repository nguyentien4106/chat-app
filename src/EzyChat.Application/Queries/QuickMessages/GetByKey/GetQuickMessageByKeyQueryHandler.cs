using EzyChat.Application.DTOs.QuickMessages;
using EzyChat.Domain.Exceptions;

namespace EzyChat.Application.Queries.QuickMessages;

public class GetQuickMessageByKeyQueryHandler(
    IRepository<QuickMessage> quickMessageRepository
) : IQueryHandler<GetQuickMessageByKeyQuery, AppResponse<List<QuickMessageDto>>>
{
    public async Task<AppResponse<List<QuickMessageDto>>> Handle(GetQuickMessageByKeyQuery request, CancellationToken cancellationToken)
    {
        var quickMessages = await quickMessageRepository.GetAllAsync(
            qm => qm.Key.StartsWith(request.Key) && qm.UserId == request.UserId,
            cancellationToken: cancellationToken
        );
        
        var dto = quickMessages.Adapt<List<QuickMessageDto>>();

        return AppResponse<List<QuickMessageDto>>.Success(dto);
    }
}
