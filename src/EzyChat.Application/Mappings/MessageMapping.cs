using EzyChat.Application.DTOs.Messages;

namespace EzyChat.Application.Mappings;

public class MessageMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Message, MessageDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Content, src => src.Content)
            .Map(dest => dest.MessageType, src => src.MessageType)
            .Map(dest => dest.FileUrl, src => src.FileUrl)
            .Map(dest => dest.FileName, src => src.FileName)
            .Map(dest => dest.FileType, src => src.FileType)
            .Map(dest => dest.FileSize, src => src.FileSize)
            .Map(dest => dest.SenderId, src => src.SenderId)
            .Map(dest => dest.ConversationId, src => src.ConversationId)
            .Map(dest => dest.GroupId, src => src.GroupId)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.IsRead, src => src.IsRead)
            .Map(dest => dest.SenderUserName, src => src.SenderUserName)
            .Ignore(dest => dest.SenderFullName)
            .Ignore(dest => dest.ReceiverId)
            .Ignore(dest => dest.GroupName)
            .Ignore(dest => dest.IsNewConversation)
            .MaxDepth(2); // Prevent circular reference issues

        config.NewConfig<Domain.Entities.PinMessage, PinMessageDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.MessageId, src => src.MessageId)
            .Map(dest => dest.PinnedByUserId, src => src.PinnedByUserId)
            .Map(dest => dest.PinnedByUserName, src => src.PinnedByUser.FirstName + " " + src.PinnedByUser.LastName)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.ConversationId, src => src.ConversationId)
            .Map(dest => dest.GroupId, src => src.GroupId)
            .Map(dest => dest.Message, src => new MessageDto
            {
                Id = src.Message.Id,
                Content = src.Message.Content,
                MessageType = src.Message.MessageType,
                FileUrl = src.Message.FileUrl,
                FileName = src.Message.FileName,
                FileType = src.Message.FileType,
                FileSize = src.Message.FileSize,
                SenderId = src.Message.SenderId,
                SenderUserName = src.Message.SenderUserName,
                ConversationId = src.Message.ConversationId,
                GroupId = src.Message.GroupId,
                CreatedAt = src.Message.CreatedAt,
                IsRead = src.Message.IsRead,
                SenderFullName = src.Message.Sender != null ? src.Message.Sender.FirstName + " " + src.Message.Sender.LastName : string.Empty
            });
    }
}
