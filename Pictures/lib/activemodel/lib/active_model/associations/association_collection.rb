module ActiveModel
  module Associations
    # AssociationCollection is an abstract class that provides common stuff to
    # ease the implementation of association proxies that represent
    # collections. See the class hierarchy in AssociationProxy.
    #
    # You need to be careful with assumptions regarding the target: The proxy
    # does not fetch records from the database until it needs them, but new
    # ones created with +build+ are added to the target. So, the target may be
    # non-empty and still lack children waiting to be read from the database.
    # If you look directly to the database you cannot assume that's the entire
    # collection because new records may have beed added to the target, etc.
    #
    # If you need to work on all current children, new and existing records,
    # +load_target+ and the +loaded+ flag are your friends.
    class AssociationCollection < AssociationProxy #:nodoc:
      # def initialize(owner, reflection)
      #   super
      # end
      
      def find(*args, &block)
        # FIXME: this is a braindead implementation
        options = args.extract_options!
        options.merge! construct_scope[:find]
        args << options
        @reflection.klass.find(*args)
      end
      
      def find_target
        records =
          if @reflection.options[:finder_sql]
            #FIXME: this block will be replaced with something like
            # if @reflection.options[:finder]
            # persistence_driver.find_on(@reflection.klass, :with_finder => options[:finder])
            @reflection.klass.find_by_sql(@finder_sql)
          else
            find(:all)
          end

        @reflection.options[:uniq] ? uniq(records) : records
      end
      # overloaded in derived Association classes to provide useful scoping depending on association type.
      def construct_scope
        #FIXME overload on subclasses.
        {}
      end
      
    
      def build(attributes = {}, &block)
        if attributes.is_a?(Array)
          attributes.collect { |attr| build(attr, &block) }
        else
          build_record(attributes) do |record|
            block.call(record) if block_given?
            set_belongs_to_association_for(record)
          end
        end
      end
      

      def build_record(attrs)
        attrs.update(@reflection.options[:conditions]) if @reflection.options[:conditions].is_a?(Hash)
        record = @reflection.klass.new(attrs)
        if block_given?
          add_record_to_target_with_callbacks(record) { |*block_args| yield(*block_args) }
        else
          add_record_to_target_with_callbacks(record)
        end
      end
      
      def add_record_to_target_with_callbacks(record)
        callback(:before_add, record)
        yield(record) if block_given?
        @target ||= [] unless loaded?
        @target << record unless @reflection.options[:uniq] && @target.include?(record)
        callback(:after_add, record)
        record
      end

      def callback(method, record)
        callbacks_for(method).each do |callback|
          ActiveSupport::Callbacks::Callback.new(method, callback, record).call(@owner, record)
        end
      end
            
      

      def callbacks_for(callback_name)
        full_callback_name = "#{callback_name}_for_#{@reflection.name}"
        @owner.class.read_inheritable_attribute(full_callback_name.to_sym) || []
      end   
      
 def method_missing(method, *args)
   load_target unless @loaded
      if @target.respond_to?(method) || (!@reflection.klass.respond_to?(method) && Class.respond_to?(method))
        if block_given?
          super { |*block_args| yield(*block_args) }
        else
          super
        end
      elsif @reflection.klass.scopes.include?(method)
        @reflection.klass.scopes[method].call(self, *args)
      else          
        with_scope(construct_scope) do
          if block_given?
            @reflection.klass.send(method, *args) { |*block_args| yield(*block_args) }
          else
            @reflection.klass.send(method, *args)
          end
        end
      end
    end
   end 
  end
end