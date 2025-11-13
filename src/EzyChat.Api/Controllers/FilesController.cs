
using EzyChat.Api.Controllers.Base;
using EzyChat.Application.Commands.Files.DeleteFile;
using EzyChat.Application.Commands.Files.UploadFile;
using EzyChat.Application.Commands.Messages.SendMessage;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Models;
using EzyChat.Application.Settings;
using Microsoft.AspNetCore.Authorization;

namespace EzyChat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController(IMediator mediator, FileUploadSettings fileUploadSettings) : AuthenticatedControllerBase
{
    [HttpPost("upload")]
    public async Task<ActionResult<AppResponse<FileUploadResponse>>> UploadFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return AppResponse<FileUploadResponse>.Error("No file uploaded");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var isImage = fileUploadSettings.AllowedImageExtensions.Contains(extension);
        var isAllowedFile = fileUploadSettings.AllowedFileExtensions.Contains(extension);

        if (!isImage && !isAllowedFile)
        {
            return AppResponse<FileUploadResponse>.Error("File type not allowed. " + 
                "Allowed image types: " + string.Join(", ", fileUploadSettings.AllowedImageExtensions) + "\n. " +
                "Allowed file types: " + string.Join(", ", fileUploadSettings.AllowedFileExtensions));
        }

        if (isImage && file.Length > fileUploadSettings.MaxImageSize)
        {
            return AppResponse<FileUploadResponse>.Error($"Image size must be less than {fileUploadSettings.MaxImageSize / 1024 / 1024}MB");
        }

        if (!isImage && file.Length > fileUploadSettings.MaxFileSize)
        {
            return AppResponse<FileUploadResponse>.Error($"File size must be less than {fileUploadSettings.MaxFileSize / 1024 / 1024}MB");
        }

        await using var stream = file.OpenReadStream();
        var command = new UploadFileCommand
        {
            FileStream = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            IsImage = isImage
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("send")]
    public async Task<ActionResult<AppResponse<MessageDto>>> SendFileMessage([FromBody] SendFileMessageRequest request)
    {
        var userId = CurrentUserId;
        
        var command = new SendMessageCommand
        {
            SenderId = userId,
            ConversationId = request.ConversationId,
            GroupId = request.GroupId,
            Content = request.Content,
            MessageType = request.Type,
            FileUrl = request.FileUrl,
            FileName = request.FileName,
            FileType = request.FileType,
            FileSize = request.FileSize
        };

        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{fileUrl}")]
    public async Task<ActionResult<AppResponse<bool>>> DeleteFile(string fileUrl)
    {
        var decodedUrl = Uri.UnescapeDataString(fileUrl);
        var command = new DeleteFileCommand { FileUrl = decodedUrl };
        var response = await mediator.Send(command);
        
        if (!response.IsSuccess)
        {
            return NotFound(response);
        }
        
        return Ok(response);
    }
}

