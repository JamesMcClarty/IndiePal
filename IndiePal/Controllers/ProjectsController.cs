using IndiePal.Data;
using IndiePal.Models;
using IndiePal.Models.RouteModels;
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


            var projectsList = await _context.Project
                .Include(d => d.CurrentPositions)
                .Include(d => d.Director)
                .ThenInclude(d => d.ApplicationUser)
                .Where(d=>d.EndDate == null)
                .ToListAsync();

            var director = await _context.Director
                .Include(d => d.Projects)
                    .ThenInclude(d => d.CurrentPositions)
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.ApplicationUserId == user.Id);

            if (director != null)
            {
                if (director.Projects != null)
                {
                    director.Projects.OrderByDescending(q => q.EndDate);
                }
            }

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
            ViewBag.positionsavailable = false;
            if (projectAndDirector.Director != null)
            {
                ViewBag.directorId = projectAndDirector.Director.Id;
            }

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

        //Details
        public async Task<ActionResult> Details(int id)
        {
            var project = await _context.Project
                .Include(d => d.CurrentPositions)
                .Include(d => d.ProjectLogs)
                .Include(d=> d.Director)
                .ThenInclude(d => d.ApplicationUser)
                .FirstOrDefaultAsync(d => d.Id == id);

            var currentUser = await GetCurrentUserAsync();

            var director = await _context.Director
                .FirstOrDefaultAsync(d => d.ApplicationUserId == currentUser.Id);

            if (director != null)
            {
                ViewBag.currentDirector = director.Id;
            }

            else
            {
                ViewBag.currentDirector = 0;
            }

            return View(project);
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
                    StartDate = System.DateTime.Now,
                    DirectorId = director.Id
                };

                _context.Project.Add(project);
                await _context.SaveChangesAsync();

                if (model.Positions != null)
                {
                    var positionsAdded = await AddPositions(model.Positions, project.StartDate, director.Id);
                }

                return RedirectToAction(nameof(AllProjects));
            }

            return View();
        }

        public async Task<IActionResult> EditProject(int id)
        {

            var project = await _context.Project
                .Include(p => p.CurrentPositions)
                .FirstOrDefaultAsync(q => q.Id == id);

            var editingProject = new NewProjectAndPositions()
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                StartDate = project.StartDate,
                Budget = project.Budget,
                Active = project.Active,
                DirectorId = project.DirectorId,
                Positions = new List<string>()
            };

            foreach(ProjectPosition position in project.CurrentPositions)
            {
                editingProject.Positions.Add(position.Postion);
            }

            ViewBag.currentPositions = project.CurrentPositions.ToList();

            return View(editingProject);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProject(int id, [Bind("Id, Active, Title, Budget, Description, StartDate, DirectorId, Positions")] NewProjectAndPositions model)
        {

            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
               try
                 {
                    Project project = new Project()
                    {
                        Id = model.Id,
                        Title = model.Title,
                        Description = model.Description,
                        Budget = model.Budget,
                        Active = model.Active,
                        StartDate = model.StartDate,                        
                        DirectorId = model.DirectorId
                    };

                    _context.Project.Update(project);
                    await _context.SaveChangesAsync();

                    if (model.Positions != null)
                    {
                        var positionsAdded = await UpdatePositions(model.Positions, model.Id);
                    }

                    return RedirectToAction(nameof(AllProjects), new ProjectListRoute() { index = 1, viewingyours = false });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            try
            {
                var closeProject = await _context.Project
                    .FirstOrDefaultAsync(p => p.Id == id);

                closeProject.EndDate = System.DateTime.Now;
                closeProject.Active = false;

                _context.Project.Update(closeProject);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(AllProjects), new ProjectListRoute() { index = 1, viewingyours = false });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.Id == id);
        }

        //Helper methods

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        private async Task<bool> AddPositions(List<string> StringList, System.DateTime dateTime, int directorId)
        {

            if (StringList.Count != 0)
            {
                var obtainedProject = await _context.Project
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.StartDate == dateTime && o.DirectorId == directorId);

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

        private async Task<bool> UpdatePositions(List<string> StringList, int id)
        {

            if (StringList.Count != 0)
            {
                var obtainedPositions = await _context.ProjectPosition
                    .Where(q => q.ProjectId == id)
                    .ToListAsync();


                foreach(ProjectPosition position in obtainedPositions)
                {
                    if(position.TalentId == null)
                    {
                        _context.ProjectPosition.Remove(position);
                        await _context.SaveChangesAsync();
                    }
                }

                foreach (string trimmedPosition in StringList)
                {
                    var projectPosition = new ProjectPosition()
                    {
                        Id = 0,
                        ProjectId = id,
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