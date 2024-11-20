using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using InspectorJournal.DataLayer.Models;

namespace InspectorJournal.DataLayer.Data
{
    public partial class InspectionsDbContext : DbContext
    {
        public InspectionsDbContext(DbContextOptions<InspectionsDbContext> options)
            : base(options)
        {
        }

        public  DbSet<Enterprise> Enterprises { get; set; }

        public  DbSet<Inspection> Inspections { get; set; }

        public  DbSet<Inspector> Inspectors { get; set; }

        public  DbSet<ViolationType> ViolationTypes { get; set; }

    }
}
