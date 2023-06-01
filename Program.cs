using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using static RaceBT_.DriverInfo;
using static RaceBT_.LapTimeInfo;
using static RaceBT_.RaceSchInfo;
using static RaceBT_.CircuitInfo;

class Program
{
    static ITelegramBotClient bot = new TelegramBotClient(constants.botId);
    static HttpClient httpClient = new HttpClient();
    //static Dictionary<long, List<List<CircuitData>>> favoriteCircuits = new Dictionary<long, List<List<CircuitData>>>();
    //static List<List<CircuitData>> favoriteCircuits = new List<List<CircuitData>>();
    static List<string> favoriteCircuits = new List<string>();

    public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message != null && !string.IsNullOrEmpty(update.Message.Text))
        {
            var message = update.Message;
            User user = message.From;
            string user_firstname = user.FirstName;
            long user_id = user.Id;

            var document = new BsonDocument
                    {
                        { "user_id", user_id},
                        { "user_firstname", user_firstname },
                {"bot_is_waiting_for_racer_lastname", false },
                {"bot_is_waiting_for_type_info", false},
                {"bot_is_waiting_for_year",false },
                {"user_is_subscribed", false },
                {"bot_is_waiting_for_circuit_year", false },
                {"bot_is_waiting_for_add", false },
                {"bot_is_waiting_for_delete", false },
                
                };

            var filter = Builders<BsonDocument>.Filter.Eq("user_id", user_id);
            var exists = constants.collection.Find(filter).Any();

            if (!exists)
            {
                constants.collection.InsertOne(document);
            }

            var resp1 = await httpClient.GetAsync($"https://{constants.host}/DriverInformation/bot_is_waiting_for_racer_lastname/{user_id}");
            var res1 = await resp1.Content.ReadAsStringAsync();
            bool bot_is_waiting_for_racer_lastname = Convert.ToBoolean(res1);

            var resp2 = await httpClient.GetAsync($"https://{constants.host}/LapTimeInformation/bot_is_waiting_for_type_info/{user_id}");
            var res2 = await resp2.Content.ReadAsStringAsync();
            bool bot_is_waiting_for_type_info = Convert.ToBoolean(res2);

            var resp3 = await httpClient.GetAsync($"https://{constants.host}/RaceSchedule/bot_is_waiting_for_year/{user_id}");
            var res3 = await resp3.Content.ReadAsStringAsync();
            bool bot_is_waiting_for_year = Convert.ToBoolean(res3);

            var resp4 = await httpClient.GetAsync($"https://{constants.host}/Subscribe/user_is_subscribed/{user_id}");
            var res4 = await resp4.Content.ReadAsStringAsync();
            bool user_is_subscribed = Convert.ToBoolean(res4);

            var resp5 = await httpClient.GetAsync($"https://{constants.host}/CircuitInformation/bot_is_waiting_for_circuit_year/{user_id}");
            var res5 = await resp5.Content.ReadAsStringAsync();
            bool bot_is_waiting_for_circuit_year = Convert.ToBoolean(res5);

            var resp6 = await httpClient.GetAsync($"https://{constants.host}/FavoriteCircuits/bot_is_waiting_for_add/{user_id}");
            var res6 = await resp6.Content.ReadAsStringAsync();
            bool bot_is_waiting_for_add = Convert.ToBoolean(res6);

            //var resp7 = await httpClient.GetAsync($"https://localhost:7162/FavoriteCircuits/bot_is_waiting_for_circuit_num/{user_id}");
            //var res7 = await resp7.Content.ReadAsStringAsync();
            //bool bot_is_waiting_for_circuit_num = Convert.ToBoolean(res7);

            var resp8 = await httpClient.GetAsync($"https://{constants.host}/FavoriteCircuits/bot_is_waiting_for_delete/{user_id}");
            var res8 = await resp8.Content.ReadAsStringAsync();
            bool bot_is_waiting_for_delete = Convert.ToBoolean(res8);

            if (message.Text.ToLower() == "/start")
            {
                await botClient.SendTextMessageAsync(message.Chat, $"✌Привіт, {user_firstname}! Що саме ти хочеш дізнатися?\n " +
                    $"\n/search_racer - Пошук гонщика по прізвищу" +
                    $"\n/search_lap - Пошук заїзду" +
                    $"\n/get_schedule - Вивести розклад гонок" +
                    $"\n/get_circuit_info - Використання трас по рокам" +
                    $"\n/get_nextrace - Найближча гонка" +
                    $"\n/subscribe - Підписатися на сповіщення про найближчу гонку" +
                    $"\n/unsubscribe - Відписатися від сповіщень" +
                    $"\n/show_favourites - Список збережених трас" +
                    $"\n/add_to_favourites - Додати трасу в обране" +
                    $"\n/delete_from_favourites - Видалити трасу з обраного" 
                    );
                
                return;
            }
            if (message.Text.ToLower() == "/search_racer")
            {
                await botClient.SendTextMessageAsync(user_id, "✦ Введіть прізвище гонщика (англ)");
                await httpClient.PutAsync($"https://{constants.host}/DriverInformation/bot_is_waiting_for_racer_lastname/{user_id}?b=true", null);
                return;
            }
            if (bot_is_waiting_for_racer_lastname)
            {
                string racer_name = message.Text;
                var response = await httpClient.GetAsync($"https://{constants.host}/DriverInformation/get_driver_info?last_name={racer_name}");
                var result = await response.Content.ReadAsStringAsync();
                DriverData driverData = JsonConvert.DeserializeObject<DriverData>(result);

                if (driverData != null && driverData.MRData != null && driverData.MRData.DriverTable != null && driverData.MRData.DriverTable.Drivers.Any())
                {
                    var driver = driverData.MRData.DriverTable.Drivers.FirstOrDefault();
                    string messaage = $"▪️Ім'я гонщика: {driver.GivenName}" +
                        $"\n▪️Прізвище гонщика: {driver.FamilyName}" +
                        $"\n▪️Дата народження: {driver.DateOfBirth}" +
                        $"\n▪️Номер: {driver.PermanentNumber}" +
                        $"\n▪️Країна: {driver.Nationality}" +
                        $"\n▪️Сторінка на Вікіпедії: {driver.Url}";
                        await botClient.SendTextMessageAsync(user_id, messaage);
                }
                else
                {
                    await botClient.SendTextMessageAsync(user_id, "Інформація про гонщика не знайдена");
                }
                await httpClient.PutAsync($"https://{constants.host}/DriverInformation/bot_is_waiting_for_racer_lastname/{user_id}?b=false", null);
                return;
            }

            if (message.Text.ToLower() == "/search_lap")
            {
                await botClient.SendTextMessageAsync(user_id, "✦ Введіть рік, раунд и номер круга через пробіл (наприклад, '2011 5 1'):");
                await httpClient.PutAsync($"https://{constants.host}/LapTimeInformation/bot_is_waiting_for_type_info/{user_id}?b=true", null);
                return;
            }
            if (bot_is_waiting_for_type_info)
            {
                string[] lapParams = message.Text.Split(' ');
                if (lapParams.Length == 3 && int.TryParse(lapParams[0], out int year) && int.TryParse(lapParams[1], out int round) && int.TryParse(lapParams[2], out int lapNumber))
                {
                    var response = await httpClient.GetAsync($"https://{constants.host}/LapTimeInformation/get_laptime_info?year={year}&round={round}&lapNumber={lapNumber}");
                    var result = await response.Content.ReadAsStringAsync();
                    LapData lapData = JsonConvert.DeserializeObject<LapData>(result);

                    string circuitName = lapData.MRData.RaceTable.Races[0].Circuit.CircuitName;
                    string country = lapData.MRData.RaceTable.Races[0].Circuit.Location.Country;
                    string driverId = lapData.MRData.RaceTable.Races[0].Laps[0].Timings[0].DriverId;
                    string position = lapData.MRData.RaceTable.Races[0].Laps[0].Timings[0].Position;
                    string lapTime = lapData.MRData.RaceTable.Races[0].Laps[0].Timings[0].Time;

                    await botClient.SendTextMessageAsync(user_id, $"▪️Інформація про круг:\n" +
                        $"▪️Назва траси: {circuitName}\n" +
                        $"▪️Країна: {country}\n" +
                        $"▪️Ідентифікатор гонщика: {driverId}\n" +
                        $"▪️Позиція: {position}\n" +
                        $"▪️Час кругу: {lapTime}");
                } 
                else
                {
                    await botClient.SendTextMessageAsync(user_id, "Інформація не знайдена");
                }
                await httpClient.PutAsync($"https://{constants.host}/LapTimeInformation/bot_is_waiting_for_type_info/{user_id}?b=false", null);
                return;
            }
            if (message.Text.ToLower() == "/get_schedule")
            {
                await botClient.SendTextMessageAsync(user_id, "✦ Введіть рік гонок (наприклад, '2022'):");
                await httpClient.PutAsync($"https://{constants.host}/RaceSchedule/bot_is_waiting_for_year/{user_id}?b=true", null);
                return;
            }
            if (bot_is_waiting_for_year)
            {
                string year = message.Text;
                var response = await httpClient.GetAsync($"https://{constants.host}/RaceSchedule/get_schedule_info?year={year}");
                var result = await response.Content.ReadAsStringAsync();
                RaceData raceData = JsonConvert.DeserializeObject<RaceData>(result);
                if (raceData != null && raceData.MRData != null && raceData.MRData.RaceTable != null && raceData.MRData.RaceTable.Races != null)
                {
                    string scheduleMessage = $"Розклад гонок на {year}:\n\n";
                    for (int i = 0; i < raceData.MRData.RaceTable.Races.Count; i++)
                    {
                        var race = raceData.MRData.RaceTable.Races[i];
                        string raceName = race.RaceName;
                        string circuitName = race.Circuit.CircuitName;
                        string country = race.Circuit.Location.Country;
                        string date = race.Date;
                        string time = race.Time;

                        scheduleMessage += $"{i + 1}. ▪️Гонка: {raceName}\n";
                        scheduleMessage += $"   ▪️Траса: {circuitName}\n";
                        scheduleMessage += $"   ▪️Країна: {country}\n";
                        scheduleMessage += $"   ▪️Дата і час: {date} {time}\n\n";
                    }

                    await botClient.SendTextMessageAsync(user_id, scheduleMessage);
                }
                else if (raceData == null && raceData.MRData == null && raceData.MRData.RaceTable == null && raceData.MRData.RaceTable.Races == null)
                {
                    await botClient.SendTextMessageAsync(user_id, $"Розклад гонок по року {year} не знайдено.");
                }
                await httpClient.PutAsync($"https://{constants.host}/RaceSchedule/bot_is_waiting_for_year/{user_id}?b=false", null);
                return;
            }

            if (message.Text.ToLower() == "/get_nextrace")
            {
                var response = await httpClient.GetAsync("http://ergast.com/api/f1/current/next.json");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var raceData = JsonConvert.DeserializeObject<RaceData>(content);
                var race = raceData.MRData.RaceTable.Races.FirstOrDefault();
                if (race != null)
                {
                    var raceInfo = $"▪️Найближча гонка:\n" +
                                   $"▪️Назва: {race.RaceName}\n" +
                                   $"▪️Траса: {race.Circuit.CircuitName}\n" +
                                   $"▪️Країна: {race.Circuit.Location.Country}\n" +
                                   $"▪️Дата: {race.Date}\n" +
                                   $"▪️Час: {race.Time}";

                    await botClient.SendTextMessageAsync(message.Chat.Id, raceInfo);
                    if (!user_is_subscribed)
                    {
                        await botClient.SendTextMessageAsync(user_id, "Хочеш отримувати сповіщення про найближчу гонку кожен день? Тисни /subscribe 😏!");
                    }
                    return;

                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Інформація про найближчу гонку не знайдена");
                    return;
                }
            }
            if (message.Text.ToLower() == "/subscribe")
            {
                await httpClient.PutAsync($"https://{constants.host}/Subscribe/user_is_subscribed/{user_id}?b=false", null);
                if (user_is_subscribed)
                {
                    await botClient.SendTextMessageAsync(user_id, "Ви вже підписані, не треба двічі");
                }
                else
                {
                    await botClient.SendTextMessageAsync(user_id, "Підписка оформлена!");
                    await httpClient.PutAsync($"https://{constants.host}/Subscribe/user_is_subscribed/{user_id}?b=true", null);
                }
                return;
            }
            if (message.Text.ToLower() == "/unsubscribe")
            {
                if (!user_is_subscribed)
                {
                    await botClient.SendTextMessageAsync(user_id, "Ви навіть не підписані, щоб відписатися");
                }
                else
                {
                    await botClient.SendTextMessageAsync(user_id, "Підписка відмінена(");
                    await httpClient.PutAsync($"https://{constants.host}/Subscribe/user_is_subscribed/{user_id}?b=false", null);
                }
                return;
            }
            if (message.Text.ToLower() == "/get_circuit_info")
            {
                await botClient.SendTextMessageAsync(user_id, "✦ Введіть рік (наприклад, '2022'):");
                await httpClient.PutAsync($"https://{constants.host}/CircuitInformation/bot_is_waiting_for_circuit_year/{user_id}?b=true", null);
                return;
            }
            if (message.Text.ToLower() == "/add_to_favourites")
            {
                await botClient.SendTextMessageAsync(user_id, "✦ Введіть рік та трасу через кому (наприклад, '2022, 3'):");
                await httpClient.PutAsync($"https://{constants.host}/FavoriteCircuits/bot_is_waiting_for_add/{user_id}?b=true", null);
                return;
            }

            if (message.Text.ToLower() == "/show_favourites")
            {
                if (favoriteCircuits.Count == 0)
                {
                    await botClient.SendTextMessageAsync(user_id, "Список обраних трас пустий");
                }
                else
                {
                    string messagee = "Список обраних трас:\n";
                    for (int i = 0; i < favoriteCircuits.Count; i++)
                    {
                        string circuitName = favoriteCircuits[i];
                        messagee += $"{i + 1}. {circuitName}\n";
                    }
                    await botClient.SendTextMessageAsync(user_id, messagee);
                }
                return;
            }
            if (bot_is_waiting_for_add)
            {
                string input = message.Text;
                string[] parts = input.Split(',');
                string year;
                int number;

                if (parts.Length == 2)
                {
                    year = parts[0];
                    number = Convert.ToInt32(parts[1]);

                    var response = await httpClient.GetAsync($"https://{constants.host}/CircuitInformation/get_circuit_info?year={year}");
                    var result = await response.Content.ReadAsStringAsync();

                    CircuitData circuitDataList = JsonConvert.DeserializeObject<CircuitData>(result);

                    if (number >= 1 && number <= circuitDataList.MRData.CircuitTable.Circuits.Count)
                    {
                        string circuitName = circuitDataList.MRData.CircuitTable.Circuits[number - 1].CircuitName;

                        // Проверяем, есть ли трасса уже в списке избранных
                        if (!favoriteCircuits.Contains(circuitName))
                        {
                            favoriteCircuits.Add(circuitName);

                            string message_ = $"Траса \"{circuitName}\" успішно додана під номером {favoriteCircuits.Count}.";
                            await botClient.SendTextMessageAsync(user_id, message_);
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(user_id, "Ця траса вже є у обраному");
                        }
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(user_id, "Неправильний номер траси");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(user_id, "Неправильний формат вводу");
                }

                await httpClient.PutAsync($"https://{constants.host}/FavoriteCircuits/bot_is_waiting_for_add/{user_id}?b=false", null);
                return;

            }


            if (bot_is_waiting_for_circuit_year)
            {
                string year = message.Text;
                var response = await httpClient.GetAsync($"https://{constants.host}/CircuitInformation/get_circuit_info?year={year}");
                var result = await response.Content.ReadAsStringAsync();

                CircuitData circuitDataList = JsonConvert.DeserializeObject<CircuitData>(result);
                if (circuitDataList != null && circuitDataList.MRData != null && circuitDataList.MRData.CircuitTable != null && circuitDataList.MRData.CircuitTable.Circuits.Any())
                {
                    int i = 0;
                    foreach (var o in circuitDataList.MRData.CircuitTable.Circuits)
                    {
                        await botClient.SendTextMessageAsync(user_id, $"▪️Номер траси:{++i}\n" +
                            $"▪️Траса:{o.CircuitName}\n" +
                            $"▪️Місцезнаходження:{o.Location.Locality}{o.Location.Country}\n" +
                            $"▪️Url:{o.Url}");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(user_id, "Траси по заданому року не знайдено");
                }
                await httpClient.PutAsync($"https://{constants.host}/CircuitInformation/bot_is_waiting_for_circuit_year/{user_id}?b=false", null);
                return;

            }
            if (message.Text.ToLower() == "/delete_from_favourites")
            {
                if (favoriteCircuits.Count == 0)
                {
                    await botClient.SendTextMessageAsync(user_id, "Список обраних трас пустий");
                }
                else
                {
                    await botClient.SendTextMessageAsync(user_id, "✦Введіть номер траси, яку ви хочете видалити:");
                    await httpClient.PutAsync($"https://{constants.host}/FavoriteCircuits/bot_is_waiting_for_delete/{user_id}?b=true", null);
                }
                return;
            }

            if (bot_is_waiting_for_delete)
            {
                int number;

                if (int.TryParse(message.Text, out number))
                {
                    if (number >= 1 && number <= favoriteCircuits.Count)
                    {
                        string circuitName = favoriteCircuits[number - 1];
                        favoriteCircuits.RemoveAt(number - 1);

                        string message_ = $"Траса \"{circuitName}\" успішно видалена";
                        await botClient.SendTextMessageAsync(user_id, message_);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(user_id, "Неправильний номер траси");
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(user_id, "Неправильний формат вводу.");
                }
                return;
            }


                await botClient.SendTextMessageAsync(user_id, "Я тебе не розумію");
            return;
        }
        else
        {
            if (update.Message != null && update.Message.From != null)
            {
                long user_id = update.Message.From.Id;
                await botClient.SendTextMessageAsync(user_id, "Не ломай 😡");
            }
        }

    }
    public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
    }
    public static async Task DailyUpdate()
    {
        ITelegramBotClient bott = new TelegramBotClient(constants.botId);
        while (true)
        {
            if (DateTime.UtcNow.Hour == 9 && DateTime.UtcNow.Minute == 0)
            {
                var filter = Builders<BsonDocument>.Filter.Empty;
                var documents = constants.collection.Find(filter).ToList();

                foreach (var document in documents)
                {
                    if (Convert.ToBoolean(document["user_is_subscribed"]) == true)
                    {
                        long user_id = Convert.ToInt64(document["user_id"]);
                        await httpClient.PostAsync($"https://{constants.host}/NextRace/post_next_info", null);

                        bott.SendTextMessageAsync(user_id, "Для відписки натисніть /unsubscribe");
                    }
                }
                DateTime nextExecutionTime = DateTime.UtcNow.Date.AddDays(1).AddHours(9);
                TimeSpan sleepTime = nextExecutionTime - DateTime.UtcNow;

                Thread.Sleep(sleepTime);
            }
        }
    }
   
    static void Main(string[] args)
    {

        Task.Run(async () => await DailyUpdate());
        Console.WriteLine("Запущен бот" + bot.GetMeAsync().Result.FirstName);

        constants.mongoClient = new MongoClient("mongodb+srv://lawssnx:kofois6102@racebt.kaiwb3h.mongodb.net/");
        constants.database = constants.mongoClient.GetDatabase("RaceBT");
        constants.collection = constants.database.GetCollection<BsonDocument>("RaceS");

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { },
        };
        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
        //Console.ReadLine();
    }
}
