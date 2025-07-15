using MediatR;
using Microsoft.AspNetCore.Http;

public record UploadProfilePictureCommand(string Email, IFormFile File) : IRequest<UploadProfilePictureResponse>;
