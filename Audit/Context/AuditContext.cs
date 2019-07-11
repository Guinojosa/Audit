using Audit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Audit.Context
{
    public class AuditContext : DbContext
    {
        public AuditContext()
        {

        }

        public DbSet<Person> Person { get; set; }
        public DbSet<AuditT> AudiT { get; set; }

        public override int SaveChanges()
        {
            try
            {
                List<AuditT> AuditList = new List<AuditT>();
                IList<DbEntityEntry> AuditIncludes = new List<DbEntityEntry>();

                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
                {
                    foreach (var propName in entry.CurrentValues.PropertyNames)
                    {
                        var current = entry.CurrentValues[propName] == null ? "" : entry.CurrentValues[propName].ToString();
                        var original = entry.OriginalValues[propName] == null ? "" : entry.OriginalValues[propName].ToString();

                        if (current != original)
                        {
                            GerateAuditObj(AuditList, entry, propName, current, original, "M");
                        }
                    }
                }

                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted))
                {
                    foreach (var propName in entry.OriginalValues.PropertyNames)
                    {
                        var original = entry.OriginalValues[propName] == null ? "" : entry.OriginalValues[propName].ToString();
                        GerateAuditObj(AuditList, entry, propName, "", original, "D");
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
                        foreach (var propName in audit.CurrentValues.PropertyNames)
                        {
                            var current = audit.CurrentValues[propName] == null ? "" : audit.CurrentValues[propName].ToString();
                            GerateAuditObj(AuditList, audit, propName, current, "", "I");
                        }
                    }
                }

                this.SaveAudit(AuditList);
                return saveChanges;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        private static void GerateAuditObj(List<AuditT> AuditList, DbEntityEntry entry, string propName, string current, string original, string action)
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