﻿using System;
using BuildingBlocks.Authentication.Jwt;
using BuildingBlocks.Core.Domain.IntegrationEvents;
using BuildingBlocks.Core.Messaging.Events;

namespace OnlineStore.Modules.Identity.Application.Authentication.Features.Login
{
    public class LoggedInIntegrationEvent : IIntegrationEvent
    {
        public LoggedInIntegrationEvent(string userName, Guid userId,JsonWebToken token)
        {
            UserName = userName;
            UserId = userId;
            JsonWebToken = token;
        }

        public string UserName { get; }
        public Guid UserId { get;  }
        public JsonWebToken JsonWebToken { get; }
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime OccurredOn { get; set; }
    }
}