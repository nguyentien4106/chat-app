namespace EzyChat.Application.Settings;

public class FileUploadSettings
{
    public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB default
    public long MaxImageSize { get; set; } = 5 * 1024 * 1024; // 5MB default
    public string[] AllowedImageExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
    public string[] AllowedFileExtensions { get; set; } = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".zip", ".rar" };
}
