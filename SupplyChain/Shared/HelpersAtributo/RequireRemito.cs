using System.ComponentModel.DataAnnotations;

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
            var noTieneFormato = string.IsNullOrEmpty(remito) || remito.Length > 13 || remito == "0000-00000000";

            return stock.TIPOO == 5 || stock.TIPOO == 1 && noTieneFormato
                ? new ValidationResult("Ingresar remito válido")
                : ValidationResult.Success;
        }
    }
}
