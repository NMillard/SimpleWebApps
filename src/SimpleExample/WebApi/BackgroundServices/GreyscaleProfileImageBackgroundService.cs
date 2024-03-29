﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Repositories;

namespace WebApi.BackgroundServices {
    public class GreyscaleProfileImageBackgroundService : BackgroundService {
        private readonly IServiceProvider provider;

        private readonly string folder = Path.Join(Directory.GetCurrentDirectory(), "staging");

        public GreyscaleProfileImageBackgroundService(IServiceProvider provider) {
            this.provider = provider;
        }
        
        protected async override Task ExecuteAsync(CancellationToken stoppingToken) {
            
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var watcher = new FileSystemWatcher(folder) {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
            };

            watcher.Created += async (sender, args) => await ProcessProfileImageAsync(args.FullPath, stoppingToken);
        }

        private async Task ProcessProfileImageAsync(string path, CancellationToken stoppingToken) {
            bool isReady = false;
            while (!isReady && !stoppingToken.IsCancellationRequested) {
                try {
                    await using FileStream image = File.OpenRead(path);
                    isReady = true;
                    
                    var bitmap = new Bitmap(image);

                    GrayScaleImage(bitmap);

                    // We expect profile images to be uploaded as userId-Guid.extension eg. 1-Guid.png
                    int userId = int.Parse(path.Split("-")[0].Split(Path.DirectorySeparatorChar)[^1]);
                    
                    await using var imageStream = new MemoryStream();
                    bitmap.Save(imageStream, ImageFormat.Jpeg);
                    
                    UpdateUserProfileImage(userId, imageStream.ToArray());
                    image.Close();
                    imageStream.Close();
                    
                    File.Delete(path); // Remove file after processing
                } catch (IOException) {
                    isReady = false;
                }
            }
        }

        private static void GrayScaleImage(Bitmap bitmap) {
            for (int x = 0; x < bitmap.Width; x++) {
                for (int y = 0; y < bitmap.Height; y++) {
                    Color color = bitmap.GetPixel(x, y);
                    int grayScale = (color.R + color.G + color.B) / 3;
                    Color grayScaleColor = Color.FromArgb(grayScale,grayScale,grayScale);
                    bitmap.SetPixel(x, y, grayScaleColor);
                }
            }
        }

        private void UpdateUserProfileImage(int userId, byte[] imageBlob) {
            // Background services need to create their own scope and request services
            using var scope = provider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<UserRepository>();
            
            var user = repository.Get(userId);
            user.ProfileImage = imageBlob;
            
            repository.Update(user);
        }
    }
}