#require "HomeModel"

class HomeModel
  def salutation
    "ASP.NET MVC <3 IronRuby!!!"
  end
end

class HomeController < Controller
#  def initialize(context = nil)
#    if(context != nil)
#        base.Initialize(context)
#    end
#  end
  
  def index
    view nil, 'layout', HomeModel.new
  end
end