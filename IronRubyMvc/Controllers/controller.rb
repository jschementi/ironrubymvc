module IronRubyMvc
  
#  module Controllers
#    
#    module Filters
#      
#      module ClassMethods
#        
#        
#        
#      end
#      
#      def self.included(base)
#        base.extend(IronRubyMvc::Controllers::Filters::ClassMethods)
#      end
#      
#    end
#    
#    module Selectors
#      
#      module ClassMethods
#        
#        
#        
#      end
#      
#      def self.included(base)
#        base.extend(IronRubyMvc::Controllers::Selectors::ClassMethods)
#      end
#    end
    
#  end
  
  #module Controllers
  
  class Controller < IronRubyMvcLibrary::Controllers::RubyController
    
    #include IronRubyMvc::Controllers::Filters
    #include IronRubyMvc::Controllers::Selectors   
    
    #cattr_accessor :action_filters, :action_selectors
    
    def fill_view_data
      instance_variables.each { |varname| view_data.Add(varname[1..-1], instance_variable_get(varname.to_sym)) }
    end
    
    def return_view(*args)
      fill_view_data
      view args
    end
    
    class << self
      
      def before_filter(name, options, &b)
        filter(name, options.merge(:when => :before), &b)     
      end
      
      def after_filter(name, options, &b)
        filter(name, options.merge(:when => :after), &b)
      end
      
      def around_filter(name, options, &b)
        filter(name, options.merge(:when => :around), &b)
      end
      
      def filter(name, options, &b)
        @@action_filters ||= {}
        options[:action] = b if block_given?
        options[:action] ||= method(name.to_sym)
        @@action_filters[name.to_sym] = options
      end
      
      def action_filters
        @@action_filters
      end
      
      def action_selector(name, options, &b)
        @@action_selectors ||= {}
        options[:action] = b if block_given?
        options[:action] ||= method(name.to_sym)
        @@action_selectors[name.to_sym] = options
      end
      
      def action_selectors
        @@action_selectors
      end
      
    end
    
  end
  
  #end
  
  
  
end

Controller = IronRubyMvc::Controller