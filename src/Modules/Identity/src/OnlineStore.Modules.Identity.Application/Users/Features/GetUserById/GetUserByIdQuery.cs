using System;
using BuildingBlocks.Core.Caching;
using BuildingBlocks.Cqrs.Queries;
using OnlineStore.Modules.Identity.Application.Users.Dtos.UseCaseResponses;

namespace OnlineStore.Modules.Identity.Application.Users.Features.GetUserById
{
    public class GetUserByIdQuery : IQuery<UserDto>
    {
        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }

        // Simply defining a CachePolicy for ICachePolicy<TRequest,TResponse> sets up caching
        // similar to setting up a FluentValidation Validator that inherits from AbstractValidator<TRequest>.
        // This could be in the same file or in a separate file, but doesn't clutter up the "Handler".
        public class CachePolicy : ICachePolicy<GetUserByIdQuery, UserDto>
        {
            public DateTimeOffset? AbsoluteExpirationRelativeToNow => DateTimeOffset.Now.AddMinutes(60);

            public string GetCacheKey(GetUserByIdQuery query)
            {
                return CacheKey.With(GetType(), query.ToString());
            }
        }
    }
}