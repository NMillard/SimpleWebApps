using WebApi.Exceptions;
using WebApi.Models;

namespace WebApi.Repositories {
    public class ArticleRepository : RepositoryBase<Article> {
        public ArticleRepository(AppDbContext context) : base(context) { }

        public override Article Get(int id) {
            Article article = base.Get(id);
            if (article is null) throw new EntityNotFoundException<Article>(id);

            return article;
        }

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