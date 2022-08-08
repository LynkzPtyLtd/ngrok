using System;
using Cake.Frosting;

namespace Lynkz.NGrok.Build;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .InstallTool(new Uri("dotnet:?package=GitVersion.Tool"))
            .InstallTool(new Uri("nuget:?package=Cake.Compression&version=0.3.0"))
            .UseContext<BuildContext>()
            .Run(args);
    }
}