using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using dialogbot.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using dialogbot.Services;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using dialogbot.Card;
using System.IO;
using AdaptiveCards.Templating;

namespace dialogbot.Dialogs
{
    public class GuestDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;
        static string AboutAdaptivePromptId = "aboutAdaptive";
        static string PlacementAdaptivePromptId = "placementAdaptive";
        static string InquiryAdaptivePromptId = "inquiryAdaptive";
        static string CourseAdaptivePromptId = "CourseAdaptive";

        #endregion  
        public GuestDialog(string dialogId, StateService stateService) : base(dialogId)
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));

            InitializeWaterfallDialog();
        }
        private void InitializeWaterfallDialog()
        {
            // Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
                GuestTypeStepAsync,
                aboutstepAsync,
                CourseSelectionStepAsync,
                ContinueChatStepAsync,
                FinalStepAsync
            };

            AddDialog(new WaterfallDialog($"{nameof(GuestDialog)}.mainFlow", waterfallSteps));

            AddDialog(new AdaptiveCardPrompt(AboutAdaptivePromptId));
            AddDialog(new AdaptiveCardPrompt(PlacementAdaptivePromptId));
            AddDialog(new AdaptiveCardPrompt(InquiryAdaptivePromptId));
            AddDialog(new AdaptiveCardPrompt(CourseAdaptivePromptId));



            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt($"{nameof(GuestDialog)}.userType", GuestTypeValidatorAsync));
            AddDialog(new ChoicePrompt($"{nameof(GuestDialog)}.degreeType", GuestTypeValidatorAsync));

            InitialDialogId = $"{nameof(GuestDialog)}.mainFlow";


        }
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
        private async Task<DialogTurnResult> GuestTypeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.PromptAsync($"{nameof(GuestDialog)}.userType",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What are you looking for"),
                    RetryPrompt = MessageFactory.Text("Not a valid response!\n\nPlease select any one option from below"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "About DRIEMS", "Degree Courses", "Placement", "Inquiry" }),
                }, cancellationToken);
        }





        private static async Task<DialogTurnResult> aboutstepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var selectedGuestOption = ((FoundChoice)stepContext.Result).Value;
            switch (selectedGuestOption)
            {
                case "About DRIEMS":
                    var opts = AboutAdaptiveCard();
                    return await stepContext.PromptAsync(AboutAdaptivePromptId, opts, cancellationToken);

                case "Degree Courses":
                    var degreePrompt = DegreeCoursesAdaptiveCard();
                    return await stepContext.PromptAsync($"{nameof(GuestDialog)}.degreeType", degreePrompt, cancellationToken);

                case "Placement":
                    var placementPrompt = PlacementAdaptiveCard();
                    return await stepContext.PromptAsync(PlacementAdaptivePromptId, placementPrompt, cancellationToken);

                case "Inquiry":
                    var inquiryPrompt = InquiryAdaptiveCard();
                    return await stepContext.PromptAsync(InquiryAdaptivePromptId, inquiryPrompt, cancellationToken);

                default:
                    break;

            }
            return await stepContext.PromptAsync(AboutAdaptivePromptId, null, cancellationToken);

        }

        private static PromptOptions AboutAdaptiveCard()
        {
            var cardJson = PrepareCard.ReadCard("About.json");
            cardJson = cardJson.Replace("@^@logo.png", GetBase64Image("logo.png"));
            cardJson = cardJson.Replace("@^@grade.png", GetBase64Image("grade.PNG"));
            var cardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson),
            };

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Attachments = new List<Attachment>() { cardAttachment },
                    Type = ActivityTypes.Message,
                }
            };
            return opts;
        }
        private static PromptOptions DegreeCoursesAdaptiveCard()
        {
            var opts = new PromptOptions
            {
                Prompt = MessageFactory.Text("Which course you looking for"),
                RetryPrompt = MessageFactory.Text("Not a valid response!\n\nPlease select any one option from below"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "Computer", "Mechanical", "Electrical", "Civil" }),
            };
            return opts;
        }

        private static async Task<DialogTurnResult> InquiryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken, Inquiry Inquiry)
        {
            var email = new Email();
            email.To = "iam.mohiniawate@gmail.com";
            email.Subject = "Dilkap Chatbot:Inquiry by Guest";
            email.Body = $"Email was sent by Guest Name: {Inquiry.SimpleVal}. \n Contact No: {Inquiry.TelVal}. \n Email Id: {Inquiry.EmailVal}. \n Query: {Inquiry.MultiLineVal}";
            var studentService = new StudentService();
            //studentService.SendEmail(email);
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Email with your Inquiry is sent to Dilkap Admin"),
            };
            studentService.SendEmailViaSendGrid(email);
            return await stepContext.PromptAsync($"{nameof(GuestDialog)}.guestInquirytResponse", promptOptions, cancellationToken);
        }

        private static async Task<DialogTurnResult> CourseSelectionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var opts = new PromptOptions();
            var value = stepContext.Result.ToString();
            if (!value.Contains("FoundChoice"))            {
                var Inquiry = JsonConvert.DeserializeObject<Inquiry>(value);
                if (Inquiry != null && !string.IsNullOrWhiteSpace(Inquiry.SimpleVal))
                {
                    await InquiryStepAsync(stepContext, cancellationToken, Inquiry);
                }
            }           
            else
            {             
                var selectedCourseOption = ((FoundChoice)stepContext.Result).Value;
                switch (selectedCourseOption)
                {
                    case "Computer":
                        opts = CourseAdaptiveCard("ComputerCourseData.json");
                        break;
                    case "Mechanical":
                        opts = CourseAdaptiveCard("MechanicalCourseData.json");
                        break;
                    case "Electrical":
                        opts = CourseAdaptiveCard("ElectricalCourseData.json");
                        break;
                    case "Civil":
                        opts = CourseAdaptiveCard("CivilCourseData.json");
                        break;
                    default:
                        break;

                }
            }
            
            return await stepContext.PromptAsync(CourseAdaptivePromptId, opts, cancellationToken);
        }
        private static PromptOptions CourseAdaptiveCard(string CourseDataFileName)
        {
            string _cards = Path.Combine(".", "Card", "Course.json");
            var adaptiveCardJson = (File.ReadAllText(_cards));
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(adaptiveCardJson);

            string _samplePath = Path.Combine(".", "CardData", CourseDataFileName);
            var sampleJsonData = (File.ReadAllText(_samplePath));
            var context = new EvaluationContext
            {
                Root = sampleJsonData
            };
            var cardJson = template.Expand(context);
            //var cardJson = PrepareCard.ReadCard("Placement.json");
            var cardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson),
            };

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Attachments = new List<Attachment>() { cardAttachment },
                    Type = ActivityTypes.Message,
                }
            };
            return opts;

        }

        private static PromptOptions PlacementAdaptiveCard()
        {
            var cardJson = PrepareCard.ReadCard("Placement.json");
            cardJson = cardJson.Replace("@^@placementtable.png", GetBase64Image("placementtable.png"));
            var cardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson),
            };

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Attachments = new List<Attachment>() { cardAttachment },
                    Type = ActivityTypes.Message,
                }
            };
            return opts;
        }
        private static PromptOptions InquiryAdaptiveCard()
        {
            var cardJson = PrepareCard.ReadCard("inquiry.json");
            var cardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson),
            };

            var opts = new PromptOptions
            {
                Prompt = new Activity
                {
                    Attachments = new List<Attachment>() { cardAttachment },
                    Type = ActivityTypes.Message,
                    
                }
            };
            return opts;
        }


        private static async Task<DialogTurnResult> ContinueChatStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var actionButton = JsonConvert.DeserializeObject<ActionButton>(stepContext.Result.ToString());

            if (actionButton.Value.ToLower().Equals("back"))
            {
                return await stepContext.ReplaceDialogAsync($"{nameof(GuestDialog)}.userType");
            }
            return await stepContext.ReplaceDialogAsync(nameof(TextPrompt), null, cancellationToken);
        }

        private Task<bool> GuestTypeValidatorAsync(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded);
        }
        
        private static string GetBase64Image(string imageName)
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, @"Content\Images", imageName);
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return $"data:image/png;base64,{imageData}";
        }
    }

    public class ActionButton
    {
        public string Value { get; set; }
    }
}
