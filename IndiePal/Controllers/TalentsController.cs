using IndiePal.Data;
using IndiePal.Models;
using IndiePal.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Controllers
{
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
        public async Task<IActionResult> AllTalents()
        {
            var user = await GetCurrentUserAsync();

            var talentList = await _context.Talent.ToListAsync();

            var talent = await _context.Talent
                .Include(t => t.ApplicationUser)
                .Include(t => t.Skills)
                .FirstOrDefaultAsync(t => t.ApplicationUserId == user.Id);

            var TalentListAndTalent = new TalentListAndTalentViewModel()
            {
                AllTalents = talentList,
                Talent = talent
            };

            return View(TalentListAndTalent);
        }

        // GET: Talents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var talent = await _context.Talent
                .Include(t => t.ApplicationUser)
                .Include(t => t.Skills)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (talent == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Create([Bind("Id,Biography,Wage,Skills")] NewTalentAndSkills model)
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

                if (!string.IsNullOrWhiteSpace(model.Skills))
                {
                    bool IsProceed = await AddSkills(model.Skills, user.Id);
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

            string skillString = "";

            var editedTalent = new NewTalentAndSkills()
            {
                Id = talent.Id,
                ApplicationUser = talent.ApplicationUser,
                Wage = talent.Wage,
                Biography = talent.Biography,
            };

            foreach (TalentSkill skill in talent.Skills)
            {
                skillString += skill.Skill;
                if (skill == talent.Skills.Last())
                {
                    editedTalent.Skills = skillString;
                }
                else
                {
                    skillString += ",";
                }
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Active,Biography,Wage,Skills")] NewTalentAndSkills editedTalent)
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

                    var skillsMade = await AddSkills(editedTalent.Skills, user.Id);

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

        // GET: Talents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var talent = await _context.Talent
                .Include(t => t.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (talent == null)
            {
                return NotFound();
            }

            return View(talent);
        }

        // POST: Talents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var talent = await _context.Talent.FindAsync(id);
            _context.Talent.Remove(talent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TalentExists(int id)
        {
            return _context.Talent.Any(e => e.Id == id);
        }

        //Helper methods

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        private async Task<Boolean> AddSkills(string skillString, string appId)
        {
            string[] skills = skillString.Split(',');

            foreach (string trimmedSkill in skills)
            {
                var obtainedTalent = await _context.Talent
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
            return true;
        }
    }
}