using dailogbot.Dialogs;
using dialogbot.Card;
using dialogbot.Models;
using dialogbot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace dialogbot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;
        private static string TOP_LEVEL_WATERFALL_NAME = "INITIAL";
        #endregion  


        public MainDialog(StateService stateService) : base(nameof(MainDialog))
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));

            InitializeWaterfallDialog();
        }
            
        private void InitializeWaterfallDialog()
        {
            // Create Waterfall Steps
      
            //AddDialog(new AdaptiveCardPrompt(AdaptivePromptId));
            //AddDialog(new TextPrompt(nameof(TextPrompt)));
           // AddDialog(new TextPrompt($"{nameof(MainDialog)}.greeting",InitialInputValidatorAsync));
            // AddDialog(new TextPrompt($"{nameof(MainDialog)}.welcome", InitialInputValidatorAsync));

         //   var topLevelWaterfallSteps = new WaterfallStep[]
         //{
         //       StartAsync
         //};

            var waterfallSteps = new WaterfallStep[]
             {
                // WelcomeStepAsync,
                InitialStepAsync,
              CheckUserTypeAsync,
                FinalStepAsync

            };

            // Add Named Dialogs
            //AddDialog(new WaterfallDialog(TOP_LEVEL_WATERFALL_NAME, topLevelWaterfallSteps));
            //AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));
            AddDialog(new GreetingDialog($"{nameof(MainDialog)}.greeting", _stateService));
          AddDialog(new BugReportDialog($"{nameof(MainDialog)}.bugReport", _stateService));
            AddDialog(new StudentDialog($"{nameof(MainDialog)}.student", _stateService));
            AddDialog(new GuestDialog($"{nameof(MainDialog)}.guest", _stateService));


            // Set the starting Dialog
            InitialDialogId = $"{nameof(MainDialog)}.mainFlow";

            // The initial child Dialog to run.
            //InitialDialogId = TOP_LEVEL_WATERFALL_NAME;

        }
        private async Task<DialogTurnResult> StartAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ////return await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Welcome to Test Bot:"), cancellationToken);
            //return await stepContext.PromptAsync($"{nameof(MainDialog)}.welcome",
            //   new PromptOptions
            //   {
            //       RetryPrompt = MessageFactory.Text("We cannot understand your response\n\nPlease enter valid text"),

            //   }, cancellationToken);
            return await stepContext.PromptAsync(nameof(WaterfallDialog), null, cancellationToken);
        }


        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        { 
            return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);

            //if (Regex.Match(stepContext.Context.Activity.Text.ToLower(), "hi").Success)
            //{
            //    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);
            //}
            //else
            //{
            //    return await stepContext.ReplaceDialogAsync(TOP_LEVEL_WATERFALL_NAME, null, cancellationToken);
            //    //return await stepContext.ReplaceDialogAsync(InitialId);
            //    //return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", null, cancellationToken);
            //    //return await stepContext.ReplaceDialogAsync($"{nameof(MainDialog)}.greeting",
            //    //new PromptOptions
            //    //{
            //    //    Prompt = MessageFactory.Text("We cannot understand your response\n\nPlease enter valid text "),
            //    //    RetryPrompt = MessageFactory.Text("We cannot understand your response\n\nPlease enter valid text"),

            //    //}, cancellationToken);
            //}
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }



        private async Task<DialogTurnResult> CheckUserTypeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (!string.IsNullOrEmpty(userProfile.UserType) && userProfile.UserType.Equals("Student"))
            {
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.student", null, cancellationToken);
                
            }
            else
            {

                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.guest", null, cancellationToken);
            }

        }


        private Task<bool> InitialInputValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;

            if (promptContext.Recognized.Succeeded)
            {
                valid = string.Equals(promptContext.Recognized.Value.ToLower(), "hi");
            }
            return Task.FromResult(valid);
        }

    }
}
