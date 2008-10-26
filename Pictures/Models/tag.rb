require 'in_memory_model'

class Tag < InMemoryModel
  def self.all(*args)
    find(:all, *args)
  end
end
