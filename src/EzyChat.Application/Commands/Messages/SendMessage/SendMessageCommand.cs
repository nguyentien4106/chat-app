using EzyChat.Application.DTOs.Common;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Models;
using EzyChat.Domain.Enums;

namespace EzyChat.Application.Commands.Messages.SendMessage;

public class SendMessageCommand : ICommand<AppResponse<MessageDto>>
{
    public Guid SenderId { get; set; }
    public Guid? ReceiverId { get; set; }
    public Guid? ConversationId { get; set; }
    public Guid? GroupId { get; set; }
    public string? Content { get; set; } = string.Empty;
    public MessageTypes MessageType { get; set; } = MessageTypes.Text;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}