window.descargarPedido = function (numoci) {
    var link = document.createElement('a');
    link.href = 'api/ReportRDLC/GetReportPedido?numOci=' + this.encodeURIComponent(numoci);
    link.setAttribute('target', '_blank');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}