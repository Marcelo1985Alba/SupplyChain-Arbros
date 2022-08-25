window.descargarPresupuestDataSheet = function (presupuesto) {
    var link = document.createElement('a');
    link.href = 'api/AdministracionArchivos/PresupuestoDataSheetPdf/' + this.encodeURIComponent(presupuesto);
    link.setAttribute('target', '_blank');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}