require 'in_memory_model'

class Comment < InMemoryModel
  belongs_to :picture
  belongs_to :person
end
