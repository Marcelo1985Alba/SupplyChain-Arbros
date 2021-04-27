using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.HelpersAtributo
{
    public class RequireRemitoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var stock = (PedidoEncabezado)validationContext.ObjectInstance;
            //if (stock.EXIGESERIE == 0)
            //    return ValidationResult.Success;

            var remito = value as string;
            return stock.TIPOO == 5 && ( string.IsNullOrEmpty(remito) || remito.Length > 13 || remito == "0000-00000000")
                ? new ValidationResult("Ingresar remito válido")
                : ValidationResult.Success;
        }
    }
}
