using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoHub.Repositories;

namespace AutoHub.Controllers;

/// <summary>
/// Trang hệ thống showroom — hiển thị bản đồ Leaflet cho khách hàng.
/// </summary>
public class ShowroomController : Controller
{
    private readonly IShowroomRepository _repo;

    public ShowroomController(IShowroomRepository repo) => _repo = repo;

    // GET /showrooms
    [Route("showrooms")]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Hệ Thống Showroom - AutoHub";
        var showrooms = await _repo.GetActiveAsync();
        return View(showrooms);
    }

    // GET /api/showrooms — trả JSON cho Leaflet markers
    [HttpGet("/api/showrooms")]
    public async Task<IActionResult> ApiList()
    {
        var list = await _repo.GetActiveAsync();
        var result = list.Select(s => new
        {
            id           = s.Id,
            name         = s.Name,
            address      = s.FullAddress,
            phone        = s.PhoneNumber,
            openingHours = s.OpeningHours,
            lat          = s.Latitude,
            lng          = s.Longitude,
            thumbnail    = s.ThumbnailImageUrl
        });
        return Json(result);
    }
}
