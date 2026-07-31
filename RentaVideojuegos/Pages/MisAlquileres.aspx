<%@ Page Title="Mis alquileres" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MisAlquileres.aspx.cs" Inherits="RentaVideojuegos.Pages.MisAlquileres" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Mis alquileres</h1>

    <p>
        <asp:Button ID="btnCrear" runat="server"
            Text="Crear alquiler"
            CssClass="btn btn-success"
            OnClick="btnCrear_Click" />
    </p>

    <asp:GridView ID="gvAlquileres" runat="server"
        AutoGenerateColumns="False"
        CssClass="table table-dark table-striped tabla-datos"
        EmptyDataText="No tiene alquileres registrados.">

        <Columns>
            <asp:BoundField DataField="IdAlquiler" HeaderText="# Alquiler" />
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