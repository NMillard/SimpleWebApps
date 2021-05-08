using System.Collections.Generic;
using System.Linq;
using WebApi.Models;

namespace WebApi.Repositories {
    public class ArticleRepository : RepositoryBase<Article> {
        public ArticleRepository(AppDbContext context) : base(context) { }

        /// <summary>
        /// Simple pagination example.
        /// </summary>
        public IEnumerable<Article> GetPaged(int page = 0, int take = 5) {
            int pageLimit = 5;
            bool isTakeLargerThanMax = take > pageLimit;
            
            return Entities
                .Skip(page * pageLimit)
                .Take(isTakeLargerThanMax ? pageLimit : take);
        }
    }
}