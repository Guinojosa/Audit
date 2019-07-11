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

        private static void GerarAuditoriaObj(List<AuditT> auditoriaList, DbEntityEntry entry, string propName, string atual, string original, string acao)
        {
            AuditT audit = new AuditT();
            audit.Entity = entry.Entity.GetType().Name.Split('_')[0];
            audit.IdAudit = acao == "D" ? Convert.ToInt32(entry.OriginalValues[entry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Count() > 0).Name]) :
                                                 Convert.ToInt32(entry.CurrentValues[entry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Count() > 0).Name]);
            audit.Column = propName;
            audit.Value_Current = atual.ToString();
            audit.Value_Original = original.ToString();
            audit.DateOccurrence = System.DateTime.Now;
            audit.IdUserModified = 1;
            audit.Action = acao;
            auditoriaList.Add(audit);
        }

        public override int SaveChanges()
        {
            try
            {
                List<AuditT> auditoriaList = new List<AuditT>();
                IList<DbEntityEntry> AuditarInclusoes = new List<DbEntityEntry>();

                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
                {
                    foreach (var propName in entry.CurrentValues.PropertyNames)
                    {
                        var atual = entry.CurrentValues[propName] == null ? "" : entry.CurrentValues[propName].ToString();
                        var original = entry.OriginalValues[propName] == null ? "" : entry.OriginalValues[propName].ToString();

                        if (atual != original)
                        {
                            GerarAuditoriaObj(auditoriaList, entry, propName, atual, original, "M");
                        }
                    }
                }

                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted))
                {
                    foreach (var propName in entry.OriginalValues.PropertyNames)
                    {
                        var original = entry.OriginalValues[propName] == null ? "" : entry.OriginalValues[propName].ToString();
                        GerarAuditoriaObj(auditoriaList, entry, propName, "", original, "D");
                    }
                }

                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                {
                    AuditarInclusoes.Add(entry);
                }

                var saveChanges = base.SaveChanges();

                if (AuditarInclusoes != null)
                {
                    foreach (var audit in AuditarInclusoes)
                    {
                        foreach (var propName in audit.CurrentValues.PropertyNames)
                        {
                            var atual = audit.CurrentValues[propName] == null ? "" : audit.CurrentValues[propName].ToString();
                            GerarAuditoriaObj(auditoriaList, audit, propName, atual, "", "I");
                        }
                    }
                }
                this.auditoria(auditoriaList);
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

        public void auditoria(List<AuditT> auditoriaList)
        {
            foreach (var audit in auditoriaList)
            {
                this.AudiT.Add(audit);
            }
            base.SaveChanges();
        }

    }
}