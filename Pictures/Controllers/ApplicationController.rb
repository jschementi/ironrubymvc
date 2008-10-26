# set path to ruby libraries
ruby_lib = "C:/Users/jimmysch/Projects/vsl1/Merlin/External/Languages/Ruby/ruby-1.8.6/lib"
ironruby_lib = "C:/Users/jimmysch/Projects/vsl1/Merlin/Main/Languages/Ruby/Libs"
$: << ironruby_lib
$: << "#{ruby_lib}/ruby/site_ruby/1.8"
$: << "#{ruby_lib}/ruby/site_ruby"
#$: << "#{ruby_lib}/ruby/1.8"
$: << "."
$: << File.dirname(__FILE__) + "/../lib/activemodel/lib"
$: << File.dirname(__FILE__) + "/../lib/activesupport/lib"

require "active_model"

class ApplicationController < Controller
  def initialize(context = nil)
    if(context != nil)
        base.Initialize(context)
    end
  end

  def fill_view_data
    instance_variables.each { |varname| view_data.Add(varname[1..-1], instance_variable_get(varname.to_sym)) }
  end

  def to_decimal value
    System::Convert::ToDecimal value
  end

  def return_view *args
    fill_view_data
    view *args
  end

  def current_person
    @current_person ||= logged_in? ? Person.find(session[:person_id]) : nil
  end

  def logged_in?
    !session[:person_id].blank?
  end

  def authenticate
    if !logged_in?
      flash[:notice] = "Please log in to continue!"
      redirect_to new_session_url
    end
  end

  def set_tags!
    @tags = Tag.all
  end
end