using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using dialogbot.Services;
using dialogbot.Helpers;
using System.Collections.Generic;

namespace dialogbot.Bots
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    {
        #region Variables
        protected readonly Dialog _dialog;
        protected readonly StateService _stateService;
        protected readonly ILogger _logger;
        #endregion  

        public DialogBot(StateService stateService, T dialog, ILogger<DialogBot<T>> logger)
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));
            _dialog = dialog ?? throw new System.ArgumentNullException(nameof(dialog));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _stateService.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _stateService.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            // Run the Dialog with the new message Activity.
            await _dialog.Run(turnContext, _stateService.DialogStateAccessor, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext,
            CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = MessageFactory.Text($"Welcome to Dilkap Bot.\n\nType anything to get started.");
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }
    }
}

