using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;

namespace EzyChat.Application.Commands.Files.DeleteFile;

public class DeleteFileCommandHandler(IStorageService storageService) 
    : ICommandHandler<DeleteFileCommand, AppResponse<bool>>
{
    public async Task<AppResponse<bool>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var result = await storageService.DeleteFileAsync(request.FileUrl);
        
        if (result)
        {
            return AppResponse<bool>.Success(true);
        }
        
        return AppResponse<bool>.Error("File not found");
    }
}
