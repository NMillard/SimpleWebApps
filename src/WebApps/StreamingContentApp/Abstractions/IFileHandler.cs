namespace StreamingContentApp.Abstractions; 

public interface IFileHandler {
    void UploadFile(byte[] file);
    void RetrieveFile();
}