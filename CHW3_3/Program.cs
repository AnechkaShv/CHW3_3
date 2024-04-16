using FileWorking;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace CHW3_3
{
    class Program
    {
        static ITelegramBotClient botClient = new TelegramBotClient("7072534298:AAGXB-P80Aqui8kBuYSCMyh8GPX_rS_7VGY");
        static void Main()
        {
            // For right encoding on english orientation devices.
            //Console.OutputEncoding = System.Text.Encoding.UTF8;
            //Console.InputEncoding = System.Text.Encoding.GetEncoding("utf-16");

            while (true)
            {
                try
                {
                    using CancellationTokenSource cts = new();

                    // Logging changes.
                    Logger.CreateLogger();

                    // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
                    ReceiverOptions receiverOptions = new()
                    {
                        AllowedUpdates = new[]
                        {
                            UpdateType.Message,
                            UpdateType.CallbackQuery
                        }// receive all update types except ChatMember related update.
                    };

                    // Firing telegram bot.
                    botClient.StartReceiving(
                        updateHandler: BotHandler.HandleUpdateAsync,
                    pollingErrorHandler: BotHandler.HandlePollingErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: cts.Token
                );

                    Console.WriteLine("Start listening");
                    Console.ReadLine();

                    // Send cancellation request to stop bot
                    cts.Cancel();
                }
                catch (ArgumentNullException)
                {
                    Log.Warning($"{nameof(Main)} {LogStates.error}");
                    continue;
                }
                catch (PathTooLongException)
                {
                    Log.Warning($"{nameof(Main)} {LogStates.error}");
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
                    Log.Warning($"{nameof(Main)} {LogStates.error}");
                    continue;
                }
                catch (UnauthorizedAccessException)
                {
                    Log.Warning($"{nameof(Main)} {LogStates.error}");
                    continue;
                }
                catch (IOException)
                {
                    Log.Warning($"{nameof(Main)} {LogStates.error}");
                    continue;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Log.Warning($"{nameof(Main)} {LogStates.error}");
                    continue;
                }
                catch (ArgumentException)
                {
                    Log.Warning($"{nameof(Main)} {LogStates.error}");
                    continue;
                }
                catch (NullReferenceException)
                {
                    Log.Warning($"{nameof(Main)} {LogStates.error}");
                    continue;
                }
            }
        }
    }
}

