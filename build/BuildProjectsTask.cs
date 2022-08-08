using System;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Build;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Frosting;
using Lynkz.NGrok.Build.Extensions;

namespace Lynkz.NGrok.Build;

[TaskName("BuildProjects")]
[IsDependentOn(typeof(CleanProjectsTask))]
[IsDependentOn(typeof(GitVersionTask))]
[IsDependentOn(typeof(DownloadNGrok))]
[IsDependentOn(typeof(RestoreProjectsTask))]
public sealed class BuildProjectsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var buildSettings = new DotNetBuildSettings
        {
            Configuration = context.MsBuildConfiguration,
            NoRestore = true,
            MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("Version", context.Version)
                .WithProperty("AssemblyVersion", context.AssemblyVersion)
                .WithProperty("FileVersion", context.AssemblyVersion)
                .WithProperty("PackageVersion", context.Version)
        };
        
        var projects = context.GetFiles("../**/*.csproj");
        foreach(var project in projects)
        {
            if (project.IsBuildProject(context))
            {
                continue;
            }

            context.Information($"Building {project}");
            context.DotNetBuild(project.ToString(),
                buildSettings);
        }
    }
}