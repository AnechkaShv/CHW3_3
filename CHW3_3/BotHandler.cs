using FileWorking;
using Serilog;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace CHW3_3
{
    public class BotHandler
    {
        private static FileChecking? _check;
        private static long _id = -1;
        private static DataProcessingInterface? _data;
        private static WiFiLibrary[]? _wifi;
        private static string[]? values;
        private static string? _button;
        private static string? _fileName;
        
        /// <summary>
        /// This method processed updates of telegram bot.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="update"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Creating inline menues.
            var inlineFile = new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>()
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Download csv file", "button1")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Download json file", "button2")
                }
            });
            var inlineProcessing = new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>()
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Make a selection by value AdmArea", "button2_1"),
                },

                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Make a selection by value WifiName", "button2_2"),
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Make a selection by values FunctionFlag and AccessFlag", "button2_3")
                },
                 new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Sort the table by value LibraryName (alphabetically)", "button2_4")
                },
                  new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Sort the table by value CoverageArea(sort descending)", "button2_5")
                },
            });
            var inlineUpload = new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>()
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Upload as a csv file", "button3_1")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Upload as a json file", "button3_2")
                }
            });
            var inlineAgree = new InlineKeyboardMarkup(new List<InlineKeyboardButton[]>()
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Upload", "button4_1")
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData("Return to the menu and continue processing", "button4_2")
                }
            });
            // Checking message format.
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        // Checking if the message is correct.
                        if (update.Message is not { } message)
                        {
                            _id = update.Id;
                            
                            await botClient.SendTextMessageAsync(
                                _id,
                                "Wrong message.Please try again.",
                                cancellationToken: cancellationToken);

                            // Logging error message.
                            Log.Warning($"{nameof(HandleUpdateAsync)} {LogStates.wrongMessage}");
                            return;
                        }
                        _id = message.Chat.Id;

                        // If it is text message.
                        if (message.Type == MessageType.Text)
                        {
                            // Starting working with bot.
                            if (message.Text == "/start")
                            {

                                await botClient.SendStickerAsync(
                                    chatId: message.Chat.Id,
                                    sticker: InputFile.FromFileId("CAACAgIAAxkBAAELvRRl-EBNEsjBz6qC93kfyc5oyjZ0TQACBQADwDZPE_lqX5qCa011NAQ"),
                                    cancellationToken: cancellationToken);

                                await botClient.SendTextMessageAsync(
                                _id,
                                "Welcome to KDZ bot!",
                                cancellationToken: cancellationToken);

                                // Offering inline menu.
                                await botClient.SendTextMessageAsync(
                                _id,
                                "Please choose an option to download your file.",
                                replyMarkup: inlineFile,
                                cancellationToken: cancellationToken
                               );
                                return;
                            }

                            // If user should coose inline option but text is sended.
                            else if (_data is null)
                            {
                                await botClient.SendTextMessageAsync(
                                _id,
                                "Unrecognizable text.Please try again.",
                                cancellationToken: cancellationToken);

                                // Logging error.
                                Log.Warning($"{nameof(HandleUpdateAsync)} {LogStates.wrongMessage}");
                                return;
                            }

                            // Processing selection.
                            else if(_button == "button2_1" || _button == "button2_2" || _button == "button2_3")
                            {
                                // If ir is 1 field selection.
                                if (_data.Idx2 == -1)
                                {
                                    _wifi = _data.SelectInterface(message.Text);

                                    // Processing wrong selected value.
                                    if (_wifi is null)
                                    {
                                        await botClient.SendTextMessageAsync
                                        (
                                            _id,
                                            "You entered wrong value.Please try again",
                                            cancellationToken: cancellationToken);

                                        // Logging wrong text.
                                        Log.Warning($"{nameof(HandleUpdateAsync)} {LogStates.wrongMessage}");

                                        return;
                                    }
                                    await botClient.SendTextMessageAsync
                                        (
                                        _id,
                                        "You entered correct value!",
                                        cancellationToken: cancellationToken);
                                    
                                    await botClient.SendTextMessageAsync(
                                            _id,
                                            "Data has been successfully processed!",
                                            cancellationToken: cancellationToken);

                                    await botClient.SendStickerAsync(
                                            chatId: _id,
                                            sticker: InputFile.FromFileId("CAACAgIAAxkBAAELvRZl-EFs8iLaAAGzIFXHmEfDVMMrVOsAAh0AA8A2TxNe2KbcQT3eSDQE"),
                                            cancellationToken: cancellationToken);

                                    // Offering inline menu to choose if user wants to upload file or not.
                                    await botClient.SendTextMessageAsync(
                                                _id,
                                                "Do you want to upload processed file?",
                                                replyMarkup: inlineAgree,
                                                cancellationToken: cancellationToken);
                                    return;
                                }

                                // 2 fields selection.
                                // If user enters first value.
                                else if (values is null)
                                {
                                    // Creating array with user's values.
                                    values = new string[2];

                                    // Initializing first value.
                                    values[0] = message.Text;

                                    await botClient.SendTextMessageAsync
                                        (_id,
                                        "You entered correct value! Enter second values",
                                        cancellationToken: cancellationToken);

                                    return;
                                }
                                // User sends second value.
                                else
                                {
                                    // Initializing second value.
                                    values[1] = message.Text;

                                    _wifi = _data.SelectInterface(values[0], values[1]);

                                    // If selection is incorrect.
                                    if (_wifi is null)
                                    {
                                        await botClient.SendTextMessageAsync
                                        (_id,
                                        "You entered wrong value.Please try again",
                                        cancellationToken: cancellationToken);

                                        // Logging error message.
                                        Log.Warning($"{nameof(HandleUpdateAsync)} {LogStates.wrongMessage}");

                                        // Deleting wrong value.
                                        values[1] = null;
                                        return;
                                    }

                                    await botClient.SendTextMessageAsync
                                        (_id,
                                        "You entered correct value!",
                                        cancellationToken: cancellationToken);

                                    await botClient.SendTextMessageAsync(
                                           _id,
                                            "Data has been successfully processed!",
                                            cancellationToken: cancellationToken);

                                    await botClient.SendStickerAsync(
                                            chatId: _id,
                                            sticker: InputFile.FromFileId("CAACAgIAAxkBAAELvRZl-EFs8iLaAAGzIFXHmEfDVMMrVOsAAh0AA8A2TxNe2KbcQT3eSDQE"),
                                            cancellationToken: cancellationToken);

                                    // Offering inline menu to choose if user wants to upload file or not.
                                    await botClient.SendTextMessageAsync(
                                    _id,
                                    "Do you want to upload processed file?",
                                    replyMarkup: inlineAgree,
                                    cancellationToken: cancellationToken);

                                    return;
                                }
                            }
                            Console.WriteLine($"Received a '{message}' message in chat {_id}.");
                            Log.Information($"{nameof(HandleUpdateAsync)} received {message}");
                        }
                        // If user sends document.
                        if (message.Type == MessageType.Document)
                        {
                            Message file = update.Message;
                            _id = file.Chat.Id;

                            // Checking file.
                            if (_check is null || !_check.IsCorrect(file))
                            {
                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"Wrong file. Please download the correct one",
                                    replyToMessageId: update.Message.MessageId,
                                    cancellationToken: cancellationToken
                                    );

                                Log.Warning($"{nameof(HandleUpdateAsync)} {LogStates.error}");

                                return;
                            }

                            
                            _fileName = file.Document.FileName;

                            Console.WriteLine($"Received a '{_fileName}' message in chat {_id}.");
                            Log.Information($"{nameof(HandleUpdateAsync)} received {_fileName}");


                            var fileId = file.Document.FileId;
                            var fileInfo = await botClient.GetFileAsync(fileId);
                            var filePath = fileInfo.FilePath;

                            _wifi = Array.Empty<WiFiLibrary>();

                            _wifi = await _check.ReadData(botClient, cancellationToken, filePath);

                            // If file's data is incorrect.
                            if (_wifi is null || _wifi == Array.Empty<WiFiLibrary>())
                            {
                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"Error while working with file. Sorry",
                                    replyToMessageId: update.Message.MessageId,
                                    cancellationToken: cancellationToken
                                    );

                                Log.Warning($"{nameof(HandleUpdateAsync)} {LogStates.error}");

                                Console.WriteLine("Error in file.");
                                return;
                            }
                            
                            await botClient.SendStickerAsync(
                                chatId: message.Chat.Id,
                                sticker: InputFile.FromFileId("CAACAgIAAxkBAAELvRhl-Elc_B_Xv9_OzsctRXK5UWKIsQACGwADwDZPE329ioPLRE1qNAQ"),
                                cancellationToken: cancellationToken);

                            // Echo received message text
                            await botClient.SendTextMessageAsync(
                                chatId: _id,
                                text: $"Congratulations! You have sent file: {file.Document.FileName} and it is correct!\n",
                                cancellationToken: cancellationToken
                                );

                            await botClient.SendTextMessageAsync(
                                _id,
                                "Please choose an option to process your file.",
                                replyMarkup: inlineProcessing,
                                cancellationToken: cancellationToken
                               );
                            return;
                        }
                        await botClient.SendTextMessageAsync(
                                _id,
                                "You have entered wrong type of message.Please try again.",
                                cancellationToken: cancellationToken);
                        Log.Warning($"{nameof(HandleUpdateAsync)} {LogStates.wrongMessage}");

                        return;
                    }
                    // If it is inline menu
                case UpdateType.CallbackQuery:
                    {

                        var callbackQuery = update.CallbackQuery;

                        _button = callbackQuery.Data;
                        _id = callbackQuery.Message.Chat.Id;

                        Console.WriteLine($"{_id} choose button {callbackQuery.Data}");

                        _check = new FileChecking($"{_button}");

                        switch (_button)
                        {
                            // If user chooses downloading csv file.
                            case "button1":
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                _check = new FileChecking(_button);

                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"You have chosen csv file format! Please download your file.");
                                return;

                            // If user chooses downloading json file.
                            case "button2":
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                _check = new FileChecking(_button);

                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"You have chosen json file format! Please download your file.");
                                return;

                            // If user chooses selecting AdmArea.
                            case "button2_1":
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"You have chosen AdmArea selection!");

                                _data = new DataProcessingInterface(1, _wifi);

                                Log.Information($"{nameof(HandleUpdateAsync)} {LogStates.selectionAdm}");

                                // Asking for value.
                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"Please enter value for selecting");
                                return;

                            // If user chooses selecting wifi name.
                            case "button2_2":
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"You have choosen WifiName selection!");

                                Log.Information($"{nameof(HandleUpdateAsync)} {LogStates.selectionWifi}");

                                _data = new DataProcessingInterface(2, _wifi);

                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"Please enter value for selecting");
                                return;

                            // If users chooses selecting Access flag and Function flag.
                            case "button2_3":
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"You have chosen FunctionFlag and AccessFlag for selection!");
                                Log.Information($"{nameof(HandleUpdateAsync)} {LogStates.selectionFlag}");

                                _data = new DataProcessingInterface(3, 4, _wifi);
                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"Enter value for selecting");
                                return;

                            // If users chooses sorting of Library name.
                            case "button2_4":
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"You have chosen LibraryName for sorting alphabetically");

                                Log.Information($"{nameof(HandleUpdateAsync)} {LogStates.sortLib}");

                                _data = new DataProcessingInterface(5, _wifi);
                                _wifi = _data.SortInterface();
                                
                                break;

                            // If users chooses sorting of Library name.
                            case "button2_5":
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                await botClient.SendTextMessageAsync(
                                    _id,
                                    $"You have chosen CoverageArea for sorting descendingly");

                                Log.Information($"{nameof(HandleUpdateAsync)} {LogStates.sortArea}");

                                _data = new DataProcessingInterface(6, _wifi);
                                _wifi = _data.SortInterface();
                                
                                break;

                            // If users chooses uploading file.
                            case "button4_1":
                                await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                                // Offering inline menu of options of uploading.
                                await botClient.SendTextMessageAsync(
                                    _id,
                                    "Please chose option how you want to upload processed file.",
                                    replyMarkup: inlineUpload,
                                    cancellationToken: cancellationToken);
                                return;

                            // If user chooses not to save new file.
                            case "button4_2":

                                await botClient.SendTextMessageAsync(
                                    _id,
                                    "Returning to the menu...",
                                    replyMarkup: inlineProcessing,
                                    cancellationToken: cancellationToken);
                                return;

                            // Uploading csv or json.
                            case "button3_1" or "button3_2":

                                Message m = await botClient.SendTextMessageAsync(
                                    _id,
                                    "Uploading file...",
                                    cancellationToken: cancellationToken);

                                await using (Stream stream = await _check.WriteData(botClient, cancellationToken, _wifi))
                                {
                                    // Checking if recording data is correct.
                                    if (stream is null)
                                        return;
                                    
                                    // Csv uploading.
                                    if (_button == "button3_1")
                                    {
                                        int i = 1;

                                        // Creating new file with unique name.
                                        while (System.IO.File.Exists($"wifi-library{i}.csv"))
                                        {
                                            i += 1;
                                        }

                                        await botClient.SendDocumentAsync(
                                            _id,
                                            InputFile.FromStream(stream, $"wifi-library{i}.csv"));

                                        await botClient.DeleteMessageAsync(
                                            _id,
                                            m.MessageId,
                                            cancellationToken: cancellationToken);

                                        Log.Information($"{nameof(HandleUpdateAsync)} {LogStates.uploadCsv}");
                                    }
                                    // Json uploading.
                                    else
                                    {
                                        int i = 1;
                                        while (System.IO.File.Exists($"wifi-library{i}.json"))
                                        {
                                            i += 1;
                                        }
                                        await botClient.SendDocumentAsync(
                                            _id,
                                            InputFile.FromStream(stream, $"wifi-library{i-1}.json"));
                                        await botClient.DeleteMessageAsync(
                                            _id,
                                            m.MessageId,
                                            cancellationToken: cancellationToken);
                                        Log.Information($"{nameof(HandleUpdateAsync)} {LogStates.uploadJson}");

                                    }
                                }
                                await botClient.SendTextMessageAsync(
                                    chatId: _id,
                                    "This is your file!",
                                    cancellationToken: cancellationToken);

                                await botClient.SendStickerAsync(
                                    chatId: _id,
                                    sticker: InputFile.FromFileId("CAACAgIAAxkBAAELvvhl-XrAHHmuDeTbU8UmjM3Q5hnA1gACDQADwDZPE6T54fTUeI1TNAQ"),
                                    cancellationToken: cancellationToken);

                                // Restarting bot.
                                await botClient.SendTextMessageAsync(
                                _id,
                                "Tap /start to restart bot",
                                cancellationToken: cancellationToken);
                                return;
                        };

                        if (_button != "button1" && _button != "button2")
                        {
                            await botClient.SendTextMessageAsync(
                            _id,
                            "Data has been successfully processed!",
                            cancellationToken: cancellationToken);

                            await botClient.SendStickerAsync(
                                    chatId: _id,
                                    sticker: InputFile.FromFileId("CAACAgIAAxkBAAELvRZl-EFs8iLaAAGzIFXHmEfDVMMrVOsAAh0AA8A2TxNe2KbcQT3eSDQE"),
                                    cancellationToken: cancellationToken);

                            // Offering inline menu of options how to save data.
                            await botClient.SendTextMessageAsync(
                            _id,
                            "Do you want to upload processed file?",
                            replyMarkup: inlineAgree,
                            cancellationToken: cancellationToken);
                        }
                        return;
                    }

            }
            // Restarting bot.
            await botClient.SendTextMessageAsync(
                                _id,
                                "Tap /start to restart bot",
                                cancellationToken: cancellationToken);
            return;

        }
        /// <summary>
        /// This method processed errors while working with bot.
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="exception"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        
    }
}
