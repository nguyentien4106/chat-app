using EzyChat.Application.Exceptions;
using EzyChat.Application.Models;
using EzyChat.Domain.Exceptions;
using EzyChat.Domain.Repositories;

namespace EzyChat.Application.Commands.QuickMessages.DeleteQuickMessage;

public class DeleteQuickMessageCommandHandler(
    IRepository<QuickMessage> quickMessageRepository
) : ICommandHandler<DeleteQuickMessageCommand, AppResponse<bool>>
{
    public async Task<AppResponse<bool>> Handle(DeleteQuickMessageCommand request, CancellationToken cancellationToken)
    {
        var quickMessage = await quickMessageRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        
        if (quickMessage == null)
        {
            throw new NotFoundException("Quick message not found");
        }

        // Verify ownership
        if (quickMessage.UserId != request.UserId)
        {
            return AppResponse<bool>.Error("You do not have permission to delete this quick message");
        }

        await quickMessageRepository.DeleteAsync(quickMessage, cancellationToken);

        return AppResponse<bool>.Success(true);
    }
}
