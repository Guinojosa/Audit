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

        public int SaveWithAudit()
        {
            try
            {
                List<AuditT> AuditList = new List<AuditT>();
                IList<EntityEntry> AuditIncludes = new List<EntityEntry>();

                AuditGerator(ChangeTracker.Entries(), AuditList, AuditIncludes);
                var saveChanges = base.SaveChanges();

                if (AuditIncludes != null)
                {
                    foreach (var audit in AuditIncludes)
                    {
                        foreach (var prop in audit.CurrentValues.Properties)
                        {
                            var current = audit.CurrentValues[prop.Name] == null ? "" : audit.CurrentValues[prop.Name].ToString();
                            GerateAuditObj(AuditList, audit, prop.Name, current, "", "I");
                        }
                    }
                }

                this.SaveAudit(AuditList);
                return saveChanges;
            }
            catch (Exception dbEx)
            {
                throw dbEx;
            }
        }

        private static void AuditGerator(IEnumerable<EntityEntry> ChangeTracker, List<AuditT> AuditList, IList<EntityEntry> AuditIncludes)
        {
            foreach (var entry in ChangeTracker)
            {
                if (entry.State == EntityState.Added)
                {
                    AuditIncludes.Add(entry);
                }
                else
                {
                    foreach (var prop in entry.CurrentValues.Properties)
                    {
                        var current = (entry.CurrentValues[prop.Name] == null) && (entry.State == EntityState.Deleted) ? "" : entry.CurrentValues[prop.Name].ToString();
                        var original = entry.OriginalValues[prop.Name] == null ? "" : entry.OriginalValues[prop.Name].ToString();

                        GerateAuditObj(AuditList, entry, prop.Name, current, original, entry.State == EntityState.Modified ? "M" : "D");
                    }
                }
            }
        }

        private static void GerateAuditObj(List<AuditT> AuditList, EntityEntry entry, string propName, string current, string original, string action)
        {
            AuditT audit = new AuditT();
            audit.Entity = entry.Entity.GetType().Name.Split('_')[0];
            audit.IdRegister = action == "D" ? Convert.ToInt32(entry.OriginalValues[entry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Count() > 0).Name]) :
                                               Convert.ToInt32(entry.CurrentValues[entry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Count() > 0).Name]);
            audit.Column = propName;
            audit.Value_Current = current.ToString();
            audit.Value_Original = original.ToString();
            audit.DateOccurrence = System.DateTime.Now;
            audit.IdUserModified = 1;
            audit.Action = action;
            AuditList.Add(audit);
        }

        private void SaveAudit(List<AuditT> AuditList)
        {
            this.AudiT.AddRange(AuditList);
            base.SaveChanges();
        }

    }
}
