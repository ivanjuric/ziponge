using System;

namespace ZipongeCore
{
    public interface IZiponge
    {
        /// <summary>
        /// Finds all matching files by search substring from source directory,
        /// creates archive for group of matched files
        /// and transfers it to destination directory to new directory named by search substring.
        /// </summary>
        /// <param name="dirPathSrc">Path to the local computer or network directory with all the files that will be filtered by substring</param>
        /// <param name="dirPathDest">Path to the local computer or network directory that will contain archived file groups</param>
        /// <param name="searchSubstring">Substring that will be searched for in filenames from source directory</param>
        void Execute(string dirPathSrc, string dirPathDest, string searchSubstring);
        /// <summary>
        /// Groups all files containing date substring, for each date in specified interval.
        /// For each date, new archive will be created with included matching files that contains specified date substring.
        /// </summary>
        /// <param name="dirPathSrc">Path to the local computer or network directory with all the files that will be filtered by substring</param>
        /// <param name="dirPathDest">Path to the local computer or network directory that will contain archived file groups</param>
        /// <param name="dateFrom">Starting date that is used as a substring in matching</param>
        /// <param name="dateTo">Last date (included) used as a substring in matching</param>
        /// <param name="format">Format used to generate date substring</param>
        void Execute(string dirPathSrc, string dirPathDest, DateTime dateFrom, DateTime dateTo, string format = "yyyy-MM-dd");
    }
}
