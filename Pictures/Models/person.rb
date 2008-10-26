require 'digest/sha1'
require 'in_memory_model'

class Person < InMemoryModel
  before_save :generate_salt!  
  before_save :hash_password!

  has_many :pictures
  has_many :comments
  has_many :taggings
  validates_presence_of :email, :first_name, :last_name
  validates_confirmation_of :password
  validates_uniqueness_of :email
  
  has_many :albums
  
  def self.search(term)
    options = {:limit => 40}
    unless term.blank? 
      options.merge!(:conditions => ['name LIKE ?', "%#{term}%"])
    end
    all(options)
  end
  
  def self.authenticate(email, password)
    person = find(:first, :conditions => {:email => email})  
    (person && person.has_password?(password)) ? person : nil
  end

  def name
    [first_name, last_name].join(" ")
  end
  
  def has_password?(password)
    hash_for(self.salt + password) == self.password
  end
  
  private
  
    def hash_password!
      self.password = hash_for(salt + self.password)
    end  

    def generate_salt!
      self.salt ||= hash_for(Time.now.to_s + first_name.object_id.to_s)
    end

    def hash_for(string)
      Digest::SHA1.hexdigest(string)
    end  
  
end
