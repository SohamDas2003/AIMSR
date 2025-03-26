using Microsoft.AspNetCore.Mvc;
using AIMSR.Data;
using System.Linq;

public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userProfile = _context.Profiles.FirstOrDefault();
        return View("Profile",userProfile);
    }
}
