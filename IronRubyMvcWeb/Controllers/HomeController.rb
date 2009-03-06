#require "HomeModel"

require 'MyFilter'

class HomeModel
  def salutation
    "ASP.NET MVC <3 IronRuby!!!"
  end
end



class HomeController < Controller
  #  def initialize(context = nil)
  #    if(context != nil)
  #        super.Initialize(context)
  #    end
  #  end
  
  before_action :index do |context|
    $before_counter ||= 0
    $before_counter += 1
    context.request_context.http_context.response.write("Hello world<br />")
  end
  
  before_action :index, :method_filter
  
  filter :index, MyFilter
  
  def index
    view(nil, 'layout', HomeModel.new)
  end
  
  def method_filter(context)
    context.request_context.http_context.response.write("From method filter<br />")
  end
end