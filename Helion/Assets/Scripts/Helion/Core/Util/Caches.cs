using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using Helion.Core.Archives;
using Helion.Core.Resource;
using Helion.Core.Util.Logging;
using Debug = UnityEngine.Debug;

namespace Helion.Core.Util
{
    public static class Caches
    {
        private static readonly Log Log = LogManager.Instance();

        private const string CacheFolder = "cache";
        private const string BspExe = "zdbsp.exe";
        private const string BspZip = "zdbsp-1.19.zip";
        private static readonly string BspZipUrl = $"https://zdoom.org/files/utils/zdbsp/{BspZip}";

        public static Optional<IArchive> LoadArchive(string path)
        {
            string cachePath = FindOrCreateCache(path);
            return ArchiveReader.ReadFile(cachePath);
        }

        private static string FindOrCreateCache(string fileNameExt)
        {
            string filePath = $"{Data.Config.Resources.Directory}/{fileNameExt}";

            if (!Directory.Exists(CacheFolder))
                Directory.CreateDirectory(CacheFolder);

            if (filePath.EndsWith(".pk3", StringComparison.OrdinalIgnoreCase))
                return filePath;

            string cachePath = $"{CacheFolder}/{fileNameExt}";
            string bspExe = $"{CacheFolder}/{BspExe}";

            if (File.Exists(cachePath))
                return cachePath;

            if (!File.Exists(bspExe))
                DownloadBspBuilder();

            using (Process process = new Process())
            {
                // Processes apparently need the full path or else it looks
                // in the system folders...
                string fullBspPath = Path.GetFullPath($"{CacheFolder}/{BspExe}");

                process.StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = fullBspPath,
                    // Was `-q -N -x -z -E` but apparently that's too hard for it...
                    Arguments = $"-x -z -o {cachePath} {filePath}",
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                Debug.Log($"Building cache with: {fullBspPath} {process.StartInfo.Arguments}");
                process.Start();

                for (int second = 0; second <= 15; second++)
                {
                    if (second == 15)
                    {
                        Debug.Log("Process timeout! Aborting...");
                        break;
                    }

                    if (process.HasExited)
                        break;

                    Thread.Sleep(1000);
                }
            }

            return cachePath;
        }

        private static bool DownloadBspBuilder()
        {
            string bspExePath = $"{CacheFolder}/{BspExe}";

            if (File.Exists(bspExePath))
                return true;

            try
            {
                if (!File.Exists(BspZip))
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(BspZipUrl, BspZip);
                    if (!File.Exists(BspZip))
                        return false;
                }

                if (!TryGetFileFromZip(BspZip, BspExe, out byte[] zipData))
                    return false;

                File.Delete(BspZip);
                File.WriteAllBytes(bspExePath, zipData);
                return File.Exists(BspZip);
            }
            catch
            {
                return false;
            }
        }

        private static bool TryGetFileFromZip(string zipPath, string fileName, out byte[] data)
        {
            try
            {
                using (FileStream fileStream = new FileStream(zipPath, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(fileStream))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (entry.Name != fileName)
                                continue;

                            using (Stream stream = entry.Open())
                            {
                                data = new byte[entry.Length];
                                stream.Read(data, 0, (int)entry.Length);
                                return true;
                            }
                        }
                    }
                }

                Log.Error($"Could not find {fileName} in zip {zipPath}");

                data = null;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Unable to read zip data: {e.Message}");

                data = null;
                return false;
            }
        }
    }
}
