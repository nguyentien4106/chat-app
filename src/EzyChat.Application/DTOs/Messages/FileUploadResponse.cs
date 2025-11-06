using EzyChat.Domain.Enums;

namespace EzyChat.Application.DTOs.Messages;

public class FileUploadResponse
{
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public MessageTypes MessageType { get; set; }
}

