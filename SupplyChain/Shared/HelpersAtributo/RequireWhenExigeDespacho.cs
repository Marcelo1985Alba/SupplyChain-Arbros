using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SupplyChain.Shared.HelpersAtributo
{
    public class RequireWhenExigeDespachoAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var stock = (Pedidos)validationContext.ObjectInstance;
            if (!stock.EXIGEDESPACHO)
                return ValidationResult.Success;

            var despacho = value as string;
            return string.IsNullOrWhiteSpace(despacho)
                ? new ValidationResult($"Ingresar Despacho: el insumo {stock.CG_ART.Trim()} exige despacho")
                : ValidationResult.Success;
        }
    }
}
