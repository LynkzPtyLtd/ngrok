using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Frosting;
using Cake.Incubator.Project;
using Lynkz.NGrok.Build.Extensions;

namespace Lynkz.NGrok.Build;

[TaskName("PackProjects")]
[IsDependentOn(typeof(ExecuteTestsTask))]
public class PackProjectTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings = new DotNetPackSettings
        {
            Configuration = context.MsBuildConfiguration,
            OutputDirectory = "../.artifacts",
            NoBuild = true,
            NoRestore = true,
            MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("PackageVersion", context.Version)
                .WithProperty("Version", context.Version)
        };
        
        var projects = context.GetFiles("../**/*.csproj");
        foreach (var project in projects)
        {
            if (project.IsBuildProject(context))
            {
                continue;
            }

            var projectResult = context.ParseProject(project, context.MsBuildConfiguration);
            var result = projectResult.GetProjectProperty("IsPackable");
            if (!bool.TryParse(result, out var isPackable) || !isPackable)
            {
                continue;
            }
            
            context.Information($"Pack {project}");
            context.DotNetPack(project.ToString(), settings);
        }
    }
}
