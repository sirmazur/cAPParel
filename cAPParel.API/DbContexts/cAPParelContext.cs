﻿using Microsoft.EntityFrameworkCore;
using cAPParel.API.Entities;
using cAPParel.API.Entities.Hierarchy;

namespace cAPParel.API.DbContexts
{
    public class cAPParelContext : DbContext
    {
        public DbSet<FileData> Files { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        
        public cAPParelContext(DbContextOptions<cAPParelContext> options)
            : base(options)
        {
        }
    }
}
