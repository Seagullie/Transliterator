using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

// References:
//https://github.com/ds4windowsapp/DS4Windows

namespace Transliterator.Services;

public static class AutostartMethods
{
    // TODO: Remove hardcode for link name
    private static string lnkPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + $"\\Transliterator.lnk";

    public static void DeleteAutostartEntry()
    {
        if (File.Exists(lnkPath) && !new FileInfo(lnkPath).IsReadOnly)
        {
            File.Delete(lnkPath);
        }
    }

    public static bool HasAutostartEntry()
    {
        return File.Exists(lnkPath);
    }

    // TODO: Handle case when this assembly is used in a different project.
    // In such case the function has to generate shortcut for top level aka calling assembly.
    public static void WriteAutostartEntry()
    {
        Type t = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")); // Windows Script Host Shell Object
        dynamic shell = Activator.CreateInstance(t);
        try
        {
            var lnk = shell.CreateShortcut(lnkPath);
            try
            {
                lnk.TargetPath = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe");
                lnk.Save();
            }
            finally
            {
                Marshal.FinalReleaseComObject(lnk);
            }
        }
        finally
        {
            Marshal.FinalReleaseComObject(shell);
        }
    }
}