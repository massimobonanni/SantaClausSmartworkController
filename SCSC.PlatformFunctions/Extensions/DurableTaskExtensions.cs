using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.WebJobs.Extensions.DurableTask
{
    internal static class DurableTaskExtensions
    {
        public static AlertStatus ToAlertStatus(this OrchestrationRuntimeStatus source)
        {
            switch (source)
            {
                case OrchestrationRuntimeStatus.Running:
                    return AlertStatus.Running;
                case OrchestrationRuntimeStatus.Unknown:
                case OrchestrationRuntimeStatus.Completed:
                case OrchestrationRuntimeStatus.ContinuedAsNew:
                case OrchestrationRuntimeStatus.Failed:
                case OrchestrationRuntimeStatus.Canceled:
                case OrchestrationRuntimeStatus.Terminated:
                case OrchestrationRuntimeStatus.Pending:
                default:
                    return AlertStatus.Closed;
            }
        }

        public static AlertInfoModel ToAlertInfoModel(this DurableOrchestrationStatus source)
        {
            if (source == null)
                throw new NullReferenceException(nameof(source));

            var inputModel = source.CustomStatus.ToObject<CreateAlertModel>();
            return new AlertInfoModel()
            {
                Id = source.InstanceId,
                Status = source.RuntimeStatus.ToAlertStatus(),
                AlertName = inputModel.AlertName,
                Data = inputModel.Data,
                Type = inputModel.Type,
                ElfId = inputModel.ElfId
            };
        }
    }
}
