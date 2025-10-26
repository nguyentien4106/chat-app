namespace ChatApp.Application.Queries.Messages.GetGroupMessages;

public class GetGroupMessageHandler: IQueryHandler<GetGroupMessagesQuery, List<MessageDto>>
{
    private readonly IChatAppDbContext _context;

    public GetGroupMessagesQueryHandler(IChatAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MessageDto>> Handle(GetGroupMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _context.Messages
            .Where(m => m.GroupId == request.GroupId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                SenderUsername = m.Sender.Username,
                GroupId = m.GroupId,
                CreatedAt = m.CreatedAt,
                IsRead = m.IsRead
            })
            .ToListAsync(cancellationToken);

        return messages;
    }
}
