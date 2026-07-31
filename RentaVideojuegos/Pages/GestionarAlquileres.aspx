<%@ Page Title="Gestionar alquileres" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionarAlquileres.aspx.cs" Inherits="RentaVideojuegos.Pages.GestionarAlquileres" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Gestionar alquileres</h1>

    <asp:Label ID="lblMensajeError" runat="server" CssClass="mensaje-error" Visible="false"></asp:Label>

    <table class="tabla-formulario">
        <tr>
            <td>Jugador:</td>
            <td>
                <asp:DropDownList ID="ddlJugador" runat="server" CssClass="form-control"></asp:DropDownList>
            </td>
        </tr>

        <tr>
            <td>Fecha inicio:</td>
            <td>
                <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Fecha devolución:</td>
            <td>
                <asp:TextBox ID="txtFechaDevolucion" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </td>
        </tr>
    </table>

    <p>
        <asp:Button ID="btnBuscar" runat="server"
            Text="Buscar"
            CssClass="btn btn-primary"
            OnClick="btnBuscar_Click" />

        <asp:Button ID="btnLimpiar" runat="server"
            Text="Limpiar"
            CssClass="btn btn-secondary"
            OnClick="btnLimpiar_Click"
            CausesValidation="false" />

        <asp:Button ID="btnCrear" runat="server"
            Text="Crear alquiler"
            CssClass="btn btn-success"
            OnClick="btnCrear_Click"
            CausesValidation="false" />
    </p>

    <asp:GridView ID="gvAlquileres" runat="server"
        AutoGenerateColumns="False"
        CssClass="table table-dark table-striped tabla-datos"
        EmptyDataText="No hay alquileres para mostrar.">

        <Columns>
            <asp:BoundField DataField="IdAlquiler" HeaderText="# Alquiler" />
            <asp:BoundField DataField="NombreCompleto" HeaderText="Jugador" />
            <asp:BoundField DataField="NombreSucursal" HeaderText="Sucursal" />
            <asp:BoundField DataField="Titulo" HeaderText="Videojuego" />
            <asp:BoundField DataField="FechaInicioTexto" HeaderText="Fecha inicio" />
            <asp:BoundField DataField="FechaDevolucionTexto" HeaderText="Fecha devolución" />
            <asp:BoundField DataField="CostoTotalTexto" HeaderText="Costo total" />
            <asp:BoundField DataField="EstadoTexto" HeaderText="Estado" />

            <asp:HyperLinkField
                HeaderText="Detalle"
                Text="Ver detalle"
                DataNavigateUrlFields="IdAlquiler"
                DataNavigateUrlFormatString="~/Pages/DetalleAlquiler.aspx?id={0}" />
        </Columns>
    </asp:GridView>

</asp:Content>