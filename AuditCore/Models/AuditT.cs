using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AuditCore.Models
{
    [Table("AuditT", Schema = "Audit")]
    public class AuditT
    {
        public int IdAudit { get; set; }

        public string Entity { get; set; }

        public long IdRegister { get; set; }

        public string Column { get; set; }

        [MaxLength(4000)]
        public string Value_Current { get; set; }

        [MaxLength(4000)]
        public string Value_Original { get; set; }

        public DateTime DateOccurrence { get; set; }

        public long IdUserModified { get; set; }

        public string Action { get; set; }

    }
}