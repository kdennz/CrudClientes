using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrudClientes;

// Esta clase representa la tabla "Clientes" de SQL Server
[Table("Clientes")]
public class Cliente
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Correo { get; set; } = string.Empty;
}
