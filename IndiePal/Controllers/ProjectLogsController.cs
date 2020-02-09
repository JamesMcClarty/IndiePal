using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IndiePal.Data;
using IndiePal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace IndiePal.Controllers
{
    [Authorize]
    public class ProjectLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectLogsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ActionResult CreateLog(int id)
        {
            return View(new ProjectLog() { ProjectId = id});
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProjectLog(int id, [Bind("Log, ProjectId")] ProjectLog model)
        {
         
            ModelState.Remove("Id");
            ModelState.Remove("DateMade");

            if (ModelState.IsValid)
            {
                model.DateMade = System.DateTime.Now;

                _context.ProjectLog.Add(model);
                await _context.SaveChangesAsync();

                return Redirect("~/Projects/Details/" + model.ProjectId);
            }

            return View();
        }

        public async Task<IActionResult> EditLog(int id)
        {
            var EditingLog = await _context.ProjectLog
                .FirstOrDefaultAsync(p => p.Id == id);
                
            return View(EditingLog);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLog(int id, [Bind("Id, DateMade, Log, ProjectId")] ProjectLog model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.ProjectLog.Update(model);
                    await _context.SaveChangesAsync();

                    return Redirect("~/Projects/Details/" + model.ProjectId);
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectLogExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var EditingLog = await _context.ProjectLog
                .FirstOrDefaultAsync(p => p.ProjectId == id);

            return View(EditingLog);
        }


        //ProjectLogs/Delete/5
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var projectLog = await _context.ProjectLog.FindAsync(id);

            if (projectLog == null)
            {
                return NotFound();
            }

            _context.ProjectLog.Remove(projectLog);
            await _context.SaveChangesAsync();

            return Redirect("~/Projects/Details/" + projectLog.ProjectId);
        }

        private bool ProjectLogExists(int id)
        {
            return _context.ProjectLog.Any(e => e.Id == id);
        }
    }
}
