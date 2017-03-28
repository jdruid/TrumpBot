// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace TrumpBot.Dialogs
{
    [Serializable]
    public class SupporterDialog : IDialog<IMessageActivity>
    {
        PartyAffiliate partyAffiliate;
        string name;
        string nickname;
        string nicknameMessage;

        public async Task StartAsync(IDialogContext context)
        {
             context.Wait(ConversationStartedAsync);            
        }

        public async Task ConversationStartedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            IMessageActivity message = await argument;
            //await context.PostAsync(message.Text);

            PromptDialog.Text(
                context: context,
                resume: ResumeAndPromptPlatformAsync,
                prompt: "Hi there my good American. I am the TrumpBot. What is your name?",
                retry: "I didn't understand. Please try again.");
        }

        public async Task ResumeAndPromptPlatformAsync(IDialogContext context, IAwaitable<string> argument)
        {
            name = await argument;

            PersonalizedMessage pm = await Trump.GetPersonalizedMessage(name);

            nickname = pm.nickname;
            nicknameMessage = pm.message;
            
            PromptDialog.Choice(
                context: context,
                resume: ResumeAndPromptDescriptionAsync,
                options: Enum.GetValues(typeof(PartyAffiliate)).Cast<PartyAffiliate>().ToArray(),
                prompt: $"Ok there {nickname}, what party are you going to vote for on Nov 28th?",
                retry: "I didn't understand. Please try again.");
        }

        public async Task ResumeAndPromptDescriptionAsync(IDialogContext context, IAwaitable<PartyAffiliate> argument)
        {
            partyAffiliate = await argument;

            PromptDialog.Text(
                context: context,
                resume: ResumeAndPromptSummaryAsync,
                prompt: $"So you are a {partyAffiliate}...Let me tell you something, {nicknameMessage}. Give me a topic and I will tell you my campaign message?",
                retry: "I didn't understand. Please try again.");
        }

        public async Task ResumeAndPromptSummaryAsync(IDialogContext context, IAwaitable<string> argument)
        {
            string topic = await argument;

            AllQuotes m = await Trump.GetAllQuotes();

            var onpoint = m.messages.non_personalized.Where(x => x.Contains(topic)).FirstOrDefault();

            if (onpoint != null)
            {
                PromptDialog.Confirm(
                    context: context,
                    resume: ResumeAndHandleConfirmAsync,
                    prompt: $"Here is one for you, {onpoint}'. Do I have your support?",
                    retry: "I didn't understand. Please try again.");
            }
            else
            {
                string campaignMessage = await Trump.GetRandomQuote();

                PromptDialog.Confirm(
                    context: context,
                    resume: ResumeAndHandleConfirmAsync,
                    prompt: $"Not sure I have one of those but I will give you one anyway. {campaignMessage}'. Do I have your support now?",
                    retry: "I didn't understand. Please try again.");
            }
            
        }

        public async Task ResumeAndHandleConfirmAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            bool choicesAreCorrect = await argument;

            if (choicesAreCorrect)
                await context.PostAsync("Thanks for supporting the TrumpBot!");
            else
                await context.PostAsync("I see. Move along then.");

            context.Wait(ConversationStartedAsync);
        }

        public enum PartyAffiliate
        {
            Democrat,
            Republican,
            Independent,
            None
        }

       
    }
}