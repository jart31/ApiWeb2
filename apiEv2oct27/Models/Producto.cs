using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace apiEv2oct27.Models;

public partial class Producto
{
   
    public int IdProducto { get; set; }

    public string DescProducto { get; set; } = null!;

    public int Precio { get; set; }

    [JsonIgnore]
    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
