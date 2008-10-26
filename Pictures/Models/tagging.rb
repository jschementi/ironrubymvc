require 'in_memory_model'

class Tagging < InMemoryModel
  belongs_to :tag
  belongs_to :person
  belongs_to :taggable, :polymorphic => true
end
