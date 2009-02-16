#require "HomeModel"

class HomeModel
  def salutation
    "ASP.NET MVC <3 IronRuby!!!"
  end
end

module IronRubyMvc
  
  module Controllers
    
    module Filters
      
      module ClassMethods
      end
    end
  end
end

class HomeController < Controller
#  def initialize(context = nil)
#    if(context != nil)
#        super.Initialize(context)
#    end
#  end
  
  def index
    view nil, 'layout', HomeModel.new
  end
end