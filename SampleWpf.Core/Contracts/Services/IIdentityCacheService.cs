using System.Text;

using Microsoft.Identity.Client;

namespace SampleWpf.Core.Contracts.Services;

public interface IIdentityCacheService
{
    void SaveMsalToken(byte[] token);

    byte[] ReadMsalToken();
}
