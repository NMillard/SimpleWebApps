﻿using System;
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

        [HttpGet("/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(int id) {
            try {
                Article article = articleRepository.Get(id);
                return Ok(article);
            } catch (EntityNotFoundException<Article> e) {
                return NotFound(new { e.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces(MediaTypeNames.Application.Json, "application/csv")]
        public IActionResult All() {
            return Ok(articleRepository.All());
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
            
            articleRepository.Create(article);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id) {
            articleRepository.Delete(id);
            return Ok();
        }
    }
}