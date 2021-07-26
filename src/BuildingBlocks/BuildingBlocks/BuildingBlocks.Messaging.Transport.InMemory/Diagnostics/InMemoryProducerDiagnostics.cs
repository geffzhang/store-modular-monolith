﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using BuildingBlocks.Core.Messaging;
using BuildingBlocks.Core.Messaging.Diagnostics;
using BuildingBlocks.Diagnostics.Messaging.Events;
using BuildingBlocks.Diagnostics.Transports;
using Microsoft.AspNetCore.Http;
using OpenTelemetry.Context.Propagation;
using NameValueHeaderValue = Microsoft.Net.Http.Headers.NameValueHeaderValue;

namespace BuildingBlocks.Messaging.Transport.InMemory.Diagnostics
{
    //https://github.com/dotnet/runtime/blob/4f9ae42d861fcb4be2fcd5d3d55d5f227d30e723/src/libraries/System.Net.Http/src/System/Net/Http/DiagnosticsHandler.cs#L78
    public class InMemoryProducerDiagnostics
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly DiagnosticSource DiagnosticListener =
            new DiagnosticListener(OTelTransportOptions.InMemoryProducerActivityName);

        private static readonly TextMapPropagator Propagator = new TraceContextPropagator();

        public InMemoryProducerDiagnostics(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Activity StartActivity<T>(T message) where T : class, IMessage
        {
            var activity = new Activity(OTelTransportOptions.InMemoryProducerActivityName);

            if (DiagnosticListener.IsEnabled(OTelTransportOptions.Events.BeforeSendInMemoryMessage))
            {
                DiagnosticListener.StartActivity(activity, new
                {
                    payload = new BeforeSendMessage(message)
                });
                // DiagnosticListener.Write(Constants.Events.BeforeSendInMemoryMessage,new
                // {
                //     payload = payload
                // });
            }
            else
            {
                activity.Start();
            }

            //https://github.com/dotnet/aspnetcore/blob/e6afd501caf0fc5d64b6f3fd47584af6f7adba43/src/Hosting/Hosting/src/Internal/HostingApplicationDiagnostics.cs#L285
            //https://www.mytechramblings.com/posts/getting-started-with-opentelemetry-and-dotnet-core/
            // var headers = _httpContextAccessor.HttpContext?.Request.Headers;
            // Propagator.Inject(new PropagationContext(activity.Context, Baggage.Current), headers,
            //     (object carrier, string fieldName, string fieldValue) =>
            //     {
            //         var headers = (IHeaderDictionary) carrier!;
            //         fieldValue = headers[fieldName];
            //     });

            InjectHeaders(activity);

            return activity;
        }

        public void StopActivity<T>(T message) where T : class, IMessage
        {
            var activity = Activity.Current;
            if (activity?.Duration == TimeSpan.Zero)
            {
                activity.SetEndTime(DateTime.UtcNow);
            }

            if (DiagnosticListener.IsEnabled(OTelTransportOptions.Events.AfterSendInMemoryMessage))
            {
                DiagnosticListener.StopActivity(activity,  new
                {
                    payload = new AfterSendMessage(message)
                });
            }
            else
                activity?.Stop();
        }

        private void InjectHeaders(Activity activity)
        {
            if (_httpContextAccessor.HttpContext is null)
                return;

            if (activity.IdFormat == ActivityIdFormat.W3C)
            {
                if (!_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(Constants
                    .TraceParentHeaderName))
                {
                    _httpContextAccessor.HttpContext.Request.Headers[
                        Constants.TraceParentHeaderName] = activity.Id;
                    if (activity.TraceStateString != null)
                    {
                        _httpContextAccessor.HttpContext.Request.Headers[
                                Constants.TraceStateHeaderName] =
                            activity.TraceStateString;
                    }
                }
            }
            else
            {
                if (!_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(Constants
                    .RequestIdHeaderName))
                {
                    _httpContextAccessor.HttpContext.Request.Headers[
                        Constants.RequestIdHeaderName] = activity.Id;
                }
            }

            // we expect baggage to be empty or contain a few items
            using IEnumerator<KeyValuePair<string, string>> e = activity.Baggage.GetEnumerator();
            if (e.MoveNext())
            {
                var baggage = new List<string>();
                do
                {
                    KeyValuePair<string, string> item = e.Current;
                    baggage.Add(new NameValueHeaderValue(WebUtility.UrlEncode(item.Key),
                        WebUtility.UrlEncode(item.Value)).ToString());
                } while (e.MoveNext());

                _httpContextAccessor.HttpContext.Request.Headers.AppendList(
                    Constants.CorrelationContextHeaderName,
                    baggage);
            }
        }
    }
}