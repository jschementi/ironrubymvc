module ActiveModel
  module Associations
    # This is the proxy that handles a has many association.
    #
    # If the association has a <tt>:through</tt> option further specialization
    # is provided by its child HasManyThroughAssociation.
    class HasManyAssociation < AssociationCollection #:nodoc:
      def construct_scope
        create_scoping = {}
        set_belongs_to_association_for(create_scoping)
        {
          :find => { :conditions => create_scoping, :readonly => false, :order => @reflection.options[:order], :limit => @reflection.options[:limit], :include => @reflection.options[:include]},
          :create => create_scoping
        }
      end
      
    end
  end
end