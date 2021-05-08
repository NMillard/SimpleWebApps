using System.ComponentModel.DataAnnotations;

namespace WebApi.Models {
    
    /*
     * This is okay for beginners starting out with attempting to improve
     * their design.
     * 
     * But, creating a base that all entities must inherit from is often poor design.
     * It looks nice, and it feels good, but it's providing you the
     * flexibility to design your entities exactly the way you want.
     *
     * It also locks entities into having a certain type of key.
     * An int is not always the best choice.
     */
    public class EntityBase {
        
        [Key]
        public int Id { get; set; }
    }
}