module IronRubyMvc
  
  class RubyController
    
    class << self
      def before_filter(name, options, &b)
        raise NotImplementedError.new
      end
      
      def after_filter(name, options, &b)
        raise NotImplementedError.new
      end
    end
    
  end
  
end