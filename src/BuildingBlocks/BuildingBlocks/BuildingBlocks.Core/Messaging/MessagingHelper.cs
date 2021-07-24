using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BuildingBlocks.Core.Utils.Reflection;

namespace BuildingBlocks.Core.Messaging
{
    public static class MessagingHelper
    {
        public static IEnumerable<Type> GetHandledMessageTypes(params Assembly[] assemblies)
        {
            var messageHandlerTypes = typeof(IMessageHandler<>).GetAllTypesImplementingOpenGenericInterface(assemblies)
                .Where(x => x.GetCustomAttributes<DecoratorAttribute>().Any() == false)
                .ToList();

            foreach (var i in messageHandlerTypes.SelectMany(x => x.GetInterfaces()))
            {
                if (!i.IsGenericType)
                    continue;

                var messageType = i.GetGenericArguments().First();
                yield return messageType;
            }
        }
    }
}