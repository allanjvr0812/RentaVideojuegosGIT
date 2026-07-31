using DataModels;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace RentaVideojuegos.Pages
{
    public partial class FormularioVideojuego : System.Web.UI.Page
    {
        /*
            Esta página se usa para dos procesos:
            crear un videojuego nuevo o editar uno existente.
            Si viene un id por la URL, se trabaja en modo edición.
            Si no viene id, se trabaja en modo creación.
        */

        protected void Page_Load(object sender, EventArgs e)
        {
            // Se valida que el usuario haya iniciado sesión.
            if (Session["IdJugador"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx", true);
                return;
            }

            bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);

            // Solo el administrador puede crear, modificar o inactivar videojuegos.
            if (!esAdministrador)
            {
                Response.Redirect("~/Pages/MisAlquileres.aspx", true);
                return;
            }

            if (!IsPostBack)
            {
                CargarSucursales();

                int idVideojuego;

                /*
                    Si la URL trae un id, se carga el videojuego para editar.
                    Ejemplo: FormularioVideojuego.aspx?id=3
                */
                if (int.TryParse(Request.QueryString["id"], out idVideojuego))
                {
                    ViewState["ModoEdicion"] = true;
                    ViewState["IdVideojuego"] = idVideojuego;
                    lblTituloPagina.Text = "Editar videojuego";
                    btnInactivar.Visible = true;
                    CargarVideojuego(idVideojuego);
                }
                else
                {
                    ViewState["ModoEdicion"] = false;
                    lblTituloPagina.Text = "Crear videojuego";
                    btnInactivar.Visible = false;
                }
            }
        }

        private void CargarSucursales()
        {
            /*
                Carga las sucursales activas para asociar el videojuego a una sucursal.
                Al editar, este campo luego se bloquea porque no debe cambiarse.
            */

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    ddlSucursal.DataSource = db.SpObtenerSucursalesActivas().ToList();
                    ddlSucursal.DataTextField = "Nombre";
                    ddlSucursal.DataValueField = "IdSucursal";
                    ddlSucursal.DataBind();
                }

                ddlSucursal.Items.Insert(0, new ListItem("Seleccione una sucursal", ""));
            }
            catch (Exception exc)
            {
                MostrarMensaje("Error al cargar sucursales: " + exc.Message, true);
            }
        }

        private void CargarVideojuego(int idVideojuego)
        {
            /*
                Busca el videojuego por id y carga sus datos en el formulario.
                También revisa si está inactivo o si tiene alquileres activos,
                porque en esos casos no se debe permitir modificarlo.
            */

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    var videojuego = db.SpObtenerVideojuegoPorId(idVideojuego).FirstOrDefault();

                    if (videojuego == null)
                    {
                        Response.Redirect("~/Pages/GestionarVideojuegos.aspx", true);
                        return;
                    }

                    ddlSucursal.SelectedValue = videojuego.IdSucursal.ToString();
                    ddlSucursal.Enabled = false;

                    txtTitulo.Text = videojuego.Titulo;
                    txtDescripcion.Text = videojuego.Descripcion;
                    txtCategoria.Text = videojuego.IdCategoria;
                    txtFechaLanzamiento.Text = videojuego.FechaLanzamiento.ToString("yyyy-MM-dd");
                    txtDesarrolladora.Text = videojuego.Desarrolladora;
                    txtDistribuidora.Text = videojuego.Distribuidora;
                    txtImagen.Text = videojuego.Imagen;
                    txtTrailer.Text = videojuego.Trailer;

                    // Si el videojuego está inactivo, no se permite editarlo.
                    if (videojuego.Estado == 'I')
                    {
                        btnGuardar.Enabled = false;
                        btnInactivar.Visible = false;
                        MostrarMensaje("Este videojuego está inactivo.", true);
                        return;
                    }

                    /*
                        Se revisa si el videojuego tiene alquileres activos.
                        Si tiene alquileres activos, no se puede modificar ni inactivar.
                    */
                    var conteo = db.SpContarAlquileresActivosPorVideojuego(idVideojuego).FirstOrDefault();
                    int cantidadActivos = conteo != null && conteo.Cantidad.HasValue ? conteo.Cantidad.Value : 0;

                    if (cantidadActivos > 0)
                    {
                        btnGuardar.Enabled = false;
                        btnInactivar.Enabled = false;
                        MostrarMensaje("Este videojuego tiene alquileres activos. No se puede modificar ni inactivar.", true);
                    }
                }
            }
            catch (Exception exc)
            {
                MostrarMensaje("Error al cargar videojuego: " + exc.Message, true);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            /*
                Valida los campos del formulario.
                Luego decide si debe crear un videojuego nuevo o modificar uno existente.
            */

            if (!Page.IsValid)
                return;

            string titulo = txtTitulo.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();
            string categoria = txtCategoria.Text.Trim();
            string desarrolladora = txtDesarrolladora.Text.Trim();
            string distribuidora = txtDistribuidora.Text.Trim();
            string imagen = txtImagen.Text.Trim();
            string trailer = txtTrailer.Text.Trim();

            DateTime fechaLanzamiento;

            if (!DateTime.TryParseExact(txtFechaLanzamiento.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaLanzamiento))
            {
                MostrarMensaje("La fecha de lanzamiento no tiene un formato válido.", true);
                return;
            }

            if (descripcion.Length > 500)
            {
                MostrarMensaje("La descripción no puede superar los 500 caracteres.", true);
                return;
            }

            try
            {
                bool modoEdicion = Convert.ToBoolean(ViewState["ModoEdicion"]);

                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    if (modoEdicion)
                    {
                        /*
                            En modo edición se usa el id guardado en ViewState
                            y se llama al procedimiento que modifica el videojuego.
                        */
                        int idVideojuego = Convert.ToInt32(ViewState["IdVideojuego"]);

                        db.SpModificarVideojuego(
                            idVideojuego,
                            titulo,
                            descripcion,
                            categoria,
                            fechaLanzamiento,
                            desarrolladora,
                            distribuidora,
                            imagen,
                            trailer
                        ).FirstOrDefault();
                    }
                    else
                    {
                        /*
                            En modo creación se toma la sucursal seleccionada
                            y se llama al procedimiento que crea el videojuego.
                        */
                        int idSucursal = Convert.ToInt32(ddlSucursal.SelectedValue);

                        db.SpCrearVideojuego(
                            idSucursal,
                            titulo,
                            descripcion,
                            categoria,
                            fechaLanzamiento,
                            desarrolladora,
                            distribuidora,
                            imagen,
                            trailer
                        ).FirstOrDefault();
                    }
                }

                Response.Redirect("~/Pages/GestionarVideojuegos.aspx", true);
            }
            catch (Exception exc)
            {
                MostrarMensaje("No se pudo guardar el videojuego: " + exc.Message, true);
            }
        }

        protected void btnInactivar_Click(object sender, EventArgs e)
        {
            /*
                Este botón cambia el estado del videojuego a inactivo.
                No elimina físicamente el registro, solo lo inactiva.
            */

            if (ViewState["IdVideojuego"] == null)
            {
                Response.Redirect("~/Pages/GestionarVideojuegos.aspx", true);
                return;
            }

            int idVideojuego = Convert.ToInt32(ViewState["IdVideojuego"]);

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    db.SpInactivarVideojuego(idVideojuego).FirstOrDefault();
                }

                Response.Redirect("~/Pages/GestionarVideojuegos.aspx", true);
            }
            catch (Exception exc)
            {
                MostrarMensaje("No se pudo inactivar el videojuego: " + exc.Message, true);
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            // Regresa a la lista de videojuegos sin guardar cambios.
            Response.Redirect("~/Pages/GestionarVideojuegos.aspx", true);
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            // Muestra mensajes usando los estilos definidos para error o confirmación.
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
            lblMensaje.CssClass = esError ? "mensaje-error" : "mensaje-correcto";
        }
    }
}