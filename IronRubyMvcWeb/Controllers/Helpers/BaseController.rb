require 'IronRubyMvcWeb, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'

class BaseController < Controller
  def fill_view_data
    instance_variables.each { |varname| view_data.Add(varname[1..-1], instance_variable_get(varname.to_sym)) }
  end
  
  def to_decimal(value)
	  System::Convert::ToDecimal value
  end
  
  def return_view(*args)
    fill_view_data
    view args
  end
end
