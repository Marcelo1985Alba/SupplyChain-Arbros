﻿using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SupplyChain.Shared.HelpersAtributo
{
    public class RequireWhenExigeSerieAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var stock = (Pedidos)validationContext.ObjectInstance;
            if (!stock.EXIGESERIE)
                return ValidationResult.Success;

            var serie = value as string;
            return string.IsNullOrWhiteSpace(serie)
                ? new ValidationResult($"Ingresar Serie: el insumo {stock.CG_ART.Trim()} exige serie")
                : ValidationResult.Success;
        }
    }
}
