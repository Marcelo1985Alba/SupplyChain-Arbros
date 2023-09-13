using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyChain.Shared.HelpersAtributo
{
    class RequireDepositoItem : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var stock = (Pedidos)validationContext.ObjectInstance;

            var cg_dep = (int)value;

            return stock.TIPOO == 9 && cg_dep == 0
                ? new ValidationResult($"{stock.CG_ART}: Ingresar deposito")
                : ValidationResult.Success;
        }
    }
}
