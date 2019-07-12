using AuditCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuditCore.Context
{
    public class AuditCoreContext : DbContext
    {
        public DbSet<Person> Person { get; set; }
        public DbSet<AuditT> AudiT { get; set; }

        public AuditCoreContext(DbContextOptions<AuditCoreContext> options) :
        base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().HasKey(x => x.idperson);
            modelBuilder.Entity<AuditT>().HasKey(x => x.IdAudit);
        }

        public override int SaveChanges()
        {
            try
            {
                List<AuditT> AuditList = new List<AuditT>();
                IList<EntityEntry> AuditIncludes = new List<EntityEntry>();


                AuditGerator(ChangeTracker.Entries());

                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
                {
                    foreach (var propName in entry.CurrentValues.Properties)
                    {
                        var current = entry.CurrentValues[propName.Name] == null ? "" : entry.CurrentValues[propName.Name].ToString();
                        var original = entry.OriginalValues[propName.Name] == null ? "" : entry.OriginalValues[propName.Name].ToString();

                        if (current != original)
                        {
                            GerateAuditObj(AuditList, entry, propName.Name, current, original, "M");
                        }
                    }
                }

                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted))
                {
                    foreach (var propName in entry.OriginalValues.Properties)
                    {
                        var original = entry.OriginalValues[propName.Name] == null ? "" : entry.OriginalValues[propName.Name].ToString();
                        GerateAuditObj(AuditList, entry, propName.Name, "", original, "D");
                    }
                }

                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                {
                    AuditIncludes.Add(entry);
                }

                var saveChanges = base.SaveChanges();

                if (AuditIncludes != null)
                {
                    foreach (var audit in AuditIncludes)
                    {
                        foreach (var propName in audit.CurrentValues.Properties)
                        {
                            var current = audit.CurrentValues[propName] == null ? "" : audit.CurrentValues[propName].ToString();
                            GerateAuditObj(AuditList, audit, propName.Name, current, "", "I");
                        }
                    }
                }

                this.SaveAudit(AuditList);
                return saveChanges;
            }
            catch (Exception dbEx)
            {
                Exception raise = dbEx;
                throw raise;
            }
        }

        private static void AuditGerator(IEnumerable<EntityEntry> ChangeTracker)
        {
            foreach (var entry in ChangeTracker)
            {
                foreach (var propName in entry.CurrentValues.Properties)
                {

                    switch(entry.State){
                        case EntityState.Added:
                            break;
                        case EntityState.Modified:
                            break;
                        case EntityState.Deleted:
                            break;
                    }

                    var current = entry.CurrentValues[propName.Name] == null ? "" : entry.CurrentValues[propName.Name].ToString();
                    var original = entry.OriginalValues[propName.Name] == null ? "" : entry.OriginalValues[propName.Name].ToString();

                    if (current != original)
                    {
                        GerateAuditObj(AuditList, entry, propName.Name, current, original, "M");
                    }
                }
            }
        }

        private static void GerateAuditObj(List<AuditT> AuditList, EntityEntry entry, string propName, string current, string original, string action)
        {
            AuditT audit = new AuditT();
            audit.Entity = entry.Entity.GetType().Name.Split('_')[0];
            audit.IdAudit = action == "D" ? Convert.ToInt32(entry.OriginalValues[entry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Count() > 0).Name]) :
                                                 Convert.ToInt32(entry.CurrentValues[entry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Count() > 0).Name]);
            audit.Column = propName;
            audit.Value_Current = current.ToString();
            audit.Value_Original = original.ToString();
            audit.DateOccurrence = System.DateTime.Now;
            audit.IdUserModified = 1;
            audit.Action = action;
            AuditList.Add(audit);
        }

        public void SaveAudit(List<AuditT> AuditList)
        {
            foreach (var audit in AuditList)
            {
                this.AudiT.Add(audit);
            }
            base.SaveChanges();
        }

    }
}
