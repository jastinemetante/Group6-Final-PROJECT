using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EchoesOfThePast.Models
{
    public enum PostType
    {
        Story,
        Photo,
        Document
    }

    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public PostType Type { get; set; } = PostType.Story;

        public string? ImageUrl { get; set; }

        public string? AiInsight { get; set; }

        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class Comment
    {
        public int Id { get; set; }
        
        [Required]
        public string Text { get; set; } = string.Empty;

        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int PostId { get; set; }
        public virtual Post Post { get; set; } = null!;
    }
}
