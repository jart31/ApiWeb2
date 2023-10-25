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
                List<Ventum> lista = new List<Ventum>();
                //lista = dbcontext.Venta.ToList();
              //  lista = dbcontext.Venta
                // .Include(v => v.IdProductoNavigation)
                 //.Include(v => v.NomUsuarioNavigation)
                 //.ToList();
                 lista = dbcontext.Venta.FromSqlRaw("SELECT * FROM VENTA").ToList();



                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = lista });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ERROR", respuesta = ex.Message });
                
            }
        }

        [HttpPost]
        [Route("GuardarVenta")]
        public IActionResult GuardarVenta([FromBody] Ventum venta)
        {
            try
            {// no esta funcionando bien LAS VALIDACIONES REVISAR
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
                
                    // Validar si el usuario existe y obtener su estado
                    var usuarioExistente = dbcontext.Usuarios.FirstOrDefault(u => u.NomUsuario == venta.NomUsuario);
                    if (usuarioExistente == null)
                    {
                        return BadRequest(new { mensaje = "ERROR", respuesta = "El usuario ya está utilizado." });
                    }
                    else if (usuarioExistente.Estado == 0) // Suponiendo que 0 es el estado "inhabilitado"
                    {
                        return BadRequest(new { mensaje = "ERROR", respuesta = "Usuario inhabilitado, imposible crear venta." });
                    }

                    // Obtener el precio del producto
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
                Producto inserto = new Producto();
                inserto.IdProducto = producto.IdProducto;
                inserto.DescProducto = producto.DescProducto;
                inserto.Precio = producto.Precio;
                dbcontext.Add(inserto);
                dbcontext.SaveChanges();
              

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "OK", respuesta = "correcto" });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }
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
                else
                {
                    // Validar si el usuario ya existe en la base de datos
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
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }
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

        [HttpGet]
        [Route("ObtenerUsuariosYEstado")]
        public IActionResult ObtenerUsuariosYEstado()
        {
            try
            {
                // Obtener todos los usuarios
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

        [HttpGet]
        [Route("ObtenerProductos")]
        public IActionResult ObtenerProductos(int? id = null)
        {
            try
            {
                List<Producto> productos;

                if (id.HasValue)
                {
                    // Buscar producto por ID específico
                    var producto = dbcontext.Productos.FirstOrDefault(p => p.IdProducto == id.Value);

                    if (producto == null)
                    {
                        return NotFound(new { mensaje = "ERROR", respuesta = $"No se encontró producto con ID {id.Value}" });
                    }

                    productos = new List<Producto> { producto };
                }
                else
                {
                    // Listar todos los productos
                    productos = dbcontext.Productos.ToList();
                }

                return Ok(new { mensaje = "OK", respuesta = productos });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }

        // no esta funcionando bien REVISAR
        [HttpGet]
        [Route("ListarVentas")]
        public IActionResult ListarVentas(int? estado = null, string nomUsuario = null)
        {
            try
            {
                // Filtrado inicial: obtener todas las ventas
                var ventasQuery = dbcontext.Venta.AsQueryable();

                // Filtrar por estado si se proporciona
                if (estado.HasValue)
                {
                    ventasQuery = ventasQuery.Where(v => v.Estado == estado.Value);
                }

                // Filtrar por nombre de usuario si se proporciona
                if (!string.IsNullOrWhiteSpace(nomUsuario))
                {
                    ventasQuery = ventasQuery.Where(v => v.NomUsuario == nomUsuario);
                }

                // Proyección: seleccionar solo los campos deseados
                var ventasListadas = ventasQuery.Select(v => new
                {
                    NombreUsuario = v.NomUsuario,
                    Producto = v.IdProductoNavigation.DescProducto, // Suponiendo que esta es la descripción del producto
                    Precio = v.IdProductoNavigation.Precio,
                    Cantidad = v.Cantidad,
                    Total = v.Total,
                    Estado = v.Estado == 0 ? "anulada" : "realizada"
                }).ToList();

                return Ok(new { mensaje = "OK", respuesta = ventasListadas });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }
        [HttpPut]
        [Route("EditarEstadoVenta")]
        public IActionResult EditarEstadoVenta(int idVenta, int estado)
        {
            try
            {
                // Buscar la venta en la base de datos por el ID proporcionado
                var venta = dbcontext.Venta.FirstOrDefault(v => v.IdVenta == idVenta);
                if (venta == null)
                {
                    return NotFound(new { mensaje = "ERROR", respuesta = "Venta no encontrada." });
                }

                // Verificar que el estado proporcionado sea válido
                if (estado != 0 && estado != 1)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "Estado inválido. Solo se aceptan valores 0 o 1." });
                }

                // Cambiar el estado de la venta
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

        // no esta funcionando bien REVISAR
        [HttpPut]
        [Route("EditarUsuario")]
        public IActionResult EditarUsuario([FromBody] Usuario usuarioEditado)
        {
            try
            {
                if (usuarioEditado == null || string.IsNullOrEmpty(usuarioEditado.NomUsuario))
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El usuario proporcionado es inválido o está vacío." });
                }

                // Buscar el usuario en la base de datos por el Nombre proporcionado
                var usuarioExistente = dbcontext.Usuarios.FirstOrDefault(u => u.NomUsuario == usuarioEditado.NomUsuario);
                if (usuarioExistente == null)
                {
                    return NotFound(new { mensaje = "ERROR", respuesta = "Usuario no encontrado." });
                }

                // Actualizar los campos del usuario
                usuarioExistente.Estado = usuarioEditado.Estado;

                // Si deseas editar otros campos, simplemente los agregas aquí, por ejemplo:
                // usuarioExistente.Password = usuarioEditado.Password;

                dbcontext.Update(usuarioExistente);
                dbcontext.SaveChanges();

                return Ok(new { mensaje = "OK", respuesta = "Usuario actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }
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

                // Buscar el usuario en la base de datos por el Nombre proporcionado
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
        // no esta funcionando bien REVISAR
        [HttpPut]
        [Route("EditarProducto")]
        public IActionResult EditarProducto([FromBody] Producto productoEditado)
        {
            try
            {
                if (productoEditado == null)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "El producto proporcionado es nulo." });
                }

                // Validar campos requeridos
                if (productoEditado.IdProducto <= 0 || string.IsNullOrEmpty(productoEditado.DescProducto) || productoEditado.Precio <= 0)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "Datos del producto inválidos o incompletos." });
                }

                // Buscar el producto en la base de datos
                var productoExistente = dbcontext.Productos.FirstOrDefault(p => p.IdProducto == productoEditado.IdProducto);
                if (productoExistente == null)
                {
                    return NotFound(new { mensaje = "ERROR", respuesta = "Producto no encontrado." });
                }

                // Actualizar datos
                productoExistente.DescProducto = productoEditado.DescProducto;
                productoExistente.Precio = productoEditado.Precio;

                dbcontext.Update(productoExistente);
                dbcontext.SaveChanges();

                return Ok(new { mensaje = "OK", respuesta = "Producto editado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = "ERROR", respuesta = ex.Message });
            }
        }

        // no esta funcionando bien REVISAR
        [HttpDelete]
        [Route("EliminarProducto/{id}")]
        public IActionResult EliminarProducto(int id)
        {
            try
            {
                // Validar que el ID proporcionado sea válido
                if (id <= 0)
                {
                    return BadRequest(new { mensaje = "ERROR", respuesta = "ID de producto inválido." });
                }

                // Buscar el producto en la base de datos
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

/*
 
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
