module IronRubyMvc
  
  #module Controllers
  
  class Controller < IronRubyMvcLibrary::Controllers::RubyController
    
    
    
    class << self
      def before_filter(name, options, &b)
        raise NotImplementedError.new
      end
      
      def after_filter(name, options, &b)
        raise NotImplementedError.new
      end
    end
    
  end
  
  #end
  
  
  
end

Controller = IronRubyMvc::Controller