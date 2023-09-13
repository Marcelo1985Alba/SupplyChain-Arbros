using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.HelpersAtributo
{
    public class EstadoCursoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var programa = (Fabricacion)validationContext.ObjectInstance;

            var fechaCurso = (DateTime?)value;
            return programa.CG_ESTADOCARGA == 3 && fechaCurso == null
                ? new ValidationResult("Ingresar Fecha Curso")
                : ValidationResult.Success;

            //var cant = (decimal?)value;
            //return cant == default || cant == 0
            //    ? new ValidationResult("Ingresar cantidad")
            //    : ValidationResult.Success;
        }
    }
}
