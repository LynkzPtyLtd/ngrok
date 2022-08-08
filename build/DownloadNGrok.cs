using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Net;
using Cake.Compression;
using Cake.Frosting;
using Mono.Unix;

namespace Lynkz.NGrok.Build;

[TaskName("DownloadNGrok")]
public sealed class DownloadNGrok : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (!context.FileExists("../src/binaries/win-x64/ngrok.exe"))
        {
            context.Information("Downloading Windows version of NGrok");
            var windows64Bit =
                context.DownloadFile("https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-windows-amd64.zip");
            context.Unzip(windows64Bit, "../src/binaries/win-x64");
        }
        else
        {
            context.Information("Windows binary already found");
        }

        if (!context.FileExists("../src/binaries/linux-x64/ngrok"))
        {
            context.Information("Downloading Linux version of NGrok");
            var linux64Bit =
                context.DownloadFile("https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-linux-amd64.tgz");
            context.GZipUncompress(linux64Bit, "../src/binaries/linux-x64");

            var fileInfo = new UnixFileInfo("../src/binaries/linux-x64/ngrok");
            fileInfo.FileAccessPermissions |= FileAccessPermissions.UserExecute;
        }
        else
        {
            context.Information("Linux binary already found");
        }
        
        context.Information("NGrok setup complete.");
    }
}