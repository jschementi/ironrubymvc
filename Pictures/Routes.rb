$routes.map_route("album", 
  "albums/{id}", 
  {:controller => 'Albums', :action => 'show'})
  
$routes.map_route("persons_pictures", 
  "people/{person_id}/pictures", 
  {:controller => 'Pictures', :action => 'index'})

$routes.map_route("person_picture",
  "people/{person_id}/pictures/{id}",
  {:controller => "Pictures", :action => 'show'})

$routes.map_route("default", 
  "{controller}/{action}/{id}", 
  {:controller => 'Pictures', :action => 'index', :id => ''})