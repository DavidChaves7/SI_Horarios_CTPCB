﻿using SI_Horarios_CTPCB.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace SI_Horarios_CTPCB.Infrastructure.Handlers
{
    internal sealed class UserCircuitHandler : CircuitHandler, IDisposable
    {
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly UserService userService;

        public UserCircuitHandler(
            AuthenticationStateProvider authenticationStateProvider,
            UserService userService)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.userService = userService;
        }

        public override Task OnCircuitOpenedAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            authenticationStateProvider.AuthenticationStateChanged +=
                AuthenticationChanged;

            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        private void AuthenticationChanged(Task<AuthenticationState> task)
        {
            _ = UpdateAuthentication(task);

            async Task UpdateAuthentication(Task<AuthenticationState> task)
            {
                try
                {
                    var state = await task;
                    userService.SetUser(state.User);
                }
                catch
                {
                }
            }
        }

        public override async Task OnConnectionUpAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            var state = await authenticationStateProvider.GetAuthenticationStateAsync();
            userService.SetUser(state.User);
        }

        public void Dispose()
        {
            authenticationStateProvider.AuthenticationStateChanged -=
                AuthenticationChanged;
        }
    }
}
