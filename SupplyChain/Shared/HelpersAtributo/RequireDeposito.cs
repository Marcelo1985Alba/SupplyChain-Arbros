using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.HelpersAtributo
{
    class RequireDeposito : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var stock = (PedidoEncabezado)validationContext.ObjectInstance;
            //if (stock.EXIGESERIE == 0)
            //    return ValidationResult.Success;
            var cg_dep = (int)value;

            return stock.TIPOO == 5 && cg_dep == 0
                ? new ValidationResult("Ingresar deposito")
                : ValidationResult.Success;
        }
    }
}
