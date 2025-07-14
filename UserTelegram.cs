using TL;
using WTelegram;

namespace PoScSe
{
    public class UserTelegram
    {

        private Client client = new Client(config);
        private InputPeerUser peer;

        public async Task TakeUserTelegram()
        {

            // Если ещё не авторизованы, будет запрошено: номер, код, возможно пароль
            var user = await client.LoginUserIfNeeded();

            Console.WriteLine($"Успешно зашли как {user.username ?? user.first_name}");

            // Находим бота по username (например, "MyTestBot")
            var resolved = await client.Contacts_ResolveUsername("poker_okbot");
            if (resolved.users.Count == 0)
            {
                Console.WriteLine("Бот не найден");
                return;
            }

            var bot = resolved.users.Values.OfType<TL.User>().First();
            peer = new InputPeerUser(bot.id, bot.access_hash);
        }

        public async Task SendMessage(MemoryStream file)
        {
            var upload = await client.UploadFileAsync(filename: "screenshot.png", stream: file);
            var media = new InputMediaUploadedPhoto();
            media.file = upload;
            await client.SendMessageAsync(peer : peer, media : media, text : "");
            //// Отправляем сообщение
            //var msg = await client.SendMessageAsync(peer, "Привет от C# пользователя!");
        }

        // Метод-конфиг для WTelegramClient. Telegram запросит недостающие данные по необходимости.
        static string config(string what)
        {
            switch (what)
            {
                case "api_id": return "20076832";
                case "api_hash": return "0f0f9a10d7fbdb1a293556b95c503cf6";
                case "phone_number": return "+79876279354";  // ваш номер
                case "verification_code": return "61320";                                             
                default: return null;
            }
        }
    }
}
