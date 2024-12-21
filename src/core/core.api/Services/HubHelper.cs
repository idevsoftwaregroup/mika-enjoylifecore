
using core.application.Contract.API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace core.api.Services
{
    public class HubHelper : Hub
    {
        private readonly IUserService _userService;
        public HubHelper(IUserService userService)
        {
            _userService = userService;
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var userId = Convert.ToInt32(httpContext.Request.Query["userId"].FirstOrDefault());
                await _userService.SetUserConnection(userId, Context.ConnectionId);
                await base.OnConnectedAsync();
            }
            catch (Exception)
            {
                await base.OnConnectedAsync();
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await this._userService.RemoveUserConnection(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
