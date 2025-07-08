using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using HelpDeskTicketing.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HelpDeskTicketing.Api.Data;

public class ApplicationDbContext :IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }


    public DbSet<Ticket> Tickets { get; set; }
}
