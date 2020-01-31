using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using IndiePal.Models;

namespace IndiePal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Director> Director { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ProjectLog> ProjectLog { get; set; }
        public DbSet<ProjectPosition> ProjectPosition { get; set; }
        public DbSet<Skill> Skill { get; set; }
        public DbSet<Talent> Talent { get; set; }
        public DbSet<TalentSkill> TalentSkill { get; set; }

    }
}
