using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Audit.Models
{
    [Table("Auditoria", Schema = "public")]
    public class AuditT
    {
        [Key]
        public long idAuditoria { get; set; }

        public string tabela { get; set; }

        public long idlinha { get; set; }

        public string coluna { get; set; }

        [MaxLength(4000)]
        public string valor_atual { get; set; }

        [MaxLength(4000)]
        public string valor_atualizado { get; set; }

        public DateTime dataOcorrencia { get; set; }

        public long idUsuario { get; set; }

        public string acao { get; set; }

    }
}