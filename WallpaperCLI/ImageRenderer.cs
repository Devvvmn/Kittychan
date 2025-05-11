using System.Diagnostics;

namespace WallpaperCLI;

public class ImageRenderer
{
    private static readonly HttpClient clientRender = new HttpClient()
    {
        Timeout = TimeSpan.FromMinutes(3)
    };
    public async Task RenderImage(string previewUrl)
    {
        if (string.IsNullOrWhiteSpace(previewUrl))
        {
            Console.WriteLine("Превью отсутствует!");
            return;
        }
        string tempFilePath = Path.GetTempFileName();
        try
        {
            var imageBytes = await clientRender.GetByteArrayAsync(previewUrl);
            await File.WriteAllBytesAsync(tempFilePath, imageBytes);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "kitty",
                Arguments = $"+kitten icat {tempFilePath}",
                UseShellExecute = false, // Важно для перенаправления потоков
                CreateNoWindow = true    // Не показывать окно консоли для kitty
            };

            using (Process process = Process.Start(psi))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync(); // Ожидаем завершения процесса kitty
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка вывода или загрузки превью: {e}");
            throw;
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    public string PathBuilder(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            Console.WriteLine("Ошибка получения ссылки для загрузки!");
        }

        //работа с путем
        Console.WriteLine("Укажите путь к папке для сохранения изображения:");
        string path = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(path))
        {
            Console.WriteLine("Путь не может быть пустым. Пожалуйста, введите путь.");
            return null;
        }
        
        string fullPath = "";
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"Папка '{path}' успешно создана.");
            }
            else
            {
                Console.WriteLine($"Папка '{path}' уже существует.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при работе с директорией: {ex.Message}");
            return null;
        }
        
        //работа с именем
        string fileName = "";
        try
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                Uri uri = new Uri(imageUrl);
                fileName = Path.GetFileName(uri.LocalPath);
                fileName = System.Text.RegularExpressions.Regex.Replace(fileName, "[^a-zA-Z0-9_.-]+", "_");
            }
            else
            {
                fileName = "image.jpg";
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ссылки не существует!");
        }
        
        fullPath = Path.Combine(path, fileName);
        return fullPath;
    }
    public async Task DownloadImageAsync(string imageUrl)
    {
        string path = PathBuilder(imageUrl);
        try
        {
            var imageBytes = await clientRender.GetByteArrayAsync(imageUrl);
            await File.WriteAllBytesAsync(path, imageBytes);
            Console.WriteLine($"Изображение успешно сохранено в: {path}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при загрузке или сохранении изображения: {e.Message}");
            throw;
        }
    }

    public async Task SetWallpaper(string imageUrl, string layout = "fill")
    {
        string path = PathBuilder(imageUrl);
        try
        {
            var imageBytes = await clientRender.GetByteArrayAsync(imageUrl);
            await File.WriteAllBytesAsync(path, imageBytes);
            Console.WriteLine($"Изображение успешно сохранено в: {path}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при загрузке или сохранении изображения: {e.Message}");
            throw;
        }
        ProcessStartInfo psi = new ProcessStartInfo()
        {
            FileName = "swww",
            Arguments = $"img {path}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        using (Process process = Process.Start(psi))
        {
            if (process != null)
            {
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine(error);
                }

                if (!string.IsNullOrWhiteSpace(output))
                {
                    Console.WriteLine(output);
                }

                if (process.ExitCode != 0)
                {
                    Console.WriteLine($"Exit code: {process.ExitCode}");
                }
            }
        }
    }
}