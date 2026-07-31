using DataModels;
using System;
using System.Linq;

namespace RentaVideojuegos.Pages
{
    public partial class DetalleAlquiler : System.Web.UI.Page
    {
        /*
            Esta página muestra la información completa de un alquiler.
            Aquí también se carga la bitácora y se decide si el usuario puede editar
            o cancelar el alquiler, según las fechas y el tipo de usuario.
        */

        protected void Page_Load(object sender, EventArgs e)
        {
            // Ningún usuario puede entrar a esta página si no ha iniciado sesión.
            if (Session["IdJugador"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx", true);
                return;
            }

            // La información solo se carga la primera vez que abre la página.
            // Esto evita que los datos se vuelvan a cargar innecesariamente en cada postback.
            if (!IsPostBack)
            {
                CargarDetalle();
            }
        }

        private void CargarDetalle()
        {
            /*
                Se obtiene el número de alquiler que viene por la URL.
                Ejemplo: DetalleAlquiler.aspx?id=5
            */

            int idAlquiler;

            if (!int.TryParse(Request.QueryString["id"], out idAlquiler))
            {
                RedirigirALista();
                return;
            }

            bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);
            int idJugadorSesion = Convert.ToInt32(Session["IdJugador"]);

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    // Se consulta el detalle del alquiler usando el procedimiento almacenado.
                    var detalle = db.SpObtenerDetalleAlquiler(idAlquiler).FirstOrDefault();

                    // Si no existe el alquiler, se regresa a la lista correspondiente.
                    if (detalle == null)
                    {
                        RedirigirALista();
                        return;
                    }

                    /*
                        Si el usuario no es administrador, solo puede ver sus propios alquileres.
                        Esto evita que un jugador normal consulte alquileres de otras personas.
                    */
                    if (!esAdministrador && detalle.IdJugador != idJugadorSesion)
                    {
                        Response.Redirect("~/Pages/MisAlquileres.aspx", true);
                        return;
                    }

                    // Se muestran los datos principales del alquiler en las etiquetas.
                    MostrarDetalle(detalle, esAdministrador);

                    // Se activan u ocultan los botones según las reglas del proyecto.
                    ConfigurarBotones(detalle, esAdministrador);

                    // Se carga la bitácora del alquiler para ver quién creó, corrigió o canceló.
                    grdBitacora.DataSource = db.SpObtenerBitacoraPorAlquiler(idAlquiler).ToList();
                    grdBitacora.DataBind();
                }
            }
            catch (Exception exc)
            {
                Response.Write("<script>alert('Error al cargar el detalle: " + exc.Message.Replace("'", "") + "');</script>");
            }
        }

        private void MostrarDetalle(RentaVideojuegosDBStoredProcedures.SpObtenerDetalleAlquilerResult detalle, bool esAdministrador)
        {
            /*
                En este método solo se pasan los datos de la base de datos
                a los controles visuales de la página.
            */

            lblIdAlquiler.Text = detalle.IdAlquiler.ToString();
            lblSucursal.Text = detalle.NombreSucursal;
            lblVideojuego.Text = detalle.Titulo;
            lblJugador.Text = detalle.NombreCompleto;
            lblFechaInicio.Text = detalle.FechaInicio.ToString("dd/MM/yyyy");
            lblFechaDevolucion.Text = detalle.FechaDevolucion.ToString("dd/MM/yyyy");
            lblDias.Text = detalle.TotalDiasAlquiler.ToString();
            lblCostoPorDia.Text = detalle.CostoPorDia.ToString("N2");
            lblCostoTotal.Text = detalle.CostoTotal.ToString("N2");
            lblEstado.Text = ObtenerEstadoTexto(detalle.Estado, detalle.FechaInicio, detalle.FechaDevolucion);

            // El botón regresar depende del tipo de usuario que está usando el sistema.
            if (esAdministrador)
                lnkRegresar.NavigateUrl = "~/Pages/GestionarAlquileres.aspx";
            else
                lnkRegresar.NavigateUrl = "~/Pages/MisAlquileres.aspx";
        }

        private void ConfigurarBotones(RentaVideojuegosDBStoredProcedures.SpObtenerDetalleAlquilerResult detalle, bool esAdministrador)
        {
            /*
                Aquí se aplican las reglas del proyecto:
                - El jugador solo puede editar si el alquiler aún no ha iniciado.
                - El administrador puede editar si la fecha de devolución no se ha cumplido.
                - La cancelación solo se permite si el alquiler todavía no inicia.
            */

            DateTime hoy = DateTime.Today;
            bool estaActivo = detalle.Estado == 'A';

            bool puedeEditar;

            if (esAdministrador)
                puedeEditar = estaActivo && detalle.FechaDevolucion.Date > hoy;
            else
                puedeEditar = estaActivo && detalle.FechaInicio.Date > hoy;

            lnkEditar.Visible = puedeEditar;
            lnkEditar.NavigateUrl = "~/Pages/ModificarAlquiler.aspx?id=" + detalle.IdAlquiler;

            btnCancelar.Visible = estaActivo && detalle.FechaInicio.Date > hoy;

            // Se guarda el id en ViewState para usarlo cuando se presione el botón cancelar.
            ViewState["IdAlquiler"] = detalle.IdAlquiler;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            /*
                Este evento se ejecuta cuando el usuario confirma que desea cancelar el alquiler.
                La cancelación real se hace en la base de datos por medio del procedimiento almacenado.
            */

            if (ViewState["IdAlquiler"] == null)
            {
                RedirigirALista();
                return;
            }

            int idAlquiler = Convert.ToInt32(ViewState["IdAlquiler"]);
            int idJugadorSesion = Convert.ToInt32(Session["IdJugador"]);

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    db.SpCancelarAlquiler(idAlquiler, idJugadorSesion).FirstOrDefault();
                }

                // Se vuelve a cargar el detalle para mostrar el alquiler ya cancelado.
                Response.Redirect("~/Pages/DetalleAlquiler.aspx?id=" + idAlquiler, true);
            }
            catch (Exception exc)
            {
                Response.Write("<script>alert('No se pudo cancelar el alquiler: " + exc.Message.Replace("'", "") + "');</script>");
            }
        }

        private string ObtenerEstadoTexto(char estadoBD, DateTime fechaInicio, DateTime fechaDevolucion)
        {
            /*
                La base de datos solo guarda A o I.
                Este método convierte ese valor en un estado más claro para el usuario.
            */

            DateTime hoy = DateTime.Today;

            if (estadoBD == 'I')
                return "Cancelado";

            if (estadoBD == 'A' && fechaDevolucion.Date < hoy)
                return "Finalizado";

            if (estadoBD == 'A' && fechaInicio.Date <= hoy && fechaDevolucion.Date >= hoy)
                return "En proceso";

            if (estadoBD == 'A' && fechaInicio.Date > hoy)
                return "En espera";

            return "Sin estado";
        }

        private void RedirigirALista()
        {
            // Envía al usuario a la pantalla que le corresponde según su rol.
            bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);

            if (esAdministrador)
                Response.Redirect("~/Pages/GestionarAlquileres.aspx", true);
            else
                Response.Redirect("~/Pages/MisAlquileres.aspx", true);
        }
    }
}