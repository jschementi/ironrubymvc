require 'active_model/core'
require 'active_support'
module ActiveModel
    module Callbacks
      CALLBACKS = %w(
        after_find after_initialize before_save after_save before_create after_create before_update after_update before_validation
        after_validation before_validation_on_create after_validation_on_create before_validation_on_update
        after_validation_on_update before_destroy after_destroy
      )

      def self.included(base) #:nodoc:
        base.extend Observable

        [:create_or_update, :valid?, :create, :update, :destroy].each do |method|
          base.send :alias_method_chain, method, :callbacks
        end

        base.send :include, ActiveSupport::Callbacks
        base.define_callbacks *CALLBACKS
      end

      # Is called when the object was instantiated by one of the finders, like <tt>Base.find</tt>.
      #def after_find() end

      # Is called after the object has been instantiated by a call to <tt>Base.new</tt>.
      #def after_initialize() end

      # Is called _before_ <tt>Base.save</tt> (regardless of whether it's a +create+ or +update+ save).
      def before_save() end

      # Is called _after_ <tt>Base.save</tt> (regardless of whether it's a +create+ or +update+ save).
      # Note that this callback is still wrapped in the transaction around +save+. For example, if you
      # invoke an external indexer at this point it won't see the changes in the database.
      #
      #  class Contact < ActiveRecord::Base
      #    after_save { logger.info( 'New contact saved!' ) }
      #  end
      def after_save()  end
      def create_or_update_with_callbacks #:nodoc:
        return false if callback(:before_save) == false
        result = create_or_update_without_callbacks
        callback(:after_save)
        result
      end
      private :create_or_update_with_callbacks

      # Is called _before_ <tt>Base.save</tt> on new objects that haven't been saved yet (no record exists).
      def before_create() end

      # Is called _after_ <tt>Base.save</tt> on new objects that haven't been saved yet (no record exists).
      # Note that this callback is still wrapped in the transaction around +save+. For example, if you
      # invoke an external indexer at this point it won't see the changes in the database.
      def after_create() end
      def create_with_callbacks #:nodoc:
        return false if callback(:before_create) == false
        result = create_without_callbacks
        callback(:after_create)
        result
      end
      private :create_with_callbacks

      # Is called _before_ <tt>Base.save</tt> on existing objects that have a record.
      def before_update() end

      # Is called _after_ <tt>Base.save</tt> on existing objects that have a record.
      # Note that this callback is still wrapped in the transaction around +save+. For example, if you
      # invoke an external indexer at this point it won't see the changes in the database.
      def after_update() end

      def update_with_callbacks(*args) #:nodoc:
        return false if callback(:before_update) == false
        result = update_without_callbacks(*args)
        callback(:after_update)
        result
      end
      private :update_with_callbacks

      # Is called _before_ <tt>Validations.validate</tt> (which is part of the <tt>Base.save</tt> call).
      def before_validation() end

      # Is called _after_ <tt>Validations.validate</tt> (which is part of the <tt>Base.save</tt> call).
      def after_validation() end

      # Is called _before_ <tt>Validations.validate</tt> (which is part of the <tt>Base.save</tt> call) on new objects
      # that haven't been saved yet (no record exists).
      def before_validation_on_create() end

      # Is called _after_ <tt>Validations.validate</tt> (which is part of the <tt>Base.save</tt> call) on new objects
      # that haven't been saved yet (no record exists).
      def after_validation_on_create()  end

      # Is called _before_ <tt>Validations.validate</tt> (which is part of the <tt>Base.save</tt> call) on
      # existing objects that have a record.
      def before_validation_on_update() end

      # Is called _after_ <tt>Validations.validate</tt> (which is part of the <tt>Base.save</tt> call) on
      # existing objects that have a record.
      def after_validation_on_update()  end

      def valid_with_callbacks? #:nodoc:
        return false if callback(:before_validation) == false
        if new_record? then result = callback(:before_validation_on_create) else result = callback(:before_validation_on_update) end
        return false if false == result

        result = valid_without_callbacks?

        callback(:after_validation)
        if new_record? then callback(:after_validation_on_create) else callback(:after_validation_on_update) end

        return result
      end

      # Is called _before_ <tt>Base.destroy</tt>.
      #
      # Note: If you need to _destroy_ or _nullify_ associated records first,
      # use the <tt>:dependent</tt> option on your associations.
      def before_destroy() end

      # Is called _after_ <tt>Base.destroy</tt> (and all the attributes have been frozen).
      #
      #  class Contact < ActiveRecord::Base
      #    after_destroy { |record| logger.info( "Contact #{record.id} was destroyed." ) }
      #  end
      def after_destroy()  end
      def destroy_with_callbacks #:nodoc:
        return false if callback(:before_destroy) == false
        result = destroy_without_callbacks
        callback(:after_destroy)
        result
      end

      private
        def callback(method)
          result = run_callbacks(method) { |result, object| false == result }

          if result != false && respond_to?(method)
            result = send(method)
          end

          notify(method)

          return result
        end

        def notify(method) #:nodoc:
          self.class.changed
          self.class.notify_observers(method, self)
        end
    end
  end
