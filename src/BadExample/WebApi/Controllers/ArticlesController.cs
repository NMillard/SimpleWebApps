using System;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers {
    
    [Route("api/[controller]/[action]")]
    public class ArticlesController : ControllerBase {
        private readonly ArticleRepository articleRepository;

        public ArticlesController(ArticleRepository articleRepository) {
            this.articleRepository = articleRepository;
        }

        [HttpGet("/{id:int}")]
        public IActionResult Get(int id) {
            Article article = articleRepository.Get(id);
            return Ok(article);
        }

        [HttpGet("")]
        public IActionResult All() {
            return Ok(articleRepository.All());
        }

        [HttpPost("")]
        public IActionResult Create(CreateArticleDto model) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            
            var article = new Article {
                Content = model.Content,
                UserId = model.UserId,
                TimePublished = DateTime.Now,
            };
            
            articleRepository.Create(article);
            return Ok();
        }
    }
}