using SampleWpf.Core.Models;

namespace SampleWpf.Core.Contracts.Services;

public interface IMicrosoftGraphService
{
    Task<User> GetUserInfoAsync(string accessToken);

    Task<string> GetUserPhoto(string accessToken);
}
