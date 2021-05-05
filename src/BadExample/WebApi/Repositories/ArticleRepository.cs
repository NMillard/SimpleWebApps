using WebApi.Models;

namespace WebApi.Repositories {
    public class ArticleRepository : RepositoryBase<Article> {
        public ArticleRepository(AppDbContext context) : base(context) { }
        
        public override void Create(Article entity) {
            Set.Add(entity);
            SaveChanges();
        }

        public override void Delete(int id) {
            Article article = Get(id);
            Set.Remove(article);
            
            SaveChanges();
        }
    }
}