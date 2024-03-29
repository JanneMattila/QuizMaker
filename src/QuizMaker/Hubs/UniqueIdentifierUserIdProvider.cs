﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace QuizMaker.Hubs;

public class UniqueIdentifierUserIdProvider : IUserIdProvider
{
    private static readonly PathString QuizResultsHub = new PathString("/QuizResultsHub");

    public string GetUserId(HubConnectionContext connection)
    {
        var context = connection.GetHttpContext();
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        if (context.Request.Path.StartsWithSegments(QuizResultsHub, StringComparison.InvariantCultureIgnoreCase))
        {
            var email = context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(email))
            {
                // This is QuizResultsHub with real authenticated user.
                return email;
            }
            throw new ArgumentException("User has not authenticated.", nameof(connection));
        }

        // This handles following scenarios:
        // 1. First request of Azure SignalR Service (before returning with token)
        // 2. Self-hosted SignalR
        var quizUserId = context.Request.Cookies["QuizUserId"];
        if (!string.IsNullOrEmpty(quizUserId))
        {
            return quizUserId;
        }

        var token = context.Request.Query["access_token"];
        if (string.IsNullOrEmpty(token) &&
            context.Request.Headers.ContainsKey(HeaderNames.Authorization))
        {
            // In case of simulator and .NET client we're receiving bearer token.
            var authenticationHeader = AuthenticationHeaderValue.Parse(context.Request.Headers[HeaderNames.Authorization]);
            token = authenticationHeader.Parameter;
        }

        // This handles parsing of the jwt token from
        // Azure SignalR Service and gets the UserId from the claim
        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Based on this:
            // https://github.com/Azure/azure-signalr/blob/36abd7aeebc119ccf72ab596bd6198abc2f12eaf/src/Microsoft.Azure.SignalR.Common/Constants.cs#L37
            // We need to get "asrs.s.uid" claim (asrs=AzureSignalRService, s=System, uid=UserId)
            const string UserIdClaim = "asrs.s.uid";
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == UserIdClaim);
            if (claim != null)
            {
                return claim.Value;
            }
        }

        throw new ArgumentException("Parameter does not contain required user identifiers.", nameof(connection));
    }
}
