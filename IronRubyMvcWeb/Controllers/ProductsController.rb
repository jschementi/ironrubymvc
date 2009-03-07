require 'helpers/BaseController'
require 'ProductsRepository'

require 'MyProductFilter'
require 'MyControllerFilter'


class ProductsController < Controller
  
  filter :index, MyProductFilter
  filter MyControllerFilter
  
  def index
    repository = ProductsRepository.new
    
    @message = "Hi Mom"
    @categories = repository.get_categories
    
    view nil, 'layout'
  end
  
  def list
    @category = params[:id]
    
    repository = ProductsRepository.new
    @products = repository.get_products_for_category @category
    
    view nil, 'layout'
  end
  
  def edit
    @id = params[:id]
    
    repository = ProductsRepository.new
    @product = repository.get_product @id
    view nil, 'layout'
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
