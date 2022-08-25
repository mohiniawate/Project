using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using dialogbot.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using dialogbot.Services;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs.Choices;
using dialogbot.Card;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using AdaptiveCards.Templating;
using System.IO;

namespace dialogbot.Dialogs
{
    public class StudentDialog : ComponentDialog
    {
        #region Variables
        private readonly StateService _stateService;
        static string AdaptivePromptId = "adaptive";
        static string AboutAdaptivePromptId = "aboutAdaptive";
        static string MyProfileAdaptivePromptId = "MyProfileAdaptive";
        static string NoticeAdaptivePromptId = "NoticeAdaptive";
        static string LectureTimeTableAdaptivePromptId = "LectureTimeTableAdaptive";
        static string ExamTimeTableAdaptivePromptId = "ExamTimeTableAdaptive";
        static string SubjectsAdaptivePromptId = "SubjectsAdaptive";
        static string AskDoubtAdaptivePromptId = "AskDoubtAdaptive";


        #endregion  
        public StudentDialog(string dialogId, StateService stateService) : base(dialogId)
        {
            _stateService = stateService ?? throw new System.ArgumentNullException(nameof(stateService));

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            // Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
               UserFormAsync,
               ResultUserFormAsync,
               StudentOptionstepAsync,
               StudentDoubtAsync,
               FinalStepAsync

            };

            // Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(StudentDialog)}.mainFlow", waterfallSteps));
           
           
            AddDialog(new AdaptiveCardPrompt(AdaptivePromptId));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt($"{nameof(StudentDialog)}.studentType", StudentTypeValidatorAsync));

            AddDialog(new AdaptiveCardPrompt(MyProfileAdaptivePromptId));
            AddDialog(new AdaptiveCardPrompt(NoticeAdaptivePromptId));
            AddDialog(new AdaptiveCardPrompt(LectureTimeTableAdaptivePromptId));
            AddDialog(new AdaptiveCardPrompt(ExamTimeTableAdaptivePromptId));
            AddDialog(new AdaptiveCardPrompt(SubjectsAdaptivePromptId));
            AddDialog(new AdaptiveCardPrompt(AskDoubtAdaptivePromptId));



            // Set the starting Dialog
            InitialDialogId = $"{nameof(StudentDialog)}.mainFlow";
        }

        private static async Task<DialogTurnResult> StudentDoubtAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var value = stepContext.Result.ToString();
            var doubt = JsonConvert.DeserializeObject<Doubt>(value);
            var email = new Email();
            email.To = doubt.selectedUser;
            email.Subject = "Dilkap Chatbot:Doubt by student";
            email.Body = doubt.MultiLineVal;
            var studentService = new StudentService();
            //studentService.SendEmail(email);
            var promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text($"Email with your doubt is sent sucessfully"),
            };
            studentService.SendEmailViaSendGrid(email);
 
            return await stepContext.PromptAsync($"{nameof(StudentDialog)}.studentDoubtResponse", promptOptions, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }


        private static async Task<DialogTurnResult> UserFormAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var cardJson = PrepareCard.ReadCard("userform.json");

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
                    Text = "Please enter your Login details",
                }
            };

            return await stepContext.PromptAsync(AdaptivePromptId, opts, cancellationToken);
        }

        private Task<bool> StudentTypeValidatorAsync(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded);
        }


        //new code function for adaptive card
        private  async Task<DialogTurnResult> ResultUserFormAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var value = stepContext.Result.ToString();
            var studentService = new StudentService();
            var login = JsonConvert.DeserializeObject<Login>(value);

            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            userProfile.login = login;
            // Save any state changes that might have occured during the turn.
            await _stateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);

            var response=studentService.Login(login);
            var promptOptions = new PromptOptions();

            if (response !=null && response.MessageId==1)
            {
                promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Login Sucessfully \n\n What are you lookin for ?"),
                    RetryPrompt = MessageFactory.Text("Not a valid response!\n\nPlease select any one option from below"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "My Profile", "Notice", "Lecture Timetable", "Exam Timetable","Subjects","Ask Doubt"}),
                };

            }
            else
            {
                promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Login Unsucessfully : Mobile Number or Password is incorect  "),
                };
            }

           
            return await stepContext.PromptAsync($"{nameof(StudentDialog)}.studentType", promptOptions, cancellationToken);
        }

        private static PromptOptions MyProfileAdaptiveCard(Student student)
        {            
            var jsonString = JsonConvert.SerializeObject(student);
            string _cards = Path.Combine(".", "Card", "MyProfile.json");
            var adaptiveCardJson = (File.ReadAllText(_cards));
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(adaptiveCardJson);
            var context = new EvaluationContext
            {
                Root = jsonString
            };
            var cardJson = template.Expand(context);
           // var cardJson = PrepareCard.ReadCard("MyProfile.json");
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


        private static PromptOptions NoticeAdaptiveCard()
        {
            var cardJson = PrepareCard.ReadCard("Notice.json");
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

        private static PromptOptions LectureAdaptiveCard()
        {
            var cardJson = PrepareCard.ReadCard("LectureTimetable.json");
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

        private static PromptOptions ExamAdaptiveCard()
        {
            var cardJson = PrepareCard.ReadCard("ExamTimetable.json");
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

        private static PromptOptions SubjectsAdaptiveCard(string department)
        {
            var professorService = new ProfessorService();
            var response = professorService.getProfessorByDepartment(department);
            var professorsData = new ProfessorsData();
            professorsData.professorList = response;
            var jsonString = JsonConvert.SerializeObject(professorsData);
            string _cards = Path.Combine(".", "Card", "Subjects.json");
            var adaptiveCardJson = (File.ReadAllText(_cards));
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(adaptiveCardJson);
            var context = new EvaluationContext
            {
                Root = jsonString
            };
            var cardJson = template.Expand(context);
           
         //   var cardJson = PrepareCard.ReadCard("Subjects.json");
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

        private static PromptOptions AskAdaptiveCard(string department)
        {
            var professorService = new ProfessorService();
            var response = professorService.getProfessorByDepartment(department);
            var professorsData = new ProfessorsData();
            professorsData.professorList = response;
            var jsonString = JsonConvert.SerializeObject(professorsData);
            string _cards = Path.Combine(".", "Card", "AskDoubt.json");
            var adaptiveCardJson = (File.ReadAllText(_cards));
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(adaptiveCardJson);
            var context = new EvaluationContext
            {
                Root = jsonString
            };
            var cardJson = template.Expand(context);

           // var cardJson = PrepareCard.ReadCard("AskDoubt.json");
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




        private  async Task<DialogTurnResult> StudentOptionstepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _stateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            var studentService = new StudentService();
            var response = studentService.Login(userProfile.login);
            var selectedStudentOption = ((FoundChoice)stepContext.Result).Value;
            switch (selectedStudentOption)
            {
            
                case "My Profile":
                    var MyProfile = MyProfileAdaptiveCard(response.student);
                    return await stepContext.PromptAsync(MyProfileAdaptivePromptId, MyProfile, cancellationToken);

                case "Notice":
                    var NoticePrompt = NoticeAdaptiveCard();
                    return await stepContext.PromptAsync(NoticeAdaptivePromptId, NoticePrompt, cancellationToken);

                case "Lecture Timetable":
                    var LecturePrompt = LectureAdaptiveCard();
                    return await stepContext.PromptAsync(LectureTimeTableAdaptivePromptId, LecturePrompt, cancellationToken);

                case "Exam Timetable":
                    var ExamPrompt = ExamAdaptiveCard();
                    return await stepContext.PromptAsync(ExamTimeTableAdaptivePromptId, ExamPrompt, cancellationToken);

                case "Subjects":
                    var SubjectsPrompt = SubjectsAdaptiveCard(response.student.Stream);
                    return await stepContext.PromptAsync(SubjectsAdaptivePromptId, SubjectsPrompt, cancellationToken);

                case "Ask Doubt":
                    var AskPrompt = AskAdaptiveCard(response.student.Stream);
                    return await stepContext.PromptAsync(AskDoubtAdaptivePromptId, AskPrompt, cancellationToken);

                default:
                    break;

            }
            return await stepContext.PromptAsync(AboutAdaptivePromptId, null, cancellationToken);

        }
    }


}
