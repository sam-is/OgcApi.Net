using OgcApi.Net.SpatiaLite.Tests.Utils;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace OgcApi.Net.SpatiaLite.Tests;

public class DatabaseFixture
{
    public DatabaseFixture()
    {
        InitSpatiaLite();

        DatabaseUtils.RecreateDatabase();
    }

    private void InitSpatiaLite()
    {
        string baseDir = AppContext.BaseDirectory;

        // add native library location to the path variable so the native libraries are correctly loaded
        // this is only neccesary when running the tests
        string rid = RuntimeInformation.RuntimeIdentifier;
        string nativeDir = Path.Combine(baseDir, "runtimes", rid, "native");

        if (Directory.Exists(nativeDir))
        {
            PatchEnvironmentPath(nativeDir);
        }
    }

    private void PatchEnvironmentPath(string nativeDir)
    {
        string envVar;
        string currentValue;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            envVar = "PATH";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            envVar = "LD_LIBRARY_PATH";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            envVar = "DYLD_LIBRARY_PATH";
        }
        else
        {
            throw new PlatformNotSupportedException();
        }

        currentValue = Environment.GetEnvironmentVariable(envVar) ?? string.Empty;

        if (!currentValue.Split(Path.PathSeparator).Contains(nativeDir))
        {
            string newValue = nativeDir + Path.PathSeparator + currentValue;
            Environment.SetEnvironmentVariable(envVar, newValue);
        }
    }
}