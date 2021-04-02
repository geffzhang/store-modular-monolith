using System;
using System.Collections.Generic;
using System.Linq;
using Common.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Exceptions
{
    internal class ExceptionToMessageMapperResolver : IExceptionToMessageMapperResolver
    {
        private readonly IExceptionToMessageMapper _defaultMapper = new ExceptionToMessageMapper();
        private readonly IDictionary<Type, Type> _mappers = new Dictionary<Type, Type>();
        private readonly IServiceProvider _serviceProvider;

        public ExceptionToMessageMapperResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            using var scope = _serviceProvider.CreateScope();
            var mappers = scope.ServiceProvider.GetServices<IExceptionToMessageMapper>();
            foreach (var mapper in mappers)
            foreach (var exceptionType in mapper.ExceptionTypes ?? Enumerable.Empty<Type>())
                _mappers.TryAdd(exceptionType, mapper.GetType());
        }

        public IActionRejected Map<T>(T exception) where T : Exception
        {
            if (!_mappers.TryGetValue(exception.GetType(), out var mapperType)) return _defaultMapper.Map(exception);

            IActionRejected result;
            using var scope = _serviceProvider.CreateScope();
            {
                result = ((IExceptionToMessageMapper) _serviceProvider.GetRequiredService(mapperType)).Map(exception);
            }

            return result ?? _defaultMapper.Map(exception);
        }
    }
}