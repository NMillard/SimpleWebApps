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
using System.Linq;
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

        public async Task<bool> StoreFile(string fileName, byte[] content, StorageOption option, string? folderName = null) {
            bool missingFolderName = option == StorageOption.FileSystem && string.IsNullOrEmpty(folderName);
            if (missingFolderName) return false;
            
            switch (option) {
                case StorageOption.Database:
                    bool saveSucceeded =  await repository.SaveAsync(fileName, content);
                    return saveSucceeded;
                case StorageOption.FileSystem:
                    string storagePath = Path.Combine(Directory.GetCurrentDirectory(), savingOptions.Path, folderName!);
                    if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);
                    
                    FileStream file = File.Create(Path.Combine(storagePath, fileName));
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
        private readonly Dictionary<Type, Func<IFileSink>> sinks;

        public FileSaver() {
            sinks = new Dictionary<Type, Func<IFileSink>>();
        }

        public async ValueTask<bool> StoreFile(FileContext context) {
            sinks.TryGetValue(context.GetType(), out Func<IFileSink>? sinkCreator);
            if (sinkCreator is null) throw new InvalidOperationException($"No sink exists for this context: {context.GetType().Name}");

            return await sinkCreator().Store(context);
        }

        public void RegisterSink<TFileContext>(Func<FileSink<TFileContext>> creator) where TFileContext : FileContext
            => sinks[typeof(TFileContext)] = creator;
    }

    /// <summary>
    /// Alternative approach to selecting the sink matching the context.
    /// </summary>
    public static class FileSinkExtensions {
        public static ValueTask<bool> SaveFile(this IEnumerable<IFileSink> sinks, FileContext context) {
            return sinks.SingleOrDefault(s => s.SupportedType == context.GetType())
                ?.Store(context)?? throw new InvalidOperationException();
        }
    }

    public interface IFileSink {
        ValueTask<bool> Store(FileContext context);
        Type SupportedType { get; }
    }
    
    public abstract class FileSink<TFileContext> : IFileSink where TFileContext : FileContext {
        public Type SupportedType => typeof(TFileContext);

        public ValueTask<bool> Store(FileContext context) => Store((TFileContext) context);
        protected abstract ValueTask<bool> Store(TFileContext context);
    }

    public abstract record FileContext(byte[] Content, string FileName);

    public record DatabaseFileContext(byte[] Content, string FileName) : FileContext(Content, FileName);

    public record FileSystemContext(byte[] Content, string FileName, string FolderName) : FileContext(Content, FileName);
    
    
    public class DatabaseSink : FileSink<DatabaseFileContext> {
        private readonly IFilesRepository repository;

        public DatabaseSink(IFilesRepository repository) => this.repository = repository;

        protected override async ValueTask<bool> Store(DatabaseFileContext context) {
            bool result = await repository.SaveAsync(context.FileName, context.Content);
            return result;
        }
    }

    public class FileSystemSink : FileSink<FileSystemContext> {
        private readonly FileSystemSavingOptions savingOptions;

        public FileSystemSink(FileSystemSavingOptions savingOptions) => this.savingOptions = savingOptions;

        protected override async ValueTask<bool> Store(FileSystemContext context) {
            string storagePath = Path.Combine(Directory.GetCurrentDirectory(), savingOptions.Path, context.FolderName);
            if (!Directory.Exists(storagePath)) Directory.CreateDirectory(storagePath);

            await using FileStream file = File.Create(Path.Combine(storagePath, context.FileName));
            await file.WriteAsync(context.Content);
            await file.FlushAsync();

            return true;
        }
    }
}


namespace RemovingTraditionalBranching {
    public interface IFilesRepository {
        Task<bool> SaveAsync(string fileName, byte[] content);
    }

    public class FileSystemSavingOptions {
        public string Path { get; set; } = "files-test";
    }
}