using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoHub.Models.Entities;
using AutoHub.Repositories;
using AutoHub.Services;

namespace AutoHub.Controllers
{
    public class CommunityController : Controller
    {
        private readonly ISocialPostRepository _socialPostRepository;
        private readonly IFileService _fileService;
        private readonly IUserRepository _userRepository;

        public CommunityController(
            ISocialPostRepository socialPostRepository,
            IFileService fileService,
            IUserRepository userRepository)
        {
            _socialPostRepository = socialPostRepository;
            _fileService = fileService;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = await _userRepository.GetByIdAsync(userId.Value);
                ViewBag.CurrentUser = user;
            }

            var posts = await _socialPostRepository.GetAllPostsWithUsersAsync();
            return View(posts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(string content, IFormFile? imageFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để đăng bài viết!";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(content) && (imageFile == null || imageFile.Length == 0))
            {
                TempData["Error"] = "Bài viết không được để trống!";
                return RedirectToAction("Index");
            }

            try
            {
                string? imageUrl = null;
                if (imageFile != null && imageFile.Length > 0)
                {
                    imageUrl = await _fileService.UploadImageAsync(imageFile, "social_posts");
                }

                var post = new SocialPost
                {
                    UserId = userId.Value,
                    Content = content ?? string.Empty,
                    ImageUrl = imageUrl,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _socialPostRepository.AddAsync(post);
                TempData["Success"] = "Đăng bài viết thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
