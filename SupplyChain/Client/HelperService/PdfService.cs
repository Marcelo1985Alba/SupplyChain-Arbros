using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.PCP;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Tables;

namespace SupplyChain.Client.HelperService;

public class PdfService : IDisposable
{
    private readonly HttpClient Http;
    private readonly IJSRuntime js;

    public PdfService(IJSRuntime js, HttpClient httpClient)
    {
        this.js = js;
        Http = httpClient;
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public async Task EtiquetaOF(int OrdenDeFabAlta, ModeloOrdenFabricacion ordenFabricacion)
    {
        //Create a new PDF document
        PdfDocument document = new()
        {
            PageSettings = new PdfPageSettings(new SizeF(227, 70))
        };
        document.PageSettings.Margins.All = 0;
        var page = document.Pages.Add();
        var graphics = page.Graphics;

        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
        PdfFont font1 = new PdfStandardFont(PdfFontFamily.TimesRoman, 7f, PdfFontStyle.Bold);
        graphics.DrawString(
            $"        OF ALTA: {OrdenDeFabAlta}\r\n            {ordenFabricacion.CG_PROD}\r\n{ordenFabricacion.DES_PROD}\r\nCANTIDAD {ordenFabricacion.CANTFAB}    {ordenFabricacion.FE_CIERRE}",
            font, PdfBrushes.Black, new PointF(30, 10));

        //Save the PDF to the MemoryStream
        var ms = new MemoryStream();
        document.Save(ms);
        //If the position is not set to '0' then the PDF will be empty.
        ms.Position = 0;
        //Close the PDF document.
        document.Close(true);
        await js.SaveAs("ETOF" + ordenFabricacion.CG_PROD.Trim() + ".pdf", ms.ToArray());
    }

    /// <summary>
    ///     Certificado de Producto
    /// </summary>
    /// <param name="Articulo"></param>
    /// <param name="TrzNro"></param>
    /// <param name="vpedidos"></param>
    /// <returns></returns>
    public async Task Catalogo(string Articulo, string TrzNro, List<vTrazabilidad> vpedidos)
    {
        var ProdCertificado = await Http.GetFromJsonAsync<List<Producto>>($"api/Prod/BuscarPorCG_PROD/{Articulo}");
        var PedcliCertificado = await Http.GetFromJsonAsync<List<PedCli>>($"api/PedCli/BuscarPorPedido/{TrzNro}");
        var PedidosCertificado = await Http.GetFromJsonAsync<List<Pedidos>>($"api/Pedidos/BusquedaParaFE_MOV/{TrzNro}");
        var roscas = "";
        var Ansi_Api = "";

        var tobera = string.Empty;
        var cuerpo = string.Empty;
        var disco = string.Empty;
        var bonete = string.Empty;
        var toberaDespacho = string.Empty;
        var cuerpoDespacho = string.Empty;
        var discoDespacho = string.Empty;
        var boneteDespacho = string.Empty;
        var cuerpoPresionPrueba = string.Empty;
        var bonetePresionPrueba = string.Empty;
        //de 0 a 60 (CAMPOCOM4)  el Patrón es 2-54275.  de 60 a 400 el Patrón es AE4213
        decimal presAJ = 0;
        decimal.TryParse(vpedidos[0].CAMPOCOM4, out presAJ);
        var patronUtilizado = "";
        if (presAJ < 60)
            patronUtilizado = "2-54275";
        else
            patronUtilizado = "AE4213";
        if (vpedidos.Any(p => p.DES_ART.ToLower().StartsWith("tobera")))
        {
            tobera = vpedidos.Where(p => p.DES_ART.ToLower().StartsWith("tobera")).FirstOrDefault().NORMA;
            toberaDespacho = vpedidos.Where(p => p.DES_ART.ToLower().StartsWith("tobera")).FirstOrDefault().DESPACHO;
        }


        if (vpedidos.Any(p => p.DES_ART.ToLower().StartsWith("cuerpo") || p.DES_ART.ToLower().StartsWith("cpo")))
        {
            cuerpo = vpedidos
                .Where(p => p.DES_ART.ToLower().StartsWith("cuerpo") || p.DES_ART.ToLower().StartsWith("cpo"))
                .FirstOrDefault().NORMA;
            cuerpoDespacho = vpedidos
                .Where(p => p.DES_ART.ToLower().StartsWith("cuerpo") || p.DES_ART.ToLower().StartsWith("cpo"))
                .FirstOrDefault().DESPACHO;
            cuerpoPresionPrueba = vpedidos
                .Where(p => p.DES_ART.ToLower().StartsWith("cuerpo") || p.DES_ART.ToLower().StartsWith("cpo"))
                .FirstOrDefault().PH.ToString();
        }


        if (vpedidos.Any(p => p.DES_ART.ToLower().StartsWith("disco")))
        {
            disco = vpedidos.Where(p => p.DES_ART.ToLower().StartsWith("disco")).FirstOrDefault().NORMA;
            discoDespacho = vpedidos.Where(p => p.DES_ART.ToLower().StartsWith("disco")).FirstOrDefault().DESPACHO;
        }


        if (vpedidos.Any(p => p.DES_ART.ToLower().StartsWith("bonete")))
        {
            bonete = vpedidos.Where(p => p.DES_ART.ToLower().StartsWith("bonete")).FirstOrDefault().NORMA;
            boneteDespacho = vpedidos.Where(p => p.DES_ART.ToLower().StartsWith("bonete")).FirstOrDefault().DESPACHO;
            bonetePresionPrueba = vpedidos.Where(p => p.DES_ART.ToLower().StartsWith("bonete")).FirstOrDefault().PH
                .ToString();
        }


        if (Articulo.Substring(0, 1) == "1")
        {
            roscas = "Aprobado";
            Ansi_Api = "-";
        }
        else
        {
            roscas = "-";
            Ansi_Api = "Aprobado";
        }

        //Create a new PDF document
        var document = new PdfDocument();
        //Create the page
        var page = document.Pages.Add();
        //Create PDF graphics for the page
        var fuente = await Http.GetStreamAsync("./Calibri 400.ttf");
        //FileStream fontStream = new FileStream(fuente);
        //Create a PdfGrid
        var pdfGrid = new PdfGrid();

        var graphics = page.Graphics;
        //PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 10, PdfFontStyle.Bold);
        //PdfFont font = new PdfTrueTypeFont("wwwroot\\Calibri 400.ttf", 24);
        PdfFont font = new PdfTrueTypeFont(fuente, 10, PdfFontStyle.Bold);

        //Create and customize the string formats
        var Centrado = new PdfStringFormat();
        Centrado.Alignment = PdfTextAlignment.Center;
        Centrado.LineAlignment = PdfVerticalAlignment.Middle;

        //Create and customize the string formats
        var CentradoTop = new PdfStringFormat();
        CentradoTop.Alignment = PdfTextAlignment.Center;
        CentradoTop.LineAlignment = PdfVerticalAlignment.Top;

        //Create and customize the string formats
        var Izquierda = new PdfStringFormat();
        Izquierda.Alignment = PdfTextAlignment.Left;
        Izquierda.LineAlignment = PdfVerticalAlignment.Middle;
        //Add columns to PdfGrid
        for (var i = 0; i < 6; i++)
        {
            var column = pdfGrid.Columns.Add();
            if (i == 0 || i == 1 || i == 4 || i == 5) column.Width = 64;
            if (i == 2 || i == 3) column.Width = 128;
        }

        //Add rows to PdfGrid
        for (var i = 0; i < 32; i++)
        {
            var row = pdfGrid.Rows.Add();
            if (i == 0 || i == 1 || i == 2 || i == 3 || i == 7 || i == 17 || i == 21)
                row.Height = 26;
            else if (i == 31)
                row.Height = 47;
            else
                row.Height = 22;
        }

        //Load the image from the stream
        //FileStream fs = new FileStream("wwwroot\\logo_aerre.jpg", FileMode.Open);
        //FileStream IMR = new FileStream("wwwroot\\IMR.jpg", FileMode.Open);
        var fs = await Http.GetStreamAsync("./logo_aerre.jpg");
        var IMR = await Http.GetStreamAsync("./IMR.jpg");
        //LINEA 0
        //Add RowSpan
        var gridCell1 = pdfGrid.Rows[0].Cells[0];
        gridCell1.ColumnSpan = 2;
        gridCell1.RowSpan = 2;
        gridCell1.StringFormat = Centrado;
        gridCell1.Value = new PdfBitmap(fs);
        //Add RowSpan
        var gridCell2 = pdfGrid.Rows[0].Cells[2];
        gridCell2.ColumnSpan = 2;
        gridCell2.RowSpan = 2;
        gridCell2.StringFormat = Centrado;
        gridCell2.Value = new PdfTextElement("CERTIFICADO DE PRODUCTO", font, new PdfPen(PdfColor.Empty),
            PdfBrushes.Black, Centrado);
        //Add RowSpan
        var gridCell3 = pdfGrid.Rows[0].Cells[4];
        gridCell3.ColumnSpan = 2;
        gridCell3.RowSpan = 2;
        gridCell3.StringFormat = Centrado;
        gridCell3.Value = new PdfTextElement($"Cliente: \n {PedcliCertificado.FirstOrDefault().DES_CLI.Trim()}", font,
            new PdfPen(PdfColor.Empty), PdfBrushes.Black, Centrado);

        //LINEA 2
        //Add RowSpan
        var gridCell4 = pdfGrid.Rows[2].Cells[0];
        gridCell4.ColumnSpan = 2;
        gridCell4.RowSpan = 2;
        gridCell4.StringFormat = Centrado;
        gridCell4.Value =
            new PdfTextElement("ARBROS S.A.", font, new PdfPen(PdfColor.Empty), PdfBrushes.Black, Centrado);
        //Add RowSpan
        var gridCell5 = pdfGrid.Rows[2].Cells[2];
        gridCell5.ColumnSpan = 2;
        gridCell5.RowSpan = 2;
        gridCell5.StringFormat = Centrado;
        gridCell5.Value = new PdfTextElement("VÁLVULAS DE SEGURIDAD Y ALIVIO", font, new PdfPen(PdfColor.Empty),
            PdfBrushes.Black, Centrado);
        //Add RowSpan
        var gridCell6 = pdfGrid.Rows[2].Cells[4];
        gridCell6.ColumnSpan = 2;
        gridCell6.RowSpan = 2;
        gridCell6.StringFormat = Centrado;
        gridCell6.Value = new PdfTextElement($"O.C.I.: {PedcliCertificado.FirstOrDefault().NUMOCI}\n" +
                                             $"{PedidosCertificado.FirstOrDefault().FE_MOV.ToShortDateString()}", font,
            new PdfPen(PdfColor.Empty), PdfBrushes.Black, Centrado);

        //LINEA 4 GENERALIDADES
        //Add RowSpan
        var gridCell7 = pdfGrid.Rows[4].Cells[0];
        gridCell7.ColumnSpan = 2;
        gridCell7.StringFormat = Izquierda;
        gridCell7.Value =
            new PdfTextElement("GENERALIDADES", font, new PdfPen(PdfColor.Empty), PdfBrushes.Black, Centrado);
        //Add RowSpan
        var gridCell8 = pdfGrid.Rows[4].Cells[2];
        gridCell8.ColumnSpan = 1;
        gridCell8.StringFormat = Izquierda;
        gridCell8.Value = "   Orden de compra:\n" +
                          "   Remito de entrega:";
        //Add RowSpan
        var gridCell9 = pdfGrid.Rows[4].Cells[3];
        gridCell9.ColumnSpan = 3;
        gridCell9.StringFormat = Izquierda;
        gridCell9.Value = $"   {PedcliCertificado.FirstOrDefault().ORCO.Trim()}\n" +
                          $"   {vpedidos.FirstOrDefault(p => p.TIPOO == 1).REMITO.Trim()}";

        //LINEA 5 DATOS DE PLACA
        //Add RowSpan
        var gridCell10 = pdfGrid.Rows[5].Cells[0];
        gridCell10.ColumnSpan = 2;
        gridCell10.RowSpan = 6;
        gridCell10.StringFormat = Izquierda;
        gridCell10.Value =
            new PdfTextElement("DATOS DE PLACA", font, new PdfPen(PdfColor.Empty), PdfBrushes.Black, Centrado);
        //Add RowSpan
        var gridCell11 = pdfGrid.Rows[5].Cells[2];
        gridCell11.ColumnSpan = 1;
        gridCell11.RowSpan = 6;
        gridCell11.StringFormat = Izquierda;
        gridCell11.Value = "   Linea de Producto:\n" +
                           "   Nro de Serie:\n" +
                           "   TAG:\n" +
                           "   Medida (Ent x Sal):\n" +
                           "   Clase (Ent x Sal):\n" +
                           "   Orif. API 526 / Area:\n" +
                           "   Presión de Apertura (Bar):\n" +
                           "   Contrapresión:\n" +
                           "   Presión de ajus en Banco (Bar):\n" +
                           "   Fluido:\n" +
                           "   Temperatura (ºC):\n" +
                           "   Código de Resorte:\n" +
                           "   Código de la válvula:";
        //Add RowSpan
        var gridCell12 = pdfGrid.Rows[5].Cells[3];
        gridCell12.ColumnSpan = 3;
        gridCell12.RowSpan = 6;
        gridCell12.StringFormat = Izquierda;
        gridCell12.Value = $"   {ProdCertificado.FirstOrDefault().CAMPOCOM1.Trim()}\n" +
                           $"   {TrzNro}\n" +
                           $"   {PedcliCertificado.FirstOrDefault().LOTE.Trim()}\n" +
                           $"   {ProdCertificado.FirstOrDefault().CAMPOCOM2.Trim()}\n" +
                           $"   {ProdCertificado.FirstOrDefault().CAMPOCOM5.Trim()}\n" +
                           $"   {ProdCertificado.FirstOrDefault().CAMPOCOM3.Trim()}\n" +
                           $"   {PedcliCertificado.FirstOrDefault().CAMPOCOM1.Trim()}\n" +
                           $"   {PedcliCertificado.FirstOrDefault().CAMPOCOM5.Trim()}\n" +
                           $"   {PedcliCertificado.FirstOrDefault().CAMPOCOM4.Trim()}\n" +
                           $"   {PedcliCertificado.FirstOrDefault().CAMPOCOM3.Trim()}\n" +
                           $"   {PedcliCertificado.FirstOrDefault().CAMPOCOM6.Trim()}\n" +
                           $"   {PedcliCertificado.FirstOrDefault().CAMPOCOM2.Trim()}\n" +
                           $"   {Articulo}";

        //CONTROL DIMENSIONAL
        //Add RowSpan
        var gridCell13 = pdfGrid.Rows[11].Cells[0];
        gridCell13.ColumnSpan = 2;
        gridCell13.RowSpan = 2;
        gridCell13.StringFormat = Izquierda;
        gridCell13.Value = new PdfTextElement("CONTROL DIMENSIONAL", font, new PdfPen(PdfColor.Empty), PdfBrushes.Black,
            Centrado);
        //Add RowSpan
        var gridCell14 = pdfGrid.Rows[11].Cells[2];
        gridCell14.ColumnSpan = 1;
        gridCell14.StringFormat = Izquierda;
        gridCell14.RowSpan = 2;
        gridCell14.Value = $"   ANSI B-16.5 (Ent-Sal)\n" +
                           $"   Roscas (Ent-Sal)\n" +
                           $"   API 526";
        //Add RowSpan
        var gridCell15 = pdfGrid.Rows[11].Cells[3];
        gridCell15.ColumnSpan = 3;
        gridCell15.StringFormat = Izquierda;
        gridCell15.RowSpan = 2;
        gridCell15.Value = $"   {Ansi_Api}\n" +
                           $"   {roscas}\n" +
                           $"   {Ansi_Api}";

        //PRUEBA HIDRAULICA Y MATERIALES
        //Add RowSpan
        var gridCell16 = pdfGrid.Rows[13].Cells[0];
        gridCell16.ColumnSpan = 2;
        gridCell16.RowSpan = 8;
        gridCell16.StringFormat = Izquierda;
        gridCell16.Value = new PdfTextElement("PRUEBA HIDRAULICA Y MATERIALES", font, new PdfPen(PdfColor.Empty),
            PdfBrushes.Black, Centrado);
        //Add RowSpan
        var gridCell17 = pdfGrid.Rows[13].Cells[2];
        gridCell17.ColumnSpan = 1;
        gridCell17.RowSpan = 2;
        gridCell17.StringFormat = Izquierda;
        gridCell17.Value = $"   Material Disco\n" +
                           $"   Identificación Disco";
        //Add RowSpan
        var gridCell18 = pdfGrid.Rows[13].Cells[3];
        gridCell18.ColumnSpan = 3;
        gridCell18.RowSpan = 2;
        gridCell18.StringFormat = Izquierda;
        gridCell18.Value = $"   {disco}\n" +
                           $"   {discoDespacho}";
        //Add RowSpan
        var gridCell19 = pdfGrid.Rows[15].Cells[2];
        gridCell19.ColumnSpan = 1;
        gridCell19.RowSpan = 2;
        gridCell19.StringFormat = Izquierda;
        gridCell19.Value = $"   Material Tobera\n" +
                           $"   Identificación Tobera";
        //Add RowSpan
        var gridCell20 = pdfGrid.Rows[15].Cells[3];
        gridCell20.ColumnSpan = 3;
        gridCell20.RowSpan = 2;
        gridCell20.StringFormat = Izquierda;
        gridCell20.Value = $"   {tobera}\n" +
                           $"   {toberaDespacho}";
        //Add RowSpan
        var gridCell21 = pdfGrid.Rows[17].Cells[2];
        gridCell21.ColumnSpan = 1;
        gridCell21.RowSpan = 2;
        gridCell21.StringFormat = Izquierda;
        gridCell21.Value = "   Material Cuerpo\n" +
                           "   Identificación Cuerpo\n" +
                           "   Presión de PH (Bar)";
        //Add RowSpan
        var gridCell22 = pdfGrid.Rows[17].Cells[3];
        gridCell22.ColumnSpan = 3;
        gridCell22.RowSpan = 2;
        gridCell22.StringFormat = Izquierda;
        gridCell22.Value = $"   {cuerpo}\n" +
                           $"   {cuerpoDespacho}\n" +
                           $"   {cuerpoPresionPrueba}";
        //Add RowSpan
        var gridCell23 = pdfGrid.Rows[19].Cells[2];
        gridCell23.ColumnSpan = 1;
        gridCell23.RowSpan = 2;
        gridCell23.StringFormat = Izquierda;
        gridCell23.Value = "   Material Bonete\n" +
                           "   Identificación Bonete\n" +
                           "   Presión de PH (Bar)";
        //Add RowSpan
        var gridCell24 = pdfGrid.Rows[19].Cells[3];
        gridCell24.ColumnSpan = 3;
        gridCell24.RowSpan = 2;
        gridCell24.StringFormat = Izquierda;
        gridCell24.Value = $"   {bonete}\n" +
                           $"   {boneteDespacho}\n" +
                           $"   {bonetePresionPrueba}";

        //ENSAYOS OPERACIONALES EN BANCO HIDRONEUMÁTICO
        //Add RowSpan
        var gridCell25 = pdfGrid.Rows[21].Cells[0];
        gridCell25.ColumnSpan = 2;
        gridCell25.RowSpan = 4;
        gridCell25.StringFormat = Izquierda;
        gridCell25.Value =
            new PdfTextElement("ENSAYOS OPERACIONALES EN BANCO HIDRONEUMÁTICO - FLUIDO AIRE A TEMP. AMBIENTE ", font,
                new PdfPen(PdfColor.Empty), PdfBrushes.Black, Centrado);
        //Add RowSpan
        var gridCell26 = pdfGrid.Rows[21].Cells[2];
        gridCell26.ColumnSpan = 3;
        gridCell26.RowSpan = 3;
        gridCell26.StringFormat = Izquierda;
        gridCell26.Value = "   Calibracion a la presión segun ASME PTC-25.3:\n" +
                           "   Ensayos de repetitividad (3):\n" +
                           "   Control estanqueidad de asientos segun API 527:";
        //Add RowSpan
        var gridCell27 = pdfGrid.Rows[21].Cells[5];
        gridCell27.ColumnSpan = 1;
        gridCell27.RowSpan = 3;
        gridCell27.StringFormat = Izquierda;
        gridCell27.Value = "   Satisfactorio\n" +
                           "   Satisfactorio\n" +
                           "   Satisfactorio";
        //Add RowSpan
        var gridCell28 = pdfGrid.Rows[24].Cells[2];
        gridCell28.ColumnSpan = 3;
        gridCell28.StringFormat = Izquierda;
        gridCell28.Value = " Patrón Utilizado:";
        //Add RowSpan
        var gridCell29 = pdfGrid.Rows[24].Cells[5];
        gridCell29.ColumnSpan = 1;
        gridCell29.StringFormat = Izquierda;
        gridCell29.Value = $"     {patronUtilizado}";

        //RESULTADO
        //Add RowSpan
        var gridCell30 = pdfGrid.Rows[25].Cells[0];
        gridCell30.ColumnSpan = 2;
        gridCell30.StringFormat = Izquierda;
        gridCell30.Value =
            new PdfTextElement("RESULTADO ", font, new PdfPen(PdfColor.Empty), PdfBrushes.Black, Centrado);
        //Add RowSpan
        var gridCell31 = pdfGrid.Rows[25].Cells[2];
        gridCell31.ColumnSpan = 4;
        gridCell31.StringFormat = Izquierda;
        gridCell31.Value = new PdfTextElement("Satisfactorio", font, new PdfPen(PdfColor.Empty), PdfBrushes.AliceBlue,
            Centrado);

        //FIRMA
        //Add RowSpan
        var gridCellx35 = pdfGrid.Rows[26].Cells[0];
        gridCellx35.ColumnSpan = 2;
        gridCellx35.RowSpan = 3;
        gridCellx35.StringFormat = Izquierda;
        gridCellx35.Value = new PdfTextElement("FIRMA INSPECTOR AERRE ", font, new PdfPen(PdfColor.Empty),
            PdfBrushes.Black, Centrado);
        //Add RowSpan
        var gridCellx36 = pdfGrid.Rows[26].Cells[2];
        gridCellx36.ColumnSpan = 1;
        gridCellx36.RowSpan = 2;
        gridCellx36.StringFormat = Izquierda;
        gridCellx36.Value = new PdfBitmap(IMR);
        //Add RowSpan
        var gridCellAclara = pdfGrid.Rows[26].Cells[3];
        gridCellAclara.ColumnSpan = 3;
        gridCellAclara.RowSpan = 2;
        gridCellAclara.StringFormat = Centrado;
        gridCellAclara.Value = $"   Ing. Iris Mónica Rabboni\n" +
                               $"   Nº en OPDS s/res 1126: 188\n" +
                               $"   Matrícula: 47642";

        //Add RowSpan
        //PdfGridCell gridCellDirecc = pdfGrid.Rows[26].Cells[4];
        //gridCellAclara.ColumnSpan = 2;
        //gridCellAclara.RowSpan = 3;
        //gridCellAclara.StringFormat = Izquierda;
        //gridCellAclara.Value = $"   ARBROS SA.\n" +
        //    $"   Parque Industrial Desarrollo Productivo\n" +
        //    $"   Ruta 24 5801, Moreno, Provincia de Buenos Aires";

        //PRECINTO
        //Add RowSpan
        var gridCellx37 = pdfGrid.Rows[28].Cells[2];
        gridCellx37.ColumnSpan = 4;
        gridCellx37.StringFormat = Izquierda;
        gridCellx37.Value = new PdfTextElement(
            "Parque Industrial Desarrollo Productivo Ruta 24 5801, Moreno, Provincia de Buenos Aires",
            font, new PdfPen(PdfColor.Empty), PdfBrushes.Black, Centrado);


        //Add RowSpan
        var gridCellPrecinto = pdfGrid.Rows[29].Cells[0];
        gridCellPrecinto.ColumnSpan = 2;
        gridCellPrecinto.StringFormat = Izquierda;
        gridCellPrecinto.Value = "";

        var gridCellx38 = pdfGrid.Rows[29].Cells[2];
        gridCellx38.ColumnSpan = 4;
        gridCellx38.StringFormat = Izquierda;
        gridCellx38.Value = "     PRECINTO ";


        //GARANTIA
        //Add RowSpan
        var gridCell58 = pdfGrid.Rows[30].Cells[0];
        gridCell58.RowSpan = 2;
        gridCell58.ColumnSpan = 6;
        gridCell58.StringFormat = CentradoTop;
        gridCell58.Value = "GARANTIA:\n" +
                           " Los productos están garantizados por el término de 12 meses a partir de la salida de fábrica," +
                           " contra todo defecto de materiales y / o fabricación, limitada a la reposición sin cargo en nuestra fábrica de un " +
                           "elemento similar. Esta garantía no ampara el desgaste anormal por la utilización en condiciones distintas a las " +
                           "especificadas\n\n" +
                           "Se recomienda efectuar un mantenimiento periódico para extender su vida útil.";

        //Draw the PdfGrid
        pdfGrid.Draw(page, new PointF(0, 0));
        //Saving the PDF to the MemoryStream
        var stream = new MemoryStream();
        document.Save(stream);
        //Set the position as '0'
        stream.Position = 0;
        //Close the document
        document.Close(true);
        await js.SaveAs($"{TrzNro} Certificado" + ".pdf", stream.ToArray());
    }


    public async Task<string> EtiquetaYPF(string espaciosPedido, List<PedCli> PedCliList,
        ModeloOrdenFabricacion ordenFabricacion, Producto producto)
    {
        //Chapa de 101 mm x 78 mm
        var document1 = new PdfDocument();
        document1.PageSettings.Size = new SizeF(382, 295);
        document1.PageSettings.Margins.All = 0;
        var pdfGrid1 = new PdfGrid();
        var page = document1.Pages.Add();
        var graphics = page.Graphics;
        PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 16);
        var pdfTable = new PdfLightTable();
        page.Graphics.RotateTransform(-360);
        for (var i = 0;
             i < 20 - PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO)
                 .FirstOrDefault().LOTE.Trim().Length;
             i++) espaciosPedido = espaciosPedido + " ";
        var tipo2 = producto.CAMPOCOM6;
        var tipo = "";
        if (tipo2 is null || tipo2.Contains("System.Linq"))
            tipo = "";
        else
            tipo = tipo2;
        FileStream fs = new("wwwroot\\logo_aerre.jpg", FileMode.Open);

        graphics.DrawString(" \r\n" +
                            "\r\n" +
                            "\r\n" +
                            $"    Año:{DateTime.Now.Year}  N°:{ordenFabricacion.PEDIDO} \r\n" +
                            $"    TAG:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().LOTE.Trim()}\r\n" +
                            "    Tipo:\r\n" +
                            $"    Codigo:{ordenFabricacion.CG_PROD.Trim()}\r\n" +
                            $"    Medida:{producto.CAMPOCOM2.Trim()}  {producto.CAMPOCOM3.Trim()}\r\n" +
                            $"    Clase:{producto.CAMPOCOM5.Trim()}\r\n" +
                            $"    Temp:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM6.Trim()}\r\n" +
                            $"    Presion SET:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM1.Trim()}\r\n" +
                            $"    P. Aj Banco:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM4.Trim()}\r\n" +
                            $"    Ctra.P:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM5.Trim()}\r\n" +
                            $"    Fluido:{PedCliList.Where(t => t.PEDIDO == ordenFabricacion.PEDIDO).OrderByDescending(t => t.PEDIDO).FirstOrDefault().CAMPOCOM3.Trim()}\r\n" +
                            "    Cuerpo:\r\n" +
                            "    Tobera:\r\n" +
                            "    Resorte:\r\n" +
                            "    T.OPDS N°:8/11\r\n" +
                            "    M.OPDS N°:47642\r\n" +
                            "          Arbros S.A.\r\n" +
                            "       www.aerre.com.ar\r\n" +
                            "     Industria  Argentina\r\n" +
                            "                               ", font, PdfBrushes.Black, new PointF(0, 0));


        var xx = new MemoryStream();
        document1.Save(xx);
        document1.Close(true);
        await js.SaveAs("ETOF" + ordenFabricacion.CG_PROD.Trim() + ".pdf", xx.ToArray());
        return espaciosPedido;
    }


    public async Task EtiquetaRecepcion(Pedidos pedidos)
    {
        PdfDocument document1 = new();
        document1.PageSettings.Margins.All = 0;
        document1.PageSettings.Size = new SizeF(227, 70); //110

        //document1.PageSettings.Margins.Left = -2;
        //document1.PageSettings.Margins.Right = -15;
        //document1.PageSettings.Margins.Top = 10;
        //document1.PageSettings.Margins.Bottom = -10;
        //document1.PageSettings.Margins.All = margin;
        var pdfGrid1 = new PdfGrid();
        var page = document1.Pages.Add();
        var graphics = page.Graphics;
        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
        PdfLightTable pdfTable = new();
        //page.Graphics.RotateTransform(-90);


        graphics.DrawString(
            $"{pedidos.CG_ART.Trim()}                  OC {pedidos.OCOMPRA}\r\n{pedidos.DES_ART.Trim()}\r\n" +
            $"Despacho {pedidos.DESPACHO} Lote {pedidos.LOTE} VALE {pedidos.VALE}\n" +
            $"{pedidos.Proveedor?.DESCRIPCION.Trim()}", font, PdfBrushes.Black, new PointF(10, 10));

        var qrBarcode = new PdfQRBarcode();
        // Sets the Input mode to Binary mode
        qrBarcode.InputMode = InputMode.BinaryMode;
        // Automatically select the Version
        qrBarcode.Version = QRCodeVersion.Auto;
        // Set the Error correction level to high
        qrBarcode.ErrorCorrectionLevel = PdfErrorCorrectionLevel.High;
        // Set dimension for each block
        qrBarcode.Size = new SizeF(60, 80); //110
        qrBarcode.XDimension = 2;
        var baseUrl = Http.BaseAddress;
        //qrBarcode.Text = $"{baseUrl}inventario/movimiento-entre-depositos/{pedidos.CG_ART.Trim()}/{pedidos.DESPACHO.Trim()}";            
        qrBarcode.Text =
            $"{baseUrl}inventario/movimiento-entre-depositos?cg_mat={pedidos.CG_ART.Trim()}&despacho={pedidos.DESPACHO.Trim()}";
        // Draw the QR barcode
        qrBarcode.Draw(page, new PointF(165, 10));

        //document1.PageSettings.Margins.Left = margin;
        //document1.PageSettings.Margins.Right = margin;
        //document1.PageSettings.Margins.Top = margin;
        //document1.PageSettings.Margins.Bottom = margin;
        MemoryStream xx = new();
        document1.Save(xx);
        document1.Close(true);
        await js.SaveAs("ETOC" + pedidos.CG_ART.Trim() + ".pdf", xx.ToArray());
    }

    public async Task EtiquetaMovimiento(Pedidos pedidos)
    {
        PdfDocument document1 = new();
        document1.PageSettings.Margins.All = 0;
        document1.PageSettings.Size = new SizeF(227, 70); //110

        //document1.PageSettings.Margins.Left = -2;
        //document1.PageSettings.Margins.Right = -15;
        //document1.PageSettings.Margins.Top = 10;
        //document1.PageSettings.Margins.Bottom = -10;
        //document1.PageSettings.Margins.All = margin;
        var pdfGrid1 = new PdfGrid();
        var page = document1.Pages.Add();
        var graphics = page.Graphics;
        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
        PdfLightTable pdfTable = new();
        //page.Graphics.RotateTransform(-90);
        var depositos = await Http.GetFromJsonAsync<List<Deposito>>("api/Deposito");
        var deposito = depositos.Find(d => d.CG_DEP == pedidos.CG_DEP).DES_DEP.Trim();

        graphics.DrawString($"Vale:{pedidos.VALE}                  Fecha:{pedidos.FE_MOV:dd/MM/yyyy}\r\n" +
                            $"{pedidos.CG_ART.Trim()} Cant:{pedidos.STOCK} {pedidos.UNID} \r\n " +
                            $"Despacho {pedidos.DESPACHO} Lote {pedidos.LOTE}\r\n" +
                            $"{deposito}", font, PdfBrushes.Black, new PointF(30, 10));

        //document1.PageSettings.Margins.Left = margin;
        //document1.PageSettings.Margins.Right = margin;
        //document1.PageSettings.Margins.Top = margin;
        //document1.PageSettings.Margins.Bottom = margin;
        MemoryStream xx = new();
        document1.Save(xx);
        document1.Close(true);
        await js.SaveAs("MOV" + pedidos.VALE + ".pdf", xx.ToArray());
    }
}