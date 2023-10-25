using System;
using System.Collections.Generic;

namespace apiEv2oct27.Models;

public partial class Ventum
{
    public int IdVenta { get; set; }

    public int IdProducto { get; set; }

    public string NomUsuario { get; set; } = null!;

    public int Cantidad { get; set; }

    public int Total { get; set; }

    public DateTime FechaVenta { get; set; }

    public int Estado { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Usuario NomUsuarioNavigation { get; set; } = null!;
}
