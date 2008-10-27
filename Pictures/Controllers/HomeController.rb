require "home_model"
require 'base_model'
require 'picture'
require 'album'
class HomeController < Controller
  def initialize(context = nil)
    if(context != nil)
        base.Initialize(context)
    end
  end
  
  def index
    Picture.find(:first)
    view nil, 'layout1', HomeModel.new
  end
end