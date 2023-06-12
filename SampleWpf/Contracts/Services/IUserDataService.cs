using SampleWpf.ViewModels;

namespace SampleWpf.Contracts.Services;

public interface IUserDataService
{
    event EventHandler<UserViewModel> UserDataUpdated;

    void Initialize();

    UserViewModel GetUser();
}
