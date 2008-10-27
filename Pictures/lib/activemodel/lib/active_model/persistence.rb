module ActiveModel
  #FIXME: These will probably raise NotImplemented
  ##
  # This module implements no-op versions of the methods an extending framework
  # needs in order to implement persistence.
  # 
  # ActiveModel::Persistence is here to provide blank implementations of these methods
  # for the benefit of other modules which depend on them (e.g. ActiveModel::Callbacks).
  #
  module Persistence
    def save(*args, &block)
      create_or_update
    end
    
    def create(*args, &block)
      #persistence_driver.create(self, *args, &block)
    end
    
    def update(*args, &block)
      #persistence_driver.update(self, *args, &block)
    end
    
    def destroy(*args, &block)
      #persistence_driver.destroy(self, *args, &block)      
    end
    
    def new_record?
      defined?(@new_record) && @new_record
    end
    
    def create_or_update(*args, &block)
      result = new_record? ? create : update
      result != false
    end
  end
end