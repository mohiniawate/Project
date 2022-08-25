using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using dialogbot.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using dialogbot.Services;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace dialogbot.Dialogs
{
    public class GreetingDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;
        #endregion  
        public GreetingDialog(string dialogId, StateService stateService) : base(dialogId)
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            // Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
               // InitialStepAsync,
                UserTypeStepAsync,
                FinalStepAsync

            };

            // Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(GreetingDialog)}.mainFlow", waterfallSteps));
            //AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.name"));
            AddDialog(new ChoicePrompt($"{nameof(GreetingDialog)}.userType", userTypeValidatorAsync));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(GreetingDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());

            if (string.IsNullOrEmpty(userProfile.Name))
            {
                return await stepContext.PromptAsync($"{nameof(GreetingDialog)}.name",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Welcome to Dilkap Chatbot")


                    }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }


        private async Task<DialogTurnResult> UserTypeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {          

            return await stepContext.PromptAsync($"{nameof(GreetingDialog)}.userType",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please select any one option from below"),
                    RetryPrompt = MessageFactory.Text("Not a valid response!\n\nPlease select any one option from below"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "Guest", "Student" }),
                }, cancellationToken);
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
          
            
                // Set the user type 

                userProfile.UserType =((FoundChoice)stepContext.Result).Value; 

                // Save any state changes that might have occured during the turn.
                await _stateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            
  

          //  await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Hi {0}. How can I help you today?", userProfile.Name)), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private Task<bool> userTypeValidatorAsync(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded);
        }
    }
}
