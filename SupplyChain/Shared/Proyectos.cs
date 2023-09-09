using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupplyChain.Shared;

[Table("Proyectos")]
public class ProyectosGBPI : EntityBase<int>
{
    [Key] [Column("TaskId")] public new int Id { get; set; } = 0;

    public string TaskName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Duration { get; set; }
    public int Progress { get; set; }
    public string Predecessor { get; set; }
    public string Notes { get; set; }
    public int? ParentId { get; set; }
}