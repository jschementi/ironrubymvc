require "HomeModel"

require 'MyFilter'

#class HomeModel
#  def salutation
#    "ASP.NET MVC <3 IronRuby!!!"
#  end
#end



class HomeController < Controller
  
  before_action :index do |context|
    context.request_context.http_context.response.write("Hello world<br />")
  end
  
  before_action :index, :method_filter
  
  filter :index, MyFilter
  
  alias_action :my_method, :index_again
  non_action :my_helper_method
  accept_verbs :only_post, :post
  
  def index
    view(nil, 'layout', HomeModel.new)
  end
  
  def my_method
    view('index', 'layout', HomeModel.new)
  end
  
  def my_helper_method
    view('index', 'layout', HomeModel.new)
  end
  
  def only_post
    view('index', 'layout', HomeModel.new)
  end
  
  def raise_error
    raise "This is supposed to happen"
  end
  
  def method_filter(context)
    context.request_context.http_context.response.write("From method filter<br />")
  end
end