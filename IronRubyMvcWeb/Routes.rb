#default routes
$routes.ignore_route("{resource}.axd/{*pathInfo}");

$routes.map_route("default", "{controller}/{action}/{id}", {:controller => 'Home', :action => 'index', :id => ''})