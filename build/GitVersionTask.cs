using Cake.Common.Build;
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

        var buildSystem = context.BuildSystem();
        if (buildSystem.IsRunningOnGitHubActions)
        {
            context.Version = gitVersion.NuGetVersionV2;
        }
        else
        {
            context.Version ??= gitVersion.NuGetVersionV2;
        }
        
        context.AssemblyVersion ??= gitVersion.AssemblySemVer;

        context.Information($"Version: {context.Version} Assembly Version: {context.AssemblyVersion}");
    }
}