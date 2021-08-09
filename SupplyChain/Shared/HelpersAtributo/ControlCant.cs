using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SupplyChain.Shared.HelpersAtributo
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    /// <summary>
    /// Control de cantidades ingresadas para los diferentes tipos de operaciones
    /// </summary>
    public class ControlCantAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var stock = (Pedidos)validationContext.ObjectInstance;

            decimal? cant = (decimal?)value;
            if (stock.TIPOO == 10 && stock.ResumenStock?.STOCK == 0)//entrega a og
            {
                return new ValidationResult($"{stock.CG_ART.Trim()}: Insumo sin stock.");
            }

            if (stock.TIPOO == 10 && stock.STOCK < 0)//entrega a og
            {
                return new ValidationResult($"{stock.CG_ART.Trim()}: No se pueden entregar cantidades negativas.");
            }

            //PendienteOC: tambien se utiliza para obtener el stock
            return ((stock.TIPOO == 6 || stock.TIPOO == 10) && stock.STOCK > stock.ResumenStock?.STOCK)
                ? new ValidationResult($"{stock.CG_ART.Trim()}: La cantidad ingresada no puede ser mayor a la del stock")
                : ValidationResult.Success;

        }
    }
}
