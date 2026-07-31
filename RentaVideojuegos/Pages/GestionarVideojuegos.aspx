<%@ Page Title="Gestionar videojuegos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="GestionarVideojuegos.aspx.cs" Inherits="RentaVideojuegos.Pages.GestionarVideojuegos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Gestionar videojuegos</h1>

    <asp:Label ID="lblMensaje" runat="server" Visible="false"></asp:Label>

    <p>
        <asp:Button ID="btnCrear" runat="server"
            Text="Crear videojuego"
            CssClass="btn btn-success"
            OnClick="btnCrear_Click" />
    </p>

    <asp:GridView ID="gvVideojuegos" runat="server"
        AutoGenerateColumns="False"
        CssClass="table table-dark table-striped tabla-datos"
        EmptyDataText="No hay videojuegos registrados.">

        <Columns>
            <asp:BoundField DataField="IdVideojuego" HeaderText="ID" />
            <asp:BoundField DataField="NombreSucursal" HeaderText="Sucursal" />
            <asp:BoundField DataField="Titulo" HeaderText="Título" />
            <asp:BoundField DataField="IdCategoria" HeaderText="Categoría" />
            <asp:BoundField DataField="EstadoTexto" HeaderText="Estado" />

            <asp:HyperLinkField
                HeaderText="Editar"
                Text="Editar"
                DataNavigateUrlFields="IdVideojuego"
                DataNavigateUrlFormatString="~/Pages/FormularioVideojuego.aspx?id={0}" />
        </Columns>
    </asp:GridView>

</asp:Content>