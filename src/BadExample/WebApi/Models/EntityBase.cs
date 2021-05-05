using System.ComponentModel.DataAnnotations;

namespace WebApi.Models {
    
    /*
     * Creating a base that all entities must inherit from is poor design.
     * It looks nice, and it feels good, but it's providing you the
     * flexibility to design your entities exactly the way you want.
     *
     * It also locks entities into having a certain type of key.
     * An int is not always the best choice.
     */
    public class EntityBase {
        
        [Key] // <- please no.
        public int Id { get; set; }
    }
}