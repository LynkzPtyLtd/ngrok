using Cake.Common.Diagnostics;
using Cake.Common.Tools.GitVersion;
using Cake.Frosting;

namespace Lynkz.NGrok.Build;

[TaskName("GitVersion")]
public sealed class GitVersionTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var gitVersion = context.GitVersion(new GitVersionSettings
        {
            UpdateAssemblyInfo = true
        });

        context.AssemblyVersion ??= gitVersion.AssemblySemVer;
        context.Version ??= gitVersion.NuGetVersionV2;

        context.Information($"Version: {context.Version} Assembly Version: {context.AssemblyVersion}");
    }
}