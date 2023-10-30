using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace apiEv2oct27.Models;

public partial class Usuario
{
    
    public string NomUsuario { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Estado { get; set; }
    [JsonIgnore]
    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
