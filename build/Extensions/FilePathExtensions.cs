using System;
using Cake.Core.IO;

namespace Lynkz.NGrok.Build.Extensions;

public static class FilePathExtensions
{
    public static bool IsBuildProject(this FilePath filePath, BuildContext context)
    {
        return filePath.GetFilenameWithoutExtension().Segments[0]
            .Equals(context.ExecutingAssemblyName, StringComparison.Ordinal);
    }
}