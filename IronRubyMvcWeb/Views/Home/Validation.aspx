<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Mvc.Html"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Validation</title>
</head>
<body>
    <div>
        <% Html.BeginForm("validate", "home"); %>
            <p>Username: <%= Html.TextBox("username", ViewData["username"]) %></p>
            <p>Password: <%= Html.Password("password", ViewData["password"]) %></p>            
            <p><button type="submit" >validate</button></p>        
        <% Html.EndForm(); %>
    </div>
</body>
</html>
