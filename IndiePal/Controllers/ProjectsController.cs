using IndiePal.Data;
using IndiePal.Models;
using IndiePal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        public async Task<ActionResult> AllProjects(int index, bool viewingyours)
        {
            var user = await GetCurrentUserAsync();

            var projectsList = await _context.Project.ToListAsync();

            var director = await _context.Director
                .Include(d => d.Projects)
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

            var projectAndDirector = new ProjectListAndDirectorViewModel()
            {
                Director = director
            };

            if (viewingyours == false)
            {
                projectAndDirector.AllProjects = projectsList.Skip((index - 1) * 5).Take(5).ToList();
            }
            else
            {
                projectAndDirector.AllProjects = director.Projects.Skip((index - 1) * 5).Take(5).ToList();
            }

            ViewBag.index = index;
            ViewBag.viewingyours = viewingyours; 

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

        public ActionResult CreateProject()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject([Bind("Title,Budget,Description,Positions")] NewProjectAndPositions model)
        {
            ModelState.Remove("Director");
            ModelState.Remove("Active");
            ModelState.Remove("Id");
            ModelState.Remove("StartDate");
            ModelState.Remove("EndDate");

            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync();

                var director = await _context.Director
                    .FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

                Project project = new Project()
                {
                    Id = 0,
                    Title = model.Title,
                    Description = model.Description,
                    Budget = model.Budget,
                    Active = true,
                    StartDate = System.DateTime.Today,
                    DirectorId = director.Id
                };

                _context.Project.Add(project);
                await _context.SaveChangesAsync();

                if (model.Positions != null)
                {
                    var positionsAdded = await AddPositions(model.Positions, project.StartDate);
                }

                return RedirectToAction(nameof(AllProjects));
            }

            return View();
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

        private async Task<bool> AddPositions(List<string> StringList, System.DateTime dateTime)
        {

            if (StringList.Count != 0)
            {
                var obtainedProject = await _context.Project
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.StartDate == dateTime);

                foreach (string trimmedPosition in StringList)
                {
                    var projectPosition = new ProjectPosition()
                    {
                        Id = 0,
                        ProjectId = obtainedProject.Id,
                        Postion = trimmedPosition

                    };

                    _context.ProjectPosition.Add(projectPosition);
                    await _context.SaveChangesAsync();
                }
            }
            return true;
        }
    }
}