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
            audit.tabela = entry.Entity.GetType().Name.Split('_')[0];
            audit.idlinha = acao == "Deletado" ? Convert.ToInt32(entry.OriginalValues[entry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Count() > 0).Name]) :
                                                 Convert.ToInt32(entry.CurrentValues[entry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Count() > 0).Name]);
            audit.coluna = propName;
            audit.valor_atual = atual.ToString();
            audit.valor_atualizado = original.ToString();
            audit.dataOcorrencia = System.DateTime.Now;
            audit.idUsuario = 1;
            audit.acao = acao;
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
                        if (!"DthInclusao.UsuarioInclusaoId.DthAtualizacao.UsuarioAlteracaoId".Contains(propName))
                        {
                            var atual = entry.CurrentValues[propName] == null ? "" : entry.CurrentValues[propName].ToString();
                            var original = entry.OriginalValues[propName] == null ? "" : entry.OriginalValues[propName].ToString();

                            if (atual != original)
                            {
                                GerarAuditoriaObj(auditoriaList, entry, propName, atual, original, "Modificado");
                            }
                        }
                    }
                }

                foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted))
                {
                    foreach (var propName in entry.OriginalValues.PropertyNames)
                    {
                        if (!"DthInclusao.UsuarioInclusaoId.DthAtualizacao.UsuarioAlteracaoId".Contains(propName))
                        {
                            var original = entry.OriginalValues[propName] == null ? "" : entry.OriginalValues[propName].ToString();
                            GerarAuditoriaObj(auditoriaList, entry, propName, "", original, "Deletado");
                        }
                    }
                }

                var saveChanges = base.SaveChanges();

                if (AuditarInclusoes != null)
                {
                    foreach (var audit in AuditarInclusoes)
                    {
                        foreach (var propName in audit.CurrentValues.PropertyNames)
                        {
                            if (!"DthInclusao.UsuarioInclusaoId.DthAtualizacao.UsuarioAlteracaoId".Contains(propName))
                            {
                                var atual = audit.CurrentValues[propName] == null ? "" : audit.CurrentValues[propName].ToString();
                                GerarAuditoriaObj(auditoriaList, audit, propName, atual, "", "Adicionado");
                            }
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