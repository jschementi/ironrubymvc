module IronRubyMvc
  
  module Controllers
    
    module Filters
      
      class RubyProcActionFilter < System::Web::Mvc::IronRuby::Controllers::RubyActionFilter      
        
        attr_accessor :before_action, :after_action
        
        def initialize(before_action=nil, after_action=nil)
          raise ArgumentError.new("You need to provide either a before or an after action") if before_action.nil? && after_action.nil?
          @before_action = before_action
          @after_action = after_action
        end 
        
        def on_action_executing(context)
          before_action.call(context) unless before_action.nil?
        end
        
        def on_action_executed(context)
          after_action.call(context) unless after_action.nil?
        end
      end
      
      class RubyProcAuthorizationFilter < System::Web::Mvc::IronRuby::Controllers::RubyAuthorizationFilter      
        
        attr_accessor :authorize
        
        def initialize(authorize)
          @authorize = authorize
        end 
        
        def on_authorization(context)
          authorize.call(context)
        end
        
      end
      
      class RubyProcExceptionFilter < System::Web::Mvc::IronRuby::Controllers::RubyExceptionFilter      
        
        attr_accessor :on_exception
        
        def initialize(on_exception)
          @on_exception = on_exception
        end 
        
        def on_action_executing(context)
          on_exception.call(context)
        end
        
      end
      
      class RubyProcResultFilter < System::Web::Mvc::IronRuby::Controllers::RubyResultFilter      
        
        attr_accessor :before_result, :after_result
        
        def initialize(before_result = nil, after_result=nil)
          @before_result = before_result
          @after_result = after_result
        end 
        
        def on_action_executing(context)
          before_result.call(context) unless before_result.nil?
        end
        
        def on_action_executed(context)
          after_result.call(context) unless after_result.nil?
        end
      end
      
      module ClassMethods
        
        def before_action(name, &b)
          filter(name, RubyProcActionFilter.new(b, nil)) 
        end

        def after_action(name, &b)
          filter(name, RubyProcActionFilter.new(nil, b)) 
        end

        def around_action(name, options={}, &b)
          options[:before] ||= b if block_given?
          options[:after] ||= b if block_given?
          filter(name, RubyProcActionFilter.new(options[:before], options[:after]))
        end
        
        def authorized_action(name, &b)
          filter(name, RubyProcAuthorizationFilter.new(&b))
        end
        
        def exception_action(name, &b)
          filter(name, RubyProcExceptionFilter.new(&b))
        end
        
        def before_result(name, options={}, &b)
          filter(name, RubyProcResultFilter.new(&b))
        end
        
        def after_result(name, options={}, &b)
          filter(name, RubyProcResultFilter.new(nil, &b))
        end
        
        def around_result(name, options={}, &b)
          options[:before] ||= b if block_given?
          options[:after] ||= b if block_given?
          filter(name, RubyProcResultFilter.new(options[:before], options[:after])) 
        end

        def filter(name, options=nil)
          @action_filters ||= {}
          klass = nil
          klass = name.new if name.is_a? Class
          klass = options.new if options.is_a? Class
          klass ||= options
          name = :controller if name.nil? or name.is_a?(Class)
          @action_filters[name.to_sym] ||= []
          @action_filters[name.to_sym] << klass          
        end

        def action_filters
          @action_filters ||= []
          @action_filters
        end
        
      end
      
      def self.included(base)
        base.extend(ClassMethods)
      end
      
    end
    
    module Selectors
      
      module ClassMethods
        
        def action_selector(name, options={}, &b)
          @action_selectors ||= []
          options[:action] = b if block_given?
          options[:action] ||= name.to_sym #class.instance_method(name.to_sym)
          @action_selectors << { :name => name.to_sym, :options => options }
        end
        
        def action_selectors
          @action_selectors ||= []
          @action_selectors
        end
        
      end
      
      def self.included(base)
        base.extend(ClassMethods)
      end
    end
    
  end
  
  #module Controllers
  
  class Controller < System::Web::Mvc::IronRuby::Controllers::RubyController
 
    
    include Controllers::Filters
    include Controllers::Selectors   
    
    def fill_view_data
      instance_variables.each { |varname| view_data.add(varname[1..-1], instance_variable_get(varname.to_sym)) }
    end
    
    def return_view(*args)
      fill_view_data
      view args
    end
            
  end
  
  #end
  
  
  
end

#alias longer namespaces for convenience
Controller = IronRubyMvc::Controller
ActionFilter = System::Web::Mvc::IronRuby::Controllers::RubyActionFilter
AuthorizationFilter = System::Web::Mvc::IronRuby::Controllers::RubyAuthorizationFilter
ExceptionFilter = System::Web::Mvc::IronRuby::Controllers::RubyExceptionFilter
ResultFilter = System::Web::Mvc::IronRuby::Controllers::RubyResultFilter