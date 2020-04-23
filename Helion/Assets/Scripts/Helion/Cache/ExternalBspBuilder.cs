using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using Helion.Core.Util.Logging;

namespace Helion.Cache
{
    public class ExternalBspBuilder
    {
        public const string BspExe = "zdbsp.exe";
        public const string BspZip = "zdbsp-1.19.zip";
        public static readonly string BspZipUrl = $"https://zdoom.org/files/utils/zdbsp/{BspZip}";
        public static readonly string BspExePath = $"{Caches.Folder}/{BspExe}";
        private static readonly Log Log = LogManager.Instance();

        public static Process GetBspBuilder()
        {
            if (!File.Exists(BspExePath))
            {
                if (!DownloadBspBuilder())
                    return null;
            }

            return new Process();
        }

        private static bool DownloadBspBuilder()
        {
            if (!Directory.Exists(Caches.Folder) || !File.Exists(BspExePath))
                return false;

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
                File.WriteAllBytes(BspExePath, zipData);
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
