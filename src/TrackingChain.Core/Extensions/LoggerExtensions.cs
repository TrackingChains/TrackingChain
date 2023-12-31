﻿using Microsoft.Extensions.Logging;
using System;
using TrackingChain.Common.Dto;
using TrackingChain.Common.Enums;

namespace TrackingChain.TrackingChainCore.Extensions
{
    /*
     * Always group similar log delegates by type, always use incremental event ids.
     * Last event id is: 42
     */
    public static class LoggerExtensions
    {
        // Fields.
        //*** DEBUG LOGS ***
        private static readonly Action<ILogger, int, Exception> _endReProcessTransactionLockedUseCase =
            LoggerMessage.Define<int>(
                LogLevel.Debug,
                new EventId(36, nameof(EndReProcessTransactionLockedUseCase)),
                "End Manage Transaction Failed UseCase Processed: {Processed}");
        private static readonly Action<ILogger, Guid, int, Exception> _manageTransactionFailedCanceledDueToErrorUseCase =
            LoggerMessage.Define<Guid, int>(
                LogLevel.Debug,
                new EventId(38, nameof(ManageTransactionFailedCanceledDueToErrorUseCase)),
                "Manage Transaction Failed Canceled Due To Error UseCase TrackingId: {TrackingId}\tErrorTimes: {ErrorTimes}");
        private static readonly Action<ILogger, Guid, int, TransactionErrorReason, Exception> _manageTransactionFailedToReprocessableUseCase =
            LoggerMessage.Define<Guid, int, TransactionErrorReason>(
                LogLevel.Debug,
                new EventId(39, nameof(ManageTransactionFailedToReprocessableUseCase)),
                "Manage Transaction Failed To Reprocessable UseCase TrackingId: {TrackingId}\tErrorTimes: {ErrorTimes}\tTransactionErrorReason: {TransactionErrorReason}");
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
        private static readonly Action<ILogger, int, Exception> _startTransactionDeleterUseCase =
            LoggerMessage.Define<int>(
                LogLevel.Debug,
                new EventId(32, nameof(StartTransactionDeleterUseCase)),
                "Start Transaction Deleter UseCase max: {Max}");
        private static readonly Action<ILogger, int, Exception> _endTransactionDeleterUseCase =
            LoggerMessage.Define<int>(
                LogLevel.Debug,
                new EventId(33, nameof(EndTransactionDeleterUseCase)),
                "End Transaction Deleter UseCase Deleted: {Deleted}");
        private static readonly Action<ILogger, int, int, Exception> _startManageTransactionFailedUseCase =
            LoggerMessage.Define<int, int>(
                LogLevel.Debug,
                new EventId(35, nameof(StartManageTransactionFailedUseCase)),
                "Start Manage Transaction Failed UseCase Max: {Max}\tFailedReTryTimes: {FailedReTryTimes}");
        private static readonly Action<ILogger, int, int, Exception> _endManageTransactionFailedUseCase =
            LoggerMessage.Define<int, int>(
                LogLevel.Debug,
                new EventId(34, nameof(EndManageTransactionFailedUseCase)),
                "End Manage Transaction Failed UseCase Cancelled:{Cancelled}\tReTry:{ReTry}");
        private static readonly Action<ILogger, int, int, int, Exception> _startReProcessTransactionLockedUseCase =
        LoggerMessage.Define<int, int, int>(
            LogLevel.Debug,
            new EventId(37, nameof(StartReProcessTransactionLockedUseCase)),
            "Start ReProcess Transaction Failed UseCase Max:{Max}\tUnlockUncompletedGeneratorAfterSeconds: {UnlockUncompletedGeneratorAfterSeconds}\tUnlockUncompletedWatcherAfterSeconds: {UnlockUncompletedWatcherAfterSeconds}");

        //*** INFORMATION LOGS ***
        private static readonly Action<ILogger, Guid, Exception> _endChildCheckerTask =
            LoggerMessage.Define<Guid>(
                LogLevel.Information,
                new EventId(20, nameof(EndChildCheckerTask)),
                "Child Checker {TaskId} task END.");
        private static readonly Action<ILogger, Guid, Exception> _endChildPoolDequeuerTask =
            LoggerMessage.Define<Guid>(
                LogLevel.Information,
                new EventId(11, nameof(EndChildPoolDequeuerTask)),
                "Child PoolDequeuer {TaskId} task END.");
        private static readonly Action<ILogger, Exception> _endAlertWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(27, nameof(EndAlertWorker)),
                "Alert END.");
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
                new EventId(19, nameof(EndPendingTransactionCheckerWorker)),
                "Pending transaction checker END.");
        private static readonly Action<ILogger, Exception> _endTransactionFailedWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(12, nameof(EndTransactionFailedWorker)),
                "Transaction failed recovery END.");
        private static readonly Action<ILogger, Exception> _endTransactionDeleterWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(25, nameof(EndTransactionDeleterWorker)),
                "Transaction deleter recovery END.");
        private static readonly Action<ILogger, Exception> _endTransactionLockedWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(23, nameof(EndTransactionLockedWorker)),
                "Transaction locked END.");
        private static readonly Action<ILogger, Exception> _startMigratorDbWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(1, nameof(StartMigratorDbWorker)),
                "Migration Db Worker running.");
        private static readonly Action<ILogger, Guid, Exception> _startChildPoolDequeuerTask =
            LoggerMessage.Define<Guid>(
                LogLevel.Information,
                new EventId(10, nameof(StartChildPoolDequeuerTask)),
                "Start Child PoolDequeuer {TaskId} task running.");
        private static readonly Action<ILogger, Guid, Exception> _startChildCheckerTask =
            LoggerMessage.Define<Guid>(
                LogLevel.Information,
                new EventId(19, nameof(StartChildCheckerTask)),
                "Start Child Checker {TaskId} task running.");
        private static readonly Action<ILogger, Exception> _startAlertWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(26, nameof(StartAlertWorker)),
                "Alert Worker running.");
        private static readonly Action<ILogger, Exception> _startPoolDequeuerWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(7, nameof(StartPoolDequeuerWorker)),
                "Pool Dequeuer Worker running.");
        private static readonly Action<ILogger, Exception> _startPoolEnqueuerWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(18, nameof(StartPoolEnqueuerWorker)),
                "Pool Enqueuer Worker running.");
        private static readonly Action<ILogger, Exception> _startPendingTransactionCheckerWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(11, nameof(StartPendingTransactionCheckerWorker)),
                "Pending Transaction Checker Worker running.");
        private static readonly Action<ILogger, Exception> _startTransactionLockedWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(22, nameof(StartTransactionLockedWorker)),
                "Transaction Locked Worker running.");
        private static readonly Action<ILogger, Exception> _startTransactionDeleterWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(24, nameof(StartTransactionDeleterWorker)),
                "Start Transaction Deleter Worker running.");
        private static readonly Action<ILogger, Exception> _startTransactionFailedWorker =
            LoggerMessage.Define(
                LogLevel.Information,
                new EventId(11, nameof(StartTransactionFailedWorker)),
                "Start Transaction Failed Worker running.");
        private static readonly Action<ILogger, Guid, TransactionDetailStatus, Exception> _transactionWatcher =
            LoggerMessage.Define<Guid, TransactionDetailStatus>(
                LogLevel.Information,
                new EventId(16, nameof(TransactionWatcher)),
                "Transaction Wacther for TrackingGuid:{TrackingGuid}\tStatus:{Status}");
        private static readonly Action<ILogger, string, string, string, string, Guid, Exception> _trackingEntry =
            LoggerMessage.Define<string, string, string, string, Guid>(
                LogLevel.Information,
                new EventId(13, nameof(TrackingEntry)),
                "Tracking Entry for Code:{Code}\tData:{DataValue}\tCategory:{Category}\tSmartContract:{SmartContract}\tProfileGroup:{ProfileGroup}");
        private static readonly Action<ILogger, Guid, string, string, Exception> _transactionOnChain =
            LoggerMessage.Define<Guid, string, string>(
                LogLevel.Information,
                new EventId(15, nameof(TransactionOnChain)),
                "Transaction OnChain for TrackingGuid:{TrackingGuid}\tTxHash:{TxHash}\tSmartContracAddress:{SmartContracAddress}");
        private static readonly Action<ILogger, Guid, string, Guid, Exception> _transactionInPool =
            LoggerMessage.Define<Guid, string, Guid>(
                LogLevel.Information,
                new EventId(14, nameof(TransactionInPool)),
                "Transaction InPool for TrackingGuid:{TrackingGuid}\tSmartContracAddress:{SmartContracAddress}\tProfileGroup:{ProfileGroup}");

        //*** WARNING LOGS ***
        private static readonly Action<ILogger, Guid, int, string, Exception> _manageTransactionFailedUndefinedRecoveryUseCase =
            LoggerMessage.Define<Guid, int, string>(
                LogLevel.Debug,
                new EventId(40, nameof(ManageTransactionFailedUndefinedRecoveryUseCase)),
                "Manage Transaction Failed Undefined Recovery UseCase TrackingId: {TrackingId}\tErrorTimes: {ErrorTimes}\rReason: {Reason}");

        //*** ERROR LOGS ***
        private static readonly Action<ILogger, Exception> _alertWorkerError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(31, nameof(AlertWorkerError)),
                "Alert Worker Error");
        private static readonly Action<ILogger, Guid, Exception> _childCheckerTaskInError =
            LoggerMessage.Define<Guid>(
                LogLevel.Error,
                new EventId(21, nameof(ChildCheckerTaskInError)),
                "Child Checker Guid:{Guid}");
        private static readonly Action<ILogger, Guid, Exception> _childPoolDequeuerTaskInError =
            LoggerMessage.Define<Guid>(
                LogLevel.Error,
                new EventId(18, nameof(ChildPoolDequeuerTaskInError)),
                "Child Pool Dequeuer Guid:{Guid}");
        private static readonly Action<ILogger, Guid, string, string, Exception> _getTrasactionReceiptInError =
            LoggerMessage.Define<Guid, string, string>(
                LogLevel.Error,
                new EventId(17, nameof(GetTrasactionReceiptInError)),
                "GetTrasactionReceiptInError TrackingGuid:{TrackingGuid}\tTxHash:{TxHash}\tApiUrl:{ApiUrl}");
        private static readonly Action<ILogger, Guid, Exception> _manageTransactionToCancelInError =
            LoggerMessage.Define<Guid>(
                LogLevel.Error,
                new EventId(41, nameof(ManageTransactionToCancelInError)),
                "_manageTransactionToCancelInError TrackingGuid:{TrackingGuid}");
        private static readonly Action<ILogger, Guid, Exception> _manageTransactionToRetryInError =
            LoggerMessage.Define<Guid>(
                LogLevel.Error,
                new EventId(42, nameof(ManageTransactionToRetryInError)),
                "_manageTransactionToCancelInError TrackingGuid:{TrackingGuid}");
        private static readonly Action<ILogger, Guid, Exception> _transactionGenerationCompletedInError =
            LoggerMessage.Define<Guid>(
                LogLevel.Error,
                new EventId(20, nameof(TransactionGenerationCompletedInError)),
                "Transaction Generation Completed in Error for  TrackingGuid:{TrackingGuid}");
        private static readonly Action<ILogger, Guid, string, Exception> _trasactionGenerationInError =
            LoggerMessage.Define<Guid, string>(
                LogLevel.Error,
                new EventId(21, nameof(TrasactionGenerationInError)),
                "Transaction Generation Error for TrackingGuid:{TrackingGuid}\tEndpoint:{Endpoint}");
        private static readonly Action<ILogger, Exception> _transactionDeleterWorkerError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(29, nameof(TransactionDeleterWorkerError)),
                "Transaction Deleter Worker Error");
        private static readonly Action<ILogger, Exception> _transactionFailedWorkerError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(30, nameof(TransactionFailedWorkerError)),
                "Transaction Failed Worker Error");
        private static readonly Action<ILogger, Exception> _transactionLockedWorkerError =
            LoggerMessage.Define(
                LogLevel.Error,
                new EventId(28, nameof(TransactionLockedWorkerError)),
                "Transaction Locked Worker Error");

        // Methods.
        public static void AlertWorkerError(this ILogger logger, Exception exception) =>
            _alertWorkerError(logger, exception);
        public static void ChildCheckerTaskInError(this ILogger logger, Guid taskId, Exception exception) =>
            _childCheckerTaskInError(logger, taskId, exception);
        public static void ChildPoolDequeuerTaskInError(this ILogger logger, Guid taskId, Exception exception) =>
            _childPoolDequeuerTaskInError(logger, taskId, exception);
        public static void EndChildCheckerTask(this ILogger logger, Guid taskId) =>
            _endChildCheckerTask(logger, taskId, null!);
        public static void EndChildPoolDequeuerTask(this ILogger logger, Guid taskId) =>
            _endChildPoolDequeuerTask(logger, taskId, null!);
        public static void EndAlertWorker(this ILogger logger) =>
            _endAlertWorker(logger, null!);
        public static void EndMigratorDbWorker(this ILogger logger) =>
            _endMigratorDbWorker(logger, null!);
        public static void EndPoolDequeuerWorker(this ILogger logger) =>
            _endPoolDequeuerWorker(logger, null!);
        public static void EndPoolEnqueuerWorker(this ILogger logger) =>
            _endPoolEnqueuerWorker(logger, null!);
        public static void EndPendingTransactionCheckerWorker(this ILogger logger) =>
            _endPendingTransactionCheckerWorker(logger, null!);
        public static void EndManageTransactionFailedUseCase(this ILogger logger, int cancelled, int reTry) =>
            _endManageTransactionFailedUseCase(logger, cancelled, reTry, null!);
        public static void EndReProcessTransactionLockedUseCase(this ILogger logger, int processed) =>
            _endReProcessTransactionLockedUseCase(logger, processed, null!);
        public static void EndTransactionDeleterUseCase(this ILogger logger, int deleted) =>
            _endTransactionDeleterUseCase(logger, deleted, null!);
        public static void EndTransactionDeleterWorker(this ILogger logger) =>
            _endTransactionDeleterWorker(logger, null!);
        public static void EndTransactionFailedWorker(this ILogger logger) =>
            _endTransactionFailedWorker(logger, null!);
        public static void EndTransactionLockedWorker(this ILogger logger) =>
            _endTransactionLockedWorker(logger, null!);
        public static void GetTrasactionReceiptInError(this ILogger logger, Guid trackingGuid, string txHash, string apiUrl, Exception exception) =>
            _getTrasactionReceiptInError(logger, trackingGuid, txHash, apiUrl, exception);
        public static void ManageTransactionFailedUndefinedRecoveryUseCase(this ILogger logger, Guid trackingGuid, int errorTimes, string reason) =>
        _manageTransactionFailedUndefinedRecoveryUseCase(logger, trackingGuid, errorTimes, reason, null!);
        public static void ManageTransactionFailedCanceledDueToErrorUseCase(this ILogger logger, Guid trackingGuid, int errorTimes) =>
            _manageTransactionFailedCanceledDueToErrorUseCase(logger, trackingGuid, errorTimes, null!);
        public static void ManageTransactionFailedToReprocessableUseCase(this ILogger logger, Guid trackingGuid, int errorTimes, TransactionErrorReason transactionErrorReason) =>
            _manageTransactionFailedToReprocessableUseCase(logger, trackingGuid, errorTimes, transactionErrorReason, null!);
        public static void ManageTransactionToCancelInError(this ILogger logger, Guid trackingGuid, Exception exception) =>
            _manageTransactionToCancelInError(logger, trackingGuid, exception);
        public static void ManageTransactionToRetryInError(this ILogger logger, Guid trackingGuid, Exception exception) =>
            _manageTransactionToRetryInError(logger, trackingGuid, exception);
        public static void RunMigrateDbTransactionPool(this ILogger logger, string dbContextName) =>
            _runMigrateDbTransactionPool(logger, dbContextName, null!);
        public static void RunPoolDequeuer(this ILogger logger) =>
            _runPoolDequeuer(logger, null!);
        public static void RunPoolEnqueuer(this ILogger logger) =>
            _runPoolEnqueuer(logger, null!);
        public static void StartChildCheckerTask(this ILogger logger, Guid taskId) =>
            _startChildCheckerTask(logger, taskId, null!);
        public static void StartChildPoolDequeuerTask(this ILogger logger, Guid taskId) =>
            _startChildPoolDequeuerTask(logger, taskId, null!);
        public static void StartMigratorDbWorker(this ILogger logger) =>
            _startMigratorDbWorker(logger, null!);
        public static void StartAlertWorker(this ILogger logger) =>
            _startAlertWorker(logger, null!);
        public static void StartPoolDequeuerWorker(this ILogger logger) =>
            _startPoolDequeuerWorker(logger, null!);
        public static void StartPoolEnqueuerWorker(this ILogger logger) =>
            _startPoolEnqueuerWorker(logger, null!);
        public static void StartPendingTransactionCheckerWorker(this ILogger logger) =>
            _startPendingTransactionCheckerWorker(logger, null!);
        public static void StartManageTransactionFailedUseCase(this ILogger logger, int max, int failedReTryTimes) =>
            _startManageTransactionFailedUseCase(logger, max, failedReTryTimes, null!);
        public static void StartReProcessTransactionLockedUseCase(this ILogger logger, int max, int unlockUncompletedGeneratorAfterSeconds, int unlockUncompletedWatcherAfterSeconds) =>
            _startReProcessTransactionLockedUseCase(logger, max, unlockUncompletedGeneratorAfterSeconds, unlockUncompletedWatcherAfterSeconds, null!);
        public static void StartTransactionDeleterUseCase(this ILogger logger, int max) =>
            _startTransactionDeleterUseCase(logger, max, null!);
        public static void StartTransactionDeleterWorker(this ILogger logger) =>
            _startTransactionDeleterWorker(logger, null!);
        public static void StartTransactionFailedWorker(this ILogger logger) =>
            _startTransactionFailedWorker(logger, null!);
        public static void StartTransactionLockedWorker(this ILogger logger) =>
            _startTransactionLockedWorker(logger, null!);
        public static void TransactionGenerationCompletedInError(this ILogger logger, Guid trackingGuid) =>
            _transactionGenerationCompletedInError(logger, trackingGuid, null!);
        public static void TrasactionGenerationInError(this ILogger logger, Guid trackingGuid, string endpoint, Exception exception) =>
            _trasactionGenerationInError(logger, trackingGuid, endpoint, exception);
        public static void TransactionWatcher(this ILogger logger, Guid trackingGuid, TransactionDetailStatus status) =>
            _transactionWatcher(logger, trackingGuid, status, null!);
        public static void TransactionDeleterWorkerError(this ILogger logger, Exception exception) =>
            _transactionDeleterWorkerError(logger, exception);
        public static void TrackingEntry(this ILogger logger, string code, string dataValue, string category, string smartContracAddress, Guid profileGroup) =>
            _trackingEntry(logger, code, dataValue, category, smartContracAddress, profileGroup, null!);
        public static void TransactionFailedWorkerError(this ILogger logger, Exception exception) =>
            _transactionFailedWorkerError(logger, exception);
        public static void TransactionInPool(this ILogger logger, Guid trackingGuid, string smartContracAddress, Guid profileGroup) =>
            _transactionInPool(logger, trackingGuid, smartContracAddress, profileGroup, null!);
        public static void TransactionLockedWorkerError(this ILogger logger, Exception exception) =>
            _transactionLockedWorkerError(logger, exception);
        public static void TransactionOnChain(this ILogger logger, Guid trackingGuid, string txHash, string smartContracAddress) =>
            _transactionOnChain(logger, trackingGuid, txHash, smartContracAddress, null!);
    }
}
