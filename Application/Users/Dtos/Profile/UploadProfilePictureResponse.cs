public class UploadProfilePictureResponse
{
    public string Url { get; set; } = string.Empty;
    public string Message { get; set; } = "Profile picture uploaded successfully.";
}

public class ErrorResponse
{
    public string Status { get; set; } = "error";
    public string Message { get; set; } = "An error occurred while uploading the profile picture.";
}