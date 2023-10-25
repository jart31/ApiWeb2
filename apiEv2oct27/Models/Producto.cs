using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace apiEv2oct27.Models;

public partial class Producto
{
    
    public int IdProducto { get; set; }

    public string DescProducto { get; set; } = null!;

    public int Precio { get; set; }

    public virtual ICollection<Ventum> Venta { get; set; } = new List<Ventum>();
}
