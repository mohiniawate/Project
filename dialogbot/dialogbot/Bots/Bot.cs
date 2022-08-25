﻿using AdaptiveCards;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
// https://adaptivecards.io/
// https://adaptivecards.io/explorer/
// https://adaptivecards.io/visualizer/
// https://adaptivecards.io/samples/
// uses NuGet pkg AdaptiveCards - https://docs.microsoft.com/en-us/adaptive-cards/sdk/authoring-cards/net
namespace demo10adaptivecards
{
    public class Bot : IBot
    {
        public async Task OnTurn(ITurnContext turnContext)
        {
            //disply result
            if (turnContext.Activity.Value! == null)
                //display adaptive card
                if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    var response = turnContext.Activity.CreateReply();
                    response.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingSdk() };

                    await turnContext.SendActivityAsync(response);

                }
        }
        private Attachment CreateAdaptiveCardUsingSdk()
        {
            var card = new AdaptiveCard();
            card.Body.Add(new AdaptiveTextBlock() { Text = "Colour", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveChoiceSetInput()
            {
                Id = "Colour",
                Style = AdaptiveChoiceInputStyle.Compact,
                Choices = new List<AdaptiveChoice>(new[] {
                        new AdaptiveChoice() { Title = "Red", Value = "RED" },
                        new AdaptiveChoice() { Title = "Green", Value = "GREEN" },
                        new AdaptiveChoice() { Title = "Blue", Value = "BLUE" } })
            });
            card.Body.Add(new AdaptiveTextBlock() { Text = "Registration number:", Size = AdaptiveTextSize.Medium, Weight = AdaptiveTextWeight.Bolder });
            card.Body.Add(new AdaptiveTextInput() { Style = AdaptiveTextInputStyle.Text, Id = "RegistrationNumber" });
            card.Actions.Add(new AdaptiveSubmitAction() { Title = "Submit" });
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }

        private Attachment CreateAdaptiveCardUsingJson()
        {
            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = AdaptiveCard.FromJson(File.ReadAllText("adaptiveCard.json")).Card
            };
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // display result
            if (turnContext.Activity.Value != null)
            {

            }

            // display adaptive card
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var response = turnContext.Activity.CreateReply();
                response.Attachments = new List<Attachment>() { CreateAdaptiveCardUsingJson() };

                await turnContext.SendActivityAsync(response);
            }
        }
    }
}
