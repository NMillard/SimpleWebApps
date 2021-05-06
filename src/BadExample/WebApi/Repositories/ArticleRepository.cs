using WebApi.Models;

namespace WebApi.Repositories {
    public class ArticleRepository : RepositoryBase<Article> {
        public ArticleRepository(AppDbContext context) : base(context) { }
    }
}