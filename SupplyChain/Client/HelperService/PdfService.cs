using Microsoft.JSInterop;
using SupplyChain.Shared.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Client.HelperService
{
    public class PdfService : IDisposable
    {
        private readonly IJSRuntime js;

        public PdfService(IJSRuntime js )
        {
            this.js = js;
        }

        public async Task<string> EtiquetaYPF(string espaciosPedido, List<PedCli> PedCliList,
            ModeloOrdenFabricacion ordenFabricacion, Producto producto)
        {
            //Chapa de 101 mm x 78 mm
            PdfDocument document1 = new PdfDocument();
            document1.PageSettings.Size = new Syncfusion.Drawing.SizeF(382, 295);
            document1.PageSettings.Margins.All = 0;
            PdfGrid pdfGrid1 = new PdfGrid();
            PdfPage page = document1.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 16);
            PdfLightTable pdfTable = new PdfLightTable();
            page.Graphics.RotateTransform(-360);
            for (int i = 0; i < (20 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim().Length); i++)
            {
                espaciosPedido = espaciosPedido + " ";
            }
            var tipo2 = producto.CAMPOCOM6;
            string tipo = "";
            if (tipo2 is null || tipo2.Contains("System.Linq"))
            {
                tipo = "";
            }
            else
            {
                tipo = tipo2;
            }
            FileStream fs = new("wwwroot\\logo_aerre.jpg", FileMode.Open);

            graphics.DrawString($" \r\n" +
                $"\r\n" +
                $"\r\n" +
                $"    Año:{DateTime.Now.Year}  N°:{ordenFabricacion.PEDIDO} \r\n" +
                $"    TAG:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim()}\r\n" +
                $"    Tipo:\r\n" +
                $"    Codigo:{ordenFabricacion.CG_PROD.Trim()}\r\n" +
                $"    Medida:{producto.CAMPOCOM2.Trim()}  {producto.CAMPOCOM3.Trim()}\r\n" +
                $"    Clase:{producto.CAMPOCOM5.Trim()}\r\n" +
                $"    Temp:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM6.Trim()}\r\n" +
                $"    Presion SET:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM1.Trim()}\r\n" +
                $"    P. Aj Banco:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM4.Trim()}\r\n" +
                $"    Ctra.P:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM5.Trim()}\r\n" +
                $"    Fluido:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM3.Trim()}\r\n" +
                $"    Cuerpo:\r\n" +
                $"    Tobera:\r\n" +
                $"    Resorte:\r\n" +
                $"    T.OPDS N°:8/11\r\n" +
                $"    M.OPDS N°:47642\r\n" +
                $"          Arbros S.A.\r\n" +
                $"       www.aerre.com.ar\r\n" +
                $"     Industria  Argentina\r\n" +
                $"                               ", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(0, 0));


            MemoryStream xx = new MemoryStream();
            document1.Save(xx);
            document1.Close(true);
            await js.SaveAs("ETOF" + ordenFabricacion.CG_PROD.Trim() + ".pdf", xx.ToArray());
            return espaciosPedido;
        }


        public async Task EtiquetaRecepcion(Pedidos pedidos)
        {

            PdfDocument document1 = new();
            document1.PageSettings.Size = new Syncfusion.Drawing.SizeF(227, 70);//110
            int margin = -25;

            document1.PageSettings.Margins.Left = -2;
            document1.PageSettings.Margins.Right = -15;
            document1.PageSettings.Margins.Top = 10;
            document1.PageSettings.Margins.Bottom = -10;
            //document1.PageSettings.Margins.All = margin;
            PdfGrid pdfGrid1 = new PdfGrid();
            PdfPage page = document1.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            PdfLightTable pdfTable = new PdfLightTable();
            page.Graphics.RotateTransform(-90);


            graphics.DrawString($"{pedidos.CG_ART.Trim()}                  OC {pedidos.OCOMPRA}\r\n{pedidos.DES_ART.Trim()}\r\n" +
                $"Despacho {pedidos.DESPACHO} Lote {pedidos.LOTE} VALE {pedidos.VALE}\n" +
                $"{pedidos.Proveedor?.DES_PROVE.Trim()}", font, PdfBrushes.Black, new Syncfusion.Drawing.PointF(-200, 10));

            //document1.PageSettings.Margins.Left = margin;
            //document1.PageSettings.Margins.Right = margin;
            //document1.PageSettings.Margins.Top = margin;
            //document1.PageSettings.Margins.Bottom = margin;
            MemoryStream xx = new MemoryStream();
            document1.Save(xx);
            document1.Close(true);
            await js.SaveAs("ETOC" + pedidos.CG_ART.Trim() + ".pdf", xx.ToArray());
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
