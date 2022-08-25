using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;


namespace dialogbot.Controllers
{
    public class BotControllerV2 : ControllerBase
    {

        
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IBot Bot;

        public BotControllerV2(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            Adapter = adapter;
            Bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await Adapter.ProcessAsync(Request, Response, Bot);
        }
    }
}
    

