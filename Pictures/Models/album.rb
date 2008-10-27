require 'in_memory_model'
require 'picture'
require 'person'

class Album < InMemoryModel
  belongs_to :person
  has_many :pictures

  def self.find_for(person, options)
    find_all_by_person_id(person.id)
  end
  
  def self.search(term, options={})
    options = {:limit => 20, :order => "created_at DESC"}.merge(options)
    unless term.blank?
      options.merge!(:conditions => ['name LIKE ?', "%#{term}%"])      
    end
    all(options)
  end

end
