using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Frosting;

namespace Lynkz.NGrok.Build;

[TaskName("PublishArtifacts")]
[IsDependentOn(typeof(PackProjectTask))]
public class PublishArtifactsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var buildSystem = context.BuildSystem();
        if (!buildSystem.GitHubActions.IsRunningOnGitHubActions)
        {
            return;
        }
        
        foreach(var file in context.GetFiles("../.artifacts/*.nupkg"))
        {
            context.Information("Publishing {0}...", file.GetFilename().FullPath);
            context.DotNetNuGetPush(file, new DotNetNuGetPushSettings
            {
                ApiKey = context.EnvironmentVariable("NUGET_API_KEY"),
                Source = "https://api.nuget.org/v3/index.json"
            });

            context.DotNetNuGetPush(file, new DotNetNuGetPushSettings
            {
                ApiKey = context.EnvironmentVariable("GITHUB_TOKEN"),
                Source = "https://nuget.pkg.github.com/LynkzPtyLtd/index.json"
            });
        }
    }
}
