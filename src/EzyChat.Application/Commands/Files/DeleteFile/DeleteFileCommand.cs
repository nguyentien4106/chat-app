using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.Files.DeleteFile;

public class DeleteFileCommand : ICommand<AppResponse<bool>>
{
    public string FileUrl { get; set; } = string.Empty;
}
