namespace WebApi.Models {
    public interface ICsvSerializable {
        
        /// <summary>
        /// Returns the CSV headers for the type. 
        /// </summary>
        string[] GetCsvPropertyNames();
        
        /// <summary>
        /// Serialize the object as a csv line/row. 
        /// </summary>
        string ToCsv();
    }
}