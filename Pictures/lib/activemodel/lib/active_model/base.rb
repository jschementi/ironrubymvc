module ActiveModel
  # Generic Active Model exception class.
  class ActiveModelError < StandardError
  end
  
  class RecordNotFound < ActiveModelError
  end
  

  # Raised when you've tried to access a column which wasn't loaded by your finder.
  # Typically this is because <tt>:select</tt> has been specified.
  class MissingAttributeError < NoMethodError
  end
    
  # These are the methods which must be implemented by concrete extensions to ActiveModel
  module ExternalInterface
    def self.included(base)
      base.extend ClassMethods
    end
    module ClassMethods
      def find(*args)
        warn "Must implement find in extensions"
        []        
      end
      
      def columns
        warn "Must implement columns in extensions"
        []
      end
    end
  end

  class Base
    include Observing
    include Associations
    include Reflection    
    include Persistence
    include Validations
    include Callbacks
    include NamedScope
    include AttributeMethods
    include ExternalInterface
    include SchemaDefinitions

    cattr_accessor :logger, :instance_writer => false
    
    # Determine whether to store the full constant name including namespace when using STI
    superclass_delegating_accessor :store_full_sti_class
    self.store_full_sti_class = false

    # Returns the value of the attribute identified by <tt>attr_name</tt> after it has been typecast (for example,
    # "2004-12-12" in a data column is cast to a date object, like Date.new(2004, 12, 12)).
    # (Alias for the protected read_attribute method).
    def [](attr_name)
      read_attribute(attr_name)
    end

    # Updates the attribute identified by <tt>attr_name</tt> with the specified +value+.
    # (Alias for the protected write_attribute method).
    def []=(attr_name, value)
      write_attribute(attr_name, value)
    end

    # New objects can be instantiated as either empty (pass no construction parameter) or pre-set with
    # attributes but not yet saved (pass a hash with key names matching the associated table column names).
    # In both instances, valid attribute keys are determined by the column names of the associated table --
    # hence you can't have attributes that aren't part of the table columns.
    def initialize(attributes = nil)
      @attributes = attributes_from_column_definition
      @attributes_cache = {}
      @new_record = true
      ensure_proper_type
      self.attributes = attributes unless attributes.nil?
      self.class.send(:scope, :create).each { |att,value| self.send("#{att}=", value) } if self.class.send(:scoped?, :create)
      result = yield self if block_given?
      callback(:after_initialize) if respond_to_without_attributes?(:after_initialize)
      result
    end

    # Allows you to set all the attributes at once by passing in a hash with keys
    # matching the attribute names (which again matches the column names). Sensitive attributes can be protected
    # from this form of mass-assignment by using the +attr_protected+ macro. Or you can alternatively
    # specify which attributes *can* be accessed with the +attr_accessible+ macro. Then all the
    # attributes not included in that won't be allowed to be mass-assigned.
    def attributes=(new_attributes, guard_protected_attributes = true)
      return if new_attributes.nil?
      attributes = new_attributes.dup
      attributes.stringify_keys!

      multi_parameter_attributes = []
      attributes = remove_attributes_protected_from_mass_assignment(attributes) if guard_protected_attributes

      attributes.each do |k, v|
        if k.include?("(")
          multi_parameter_attributes << [ k, v ]
        else
          respond_to?(:"#{k}=") ? send(:"#{k}=", v) : raise(UnknownAttributeError, "unknown attribute: #{k}")
        end
      end

      assign_multiparameter_attributes(multi_parameter_attributes)
    end
    
    # Instantiates objects for all attribute classes that needs more than one constructor parameter. This is done
    # by calling new on the column type or aggregation type (through composed_of) object with these parameters.
    # So having the pairs written_on(1) = "2004", written_on(2) = "6", written_on(3) = "24", will instantiate
    # written_on (a date type) with Date.new("2004", "6", "24"). You can also specify a typecast character in the
    # parentheses to have the parameters typecasted before they're used in the constructor. Use i for Fixnum, f for Float,
    # s for String, and a for Array. If all the values for a given attribute are empty, the attribute will be set to nil.
    def assign_multiparameter_attributes(pairs)
      execute_callstack_for_multiparameter_attributes(
        extract_callstack_for_multiparameter_attributes(pairs)
      )
    end
    
    def execute_callstack_for_multiparameter_attributes(callstack)
      errors = []
      callstack.each do |name, values|
        klass = (self.class.reflect_on_aggregation(name.to_sym) || column_for_attribute(name)).klass
        if values.empty?
          send(name + "=", nil)
        else
          begin
            value = if Time == klass
              instantiate_time_object(name, values)
            elsif Date == klass
              begin
                Date.new(*values)
              rescue ArgumentError => ex # if Date.new raises an exception on an invalid date
                instantiate_time_object(name, values).to_date # we instantiate Time object and convert it back to a date thus using Time's logic in handling invalid dates
              end
            else
              klass.new(*values)
            end

            send(name + "=", value)
          rescue => ex
            errors << AttributeAssignmentError.new("error on assignment #{values.inspect} to #{name}", ex, name)
          end
        end
      end
      unless errors.empty?
        raise MultiparameterAssignmentErrors.new(errors), "#{errors.size} error(s) on assignment of multiparameter attributes"
      end
    end

    def extract_callstack_for_multiparameter_attributes(pairs)
      attributes = { }

      for pair in pairs
        multiparameter_name, value = pair
        attribute_name = multiparameter_name.split("(").first
        attributes[attribute_name] = [] unless attributes.include?(attribute_name)

        unless value.empty?
          attributes[attribute_name] <<
            [ find_parameter_position(multiparameter_name), type_cast_attribute_value(multiparameter_name, value) ]
        end
      end

      attributes.each { |name, values| attributes[name] = values.sort_by{ |v| v.first }.collect { |v| v.last } }
    end

    
    def remove_attributes_protected_from_mass_assignment(attributes)
      safe_attributes =
        if self.class.accessible_attributes.nil? && self.class.protected_attributes.nil?
          attributes.reject { |key, value| attributes_protected_by_default.include?(key.gsub(/\(.+/, "")) }
        elsif self.class.protected_attributes.nil?
          attributes.reject { |key, value| !self.class.accessible_attributes.include?(key.gsub(/\(.+/, "")) || attributes_protected_by_default.include?(key.gsub(/\(.+/, "")) }
        elsif self.class.accessible_attributes.nil?
          attributes.reject { |key, value| self.class.protected_attributes.include?(key.gsub(/\(.+/,"")) || attributes_protected_by_default.include?(key.gsub(/\(.+/, "")) }
        else
          raise "Declare either attr_protected or attr_accessible for #{self.class}, but not both."
        end

      removed_attributes = attributes.keys - safe_attributes.keys

      if removed_attributes.any?
        log_protected_attribute_removal(removed_attributes)
      end

      safe_attributes
    end

    # Removes attributes which have been marked as readonly.
    def remove_readonly_attributes(attributes)
      unless self.class.readonly_attributes.nil?
        attributes.delete_if { |key, value| self.class.readonly_attributes.include?(key.gsub(/\(.+/,"")) }
      else
        attributes
      end
    end

    def log_protected_attribute_removal(*attributes)
      logger.debug "WARNING: Can't mass-assign these protected attributes: #{attributes.join(', ')}"
    end

    # The primary key and inheritance column can never be set by mass-assignment for security reasons.
    def attributes_protected_by_default
      default = [ self.class.primary_key, self.class.inheritance_column ]
      default << 'id' unless self.class.primary_key.eql? 'id'
      default
    end
    
    
    # Sets the attribute used for single table inheritance to this class name if this is not the ActiveModel::Base descendent.
    # Considering the hierarchy Reply < Message < ActiveModel::Base, this makes it possible to do Reply.new without having to
    # set <tt>Reply[Reply.inheritance_column] = "Reply"</tt> yourself. No such attribute would be set for objects of the
    # Message class in that example.
    def ensure_proper_type
      unless self.class.descends_from_active_model?
        write_attribute(self.class.inheritance_column, self.class.sti_name)
      end
    end
    
    

    
    # Initializes the attributes array with keys matching the columns from the linked table and
    # the values matching the corresponding default value of that column, so
    # that a new instance, or one populated from a passed-in Hash, still has all the attributes
    # that instances loaded from the database would.
    def attributes_from_column_definition
      self.class.columns.inject({}) do |attributes, column|
        attributes[column.name] = column.default unless column.name == self.class.primary_key
        attributes
      end
    end

    # Returns the column object for the named attribute.
    def column_for_attribute(name)
      self.class.columns_hash[name.to_s]
    end

    # Returns true if the +comparison_object+ is the same object, or is of the same type and has the same id.
    def ==(comparison_object)
      comparison_object.equal?(self) ||
        (comparison_object.instance_of?(self.class) &&
          comparison_object.id == id &&
          !comparison_object.new_record?)
    end

    # A model instance's primary key is always available as model.id
    # whether you name it the default 'id' or set it to something else.
    def id
      attr_name = self.class.primary_key
      column = column_for_attribute(attr_name)

      self.class.send(:define_read_method, :id, attr_name, column)
      # now that the method exists, call it
      self.send attr_name.to_sym

    end

    # Delegates to ==
    def eql?(comparison_object)
      self == (comparison_object)
    end

    # Delegates to id in order to allow two records of the same type and id to work with something like:
    #   [ Person.find(1), Person.find(2), Person.find(3) ] & [ Person.find(1), Person.find(4) ] # => [ Person.find(1) ]
    def hash
      id.hash
    end

    # Enables Active Model objects to be used as URL parameters in Action Pack automatically.
    def to_param
      # We can't use alias_method here, because method 'id' optimizes itself on the fly.
      (id = self.id) ? id.to_s : nil # Be sure to stringify the id for routes
    end
    

    class << self

      # Retrieve the scope for the given method and optional key.
      def scope(method, key = nil) #:nodoc:
        if current_scoped_methods && (scope = current_scoped_methods[method])
          key ? scope[key] : scope
        end
      end

      
      # Attributes named in this macro are protected from mass-assignment,
      # such as <tt>new(attributes)</tt>,
      # <tt>update_attributes(attributes)</tt>, or
      # <tt>attributes=(attributes)</tt>.
      #
      # Mass-assignment to these attributes will simply be ignored, to assign
      # to them you can use direct writer methods. This is meant to protect
      # sensitive attributes from being overwritten by malicious users
      # tampering with URLs or forms.
      #
      #   class Customer < ActiveRecord::Base
      #     attr_protected :credit_rating
      #   end
      #
      #   customer = Customer.new("name" => David, "credit_rating" => "Excellent")
      #   customer.credit_rating # => nil
      #   customer.attributes = { "description" => "Jolly fellow", "credit_rating" => "Superb" }
      #   customer.credit_rating # => nil
      #
      #   customer.credit_rating = "Average"
      #   customer.credit_rating # => "Average"
      #
      # To start from an all-closed default and enable attributes as needed,
      # have a look at +attr_accessible+.
      def attr_protected(*attributes)
        write_inheritable_attribute(:attr_protected, Set.new(attributes.map(&:to_s)) + (protected_attributes || []))
      end

      # Returns an array of all the attributes that have been protected from mass-assignment.
      def protected_attributes # :nodoc:
        read_inheritable_attribute(:attr_protected)
      end

      # Specifies a white list of model attributes that can be set via
      # mass-assignment, such as <tt>new(attributes)</tt>,
      # <tt>update_attributes(attributes)</tt>, or
      # <tt>attributes=(attributes)</tt>
      #
      # This is the opposite of the +attr_protected+ macro: Mass-assignment
      # will only set attributes in this list, to assign to the rest of
      # attributes you can use direct writer methods. This is meant to protect
      # sensitive attributes from being overwritten by malicious users
      # tampering with URLs or forms. If you'd rather start from an all-open
      # default and restrict attributes as needed, have a look at
      # +attr_protected+.
      #
      #   class Customer < ActiveRecord::Base
      #     attr_accessible :name, :nickname
      #   end
      #
      #   customer = Customer.new(:name => "David", :nickname => "Dave", :credit_rating => "Excellent")
      #   customer.credit_rating # => nil
      #   customer.attributes = { :name => "Jolly fellow", :credit_rating => "Superb" }
      #   customer.credit_rating # => nil
      #
      #   customer.credit_rating = "Average"
      #   customer.credit_rating # => "Average"
      def attr_accessible(*attributes)
        write_inheritable_attribute(:attr_accessible, Set.new(attributes.map(&:to_s)) + (accessible_attributes || []))
      end

      # Returns an array of all the attributes that have been made accessible to mass-assignment.
      def accessible_attributes # :nodoc:
        read_inheritable_attribute(:attr_accessible)
      end

       # Attributes listed as readonly can be set for a new record, but will be ignored in database updates afterwards.
       def attr_readonly(*attributes)
         write_inheritable_attribute(:attr_readonly, Set.new(attributes.map(&:to_s)) + (readonly_attributes || []))
       end

       # Returns an array of all the attributes that have been specified as readonly.
       def readonly_attributes
         read_inheritable_attribute(:attr_readonly)
       end
             
      def primary_key
        raise NotImplementedError
      end

      # Test whether the given method and optional key are scoped.
      def scoped?(method, key = nil) #:nodoc:
        if current_scoped_methods && (scope = current_scoped_methods[method])
          !key || scope.has_key?(key)
        end
      end

      # Defines the column name for use with single table inheritance
      # -- can be set in subclasses like so: self.inheritance_column = "type_id"
      def inheritance_column
        @inheritance_column ||= "type".freeze
      end

      # Returns whether this class is a base AM class.  If A is a base class and
      # B descends from A, then B.base_class will return B.
      def abstract_class?
        defined?(@abstract_class) && @abstract_class == true
      end

      # True if this isn't a concrete subclass needing a STI type condition.
      def descends_from_active_model?
        if superclass.abstract_class?
          superclass.descends_from_active_model?
        else
          superclass == Base || !columns_hash.include?(inheritance_column)
        end
      end
      
      # Returns a hash of column objects for the table associated with this class.
       def columns_hash
         @columns_hash ||= columns.inject({}) { |hash, column| hash[column.name] = column; hash }
       end

   
      
      # Returns the class type of the record using the current module as a prefix. So descendents of
      # MyApp::Business::Account would appear as MyApp::Business::AccountSubclass.
      def compute_type(type_name)
        modularized_name = type_name_with_module(type_name)
        silence_warnings do
          begin
            class_eval(modularized_name, __FILE__, __LINE__)
          rescue NameError
            class_eval(type_name, __FILE__, __LINE__)
          end
        end
      end
      
      # Nest the type name in the same module as this class.
      # Bar is "MyApp::Business::Bar" relative to MyApp::Business::Foo
      def type_name_with_module(type_name)
        if store_full_sti_class
          type_name
        else
          (/^::/ =~ type_name) ? type_name : "#{parent.name}::#{type_name}"
        end
      end
      
      VALID_FIND_OPTIONS = [ :conditions, :include, :joins, :limit, :offset,
                             :order, :select, :readonly, :group, :from, :lock ]

     def set_readonly_option!(options) #:nodoc:
       # Inherit :readonly from finder scope if set.  Otherwise,
       # if :joins is not blank then :readonly defaults to true.
       unless options.has_key?(:readonly)
         if scoped_readonly = scope(:find, :readonly)
           options[:readonly] = scoped_readonly
         elsif !options[:joins].blank? && !options[:select]
           options[:readonly] = true
         end
       end
     end

      
      
      # Scope parameters to method calls within the block.  Takes a hash of method_name => parameters hash.
      # method_name may be <tt>:find</tt> or <tt>:create</tt>. <tt>:find</tt> parameters may include the <tt>:conditions</tt>, <tt>:joins</tt>,
      # <tt>:include</tt>, <tt>:offset</tt>, <tt>:limit</tt>, and <tt>:readonly</tt> options. <tt>:create</tt> parameters are an attributes hash.
      #
      #   class Article < ActiveRecord::Base
      #     def self.create_with_scope
      #       with_scope(:find => { :conditions => "blog_id = 1" }, :create => { :blog_id => 1 }) do
      #         find(1) # => SELECT * from articles WHERE blog_id = 1 AND id = 1
      #         a = create(1)
      #         a.blog_id # => 1
      #       end
      #     end
      #   end
      #
      # In nested scopings, all previous parameters are overwritten by the innermost rule, with the exception of
      # <tt>:conditions</tt> and <tt>:include</tt> options in <tt>:find</tt>, which are merged.
      #
      #   class Article < ActiveRecord::Base
      #     def self.find_with_scope
      #       with_scope(:find => { :conditions => "blog_id = 1", :limit => 1 }, :create => { :blog_id => 1 }) do
      #         with_scope(:find => { :limit => 10 })
      #           find(:all) # => SELECT * from articles WHERE blog_id = 1 LIMIT 10
      #         end
      #         with_scope(:find => { :conditions => "author_id = 3" })
      #           find(:all) # => SELECT * from articles WHERE blog_id = 1 AND author_id = 3 LIMIT 1
      #         end
      #       end
      #     end
      #   end
      #
      # You can ignore any previous scopings by using the <tt>with_exclusive_scope</tt> method.
      #
      #   class Article < ActiveModel::Base
      #     def self.find_with_exclusive_scope
      #       with_scope(:find => { :conditions => "blog_id = 1", :limit => 1 }) do
      #         with_exclusive_scope(:find => { :limit => 10 })
      #           find(:all) # => SELECT * from articles LIMIT 10
      #         end
      #       end
      #     end
      #   end
      def with_scope(method_scoping = {}, action = :merge, &block)
        method_scoping = method_scoping.method_scoping if method_scoping.respond_to?(:method_scoping)

        # Dup first and second level of hash (method and params).
        method_scoping = method_scoping.inject({}) do |hash, (method, params)|
          hash[method] = (params == true) ? params : params.dup
          hash
        end

        method_scoping.assert_valid_keys([ :find, :create ])

        if f = method_scoping[:find]
          f.assert_valid_keys(VALID_FIND_OPTIONS)
          set_readonly_option! f
        end

        # Merge scopings
        if action == :merge && current_scoped_methods
          method_scoping = current_scoped_methods.inject(method_scoping) do |hash, (method, params)|
            case hash[method]
              when Hash
                if method == :find
                  (hash[method].keys + params.keys).uniq.each do |key|
                    merge = hash[method][key] && params[key] # merge if both scopes have the same key
                    if key == :conditions && merge
                      hash[method][key] = merge_conditions(params[key], hash[method][key])
                    elsif key == :include && merge
                      hash[method][key] = merge_includes(hash[method][key], params[key]).uniq
                    elsif key == :joins && merge
                      hash[method][key] = merge_joins(params[key], hash[method][key])
                    else
                      hash[method][key] = hash[method][key] || params[key]
                    end
                  end
                else
                  hash[method] = params.merge(hash[method])
                end
              else
                hash[method] = params
            end
            hash
          end
        end

        self.scoped_methods << method_scoping

        begin
          yield
        ensure
          self.scoped_methods.pop
        end
      end
      
      def scoped_methods #:nodoc:
        scoped_methods = (Thread.current[:scoped_methods] ||= {})
        scoped_methods[self] ||= []
      end

      def current_scoped_methods #:nodoc:
        scoped_methods.last
      end
      
      # Returns a hash of all the attributes that have been specified for serialization as keys and their class restriction as values.
      def serialized_attributes
        read_inheritable_attribute(:attr_serialized) or write_inheritable_attribute(:attr_serialized, {})
      end


      #
      
      
    end 
  end
end