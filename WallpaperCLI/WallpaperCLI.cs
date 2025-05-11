using Newtonsoft.Json;

namespace WallpaperCLI;
public class WallpaperCLI
{
    //использование фабрики - избыточно, поэтому мы создаем статичный клиент, который получает актуальный днс
    //каждые 2 минуты
    private static readonly HttpClient client = new HttpClient
    {
        Timeout = TimeSpan.FromMinutes(3)
    };
    public static async Task<string> GetXMLElementAsync(string url)
    {
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public (List<string>, bool) GetTagsFromUser()
    {
        Console.WriteLine("Введите через запятую теги, которые хотите получить в выводе: \n" +
                          "Для вывода без тегов, оставьте поле пустым");
        string tagInput = Console.ReadLine();
        bool tagsHave = !string.IsNullOrWhiteSpace(tagInput);
        List<string> tags = tagsHave
            ? tagInput.ToLower().Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList()
            : new List<string>();

        return (tags, tagsHave);
    }

    public string LinkConstructor()
    {
        string urlBase = "https://konachan.net/post.json?";
        string url = urlBase;
        List<string> tags = new List<string>(); // Храним теги
        int limit = 0; // Значения по умолчанию
        int page = 0;
        string rating = "";

        Console.WriteLine("Введите параметры выдачи, которые хотите настроить (exit для завершения):\n" +
                          "Доступные параметры: tags, limit, page, rating.");

        while (true)
        {
            string input = Console.ReadLine()?.ToLower(); // Добавлена проверка на null

            switch (input)
            {
                case "exit":
                    return url; // Возвращаем построенный URL
                case "tags":
                    (List<string> newTags, bool tagsHave) = GetTagsFromUser();
                    if (tagsHave)
                    {
                        tags.AddRange(newTags);
                        url += $"&tags={string.Join("%20", tags)}"; // Добавляем теги в URL
                    }
                    break;
                case "limit":
                    Console.WriteLine("Целевое колличество работ (только целые числа):");
                    if (int.TryParse(Console.ReadLine(), out limit))
                    {
                        url += $"&limit={limit}";
                    }
                    else
                    {
                        Console.WriteLine("Некорректный ввод. Параметр limit пропущен.");
                    }
                    break;
                case "page":
                    Console.WriteLine("Целевая страница (только целые числа):");
                    if (int.TryParse(Console.ReadLine(), out page))
                    {
                        url += $"&page={page}";
                    }
                    else
                    {
                        Console.WriteLine("Некорректный ввод. Параметр page пропущен.");
                    }
                    break;
                case "rating":
                    Console.WriteLine("Включить безопасный режим? (y/n):");
                    string ratingInput = Console.ReadLine()?.ToLower(); // Добавлена проверка на null
                    if (ratingInput == "y" || string.IsNullOrWhiteSpace(ratingInput))
                    {
                        rating = "safe";
                        url += $"&rating={rating}";
                    }
                    else if (ratingInput == "n")
                    {
                        Console.WriteLine("Безопасный режим отключен.");
                    }
                    else
                    {
                        Console.WriteLine("Некорректный ввод. Параметр rating пропущен.");
                    }
                    break;
                default:
                    Console.WriteLine("Неизвестная команда.");
                    break;
            }
        }
    }

    public static async Task Main(string[] args)
    {
        WallpaperCLI wallpaper = new WallpaperCLI();
        ImageRenderer imageRenderer = new ImageRenderer();

        while (true)
        {
            string url = wallpaper.LinkConstructor();
            string json = await GetXMLElementAsync(url);
            KonachanPost[] posts = JsonConvert.DeserializeObject<KonachanPost[]>(json);

            int number = 0;
            
            foreach (KonachanPost post in posts)
            {
                Console.WriteLine($"============================================================================\n" +
                                  $"Превью: \n" +
                                  $"Номер поста {number}");
                await imageRenderer.RenderImage(post.preview_url);
                Console.WriteLine($"Рейтинг {post.rating}\n" +
                                  $"ID {post.id}\n" +
                                  $"Автор{post.author}\n" +
                                  $"Ссылка на изображение {post.file_url}\n" +
                                  $"============================================================================");
                number++;
            }
            
            //подсказки
            Console.WriteLine("Для выхода из программы введите: 'e!'.\n" +
                              "Для загрузки изображения введите команду 'dw'.\n" +
                              "Что бы установить обои из поста введите  команду 'sw'.");
            
            if (Console.ReadLine() == "e!")
                break;
            else if (Console.ReadLine() == "dw")
            {
                Console.WriteLine("Номер поста:");
                string id = Console.ReadLine();
                await imageRenderer.DownloadImageAsync(posts[int.Parse(id)].file_url);
            }

            else if (Console.ReadLine() == "sw")
            {
                Console.WriteLine("Номер поста:");
                string id = Console.ReadLine();
                imageRenderer.SetWallpaper(posts[int.Parse(id)].file_url, "fill");
            }
        }
    }
}