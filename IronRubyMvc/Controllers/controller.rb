module IronRubyMvc
  
  #module Controllers
  
  class Controller < IronRubyMvcLibrary::Controllers::RubyController
    
    
    
    class << self
      def before_filter(name, options, &b)
        filter(name, options.merge(:type => :before), &b)     
      end
      
      def after_filter(name, options, &b)
        filter(name, options.merge(:type => :after), &b)
      end
      
      def filter(name, options, &b)
        @filters ||= {}
        options[:condition] = b if block_given?
        @filters[name.to_sym] = options
      end
      
      def filters
        @filters
      end
    end
    
  end
  
  #end
  
  
  
end

Controller = IronRubyMvc::Controller