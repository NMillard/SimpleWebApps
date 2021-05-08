using System;
using System.Collections.Generic;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers {
    
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase {
        private readonly ArticleRepository articleRepository;

        public ArticlesController(ArticleRepository articleRepository) {
            this.articleRepository = articleRepository;
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id) {
            try {
                Article article = articleRepository.Get(id);
                return Ok(article);
            } catch (EntityNotFoundException<Article> e) {
                return NotFound(new { e.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Article>), StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json, "text/csv")]
        public IActionResult All() {
            return Ok(articleRepository.All());
        }

        [HttpGet("paged")]
        public IActionResult Paged(int page = 0, int limit = 5) {
            return Ok(articleRepository.GetPaged(page, limit));
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateArticleDto model) {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var article = new Article {
                Content = model.Content,
                UserId = model.UserId,
                TimePublished = DateTime.Now,
            };

            int id = articleRepository.Create(article);
            
            return Ok(new { Id = id });
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) {
            articleRepository.Delete(id);
            return Ok();
        }
    }
}