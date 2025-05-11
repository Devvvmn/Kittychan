# 📷 WallpaperCLI — загрузка и установка обоев из Konachan

**WallpaperCLI** — это консольное приложение на C# под Linux, которое позволяет:

- 🔍 Получать превью обоев с Konachan
- ⬇️ Скачивать изображения в нужную папку
- 🖼️ Устанавливать изображение как обои рабочего стола (`swww`)
- 🐈 Работает в `kitty` с поддержкой `icat` для превью

---

## ⚙️ Зависимости

- [.NET 8+](https://dotnet.microsoft.com/)
- [`kitty`](https://sw.kovidgoyal.net/kitty/) с включённым `kitten icat`
- [`swww`](https://github.com/LionyxML/swww) (Wayland wallpaper daemon)
- Linux-дистрибутив с поддержкой Wayland (например, Hyprland)
- Подключение к интернету

---

## 📦 Установка и запуск

1. Клонируй репозиторий:
```bash
git clone https://github.com/Devvvmn/WallpaperCLI.git
cd WallpaperCLI
dotnet build
Запусти:
dotnet run

📁 Структура проекта
KonachanPost.cs — модель данных изображения

ImageRenderer.cs — логика загрузки, сохранения, отображения, установки

Program.cs — точка входа (здесь запускается всё волшебство)

💡 Планы на будущее

 Интерфейс выбора из списка (с превью)

 Поддержка других источников

 Кэш изображений

🛡️ Лицензия
MIT😉

