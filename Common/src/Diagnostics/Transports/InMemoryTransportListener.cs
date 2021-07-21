﻿using System;
using System.Diagnostics;
using Common.Core.Messaging.Diagnostics;
using Common.Core.Messaging.Diagnostics.Events;
using Microsoft.Extensions.DiagnosticAdapter;
using OpenTelemetry.Trace;

namespace Common.Diagnostics.Transports
{
    public class InMemoryTransportListener
    {
        public static string InBoundName =>
            OTelTransportOptions.InMemoryConsumerActivityName;

        public static string OutBoundName =>
            OTelTransportOptions.InMemoryConsumerActivityName;

        [DiagnosticName(OTelTransportOptions.Events.BeforeProcessInMemoryMessage)]
        //Published message parameter name and published property name should be identical
        public virtual void BeforeProcessInMemoryMessage(BeforeProcessMessage payload)
        {
            Console.WriteLine(
                $"raising BeforeProcessInMemoryMessage event for message with message id: {payload.EventData.Id} - activity id: '{Activity.Current?.Id}'");
        }

        [DiagnosticName(OTelTransportOptions.Events.AfterProcessInMemoryMessage)]
        public virtual void AfterProcessInMemoryMessage(AfterProcessMessage payload)
        {
            Console.WriteLine(
                $"raising AfterProcessInMemoryMessage event for message with message id: {payload.EventData.Id} - activity id: '{Activity.Current?.Id}'");
        }


        [DiagnosticName(OTelTransportOptions.Events.AfterSendInMemoryMessage)]
        public virtual void AfterSendInMemoryMessage(AfterSendMessage payload)
        {
            Console.WriteLine(
                $"raising AfterSendInMemoryMessage event for message with message id: {payload.EventData.Id} - activity id: '{Activity.Current?.Id}'");
        }


        [DiagnosticName(OTelTransportOptions.Events.BeforeSendInMemoryMessage)]
        public virtual void BeforeSendInMemoryMessage(BeforeSendMessage payload)
        {
            Console.WriteLine(
                $"raising BeforeSendInMemoryMessage event for message with message id: {payload.EventData.Id} - activity id: '{Activity.Current?.Id}'");
        }
    }
}