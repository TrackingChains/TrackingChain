using Microsoft.Extensions.Logging;
using System;

namespace TrackingChain.TrackingChainCore.Extensions
{
    /*
     * Always group similar log delegates by type, always use incremental event ids.
     * Last event id is: 12
     */
    public static class LoggerExtensions
    {
        // Fields.
        //*** DEBUG LOGS ***
        private static readonly Action<ILogger, string, Exception> _runMigrateDbTransactionPool =
            LoggerMessage.Define<string>(
                LogLevel.Debug,
                new EventId(3, nameof(RunMigrateDbTransactionPool)),
                "Run Migrate {DbContext}");
        private static readonly Action<ILogger, Exception> _runPoolEnqueuer =
            LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(6, nameof(RunPoolEnqueuer)),
                "Run Pool Enqueuer");
        private static readonly Action<ILogger, Exception> _runPoolDequeuer =
            LoggerMessage.Define(
                LogLevel.Debug,
                new EventId(9, nameof(RunPoolDequeuer)),
                "Run Pool Dequeuer");

        //*** INFORMATION LOGS ***
        private static readonly Action<ILogger, Exception> _startMigratorDbWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(1, nameof(StartMigratorDbWorker)),
                "Migration Db Worker running.");
        private static readonly Action<ILogger, Guid, Exception> _startChildPoolDequeuerTask =
            LoggerMessage.Define<Guid>(
                LogLevel.Information,
                new EventId(10, nameof(StartChildPoolDequeuerTask)),
                "Start Child {TaskId} task running.");
        private static readonly Action<ILogger, Exception> _startPoolDequeuerWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(7, nameof(StartPoolDequeuerWorker)),
                "Pool Dequeuer Worker running.");
        private static readonly Action<ILogger, Exception> _startPoolEnqueuerWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(4, nameof(StartPoolEnqueuerWorker)),
                "Pool Enqueuer Worker running.");
        private static readonly Action<ILogger, Exception> _startPendingTransactionCheckerWorker = 
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(11, nameof(StartPendingTransactionCheckerWorker)),
                "Pending Transaction Checker Worker running.");
        private static readonly Action<ILogger, Guid, Exception> _endChildPoolDequeuerTask =
            LoggerMessage.Define<Guid>(
                LogLevel.Information,
                new EventId(11, nameof(EndChildPoolDequeuerTask)),
                "Child {TaskId} task END.");
        private static readonly Action<ILogger, Exception> _endMigratorDbWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(2, nameof(EndMigratorDbWorker)),
                "Migration Db END.");
        private static readonly Action<ILogger, Exception> _endPoolDequeuerWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(5, nameof(EndPoolDequeuerWorker)),
                "Pool Dequeuer END.");
        private static readonly Action<ILogger, Exception> _endPoolEnqueuerWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(5, nameof(EndPoolEnqueuerWorker)),
                "Pool Enqueuer END.");
        private static readonly Action<ILogger, Exception> _endPendingTransactionCheckerWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(12, nameof(EndPendingTransactionCheckerWorker)),
                "Pending transaction checker END.");
        //*** WARNING LOGS ***
        //*** ERROR LOGS ***

        // Methods.
        public static void EndChildPoolDequeuerTask(this ILogger logger, Guid taskId) =>
            _endChildPoolDequeuerTask(logger, taskId, null!);
        public static void EndMigratorDbWorker(this ILogger logger) =>
            _endMigratorDbWorker(logger, null!);
        public static void EndPoolDequeuerWorker(this ILogger logger) =>
            _endPoolDequeuerWorker(logger, null!);
        public static void EndPoolEnqueuerWorker(this ILogger logger) =>
            _endPoolEnqueuerWorker(logger, null!);
        public static void EndPendingTransactionCheckerWorker(this ILogger logger) =>
            _endPendingTransactionCheckerWorker(logger, null!);
        public static void RunMigrateDbTransactionPool(this ILogger logger, string dbContextName) =>
            _runMigrateDbTransactionPool(logger, dbContextName, null!);
        public static void RunPoolDequeuer(this ILogger logger) =>
            _runPoolDequeuer(logger, null!);
        public static void RunPoolEnqueuer(this ILogger logger) =>
            _runPoolEnqueuer(logger, null!);
        public static void StartChildPoolDequeuerTask(this ILogger logger, Guid taskId) =>
            _startChildPoolDequeuerTask(logger, taskId, null!);
        public static void StartMigratorDbWorker(this ILogger logger) =>
            _startMigratorDbWorker(logger, null!);
        public static void StartPoolDequeuerWorker(this ILogger logger) =>
            _startPoolDequeuerWorker(logger, null!);
        public static void StartPoolEnqueuerWorker(this ILogger logger) =>
            _startPoolEnqueuerWorker(logger, null!);
        public static void StartPendingTransactionCheckerWorker(this ILogger logger) =>
            _startPendingTransactionCheckerWorker(logger, null!);
        
    }
}
