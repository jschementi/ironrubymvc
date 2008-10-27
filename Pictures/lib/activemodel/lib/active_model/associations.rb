require 'active_model/associations/association_proxy'

require 'active_model/associations/association_collection'

require 'active_model/associations/has_many_association'
require 'active_model/associations/has_many_through_association'

require 'active_model/associations/belongs_to_association'
require 'active_model/associations/belongs_to_polymorphic_association'

require 'active_model/associations/has_one_through_association'


module ActiveModel
  # See AcitveModel::Associations::ClassMethods for documentation.
  module Associations # :nodoc:
    def self.included(base)
      base.extend(ClassMethods)
    end

    # Clears out the association cache
    def clear_association_cache #:nodoc:
      self.class.reflect_on_all_associations.to_a.each do |assoc|
        instance_variable_set "@#{assoc.name}", nil
      end unless self.new_record?
    end  

    module ClassMethods    
      
      # Specifies a one-to-one association with another class. This method should only be used
      # if this class contains the foreign key. If the other class contains the foreign key,
      # then you should use +has_one+ instead. See also ActiveRecord::Associations::ClassMethods's overview
      # on when to use +has_one+ and when to use +belongs_to+.
      #
      # Methods will be added for retrieval and query for a single associated object, for which
      # this object holds an id:
      #
      # [association(force_reload = false)]
      #   Returns the associated object. +nil+ is returned if none is found.
      # [association=(associate)]
      #   Assigns the associate object, extracts the primary key, and sets it as the foreign key.
      # [association.nil?]
      #   Returns +true+ if there is no associated object.
      # [build_association(attributes = {})]
      #   Returns a new object of the associated type that has been instantiated
      #   with +attributes+ and linked to this object through a foreign key, but has not yet been saved.
      # [create_association(attributes = {})]
      #   Returns a new object of the associated type that has been instantiated
      #   with +attributes+, linked to this object through a foreign key, and that
      #   has already been saved (if it passed the validation).
      #
      # (+association+ is replaced with the symbol passed as the first argument, so
      # <tt>belongs_to :author</tt> would add among others <tt>author.nil?</tt>.)
      #
      # === Example
      #
      # A Post class declares <tt>belongs_to :author</tt>, which will add:
      # * <tt>Post#author</tt> (similar to <tt>Author.find(author_id)</tt>)
      # * <tt>Post#author=(author)</tt> (similar to <tt>post.author_id = author.id</tt>)
      # * <tt>Post#author?</tt> (similar to <tt>post.author == some_author</tt>)
      # * <tt>Post#author.nil?</tt>
      # * <tt>Post#build_author</tt> (similar to <tt>post.author = Author.new</tt>)
      # * <tt>Post#create_author</tt> (similar to <tt>post.author = Author.new; post.author.save; post.author</tt>)
      # The declaration can also include an options hash to specialize the behavior of the association.
      #
      # === Options
      #
      # [:class_name]
      #   Specify the class name of the association. Use it only if that name can't be inferred
      #   from the association name. So <tt>has_one :author</tt> will by default be linked to the Author class, but
      #   if the real class name is Person, you'll have to specify it with this option.
      # [:conditions]
      #   Specify the conditions that the associated object must meet in order to be included as a +WHERE+
      #   SQL fragment, such as <tt>authorized = 1</tt>.
      # [:select]
      #   By default, this is <tt>*</tt> as in <tt>SELECT * FROM</tt>, but can be changed if, for example, you want to do a join
      #   but not include the joined columns. Do not forget to include the primary and foreign keys, otherwise it will raise an error.
      # [:foreign_key]
      #   Specify the foreign key used for the association. By default this is guessed to be the name
      #   of the association with an "_id" suffix. So a class that defines a <tt>belongs_to :person</tt> association will use
      #   "person_id" as the default <tt>:foreign_key</tt>. Similarly, <tt>belongs_to :favorite_person, :class_name => "Person"</tt>
      #   will use a foreign key of "favorite_person_id".
      # [:dependent]
      #   If set to <tt>:destroy</tt>, the associated object is destroyed when this object is. If set to
      #   <tt>:delete</tt>, the associated object is deleted *without* calling its destroy method. This option should not be specified when
      #   <tt>belongs_to</tt> is used in conjunction with a <tt>has_many</tt> relationship on another class because of the potential to leave
      #   orphaned records behind.
      # [:counter_cache]
      #   Caches the number of belonging objects on the associate class through the use of +increment_counter+
      #   and +decrement_counter+. The counter cache is incremented when an object of this class is created and decremented when it's
      #   destroyed. This requires that a column named <tt>#{table_name}_count</tt> (such as +comments_count+ for a belonging Comment class)
      #   is used on the associate class (such as a Post class). You can also specify a custom counter cache column by providing
      #   a column name instead of a +true+/+false+ value to this option (e.g., <tt>:counter_cache => :my_custom_counter</tt>.)
      #   When creating a counter cache column, the database statement or migration must specify a default value of <tt>0</tt>, failing to do 
      #   this results in a counter with +NULL+ value, which will never increment.
      #   Note: Specifying a counter cache will add it to that model's list of readonly attributes using +attr_readonly+.
      # [:include]
      #   Specify second-order associations that should be eager loaded when this object is loaded.
      # [:polymorphic]
      #   Specify this association is a polymorphic association by passing +true+.
      #   Note: If you've enabled the counter cache, then you may want to add the counter cache attribute
      #   to the +attr_readonly+ list in the associated classes (e.g. <tt>class Post; attr_readonly :comments_count; end</tt>).
      # [:readonly]
      #   If true, the associated object is readonly through the association.
      # [:validate]
      #   If false, don't validate the associated objects when saving the parent object. +false+ by default.
      #
      # Option examples:
      #   belongs_to :firm, :foreign_key => "client_of"
      #   belongs_to :author, :class_name => "Person", :foreign_key => "author_id"
      #   belongs_to :valid_coupon, :class_name => "Coupon", :foreign_key => "coupon_id",
      #              :conditions => 'discounts > #{payments_count}'
      #   belongs_to :attachable, :polymorphic => true
      #   belongs_to :project, :readonly => true
      #   belongs_to :post, :counter_cache => true
      def belongs_to(association_id, options = {})
        reflection = create_belongs_to_reflection(association_id, options)

        ivar = "@#{reflection.name}"

        if reflection.options[:polymorphic]
          association_accessor_methods(reflection, BelongsToPolymorphicAssociation)

          method_name = "polymorphic_belongs_to_before_save_for_#{reflection.name}".to_sym
          define_method(method_name) do
            association = instance_variable_get(ivar) if instance_variable_defined?(ivar)

            if association && association.target
              if association.new_record?
                association.save(true)
              end

              if association.updated?
                self[reflection.primary_key_name] = association.id
                self[reflection.options[:foreign_type]] = association.class.base_class.name.to_s
              end
            end
          end
          before_save method_name
        else
          association_accessor_methods(reflection, BelongsToAssociation)
          association_constructor_method(:build,  reflection, BelongsToAssociation)
          association_constructor_method(:create, reflection, BelongsToAssociation)

          method_name = "belongs_to_before_save_for_#{reflection.name}".to_sym
          define_method(method_name) do
            association = instance_variable_get(ivar) if instance_variable_defined?(ivar)

            if !association.nil?
              if association.new_record?
                association.save(true)
              end

              if association.updated?
                self[reflection.primary_key_name] = association.id
              end
            end
          end
          before_save method_name
        end

        # Create the callbacks to update counter cache
        if options[:counter_cache]
          cache_column = options[:counter_cache] == true ?
            "#{self.to_s.demodulize.underscore.pluralize}_count" :
            options[:counter_cache]

          method_name = "belongs_to_counter_cache_after_create_for_#{reflection.name}".to_sym
          define_method(method_name) do
            association = send(reflection.name)
            association.class.increment_counter(cache_column, send(reflection.primary_key_name)) unless association.nil?
          end
          after_create method_name

          method_name = "belongs_to_counter_cache_before_destroy_for_#{reflection.name}".to_sym
          define_method(method_name) do
            association = send(reflection.name)
            association.class.decrement_counter(cache_column, send(reflection.primary_key_name)) unless association.nil?
          end
          before_destroy method_name

          module_eval(
            "#{reflection.class_name}.send(:attr_readonly,\"#{cache_column}\".intern) if defined?(#{reflection.class_name}) && #{reflection.class_name}.respond_to?(:attr_readonly)"
          )
        end

        add_single_associated_validation_callbacks(reflection.name) if options[:validate] == true

        configure_dependency_for_belongs_to(reflection)
      end
      
      def has_many(association_id, options = {}, &extension)
        reflection = create_has_many_reflection(association_id, options, &extension)

        configure_dependency_for_has_many(reflection)
        add_multiple_associated_validation_callbacks(reflection.name) unless options[:validate] == false
        add_multiple_associated_save_callbacks(reflection.name)
        add_association_callbacks(reflection.name, reflection.options)
        if options[:through]
          collection_accessor_methods(reflection, HasManyThroughAssociation)
        else
          collection_accessor_methods(reflection, HasManyAssociation)
        end
      end
      
      private
      mattr_accessor :valid_keys_for_belongs_to_association
      @@valid_keys_for_belongs_to_association = [
        :class_name, :foreign_key, :foreign_type, :remote, :select, :conditions,
        :include, :dependent, :counter_cache, :extend, :polymorphic, :readonly,
        :validate
      ]

      def create_belongs_to_reflection(association_id, options)
        options.assert_valid_keys(valid_keys_for_belongs_to_association)
        reflection = create_reflection(:belongs_to, association_id, options, self)

        if options[:polymorphic]
          reflection.options[:foreign_type] ||= reflection.class_name.underscore + "_type"
        end

        reflection
      end

      
      mattr_accessor :valid_keys_for_has_many_association
      @@valid_keys_for_has_many_association = [
        :class_name, :table_name, :foreign_key, :primary_key,
        :dependent,
        :select, :conditions, :include, :order, :group, :limit, :offset,
        :as, :through, :source, :source_type,
        :uniq,
        :finder_sql, :counter_sql,
        :before_add, :after_add, :before_remove, :after_remove,
        :extend, :readonly,
        :validate
      ]

      def create_has_many_reflection(association_id, options, &extension)
        options.assert_valid_keys(valid_keys_for_has_many_association)
        options[:extend] = create_extension_modules(association_id, extension, options[:extend])

        create_reflection(:has_many, association_id, options, self)
      end
      
      def create_extension_modules(association_id, block_extension, extensions)
        if block_extension
          extension_module_name = "#{self.to_s.demodulize}#{association_id.to_s.camelize}AssociationExtension"

          silence_warnings do
            self.parent.const_set(extension_module_name, Module.new(&block_extension))
          end
          Array(extensions).push("#{self.parent}::#{extension_module_name}".constantize)
        else
          Array(extensions)
        end        
      end
      
      # See HasManyAssociation#delete_records.  Dependent associations
      # delete children, otherwise foreign key is set to NULL.
      def configure_dependency_for_has_many(reflection)
        if reflection.options.include?(:dependent)
          warn("Currently unsupported in ActiveModel")
        end
      end
      
      def add_multiple_associated_validation_callbacks(association_name)
        method_name = "validate_associated_records_for_#{association_name}".to_sym
        ivar = "@#{association_name}"

        define_method(method_name) do
          association = instance_variable_get(ivar) if instance_variable_defined?(ivar)

          if association.respond_to?(:loaded?)
            if new_record?
              association
            elsif association.loaded?
              association.select { |record| record.new_record? }
            else
              association.target.select { |record| record.new_record? }
            end.each do |record|
              errors.add association_name unless record.valid?
            end
          end
        end

        validate method_name
      end
      
      def add_multiple_associated_save_callbacks(association_name)
        ivar = "@#{association_name}"

        method_name = "before_save_associated_records_for_#{association_name}".to_sym
        define_method(method_name) do
          @new_record_before_save = new_record?
          true
        end
        before_save method_name

        method_name = "after_create_or_update_associated_records_for_#{association_name}".to_sym
        define_method(method_name) do
          association = instance_variable_get(ivar) if instance_variable_defined?(ivar)

          records_to_save = if @new_record_before_save
            association
          elsif association.respond_to?(:loaded?) && association.loaded?
            association.select { |record| record.new_record? }
          elsif association.respond_to?(:loaded?) && !association.loaded?
            association.target.select { |record| record.new_record? }
          else
            []
          end
          records_to_save.each { |record| association.send(:insert_record, record) } unless records_to_save.blank?

          # reconstruct the SQL queries now that we know the owner's id
          # association.send(:construct_) if association.respond_to?(:construct_sql)
          warn("FIXME: what to do to avoid SQL-specific code here but still support the functionality needed?")
        end

        # Doesn't use after_save as that would save associations added in after_create/after_update twice
        after_create method_name
        after_update method_name
      end
      
      def add_association_callbacks(association_name, options)
        callbacks = %w(before_add after_add before_remove after_remove)
        callbacks.each do |callback_name|
          full_callback_name = "#{callback_name}_for_#{association_name}"
          defined_callbacks = options[callback_name.to_sym]
          if options.has_key?(callback_name.to_sym)
            class_inheritable_reader full_callback_name.to_sym
            write_inheritable_attribute(full_callback_name.to_sym, [defined_callbacks].flatten)
          else
            write_inheritable_attribute(full_callback_name.to_sym, [])
          end
        end
      end

      def association_accessor_methods(reflection, association_proxy_class)
        ivar = "@#{reflection.name}"

        define_method(reflection.name) do |*params|
          force_reload = params.first unless params.empty?

          association = instance_variable_get(ivar) if instance_variable_defined?(ivar)

          if association.nil? || !association.loaded? || force_reload
            association = association_proxy_class.new(self, reflection)
            retval = association.reload
            if retval.nil? and association_proxy_class == BelongsToAssociation
              instance_variable_set(ivar, nil)
              return nil
            end
            instance_variable_set(ivar, association)
          end

          association.target.nil? ? nil : association
        end

        define_method("#{reflection.name}=") do |new_value|
          association = instance_variable_get(ivar) if instance_variable_defined?(ivar)

          if association.nil? || association.target != new_value
            association = association_proxy_class.new(self, reflection)
          end

          if association_proxy_class == HasOneThroughAssociation
            association.create_through_record(new_value)
            self.send(reflection.name, new_value)
          else
            association.replace(new_value)
            instance_variable_set(ivar, new_value.nil? ? nil : association)
          end
        end

        if association_proxy_class == BelongsToAssociation
          define_method("#{reflection.primary_key_name}=") do |target_id|
            if instance_variable_defined?(ivar)
              if association = instance_variable_get(ivar)
                association.reset
              end
            end
            write_attribute(reflection.primary_key_name, target_id)
          end
        end

        define_method("set_#{reflection.name}_target") do |target|
          return if target.nil? and association_proxy_class == BelongsToAssociation
          association = association_proxy_class.new(self, reflection)
          association.target = target
          instance_variable_set(ivar, association)
        end
      end
            
      def collection_reader_method(reflection, association_proxy_class)
        define_method(reflection.name) do |*params|
          ivar = "@#{reflection.name}"
          force_reload = params.first unless params.empty?
          association = instance_variable_get(ivar) if instance_variable_defined?(ivar)
          unless association.respond_to?(:loaded?)
            association = association_proxy_class.new(self, reflection)
            instance_variable_set(ivar, association)
          end

          association.reload if force_reload

          association
        end
      end

      def association_constructor_method(constructor, reflection, association_proxy_class)
        define_method("#{constructor}_#{reflection.name}") do |*params|
          ivar = "@#{reflection.name}"

          attributees      = params.first unless params.empty?
          replace_existing = params[1].nil? ? true : params[1]
          association      = instance_variable_get(ivar) if instance_variable_defined?(ivar)

          if association.nil?
            association = association_proxy_class.new(self, reflection)
            instance_variable_set(ivar, association)
          end

          if association_proxy_class == HasOneAssociation
            association.send(constructor, attributees, replace_existing)
          else
            association.send(constructor, attributees)
          end
        end
      end

      def configure_dependency_for_belongs_to(reflection)
        if reflection.options.include?(:dependent)
          case reflection.options[:dependent]
            when :destroy
              method_name = "belongs_to_dependent_destroy_for_#{reflection.name}".to_sym
              define_method(method_name) do
                association = send(reflection.name)
                association.destroy unless association.nil?
              end
              before_destroy method_name
            when :delete
              method_name = "belongs_to_dependent_delete_for_#{reflection.name}".to_sym
              define_method(method_name) do
                association = send(reflection.name)
                association.delete unless association.nil?
              end
              before_destroy method_name
            else
              raise ArgumentError, "The :dependent option expects either :destroy or :delete (#{reflection.options[:dependent].inspect})"
          end
        end
      end

      def collection_accessor_methods(reflection, association_proxy_class, writer = true)
        collection_reader_method(reflection, association_proxy_class)

        if writer
          define_method("#{reflection.name}=") do |new_value|
            # Loads proxy class instance (defined in collection_reader_method) if not already loaded
            association = send(reflection.name)
            association.replace(new_value)
            association
          end

          define_method("#{reflection.name.to_s.singularize}_ids=") do |new_value|
            ids = (new_value || []).reject { |nid| nid.blank? }
            send("#{reflection.name}=", reflection.class_name.constantize.find(ids))
          end
        end
      end      
    end
  end
end