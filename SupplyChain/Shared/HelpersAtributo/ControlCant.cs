using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SupplyChain.Shared.HelpersAtributo
{
    /// <summary>
    /// Control de cantidades ingresadas para los diferentes tipos de operaciones
    /// </summary>
    public class ControlCantAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var stock = (Pedidos)validationContext.ObjectInstance;

            decimal? cant = (decimal?)value;
            if (stock.TIPOO == 10 && stock.PENDIENTEOC == 0)//entrega a og
            {
                return new ValidationResult($"{stock.CG_ART.Trim()}: Insumo sin stock.");
            }

            //PendienteOC: tambien se utiliza para obtener el stock
            return ((stock.TIPOO == 6 || stock.TIPOO == 10) && stock.STOCK > stock.PENDIENTEOC)
                ? new ValidationResult($"{stock.CG_ART.Trim()}:La cantidad ingresada no puede ser mayor a la del stock")
                : ValidationResult.Success;

            //var cant = (decimal?)value;
            //return cant == default || cant == 0
            //    ? new ValidationResult("Ingresar cantidad")
            //    : ValidationResult.Success;
        }
    }
}
