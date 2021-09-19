/*
 * In this example we'll create a mechanism for storing files that a user provides.
 * The user will pick if they want to the file stored in a database or on the file system.
 * ------
 * We'll have two approaches providing the file storing functionality.
 * 
 * First we'll implement the functionality using the regular control flow structures
 * such as if-else, and switch.
 *
 * The other approach will be entirely object-oriented.
 */

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RemovingTraditionalBranching.Traditional {
    public class FileSaver {
        private readonly IFilesRepository repository;
        private readonly FileSystemSavingOptions savingOptions;

        // The repository is only used for saving the file in a database. This is a major code smell as you'll begin
        // to have tightly branch-coupled dependencies, where each dependency is only used in one branch.
        // It's just a matter of time before dependency hell sets in.
        // Classes shouldn't rely on dependencies that aren't necessary.
        public FileSaver(IFilesRepository repository, FileSystemSavingOptions savingOptions) {
            this.repository = repository;
            this.savingOptions = savingOptions;
        }

        public async ValueTask<bool> StoreFile(
            string fileName,
            byte[] content,
            StorageOption option,
            string? folderName = null) {
            
            bool missingFolderName = option == StorageOption.FileSystem && string.IsNullOrEmpty(folderName);
            if (missingFolderName) return false;
            
            switch (option) {
                case StorageOption.Database:
                    bool saveSucceeded = await repository.SaveAsync(fileName, content);
                    return saveSucceeded;
                case StorageOption.FileSystem:
                    string storagePath = Path.Combine(Directory.GetCurrentDirectory(), savingOptions.Path, folderName!);
                    bool missingFolder = !Directory.Exists(storagePath);
                    if (missingFolder) Directory.CreateDirectory(storagePath);

                    FileStream file;
                    try {
                        file = File.Create(Path.Combine(storagePath, fileName));
                    } catch (Exception e) {
                        Console.WriteLine(e.Message); // or some other logging statement
                        return false;
                    }
                    
                    await file.WriteAsync(content);
                    await file.FlushAsync();
                    file.Close();
                    
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }
    }

    public enum StorageOption {
        Database,
        FileSystem,
    }
}


namespace RemovingTraditionalBranching.Branchless {

    public class FileSaver {
        private readonly Dictionary<Type, Func<object>> sinks;

        public FileSaver() => sinks = new Dictionary<Type, Func<object>>();

        public async ValueTask<bool> StoreFile<TContext>(TContext context) where TContext : FileContext {
            bool sinkMissing = !sinks.TryGetValue(typeof(TContext), out Func<object>? sinkCreator);
            if (sinkMissing) throw new InvalidOperationException($"No sink exists for this context: {context.GetType().Name}");
            if (sinkCreator!.Invoke() is not FileSink<TContext> sink) throw new InvalidOperationException("Was not a sink");

            return await sink.Store(context);
        }

        public void RegisterSink<TFileContext>(Func<FileSink<TFileContext>> creator) where TFileContext : FileContext
            => sinks[typeof(TFileContext)] = creator;
    }

    public abstract class FileSink<TFileContext> where TFileContext : FileContext {
        public abstract ValueTask<bool> Store(TFileContext context);
    }

    public abstract record FileContext(byte[] Content, string FileName);

    public record DatabaseFileContext(byte[] Content, string FileName) : FileContext(Content, FileName);

    public record FileSystemContext(byte[] Content, string FileName, string FolderName) : FileContext(Content, FileName);
    
    
    public class DatabaseSink : FileSink<DatabaseFileContext> {
        private readonly IFilesRepository repository;

        public DatabaseSink(IFilesRepository repository) => this.repository = repository;

        public override async ValueTask<bool> Store(DatabaseFileContext context) => await repository.SaveAsync(context.FileName, context.Content);
    }

    public class FileSystemSink : FileSink<FileSystemContext> {
        private readonly FileSystemSavingOptions savingOptions;

        public FileSystemSink(FileSystemSavingOptions savingOptions) => this.savingOptions = savingOptions;

        public override async ValueTask<bool> Store(FileSystemContext context) {
            string storagePath = Path.Combine(Directory.GetCurrentDirectory(), savingOptions.Path, context.FolderName!);
            bool missingFolder = !Directory.Exists(storagePath);
            if (missingFolder) Directory.CreateDirectory(storagePath);

            FileStream file;
            try {
                file = File.Create(Path.Combine(storagePath, context.FileName));
            } catch (Exception e) {
                Console.WriteLine(e.Message); // or some other logging statement
                return false;
            }
            
            await file.WriteAsync(context.Content);
            await file.FlushAsync();
            file.Close();

            return true;
        }
    }
}


namespace RemovingTraditionalBranching {
    public interface IFilesRepository {
        Task<bool> SaveAsync(string fileName, byte[] content);
    }

    public class FakeFileRepository : IFilesRepository {
        public Task<bool> SaveAsync(string fileName, byte[] content) {
            Console.WriteLine($"Storing {fileName} containing {content.Length} bytes");
            return Task.FromResult(true);
        }
    }

    public class FileSystemSavingOptions {
        public string Path { get; set; } = "files-test";
    }
}