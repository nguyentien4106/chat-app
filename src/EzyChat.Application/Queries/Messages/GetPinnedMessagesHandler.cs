using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EzyChat.Application.Queries.Messages;

public class GetPinnedMessagesHandler(
    IRepository<PinMessage> repository
    ) : IQueryHandler<GetPinnedMessagesQuery, AppResponse<List<PinMessageDto>>>
{
    public async Task<AppResponse<List<PinMessageDto>>> Handle(GetPinnedMessagesQuery request, CancellationToken cancellationToken)
    {
        // Validate that either ConversationId or GroupId is provided, but not both
        if (request.ConversationId == null && request.GroupId == null)
        {
            throw new BadRequestException("Either ConversationId or GroupId must be provided");
        }
        
        if (request.ConversationId != null && request.GroupId != null)
        {
            throw new BadRequestException("Cannot provide both ConversationId and GroupId");
        }

        IEnumerable<PinMessage> pinnedMessages;
        
        if (request.ConversationId != null)
        {
            pinnedMessages = await repository.GetQuery()
                .AsNoTracking()
                .Where(pm => pm.ConversationId == request.ConversationId.Value)
                .Include(pm => pm.Message)
                    .ThenInclude(m => m.Sender)
                .Include(pm => pm.PinnedByUser)
                .OrderByDescending(pm => pm.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        else if (request.GroupId != null)
        {
            pinnedMessages = await repository.GetQuery()
                .AsNoTracking()
                .Where(pm => pm.GroupId == request.GroupId.Value)
                .Include(pm => pm.Message)
                    .ThenInclude(m => m.Sender)
                .Include(pm => pm.PinnedByUser)
                .OrderByDescending(pm => pm.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        else
        {
            pinnedMessages = [];
        }

        var pinnedMessageDtos = pinnedMessages.Adapt<List<PinMessageDto>>();

        return AppResponse<List<PinMessageDto>>.Success(pinnedMessageDtos);
    }
}
