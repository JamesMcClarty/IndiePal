using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IndiePal.Data;
using IndiePal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using IndiePal.Models.ViewModels;

namespace IndiePal.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Messages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Message.Include(m => m.Project).Include(m => m.ProjectPosition).Include(m => m.Reciever).Include(m => m.Sender);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Messages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.Project)
                .Include(m => m.ProjectPosition)
                .Include(m => m.Reciever)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        public async Task<IActionResult> SendHireMessage(int id)
        {
            ListOfPositionsAndProjects newMessage = new ListOfPositionsAndProjects()
            {
                SeperatePositions = new Dictionary<string, List<ProjectPosition>>()
            };

            var talent = await _context.Talent
                .Include(q => q.ApplicationUser)
                .FirstOrDefaultAsync(q => q.Id == id);

            var user = await GetCurrentUserAsync();

            var director = await _context.Director
                .FirstOrDefaultAsync(q => q.ApplicationUserId == user.Id);

            newMessage.RecieverId = talent.ApplicationUserId;
            newMessage.SenderId = user.Id;

            ViewBag.talentName = talent.ApplicationUser.UserName;
            ViewBag.userName = user.UserName;

            var projects = await _context.Project
                .Where(q => q.DirectorId == director.Id)
                .ToListAsync();

            var projectPositions = await _context.ProjectPosition
                .Where(q => q.TalentId == null)
                .ToListAsync();

            foreach (Project project in projects)
            {
                var filteredItems = new List<ProjectPosition>();

                foreach (ProjectPosition projectPosition in projectPositions)
                {
                    if (projectPosition.ProjectId == project.Id)
                    {
                        filteredItems.Add(projectPosition);
                    }
                }

                newMessage.SeperatePositions.Add(project.Id.ToString(), filteredItems);
            };

            newMessage.SelectProjects = projects.Select(q => new SelectListItem()
            {
                Text = q.Title,
                Value = q.Id.ToString()
            }).ToList();

            return View(newMessage);
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendHireMessage([Bind("Id,Header,Body,SenderId,RecieverId,ProjectId,ProjectPositionId")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Description", message.ProjectId);
            ViewData["ProjectPositionId"] = new SelectList(_context.ProjectPosition, "Id", "Postion", message.ProjectPositionId);
            ViewData["RecieverId"] = new SelectList(_context.ApplicationUser, "Id", "Id", message.RecieverId);
            ViewData["SenderId"] = new SelectList(_context.ApplicationUser, "Id", "Id", message.SenderId);
            return View(message);
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Description", message.ProjectId);
            ViewData["ProjectPositionId"] = new SelectList(_context.ProjectPosition, "Id", "Postion", message.ProjectPositionId);
            ViewData["RecieverId"] = new SelectList(_context.ApplicationUser, "Id", "Id", message.RecieverId);
            ViewData["SenderId"] = new SelectList(_context.ApplicationUser, "Id", "Id", message.SenderId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Header,Body,SenderId,RecieverId,ProjectId,ProjectPositionId")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "Id", "Description", message.ProjectId);
            ViewData["ProjectPositionId"] = new SelectList(_context.ProjectPosition, "Id", "Postion", message.ProjectPositionId);
            ViewData["RecieverId"] = new SelectList(_context.ApplicationUser, "Id", "Id", message.RecieverId);
            ViewData["SenderId"] = new SelectList(_context.ApplicationUser, "Id", "Id", message.SenderId);
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.Project)
                .Include(m => m.ProjectPosition)
                .Include(m => m.Reciever)
                .Include(m => m.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await _context.Message.FindAsync(id);
            _context.Message.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return _context.Message.Any(e => e.Id == id);
        }

        //Helper methods
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
