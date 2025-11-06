
using EzyChat.Api.Controllers.Base;
using EzyChat.Application.Commands.Messages.SendMessage;
using EzyChat.Application.DTOs.Messages;
using EzyChat.Application.Interfaces;
using EzyChat.Application.Models;
using EzyChat.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController(IStorageService storageService, IMediator mediator) : AuthenticatedControllerBase
{
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
    private static readonly string[] AllowedFileExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip", ".rar" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private const long MaxImageSize = 5 * 1024 * 1024; // 5MB

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB
    public async Task<ActionResult<AppResponse<FileUploadResponse>>> UploadFile(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var isImage = AllowedImageExtensions.Contains(extension);
        var isAllowedFile = AllowedFileExtensions.Contains(extension);

        switch (isImage)
        {
            case false when !isAllowedFile:
                return BadRequest("File type not allowed");
            case true when file.Length > MaxImageSize:
                return BadRequest($"Image size must be less than {MaxImageSize / 1024 / 1024}MB");
            case false when file.Length > MaxFileSize:
                return BadRequest($"File size must be less than {MaxFileSize / 1024 / 1024}MB");
            default:
                try
                {
                    await using var stream = file.OpenReadStream();
                    var fileUrl = await storageService.UploadFileAsync(stream, file.FileName, file.ContentType);
                    var result = AppResponse<FileUploadResponse>.Success(new FileUploadResponse
                    {
                        FileUrl = fileUrl,
                        FileName = file.FileName,
                        FileType = file.ContentType,
                        FileSize = file.Length,
                        MessageType = isImage ? MessageTypes.Image : MessageTypes.File
                    });
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error uploading file: {ex.Message}");
                }

                break;
        }
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
    public async Task<ActionResult> DeleteFile(string fileUrl)
    {
        var decodedUrl = Uri.UnescapeDataString(fileUrl);
        var result = await storageService.DeleteFileAsync(decodedUrl);
        
        if (result)
        {
            return Ok();
        }
        
        return NotFound("File not found");
    }

}

