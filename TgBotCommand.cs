using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TgBotCommand
{
    public abstract class TgBotCommand
    {
        #region Properties
        public virtual string Command => string.Empty;
        public TgBotCommand? Succsessor { get; set; }
        #endregion

        #region Methods
        public Task Handle(ITelegramBotClient botClient, Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        Handle(botClient, update.Message!);
                        break;
                    }
                case UpdateType.CallbackQuery:
                    {
                        Handle(botClient, update.CallbackQuery!);
                        break;
                    }
            }
            return Task.CompletedTask;
        }

        public virtual Task Handle(ITelegramBotClient botClient, Message message)
        {
            Succsessor?.Handle(botClient, message);
            return Task.CompletedTask;  
        }

        public virtual Task Handle(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            Succsessor?.Handle(botClient, callbackQuery);
            return Task.CompletedTask;
        }

        public virtual bool CheckCommand(ITelegramBotClient botClient, Message message) =>
            message.Text!.Replace("@" + botClient.GetMeAsync().Result.Username, string.Empty) == Command;

        public virtual bool CheckCommand(ITelegramBotClient botClient, CallbackQuery callbackQuery) =>
            callbackQuery.Data == Command ||
            (callbackQuery.Data!.StartsWith(Command) && long.TryParse(callbackQuery.Data.Replace(Command, string.Empty), out _));

        public void AddSuccsessor(TgBotCommand succsessor)
        {
            if (this.Succsessor is null) this.Succsessor = succsessor;
            else this.Succsessor.AddSuccsessor(succsessor);
        }
        #endregion
    }
}
