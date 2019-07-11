using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Audit.Models
{

    [Table("Person", Schema = "public")]
    public class Person
    {
        [Key]
        public long idperson { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Number { get; set; }


    }
}