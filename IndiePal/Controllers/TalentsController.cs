using IndiePal.Data;
using IndiePal.Models;
using IndiePal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Controllers
{
    [Authorize]
    public class TalentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TalentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Talents
        public async Task<IActionResult> AllTalents(int index)
        {
            var user = await GetCurrentUserAsync();

            var talent = await _context.Talent
                .Include(t => t.ApplicationUser)
                .Include(t => t.Skills)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            var talentList = await _context.Talent
                .Include(q => q.ApplicationUser)
                .Include(q => q.Skills)
                .Where(q => q.ApplicationUserId != user.Id && q.Active == true)
                .ToListAsync();

            var TalentListAndTalent = new TalentListAndTalentViewModel()
            {
                AllTalents = talentList.Skip((index - 1) * 5).Take(5).ToList(),
                Talent = talent
            };

            ViewBag.index = index;

            return View(TalentListAndTalent);
        }

        // GET: Talents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await GetCurrentUserAsync();

            var director = await _context.Director
                .FirstOrDefaultAsync(q => q.ApplicationUserId == user.Id);

            var talent = await _context.Talent
                .Include(t => t.ApplicationUser)
                .Include(t => t.Skills)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (talent == null)
            {
                return NotFound();
            }

            ViewBag.IsUserCurrentTalent = talent.ApplicationUserId == user.Id;
            ViewBag.IsUserDirector = director != null && talent.ApplicationUserId != user.Id; 

            return View(talent);
        }

        // GET: Talents/Create
        public IActionResult CreatingTalent()
        {
            return View();
        }

        // POST: Talents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatingTalent([Bind("Id,Biography,Wage,SkillList")] NewTalentAndSkills model)
        {
            ModelState.Remove("ApplicationUserId");
            ModelState.Remove("ApplicationUser");
            var user = await GetCurrentUserAsync();

            if (ModelState.IsValid)
            {
                Talent talent = new Talent()
                {
                    Id = model.Id,
                    Active = true,
                    Biography = model.Biography,
                    Wage = model.Wage,
                    ApplicationUserId = user.Id
                };

                _context.Add(talent);
                await _context.SaveChangesAsync();

                if (model.SkillList != null)
                {
                    bool skillsProcessed = await AddSkills(model.SkillList, user.Id);
                }

                return RedirectToAction(nameof(AllTalents));
            }

            return View();
        }

        // GET: Talents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var talent = await _context.Talent
                .Include(t => t.ApplicationUser)
                .Include(t => t.Skills)
                .FirstOrDefaultAsync(t => t.Id == id);

            var editedTalent = new NewTalentAndSkills()
            {
                Id = talent.Id,
                Active = talent.Active,
                ApplicationUser = talent.ApplicationUser,
                Wage = talent.Wage,
                Biography = talent.Biography,
                SkillList = new List<string>()
            };

            foreach (TalentSkill skill in talent.Skills)
            {
                editedTalent.SkillList.Add(skill.Skill);
            }

            if (talent == null)
            {
                return NotFound();
            }

            return View(editedTalent);
        }

        // POST: Talents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Active,Biography,Wage,SkillList")] NewTalentAndSkills editedTalent)
        {
            ModelState.Remove("ApplicationUserId");
            ModelState.Remove("ApplicationUser");
            var user = await GetCurrentUserAsync();

            if (id != editedTalent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var skills = await _context.TalentSkill
                        .Where(c => c.TalentID == editedTalent.Id)
                        .ToListAsync();

                    foreach (TalentSkill skill in skills)
                    {
                        _context.TalentSkill.Remove(skill);
                        await _context.SaveChangesAsync();
                    }

                    if (editedTalent.SkillList != null)
                    {
                        var skillsMade = await AddSkills(editedTalent.SkillList, user.Id);
                    }

                    var talent = new Talent()
                    {
                        Id = id,
                        ApplicationUserId = user.Id,
                        Biography = editedTalent.Biography,
                        Wage = editedTalent.Wage,
                        Active = editedTalent.Active
                    };

                    _context.Update(talent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TalentExists(editedTalent.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AllTalents));
            }
            return View(editedTalent);
        }


        // POST: Talents/Close/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int id)
        {
            var talent = await _context.Talent.FindAsync(id);
            talent.Active = false;
            _context.Talent.Update(talent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllTalents));
        }
        // POST: Talents/Reopen/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reopen(int id)
        {
            var talent = await _context.Talent.FindAsync(id);
            talent.Active = true;
            _context.Talent.Update(talent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllTalents));
        }

        private bool TalentExists(int id)
        {
            return _context.Talent.Any(e => e.Id == id);
        }

        //Helper methods

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        private async Task<Boolean> AddSkills(List<string> skills, string appId)
        {
            if (skills.Count != 0)
            {
                foreach (string trimmedSkill in skills)
                {
                    var obtainedTalent = await _context.Talent
                        .AsNoTracking()
                        .FirstOrDefaultAsync(o => o.ApplicationUserId == appId);

                    var talentSkill = new TalentSkill()
                    {
                        Id = 0,
                        Skill = trimmedSkill,
                        TalentID = obtainedTalent.Id
                    };

                    _context.Add(talentSkill);
                    await _context.SaveChangesAsync();
                }
            }
            return true;
        }
    }
}