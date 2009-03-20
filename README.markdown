IronRubyMvc - an extension to ASP.NET MVC to support IronRuby
=============================================================

Getting started
---------------

1. Install ASP.NET MVC (link?)
2. Create a new ASP.NET MVC project in Visual Studio
3. Add a reference to System.Web.Mvc.IronRuby.dll
3. Open Global.asax.cs and ...

    // add this to your usings
    using System.Web.Mvc;
  
    // make the subclass of "MvcApplication" be "RubyMvcApplication"
    namespace MyIronRubyMvcApp {
        public class MvcApplication : RubyMvcApplication {
      
        }
    }

4. Create Routes.rb and define a default route:

    $routes.ignore_route "{resource}.axd/{*pathInfo}"
    $routes.map_route "default", "{controller}/{action}/{id}", :controller => 'Home', :action => 'index', :id => ''

Controllers
-----------

Ruby Controllers live in the same directory as normal MVC controllers: the "Controllers" directory.

  # Controllers\HomeController.rb
  class HomeController < Controller
    def index
      "Hello, World"
    end
  end

Run the application from Visual Studio, and it will display "Hello, World"

Filters
-------

You can create filters in a number of ways.

  class YourController < Controller

     # before_action, after_action, around_action, before_result, after_result, authorized_action, exception_action all work
     before_action :index do |context|
       # do some filtering stuff here
     end
     
     around_result :index do |context|
       # do some result filtering stuff here
     end
     
     before_action :index, :method_filter
        
     authorized_action :some_other_action do |context|
       # do some authorization checking work here
     end
     
     filter :index, YourFilter
     filter :some_other_action, DifferentYourFilter
     
     # executes for each action
     filter YourControllerFilter 
     
     def index
       # index action
     end
     
     def some_other_action
       # index action
     end
     
     def method_filter
      # do some filter stuff here
     end

  end

You can define the following types of filters:
ActionFilter, ResultFilter, AuthorizationFilter, ExceptionFilter

  class YourFilter < ActionFilter

    def on_action_executing(context)
      # Do some filter work here
    end
    
    def on_action_executed(context)
      # Do some filter work here
    end
  end

Views
-----

Ruby views exist in the Views directory, in a sub-folder with the same name as the Controller the View
is intended to be used in. To use ERb views, the file should have the ".html.erb" extension. Shared views, 
such as layouts, are in the "Views/Shared" directory. 

For example, an "index" view for the HomeController would be in
Views/Home/index.html.erb.

  # Controllers/HomeController
  class HomeController < Controller
    def index
      data = "Hello, World"
      view 'index', 'layout', data
    end
  end

  <!-- Views/Home/index.html.erb -->
  MVC says: <%= model %>

  <!-- Views/Shared/layout.html.erb -->
  <h1><% yield %></h1>

Samples
-------

IronRubyMvc.Test - Tests Library.
IronRubyMvcWeb - "Hello, World" test website. You need to change the connection string and attach the database or use sql express.
Pictures - "Real" Demo application

Running IronRubyMvcWeb:
1. Start cassini against the website

Running Pictures:
1. Start cassini against Pictures
2. ruby Pictures\server\app.rb
