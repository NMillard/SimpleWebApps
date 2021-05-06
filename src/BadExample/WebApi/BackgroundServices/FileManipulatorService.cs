using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace WebApi.BackgroundServices {
    public class FileManipulatorService : BackgroundService {
        private readonly string folder = Path.Join(Directory.GetCurrentDirectory(), "staging");
        private readonly string processed = Path.Join(Directory.GetCurrentDirectory(), "uploads");

        protected async override Task ExecuteAsync(CancellationToken stoppingToken) {
            
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            if (!Directory.Exists(processed)) Directory.CreateDirectory(processed);

            var watcher = new FileSystemWatcher(folder) {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
            };

            watcher.Created += async (sender, args) => await ProcessImageAsync(args.FullPath, args.Name, stoppingToken);
        }

        private async Task ProcessImageAsync(string path, string imageName, CancellationToken stoppingToken) {
            bool isReady = false;
            while (!isReady && !stoppingToken.IsCancellationRequested) {
                try {
                    await using FileStream image = File.OpenRead(path);
                    isReady = true;
                    
                    var bitmap = new Bitmap(image);

                    // Turn colored image into greyscale
                    for (int x = 0; x < bitmap.Width; x++) {
                        for (int y = 0; y < bitmap.Height; y++) {
                            Color color = bitmap.GetPixel(x, y);
                            int grayScale = (color.R + color.G + color.B) / 3;
                            Color greyScale = Color.FromArgb(grayScale, grayScale, grayScale);
                            bitmap.SetPixel(x, y, greyScale);
                        }
                    }

                    string processedPath = Path.Join(processed, Path.GetDirectoryName(imageName));
                    if (!Directory.Exists(processedPath)) Directory.CreateDirectory(processedPath);
                    
                    string fileName = Path.Join(processedPath, $"processed-{Path.GetFileName(path)}");
                    bitmap.Save(fileName);
                } catch (IOException) {
                    isReady = false;
                }
            }
        }
    }
}