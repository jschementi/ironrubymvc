require 'in_memory_model'

class Picture < InMemoryModel
  has_many :comments
  belongs_to :album
  belongs_to :person
  has_many :taggings, :as => :taggable
  has_many :tags, :through => :taggings

  def self.find_for(person, options)
    find_all_by_person_id(person.id, options)
  end
  
  #FIXME: still SQL
  def self.search(term, options={})
    options = {:limit => 20, :order => "created_at DESC"}.merge(options)
    unless term.blank?
      options.merge!(:conditions => ['title LIKE ?', "%#{term}%"])      
    end
    all(options)
  end

  def self.tagged_with(tag_name)
    Picture.all(:include => [:taggings => :tag], :conditions => {:tags => {:name => tag_name}})
  end
  
  def data=(data)
    if data.respond_to?(:read)
      self.filename = data.original_filename
      data = data.read
    end
    write_attribute(:data, data)
  end

  def to_xml
    super(:except => :data)
  end  
end
