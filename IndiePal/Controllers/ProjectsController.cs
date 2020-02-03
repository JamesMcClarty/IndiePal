using IndiePal.Data;
using IndiePal.Models;
using IndiePal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Projects
        [HttpGet]
        public async Task<ActionResult> AllProjects()
        {
            var user = await GetCurrentUserAsync();

            var projectsList = await _context.Project.ToListAsync();

            var director = await _context.Director
                .Include(d => d.Projects)
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

            var projectAndDirector = new ProjectListAndDirectorViewModel()
            {
                AllProjects = projectsList,
                Director = director
            };

            return View(projectAndDirector);
        }

        //Get: Projects/
        public async Task<ActionResult> DirectorAgreement()
        {
            var user = await GetCurrentUserAsync();

            var director = await _context.Director
                .FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

            var UserDirector = new UserHasDirectorViewModel()
            {
                hasDirectorAccout = director != null,
                User = user
            };

            return View(UserDirector);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDirector(string id)
        {
            if (ModelState.IsValid)
            {
                Director newDirector = new Director()
                {
                    Id = 0,
                    ApplicationUserId = id
                };

                _context.Add(newDirector);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AllProjects));
            }

            var user = await GetCurrentUserAsync();

            var director = _context.Director
                .FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

            var UserDirector = new UserHasDirectorViewModel()
            {
                hasDirectorAccout = director == null,
                User = user
            };

            return View(UserDirector);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return project;
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }

        //Helper methods

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}