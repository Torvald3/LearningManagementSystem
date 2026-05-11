namespace LMS.Media.Core.Configurations;

public class MediaStorageConfiguration
{
    public string Endpoint { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public string Bucket { get; set; } = string.Empty;

    public bool UseSsl { get; set; }

    public int PresignedUrlExpirationMinutes { get; set; } = 10;

    public int MaxFileSizeMb { get; set; } = 100;
}
