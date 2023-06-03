
using System.Security.Claims;
using System.Threading.Tasks;
using CooksCornerAPP.Data;
using CooksCornerAPP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CooksCornerAPP.Hubs
{
    public class RecipeHub : Hub
    {
        private readonly ILogger<RecipeHub> _logger;
        private readonly ApplicationDbContext _context;

        public RecipeHub(ApplicationDbContext context, ILogger<RecipeHub> logger)
        {
            _context = context;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.ConnectionId = Context.ConnectionId;
                    Console.WriteLine($"Connection {user.ConnectionId} connected.");
                    await _context.SaveChangesAsync();

                    var notifications = await _context.Notifications
                      .Where(n => n.UserId == userId && !n.IsRead)
                      .ToListAsync();

                    foreach (var notification in notifications)
                    {
                        await Clients.Caller.SendAsync("ShowNotification", notification.Message);
                        notification.IsRead = true;
                    }

                    await _context.SaveChangesAsync();
                }
            }
            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.ConnectionId = null;
                    await _context.SaveChangesAsync();
                }
            }
            await base.OnDisconnectedAsync(exception);
        }


        public async Task NotifyOwner(int recipeId)
        {

            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FindAsync(userId);
            var recipe = await _context.Recipes.FindAsync(recipeId);

            if (recipe == null || recipe.OwnerId == userId)
            {

                return;

            }

            var recipeOwner = await _context.Users.FirstOrDefaultAsync(u => u.Id == recipe.OwnerId);


            if (recipeOwner != null)
            {

                var notification = new Notification
                {
                    UserId = recipe.OwnerId,
                    Message = "Someone favorited your recipe!",
                    RecipeId = recipeId,
                    IsRead = false
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();


            }
        }

        public async Task UpdateConnectionId(string userId, string connectionId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user != null)
            {
                user.ConnectionId = connectionId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendMessage(string message)
            {
            await Clients.All.SendAsync("ShowNotification", message);
        }

    }
}
