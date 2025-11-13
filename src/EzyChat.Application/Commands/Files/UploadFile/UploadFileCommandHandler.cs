using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Enums;

namespace EzyChat.Application.Commands.Files.UploadFile;

public class UploadFileCommandHandler(IStorageService storageService) 
    : ICommandHandler<UploadFileCommand, AppResponse<FileUploadResponse>>
{
    public async Task<AppResponse<FileUploadResponse>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var fileUrl = await storageService.UploadFileAsync(request.FileStream, request.FileName, request.ContentType);
            
            var response = new FileUploadResponse
            {
                FileUrl = fileUrl,
                FileName = request.FileName,
                FileType = request.ContentType,
                FileSize = request.FileSize,
                MessageType = request.IsImage ? MessageTypes.Image : MessageTypes.File
            };
            
            return AppResponse<FileUploadResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return AppResponse<FileUploadResponse>.Error($"Error uploading file: {ex.Message}");
        }
    }
}
