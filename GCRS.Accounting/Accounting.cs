using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GCRS.Accounting
{
    public class DailySit
    {
        public int ID { get; set; }
        [Display(Name ="Fecha")]
        public virtual DateTime Date { get; set; }
        [Display(Name = "Valor")]
        public double Value { get; set; }
        [Display(Name = "Concepto")]
        public virtual Concept TypeOfCompensation { get; set; }
    }

    //agregarle virtual a todo lo que sea navegable

    public class Concept
    {
        public int ConceptID { get; set; }
        [Display(Name = "Concepto")]
        public string Name { get; set; }
        [Display(Name = "Ganancia")]
        public bool Income { get; set; }
    }
}
