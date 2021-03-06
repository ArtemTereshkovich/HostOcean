﻿using System;
using System.Linq.Expressions;
using Hangfire;
using Hangfire.Common;
using HostOcean.Infrastructure.Hangfire.CommandExecutor;
using MediatR;
using Newtonsoft.Json;

namespace HostOcean.Infrastructure.Hangfire
{
    public class CommandsScheduler : IStartupCommandSheduler
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public CommandsScheduler(
            ICommandExecutor commandExecutor,
            IRecurringJobManager recurringJobManager,
            IBackgroundJobClient backgroundJobClient)
        {
            _commandExecutor = commandExecutor;
            _recurringJobManager = recurringJobManager;
            _backgroundJobClient = backgroundJobClient;
        }

        public string ExecuteExpressionWithDelay(Expression<Action> expression, TimeSpan delay)
        {
            var newTime = DateTime.Now + delay;

            return _backgroundJobClient.Schedule(expression, newTime);
        }

        public string ExecuteExpression(Expression<Action> expression)
        {
            return _backgroundJobClient.Enqueue(expression);
        }

        public string ExecuteExpressionAfterSuccessPrevious(Expression<Action> expression, string previousId)
        {
            return _backgroundJobClient.ContinueWith(previousId, expression);
        }

        public string ExecuteNow(IRequest request, string description = null)
        {
            var scheduleCommand = SerializedCommand(request, description);

            return _backgroundJobClient.Enqueue(() => _commandExecutor.ExecuteCommand(scheduleCommand));
        }

        public string ExecuteAfterSuccessParentCommand(IRequest request, string parentCommandId,
            string description = null)
        {
            var scheduleCommand = SerializedCommand(request, description);

            return _backgroundJobClient.ContinueWith(parentCommandId,
                () => _commandExecutor.ExecuteCommand(scheduleCommand),
                JobContinuationOptions.OnlyOnSucceededState);
        }

        public string ExecuteAtDateTime(IRequest request, DateTime scheduleAt, string description = null)
        {
            var scheduleCommand = SerializedCommand(request, description);

            return _backgroundJobClient.Schedule(() => _commandExecutor.ExecuteCommand(scheduleCommand), scheduleAt);
        }

        public string ExecuteWithDelay(IRequest request, TimeSpan delay, string description = null)
        {
            var newTime = DateTime.Now + delay;
            var scheduleCommand = SerializedCommand(request, description);

            return _backgroundJobClient.Schedule(() => _commandExecutor.ExecuteCommand(scheduleCommand), newTime);
        }

        public string ExecutesByCronExpression(IRequest request, string jobId, string cronExpression,
            string description = null)
        {
            var scheduleCommand = SerializedCommand(request, description);

            var job = Job.FromExpression(() => _commandExecutor.ExecuteCommand(scheduleCommand));
            _recurringJobManager.AddOrUpdate(jobId, job,
                cronExpression);

            return jobId;
        }

        private SerializedScheduleCommand SerializedCommand(IRequest command, string description)
        {
            var commandType = command.GetType().FullName;
            var serializedCommand = JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                Formatting = Formatting.None
            });

            return new SerializedScheduleCommand(commandType, serializedCommand, description);
        }
    }
}