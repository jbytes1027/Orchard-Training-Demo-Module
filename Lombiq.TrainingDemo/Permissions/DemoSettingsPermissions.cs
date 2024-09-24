using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.TrainingDemo.Permissions;

public sealed class DemoSettingsPermissions : IPermissionProvider
{
    public static readonly Permission ManageDemoSettings = new(
        nameof(ManageDemoSettings),
        "Manage demo settings.");

    public Task<IEnumerable<Permission>> GetPermissionsAsync() =>
        Task.FromResult(new[]
        {
            ManageDemoSettings,
        }
        .AsEnumerable());

    public IEnumerable<PermissionStereotype> GetDefaultStereotypes() =>
        [
            new PermissionStereotype
            {
                Name = "Administrator",
                Permissions = [ManageDemoSettings],
            },
        ];
}
