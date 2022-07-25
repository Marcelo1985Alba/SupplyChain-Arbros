window.descargarCertificado = function (pedido) {
    var link = document.createElement('a');
    link.href = 'api/AdministracionArchivos/MergePdf/' + this.encodeURIComponent(pedido);
    link.setAttribute('target', '_blank');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}