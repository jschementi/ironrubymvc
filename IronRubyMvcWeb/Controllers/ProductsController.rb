#require 'helpers/BaseController'
#require 'ProductsRepository'

require 'IronRubyMvcWeb, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'
include IronRubyMvcWeb::Models
include IronRubyMvcWeb::Models::Northwind

class ProductsRepository < IronRubyRepository 
end

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

class ProductsController < Controller
  def index
    repository = ProductsRepository.new
    
    @message = "Hi Mom"
    @categories = repository.get_categories
    
    return_view nil, 'layout'
  end
  
  def return_view(view, layout)
    fill_view_data
    view view, layout
  end
  
  def list
    @category = params[:id]
    
    repository = ProductsRepository.new
    @products = repository.get_products_for_category @category
    
    return_view nil, 'layout'
  end
  
  def edit
    @id = params[:id]
    
    repository = ProductsRepository.new
    @product = repository.get_product @id
    return_view nil, 'layout'
  end
  
  def update
    id = params[:id]
    category = params[:category_name]
    
    repository = ProductsRepository.new
    @product = repository.get_product id
    @product.product_name = request.form.get_Item('Product.productname')
    @product.unit_price = to_decimal request.form.get_Item('Product.unitprice')
    repository.submit_changes
    
    redirect_to_action 'list', {:id => category }
  end
  
  def to_decimal(value)
	  System::Convert::ToDecimal value
  end
end
