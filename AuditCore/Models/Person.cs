using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditCore.Models
{

    [Table("Person", Schema = "Audit")]
    public class Person
    {
        [Key]
        public int idperson { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Number { get; set; }


    }
}