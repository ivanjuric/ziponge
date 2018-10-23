using System;
using Microsoft.Extensions.Logging;
using ZipongeCore;

namespace ZipongeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure logger and output to console
            var loggerFactory = new LoggerFactory().AddConsole();
            var logger = loggerFactory.CreateLogger("ZipongeApp");

            // Read and validate args
            var expectedArgsLength = 4;
            if(args.Length != expectedArgsLength)
            {
                logger.LogError($"Insufficient number of arguments passed: {args.Length} instead of {expectedArgsLength}!");
                return;
            }

            var dirPathSrc = args[0];
            var dirPathDest = args[1];
            if(!DateTime.TryParse(args[2], out DateTime dateFrom))
            {
                logger.LogError($"Cannot parse date from argument: {args[2]}");
                return;
            }
            if (!DateTime.TryParse(args[3], out DateTime dateTo))
            {
                logger.LogError($"Cannot parse date from argument: {args[3]}");
                return;
            }

            if(dateFrom > dateTo)
            {
                logger.LogError($"Date FROM ({dateFrom}) is greater than date TO ({dateTo})");
                return;
            }

            var paramInfo = $@"PARAMS: 
    {dirPathSrc} => {dirPathDest}
    Date range: {dateFrom.ToShortDateString()} - {dateTo.ToShortDateString()}";

            logger.LogInformation(paramInfo);

            // Create archives for specified dates
            var ziponge = new Ziponge();
            ziponge.Execute(dirPathSrc, dirPathDest, dateFrom, dateTo);

            Console.ReadLine();
        }
    }
}
