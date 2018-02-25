﻿using Bit.Core.Implementations;
using Bit.Core.Models;
using Bit.WebApi;
using Bit.WebApi.Implementations;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Web.Http;

namespace Swashbuckle.Application
{
    public static class OpenApiExtensions
    {
        /// <summary>
        /// Calls <see cref="SwaggerDocsConfig.DescribeAllEnumsAsStrings(bool)"/>
        /// | Make it compatible with owin branching
        /// | Ignores CancellationToken parameter type
        /// </summary>
        public static SwaggerDocsConfig ApplyDefaultApiConfig(this SwaggerDocsConfig doc, HttpConfiguration webApiConfig)
        {
            doc.DescribeAllEnumsAsStrings();
            doc.RootUrl(req => new Uri(req.RequestUri, req.GetOwinContext().Request.PathBase.Value).ToString());
            doc.OperationFilter<OpenApiIgnoreParameterTypeOperationFilter<CancellationToken>>();
            doc.OperationFilter<DefaultAuthorizationOperationFilter>();

            AppEnvironment appEnv = DefaultAppEnvironmentProvider.Current.GetActiveAppEnvironment();

            doc.OAuth2("oauth2")
                .Flow("password")
                .TokenUrl($"{appEnv.GetSsoUrl()}/connect/token")
                .Scopes(scopes =>
                 {
                     if (!appEnv.Security.Scopes.SequenceEqual(new[] { "openid", "profile", "user_info" }))
                     {
                         foreach (string scope in appEnv.Security.Scopes)
                         {
                             scopes.Add(scope, scope);
                         }
                     }
                 });

            return doc;
        }

        private static Action<SwaggerUiConfig> GetBitSwaggerUiConfig(Action<SwaggerUiConfig> configure = null)
        {
            void CreateBitSwaggerUiConfig(SwaggerUiConfig c)
            {
                c.InjectJavaScript(typeof(WebApiMiddlewareConfiguration).GetTypeInfo().Assembly, "Bit.WebApi.Extensions.SwaggerExtender.js", isTemplate: false);

                configure?.Invoke(c);
            }

            return CreateBitSwaggerUiConfig;
        }

        public static void EnableBitSwaggerUi(this SwaggerEnabledConfiguration doc, Action<SwaggerUiConfig> configure = null)
        {
            doc.EnableSwaggerUi(GetBitSwaggerUiConfig(configure));
        }

        public static void EnableBitSwaggerUi(this SwaggerEnabledConfiguration doc, string routeTemplate, Action<SwaggerUiConfig> configure = null)
        {
            doc.EnableSwaggerUi(routeTemplate, GetBitSwaggerUiConfig(configure));
        }
    }
}
