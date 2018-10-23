using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace ZipongeCore
{
    /// <summary>
    /// Main class for manipulation with archived backups
    /// </summary>
    public class Ziponge : IZiponge
    {
        public void Execute(string dirPathSrc, string dirPathDest, string searchSubstring)
        {
            // 1. Get files that match pattern
            var searchPattern = $"*{searchSubstring}*";
            var matchedFiles = Directory.GetFiles(dirPathSrc, searchPattern);
            if (matchedFiles == null || matchedFiles.Length == 0)
                throw new Exception("No files matches search substring");

            // 2. Create temporary directory for matched files
            var archiveTempDirPath = Path.Combine(dirPathDest, searchSubstring);

            // Do not ovewrite existing backup archive if already exists
            var archiveFilePath = $"{archiveTempDirPath}.zip";
            if(File.Exists(archiveFilePath))
                throw new Exception($"File {archiveFilePath} already exists!");

            if (!Directory.Exists(archiveTempDirPath))
                Directory.CreateDirectory(archiveTempDirPath);

            // 3. Move matched files to temporary directory
            var totalFiles = matchedFiles.Length;
            var i = 0;
            foreach (var file in matchedFiles)
            {
                var fileInfo = new FileInfo(file);
                var srcFilePath = fileInfo.FullName;
                var destFilePath = Path.Combine(archiveTempDirPath, fileInfo.Name);
                File.Move(srcFilePath, destFilePath);
            }

            // 4. Archive files from temporary directory
            ZipFile.CreateFromDirectory(archiveTempDirPath, archiveFilePath);

            // 5. Delete temporary directory
            Directory.Delete(archiveTempDirPath, true);
        }

        public void Execute(string dirPathSrc, string dirPathDest, DateTime dateFrom, DateTime dateTo, string format = "yyyy-MM-dd")
        {
            var dates = Enumerable.Range(0, (dateTo - dateFrom).Days + 1).Select(x => $"{dateFrom.AddDays(x).ToString("yyyy-MM-dd")}");

            var sw = new Stopwatch();
            var totalElapsedSeconds = 0;

            foreach (var date in dates)
            {
                var searchSubstring = date;

                sw.Restart();

                Execute(dirPathSrc, dirPathDest, searchSubstring);

                var elapsedSeconds = sw.Elapsed.Seconds;
                totalElapsedSeconds += elapsedSeconds;
            }
        }
    }
}
