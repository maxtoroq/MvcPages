<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Web.Mvc" %>

<script RunAt="server">

   void Application_Start(object sender, EventArgs e) {
      RegisterRoutes(RouteTable.Routes);
   }

   void RegisterRoutes(RouteCollection routes) {
      routes.MapRoute(null, "Test/{action}",
         new { controller = "Test", action = "Index" });
   }
       
</script>
