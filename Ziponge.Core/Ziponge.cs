﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ZipongeCore
{
    /// <summary>
    /// Main class for manipulation with archived backups
    /// </summary>
    public class Ziponge : IZiponge
    {
        private readonly ILogger _logger;

        public Ziponge(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public void Execute(string dirPathSrc, string dirPathDest, string searchSubstring)
        {
            // 1. Get files that match pattern
            var searchPattern = $"*{searchSubstring}*";
            var matchedFiles = Directory.GetFiles(dirPathSrc, searchPattern);
            if (matchedFiles == null || matchedFiles.Length == 0)
            {
                _logger.LogWarning("No files matches search substring");
                return;
            }

            // 2. Create temporary directory for matched files
            var archiveTempDirPath = Path.Combine(dirPathDest, searchSubstring);
            if (!Directory.Exists(archiveTempDirPath))
                Directory.CreateDirectory(archiveTempDirPath);
            _logger.LogInformation($"Temp directory path: {archiveTempDirPath}!");

            // 3. Move matched files to temporary directory
            var totalFiles = matchedFiles.Length;
            var i = 0;
            foreach (var file in matchedFiles)
            {
                var fileInfo = new FileInfo(file);
                var srcFilePath = fileInfo.FullName;
                var destFilePath = Path.Combine(archiveTempDirPath, fileInfo.Name);
                File.Move(srcFilePath, destFilePath);
                _logger.LogInformation($"Moving file {++i}/{totalFiles}");
            }

            // 4. Archive files from temporary directory
            var archiveFilePath = $"{archiveTempDirPath}.zip";
            ZipFile.CreateFromDirectory(archiveTempDirPath, archiveFilePath);
            _logger.LogInformation($"Created archive {archiveFilePath}");

            // 5. Delete temporary directory
            Directory.Delete(archiveTempDirPath, true);
            _logger.LogInformation($"Deleted temp directory: {archiveTempDirPath}!");
        }

        public void Execute(string dirPathSrc, string dirPathDest, DateTime dateFrom, DateTime dateTo, string format = "yyyy-MM-dd")
        {
            var dates = Enumerable.Range(0, (dateTo - dateFrom).Days + 1).Select(x => $"{dateFrom.AddDays(x).ToString("yyyy-MM-dd")}");

            var sw = new Stopwatch();
            var totalElapsedSeconds = 0;

            foreach (var date in dates)
            {
                var searchSubstring = date;

                _logger.LogInformation($"Current date processed: {date}");
                _logger.LogInformation($"Arguments: dirPath={dirPathSrc}; searchSubstring={searchSubstring}");

                sw.Restart();

                Execute(dirPathSrc, dirPathDest, searchSubstring);

                var elapsedSeconds = sw.Elapsed.Seconds;
                totalElapsedSeconds += elapsedSeconds;
                _logger.LogInformation($"Elapsed {elapsedSeconds} seconds");
            }

            _logger.LogInformation($"Finished!");
            _logger.LogInformation($"TOTAL elapsed {totalElapsedSeconds} seconds");
        }
    }
}