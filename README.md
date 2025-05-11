# WallpaperCLI

WallpaperCLI is a C# console application that allows users to download, view, and set wallpapers from the [Konachan](https://konachan.net/) website. It leverages asynchronous methods and HTTP requests to provide an efficient and smooth experience. The app also utilizes external utilities to display images in the terminal and set wallpapers.

## Features:

- Search for images by tags, limits (count, page), and rating.
- Download images and save them locally.
- View images in the console using the `kitty` utility.
- Set wallpapers using the `swww` utility.

## Requirements:

- **.NET 6.0** or higher
- Utilities:
  - `kitty` (to display images in the terminal)
  - `swww` (to set wallpapers)

## Installation:

1. Clone the repository:
    ```bash
    git clone https://github.com/yourusername/Kittychan.git
    ```

2. Navigate to the project directory:
    ```bash
    cd Kittychan
    ```

3. Restore dependencies and build the project:
    ```bash
    dotnet build
    ```

4. Run the application:
    ```bash
    dotnet run
    ```

## Usage:

Once the program is running, it will ask for search parameters such as tags, image count, page number, and rating.


### Notes:

- The program uses the `kitty` utility to display images in the terminal.
- The `swww` utility is used to set wallpapers.

### License:

-MIT License
