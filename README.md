# Carrito de Compras

## Introducción

Este es un proyecto académico, realizado en Visual Studio 2019 y utilizando `ASP.NET MVC Core versión 3.1`.
El sistema permite la administración del stock de productos a una PYME que tiene algunas sucursales de venta de ropa.\
También permite a los clientes realizar compras online.\

---------------------------------------

## Funcionalidades

### Cliente

- Los Clientes pueden auto registrarse.
- Un usuario puede navegar los productos y sus descripciones sin iniciar sesión, es decir, de forma anónima.
- Para agregar productos al carrito debe iniciar sesión primero.
- El cliente puede agregar diferentes productos en el carrito y por cada producto modificar la cantidad que quiere.
- El cliente puede finalizar la compra. Se le solicitará una sucursal para retirar.
- El cliente puede vaciar el carrito.

### Empleado y Administrador

- Los empleados deben ser agregados por otro empleado o administrador.
- Puede listar las compras realizadas en el mes en modo listado, ordenado de forma descendente por valor de compra.
- Puede dar de alta otros empleados.
- Puede crear productos, categorias, Sucursales, agregar productos al stock de cada sucursal.
- Puede habilitar y/o deshabilitar productos.
- Los administradores solo pueden ser agregados o eliminados por otro administrador.

### Producto y Categoría

- No pueden eliminarse del sistema.
- Solo los productos pueden dehabilitarse.

### Sucursal

- Cada sucursal tendrá su propio stock y sus datos de locación y contacto.
- Por el mercado tan volátil las sucursales pueden crearse y eliminarse en todo momento.
  - Para poder eliminar una sucursal, la misma no tiene que tener productos en su stock.

### Carrito

- El carrito se crea automáticamente en estado activo con la creación de un cliente (todo cliente tendrá un carrito en estado activo cuando éste sea creado).
- Solo puede haber un carrito activo por usuario en el sistema.
- Un carrito que no esté activo no puede modificarse en ningún aspecto.
- No se pueden eliminar carritos.
- El carrito se desactiva al momento de concretarse una compra de manera automática y se creará un nuevo carrito activo para el usuario.
- Al vaciar el carrito, se eliminan todos los CarritoItems y datos que no sean necesarios.
- El subtotal del carrito es un dato calculado en el momento.

### Compra

- Al generarse la compra el carrito que tiene asociado pasa a estar en estado *Inactivo*.
- Al finalizar la compra se validará si hay disponibilidad de stock de todos los CarritoItem en la sucursal que seleccionó el cliente.
  - Si hay stock disminuye el mismo en la sucursal seleccionada y se crea la compra.
  - Si no hay stock verificar en otras sucursales si cuentan con el stock de lo que se quiere comprar.
    - Si se encuentran sucursales con stock, se informan dichas sucursales al cliente.
    - Cuando selecciona una de las sucursales sugeridas con stock, finalizar la compra.
- Al finalizar la compra, se le muestra el detalle de la compra al cliente y se le da las gracias. Se le dá el Id de compra y los datos de la Sucursal que eligió.