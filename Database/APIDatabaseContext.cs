using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Database;

public class APIDatabaseContext : IdentityDbContext
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public APIDatabaseContext(DbContextOptions<APIDatabaseContext> options) : base(options){ 
        
    }
}
