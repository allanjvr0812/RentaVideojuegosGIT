<%@ Page Title="Detalle de alquiler" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DetalleAlquiler.aspx.cs" Inherits="RentaVideojuegos.Pages.DetalleAlquiler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Detalle de alquiler</h1>

    <table class="tabla-detalle">
        <tr>
            <td># Alquiler:</td>
            <td><asp:Label ID="lblIdAlquiler" runat="server" /></td>
        </tr>

        <tr>
            <td>Sucursal:</td>
            <td><asp:Label ID="lblSucursal" runat="server" /></td>
        </tr>

        <tr>
            <td>Videojuego:</td>
            <td><asp:Label ID="lblVideojuego" runat="server" /></td>
        </tr>

        <tr>
            <td>Jugador:</td>
            <td><asp:Label ID="lblJugador" runat="server" /></td>
        </tr>

        <tr>
            <td>Fecha inicio:</td>
            <td><asp:Label ID="lblFechaInicio" runat="server" /></td>
        </tr>

        <tr>
            <td>Fecha devolución:</td>
            <td><asp:Label ID="lblFechaDevolucion" runat="server" /></td>
        </tr>

        <tr>
            <td>Días de alquiler:</td>
            <td><asp:Label ID="lblDias" runat="server" /></td>
        </tr>

        <tr>
            <td>Costo por día:</td>
            <td><asp:Label ID="lblCostoPorDia" runat="server" /></td>
        </tr>

        <tr>
            <td>Costo total:</td>
            <td><asp:Label ID="lblCostoTotal" runat="server" /></td>
        </tr>

        <tr>
            <td>Estado:</td>
            <td><asp:Label ID="lblEstado" runat="server" /></td>
        </tr>
    </table>

    <h3>Bitácora</h3>

    <asp:GridView ID="grdBitacora" runat="server"
        AutoGenerateColumns="false"
        CssClass="table table-dark table-striped tabla-datos"
        EmptyDataText="Este alquiler no tiene movimientos registrados.">

        <Columns>
            <asp:BoundField DataField="AccionRealizada" HeaderText="Acción" />
            <asp:BoundField DataField="NombreCompleto" HeaderText="Realizada por" />
            <asp:BoundField DataField="FechaDeLaAccion" HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
        </Columns>
    </asp:GridView>

    <p>
        <asp:HyperLink ID="lnkEditar" runat="server"
            Text="Editar alquiler"
            CssClass="btn btn-primary"
            Visible="false" />

        <asp:Button ID="btnCancelar" runat="server"
            Text="Cancelar alquiler"
            CssClass="btn btn-danger"
            Visible="false"
            OnClientClick="return confirm('¿Desea cancelar este alquiler?');"
            OnClick="btnCancelar_Click" />

        <asp:HyperLink ID="lnkRegresar" runat="server"
            Text="Regresar"
            CssClass="btn btn-secondary" />
    </p>

</asp:Content>