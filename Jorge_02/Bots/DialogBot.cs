using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Jorge_02.Bots
{
    public class DialogBot<T> : BaseBot where T : Dialog
    {
        protected readonly Dialog _dialog;

        public DialogBot(UserState userState, ConversationState conversationState, T dialog, ILogger<DialogBot<T>> logger)
            : base(userState, conversationState, logger)
        {
            _dialog = dialog;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            // Run the Dialog with the new message Activity.
            await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }
    }
}
