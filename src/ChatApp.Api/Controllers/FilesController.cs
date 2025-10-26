<<<<<<< HEAD
namespace ChatApp.Api.Controllers;

public class FilesController
{
    
}
=======
using System.Security.Claims;
using ChatApp.Api.Controllers.Base;
using ChatApp.Application.Commands.Messages.SendMessage;
using ChatApp.Application.DTOs.Common;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Models;
using ChatApp.Domain.Enums;

namespace ChatApp.Api.Controllers;

// API/Controllers/FilesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : AuthenticatedControllerBase
{
    private readonly IStorageService _storageService;
    private readonly IMediator _mediator;
    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
    private static readonly string[] AllowedFileExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip", ".rar" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private const long MaxImageSize = 5 * 1024 * 1024; // 5MB

    public FilesController(IStorageService storageService, IMediator mediator)
    {
        _storageService = storageService;
        _mediator = mediator;
    }

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
                    var fileUrl = await _storageService.UploadFileAsync(stream, file.FileName, file.ContentType);
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
            ReceiverId = request.ReceiverId,
            GroupId = request.GroupId,
            Content = request.Content,
            Type = request.Type,
            FileUrl = request.FileUrl,
            FileName = request.FileName,
            FileType = request.FileType,
            FileSize = request.FileSize
        };

        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpDelete("{fileUrl}")]
    public async Task<ActionResult> DeleteFile(string fileUrl)
    {
        var decodedUrl = Uri.UnescapeDataString(fileUrl);
        var result = await _storageService.DeleteFileAsync(decodedUrl);
        
        if (result)
        {
            return Ok();
        }
        
        return NotFound("File not found");
    }

}

public class FileUploadResponse
{
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public MessageTypes MessageType { get; set; }
}

public class SendFileMessageRequest
{
    public Guid? ReceiverId { get; set; }
    public Guid? GroupId { get; set; }
    public string? Content { get; set; }
    public MessageTypes Type { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}
>>>>>>> a957673 (initial)
