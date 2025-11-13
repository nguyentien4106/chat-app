using EzyChat.Application.DTOs.Messages;

namespace EzyChat.Application.Commands.Files.UploadFile;

public class UploadFileCommand : ICommand<AppResponse<FileUploadResponse>>
{
    public Stream FileStream { get; set; } = null!;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public bool IsImage { get; set; }
}
