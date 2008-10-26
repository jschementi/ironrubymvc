require "home_model"

class HomeController < Controller
  def initialize(context = nil)
    if(context != nil)
        base.Initialize(context)
    end
  end
  
  def index
    view nil, 'layout1', HomeModel.new
  end
end