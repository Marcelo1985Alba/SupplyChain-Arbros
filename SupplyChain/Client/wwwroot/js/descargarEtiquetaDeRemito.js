window.descargarEtiquetaDeRemito = function (remito) {
    var link = document.createElement('a');
    link.href = 'api/ReportRDLC/GetReportEtiquetaDeRemito?remito=' + this.encodeURIComponent(remito);
    link.setAttribute('target', '_blank');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}