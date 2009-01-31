module IronRubyMvc
  
  class RubyController
    
    class << self
      def before_filter(b)
        raise NotImplementedError.new
      end
      
      def after_filter
      end
    end
    
  end
  
end