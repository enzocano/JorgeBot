// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.13.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Jorge_02.Bots
{
    public class BaseBot : ActivityHandler
    {
        protected readonly BotState _userState;
        protected readonly BotState _conversationState;
        protected readonly ILogger _logger;
        protected readonly IStatePropertyAccessor<UserData> _userDataAccessor;

        public BaseBot(UserState userState, ConversationState conversationState, ILogger<BaseBot> logger)
        {
            _userState = userState;
            _conversationState = conversationState;
            _logger = logger;
            _userDataAccessor = _userState.CreateProperty<UserData>("UserData");
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            var welcomeAgainText = "You again? Welcome back";

            var userData = await _userDataAccessor.GetAsync(turnContext, () => new UserData() { IsFirstTime = true }, cancellationToken);

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    if (userData.IsFirstTime)
                    {
                        userData.IsFirstTime = false;
                        await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(userData.Name))
                        {
                            welcomeAgainText += $" {userData.Name}!!";
                        }
                        
                        await turnContext.SendActivityAsync(MessageFactory.Text(welcomeAgainText, welcomeAgainText), cancellationToken);
                    }
                }
            }
        }
    }
}
