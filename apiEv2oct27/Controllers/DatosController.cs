using apiEv2oct27.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiEv2oct27.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatosController : ControllerBase
    {
        public readonly ApiContext dbcontext;

        public DatosController(ApiContext _dcontext) { 
            dbcontext = _dcontext;
        }

        [HttpGet]
        [Route("ListadoVentas")]
        public IActionResult ListadoVentas()
        {
            try
            {
                var ventas = dbcontext.Venta.ToList();
                List<object> listaResultados = new List<object>();

                foreach (var venta in ventas)
                {
                    var usuario = dbcontext.Usuarios.FirstOrDefault(u => u.NomUsuario == venta.NomUsuario);
                    var producto = dbcontext.Productos.FirstOrDefault(p => p.IdProducto == venta.IdProducto);

                    if (usuario == null || producto == null) continue;

                    var estadoTexto = venta.Estado == 0 ? "anulada" : "realizada";

                    listaResultados.Add(new
                    {
                        NombreUsuario = usuario.NomUsuario,
                        Producto = producto.DescProducto,
                        Precio = producto.Precio,
                        Cantidad = venta.Cantidad,
                        Total = venta.Total,
                        Estado = estadoTexto
                    });
                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = listaResultados });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ERROR", respuesta = ex.Message });
                
            }
        }


        /*
 ------------------------------- DATOS DE PRUEBA para agregar venta ---------------------------------------
 {
  "idVenta": 0,
  "idProducto": 1,
  "nomUsuario": "juan",
  "cantidad": 1,
  "total": 1000,
  "fechaVenta": "2023-10-25T04:37:34.168Z",
  "estado": 1,
  "idProductoNavigation": {
    "idProducto": 1,
    "descProducto": "pelota",
    "precio": 1000,
    "venta": []
  },
  "nomUsuarioNavigation": {
    "nomUsuario": "juan",
    "password": "123",
    "estado": 1,
    "venta": []
  }
}

 
 */

        [HttpPost]
        [Route("GuardarVenta")]

        public IActionResult GuardarVenta([FromBody] Ventum venta)
        {
            try
            {
                if (venta == null)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "La venta proporcionada es nula." });
                }
                if (string.IsNullOrEmpty(venta.NomUsuario))
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El nombre de usuario es requerido." });
                }
                if (venta.IdProducto <= 0)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El ID del producto es inválido." });
                }
                if (venta.Cantidad <= 0)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "La cantidad es inválida." });
                }
                if (venta.Total <= 0)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El total es inválido." });
                }
                if (venta.FechaVenta == default(DateTime))
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "La fecha de venta es requerida." });
                }
                if (venta.Estado < 0)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El estado es inválido." });
                }
                
                    var usuarioExistente = dbcontext.Usuarios.FirstOrDefault(u => u.NomUsuario == venta.NomUsuario);
                    if (usuarioExistente == null)
                    {
                        return BadRequest(new { mensaje = "ERROR", respuesta = "El usuario ya está utilizado o no existe." });
                    }
                    else if (usuarioExistente.Estado == 0) 
                    {
                        return BadRequest(new { mensaje = "ERROR", respuesta = "Usuario inhabilitado, imposible crear venta." });
                    }

                    var producto = dbcontext.Productos.FirstOrDefault(p => p.IdProducto == venta.IdProducto);
                    if (producto == null)
                    {
                        return BadRequest(new { mensaje = "ERROR", respuesta = "El producto no existe." });
                    }

                    // Calcular el total
                    venta.Total = producto.Precio * venta.Cantidad;

                    Ventum insertar = new Ventum();
                    insertar.IdVenta = venta.IdVenta;
                    insertar.IdProducto = venta.IdProducto;
                    insertar.NomUsuario = venta.NomUsuario;
                    insertar.Cantidad = venta.Cantidad;
                    insertar.Total = venta.Total;
                    insertar.FechaVenta = venta.FechaVenta;
                    insertar.Estado = venta.Estado;

                    dbcontext.Add(insertar);
                    dbcontext.SaveChanges();



                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "correcto" });
                
                
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }

        [HttpPost]
        [Route("GuardarProducto")]
        public IActionResult GuardarProducto([FromBody] Producto producto)
        {
            try
            {
                
                if (producto == null)
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "ERROR", respuesta = "El producto enviado es nulo." });

                if (string.IsNullOrEmpty(producto.DescProducto))
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "ERROR", respuesta = "La descripción del producto no puede estar vacía." });

                if (producto.Precio <= 0) 
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "ERROR", respuesta = "El precio del producto es inválido." });

                Producto inserto = new Producto
                {
                    
                    DescProducto = producto.DescProducto,
                    Precio = producto.Precio
                };

                dbcontext.Add(inserto);
                dbcontext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "correcto" });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = innerMessage });
            }
        }
//------------------ AGREGAR USUARIO
        [HttpPost]
        [Route("AgregarUsuario")]
        public IActionResult AgregarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                if (usuario == null)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El usuario proporcionado es nulo." });
                }

                
                if (string.IsNullOrWhiteSpace(usuario.NomUsuario) || string.IsNullOrWhiteSpace(usuario.Password))
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El nombre de usuario y la contraseña no pueden estar vacíos." });
                }

                
                var usuarioExistente = dbcontext.Usuarios.FirstOrDefault(u => u.NomUsuario == usuario.NomUsuario);
                if (usuarioExistente != null)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El usuario ya está utilizado." });
                }

                Usuario usuario1 = new Usuario();
                usuario1.NomUsuario = usuario.NomUsuario;
                usuario1.Password = usuario.Password;
                usuario1.Estado = usuario.Estado;
                dbcontext.Usuarios.Add(usuario);
                dbcontext.SaveChanges();

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "Usuario creado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }
//------------------ EDITAR ESTADO DE USUARIO
        [HttpPut]
        [Route("EditarEstadoUsuario")]
        public IActionResult EditarEstadoUsuario([FromBody] EditarEstadoUsuarioRequest request)
        {
            try
            {
                // Validación básica de la petición
                if (request == null)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "La petición es nula." });
                }

                if (string.IsNullOrEmpty(request.NomUsuario))
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El nombre de usuario es requerido." });
                }

                if (request.Estado != 0 && request.Estado != 1)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El estado proporcionado es inválido." });
                }

                // Buscar el usuario por su nombre
                var usuarioExistente = dbcontext.Usuarios.FirstOrDefault(u => u.NomUsuario == request.NomUsuario);

                // Validar si el usuario existe
                if (usuarioExistente == null)
                {
                    return NotFound(new { mensaje = "ERROR", respuesta = "Usuario no encontrado." });
                }

                // Actualizar el estado del usuario y guardar cambios
                usuarioExistente.Estado = request.Estado;
                dbcontext.Update(usuarioExistente);
                dbcontext.SaveChanges();

                return Ok(new { mensaje = "OK", respuesta = "Estado del usuario actualizado exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }

        public class EditarEstadoUsuarioRequest
        {
            public string NomUsuario { get; set; }
            public int Estado { get; set; }
        }
//------------------ OBTENER USUARIOS POR ESTADO
        [HttpGet]
        [Route("ObtenerUsuariosYEstado")]
        public IActionResult ObtenerUsuariosYEstado()
        {
            try
            {
                
                var usuarios = dbcontext.Usuarios.Select(u => new UsuarioEstadoResponse
                {
                    NomUsuario = u.NomUsuario,
                    Estado = u.Estado == 1 ? "habilitado" : "inhabilitado"
                }).ToList();

                return Ok(new { mensaje = "OK", respuesta = usuarios });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }

        public class UsuarioEstadoResponse
        {
            public string NomUsuario { get; set; }
            public string Estado { get; set; }
        }
//------------------ OBTENER PRODUCTOS 
        [HttpGet]
        [Route("ObtenerProductos")]
        public IActionResult ObtenerProductos(int? id = null)
        {
            try
            {
                List<Producto> productos;

                if (id.HasValue)
                {
                    
                    var producto = dbcontext.Productos.FirstOrDefault(p => p.IdProducto == id.Value);

                    if (producto == null)
                    {
                        return NotFound(new { mensaje = "ERROR", respuesta = $"No se encontró producto con ID {id.Value}" });
                    }

                    productos = new List<Producto> { producto };
                }
                else
                {
                    
                    productos = dbcontext.Productos.ToList();
                }

                return Ok(new { mensaje = "OK", respuesta = productos });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }
//------------------ OBTENER VENTAS POR ESTADO
        [HttpGet]
        [Route("BuscarVentasPorEstado")]
        public IActionResult BuscarVentasPorEstado(int? estado)
        {
            try
            {
                if (!estado.HasValue)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El estado es requerido." });
                }

                var ventasPorEstado = dbcontext.Venta.Where(v => v.Estado == estado).ToList();
                

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = ventasPorEstado });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }
//------------------ BUSCAR VENTAS POR USUARIO 
        [HttpGet]
        [Route("BuscarVentasPorUsuario")]
        public IActionResult BuscarVentasPorUsuario(string nomUsuario)
        {
            try
            {
                if (string.IsNullOrEmpty(nomUsuario))
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El nombre de usuario es requerido." });
                }

                var ventasPorUsuario = dbcontext.Venta.Where(v => v.NomUsuario == nomUsuario).ToList();
                

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = ventasPorUsuario });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }
        //------------------ EDITAR ESTADO DE VENTA
        [HttpPut]
        [Route("EditarEstadoVenta")]
        public IActionResult EditarEstadoVenta(int idVenta, int estado)
        {
            try
            {
                
                var venta = dbcontext.Venta.FirstOrDefault(v => v.IdVenta == idVenta);
                if (venta == null)
                {
                    return NotFound(new { mensaje = "ERROR", respuesta = "Venta no encontrada." });
                }

                
                if (estado != 0 && estado != 1)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "Estado inválido. Solo se aceptan valores 0 o 1." });
                }

                
                venta.Estado = estado;
                dbcontext.Update(venta);
                dbcontext.SaveChanges();

                return Ok(new { mensaje = "OK", respuesta = "Estado de la venta actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }

        //------------------ EDITAR USUARIO
        [HttpPut]
        [Route("EditarUsuario")]
        public IActionResult EditarUsuario(string nomUsuario, string password, int estado)
        {
            // Validaciones iniciales
            if (string.IsNullOrWhiteSpace(nomUsuario))
            {
                return BadRequest(new { mensaje = "ERROR", respuesta = "El nombre del usuario no puede estar vacío." });
            }

            try
            {
                var usuarioExistente = dbcontext.Usuarios.FirstOrDefault(u => u.NomUsuario == nomUsuario);

                if (usuarioExistente == null)
                {
                    return NotFound(new { mensaje = "ERROR", respuesta = "No se encontró el usuario con el nombre especificado." });
                }

                usuarioExistente.Estado = estado;
                usuarioExistente.Password = password;

                dbcontext.Entry(usuarioExistente).State = EntityState.Modified;
                dbcontext.SaveChanges();

                return Ok(new { mensaje = "OK", respuesta = "Usuario actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }


        //---------------------- ELIMINAR USUARIO
        [HttpDelete]
        [Route("EliminarUsuario/{nomUsuario}")]
        public IActionResult EliminarUsuario(string nomUsuario)
        {
            try
            {
                
                if (string.IsNullOrEmpty(nomUsuario))
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El nombre de usuario proporcionado es inválido o está vacío." });
                }

            
                var usuarioExistente = dbcontext.Usuarios.FirstOrDefault(u => u.NomUsuario == nomUsuario);
                if (usuarioExistente == null)
                {
                    return NotFound(new { mensaje = "ERROR", respuesta = "Usuario no encontrado." });
                }

                dbcontext.Usuarios.Remove(usuarioExistente);
                dbcontext.SaveChanges();

                return Ok(new { mensaje = "OK", respuesta = "Usuario eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }
        //------------------ EDITAR PRODUCTO
        [HttpPut]
        [Route("EditarProducto")]
        public IActionResult EditarProducto(int id, string nombre, int precio)
        {
            
            if (id <= 0)
            {
                return BadRequest(new { mensaje = "ERROR", respuesta = "El ID proporcionado no es válido." });
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                return BadRequest(new { mensaje = "ERROR", respuesta = "El nombre del producto no puede estar vacío." });
            }

            if (precio <= 0) 
            {
                return BadRequest(new { mensaje = "ERROR", respuesta = "El precio proporcionado no es válido." });
            }

            try
            {
                Producto? prdt = dbcontext.Productos.FirstOrDefault(a => a.IdProducto == id);

                if (prdt == null)
                {
                    return NotFound(new { mensaje = "ERROR", respuesta = "No se encontró el producto con el ID especificado." });
                }

                prdt.DescProducto = nombre;
                prdt.Precio = precio;

                dbcontext.Entry(prdt).State = EntityState.Modified;
                dbcontext.SaveChanges();

                return Ok(new { mensaje = "OK", respuesta = "Producto editado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }


        //------------------- ELIMINAR PRODUCTO
        [HttpDelete]
        [Route("EliminarProducto/{id}")]
        public IActionResult EliminarProducto(int id)
        {
            try
            {
                
                if (id <= 0)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "ID de producto inválido." });
                }

                
                var productoExistente = dbcontext.Productos.FirstOrDefault(p => p.IdProducto == id);
                if (productoExistente == null)
                {
                    return NotFound(new { mensaje = "ERROR", respuesta = "Producto no encontrado." });
                }

                dbcontext.Productos.Remove(productoExistente);
                dbcontext.SaveChanges();

                return Ok(new { mensaje = "OK", respuesta = "Producto eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }

    }
}

