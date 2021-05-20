using Common;
using Microsoft.AspNetCore.Http;
using OnlineStore.Modules.Identity.Application.Contracts;
using OnlineStore.Modules.Identity.Domain.Common;

namespace OnlineStore.Modules.Identity.Infrastructure.Context
{
    public class OnlineStoreContextBuilder : IOnlineStoreContextBuilder
    {
        public OnlineStoreContextBuilder(HttpContext httpContext, OnlineStoreOptions options)
        {
            HttpContext = httpContext;

            // var qs = HttpContext.Request.Query.ToNameValueCollection();
            //
            // OnlineStoreContext = new StoreContext.OnlineStoreContext()
            // {
            //     RequestUrl = HttpContext.Request.GetUri(),
            //     QueryString = qs,
            //     PageNumber = qs["page"].ToNullableInt(),
            // };
            //
            // var pageSize = qs["count"].ToNullableInt() ?? qs["page_size"].ToNullableInt();
            // if (pageSize != null && pageSize.Value > options.PageSizeMaxValue)
            // {
            //     pageSize = options.PageSizeMaxValue;
            // }
            //
            // OnlineStoreContext.PageSize = pageSize;
            // //To interpret as true the value of preview_mode from the query string according to its actual presence, since another value of this parameter can be passed.
            // OnlineStoreContext.IsPreviewMode =
            //     !string.IsNullOrEmpty(OnlineStoreContext.QueryString.Get("preview_mode"));
        }

        public HttpContext HttpContext { get; }
        public StoreContext.OnlineStoreContext OnlineStoreContext { get; }
    }
}