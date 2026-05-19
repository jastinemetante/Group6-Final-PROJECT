using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EchoesOfThePast.Models;
using EchoesOfThePast.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EchoesOfThePast.Controllers
{
    public class HistoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IGeminiService _geminiService;

        public HistoryController(ApplicationDbContext context, IGeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        // GET: History
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(posts);
        }

        // GET: History/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Post post, bool generateAiInsight)
        {
            if (ModelState.IsValid)
            {
                post.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                post.AuthorName = User.Identity?.Name ?? "Unknown";
                
                if (generateAiInsight)
                {
                    post.AiInsight = await _geminiService.GetHistoricalInsightAsync(post.Title, post.Content);
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(int postId, string text)
        {
            if (string.IsNullOrEmpty(text)) return BadRequest();

            var comment = new Comment
            {
                PostId = postId,
                Text = text,
                AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                AuthorName = User.Identity?.Name ?? "Anonymous",
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = postId });
        }
    }
}
