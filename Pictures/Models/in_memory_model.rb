class InMemoryModel < ActiveModel::Base
  self.logger = nil
  class FileDB
    class << self
      def mock_database
        $mock_database ||= {}
      end
    
      def load_mock_database!(refresh = false)
        return if $already_loaded_database && !refresh
        load_data!
        $already_loaded_database = true
      end
    
      def load_data!
        data_files.each do |marshal_file|
          mock_database[key_name_for(marshal_file)] = Marshal.load(File.open(marshal_file, "rb"){|f| f.read})
        end
      end
    
      def write_data!
        data_files.each do |marshal_file_name|
          File.open(marshal_file_name, "wb"){|f| f.write Marshal.dump(mock_database[key_name_for(marshal_file_name)])}
        end
      end
      
      def data_files
        Dir[File.join(File.dirname(__FILE__), "/../sample_data/marshal/*.dat")]
      end
    
      def key_name_for(marshal_file)
        File.basename(marshal_file).sub(/\.dat$/, '')
      end
    
      def columns(class_name)
        SCHEMA[class_name].map{|name| ActiveModel::SchemaDefinitions::Column.new(name.to_s, nil)}
      rescue
        raise "#{$!}: #{class_name}"
      end
      
      def create(klass, *args)
        attributes = args.extract_options!
        id = id_for(attributes)
        scope = klass.scope(:create)
        attributes.merge!(scope) if scope
        attributes.merge!('id' => id)
        table_for(klass)[id] = attributes
        write_data!
        instantiate(klass, attributes)
      end
      
      def count(klass, *args)
        # FIXME: no filtering yet
        options = args.extract_options!
        scope = klass.scope(:find)
        options.merge!(scope) if scope
        filtered(records_for(klass), options).size
      end
      
      def find(klass,*args)
        load_mock_database!
        options = args.extract_options!
        scope = klass.scope(:find)
        options.merge!(scope) if scope
        
        case args.first
          when :first then find_initial(klass, options)
          when :last  then find_last(klass, options)
          when :all   then find_every(klass, options)
          else             find_from_ids(klass, args, options)
        end      
      end
    
      def find_initial(klass, options)
        filtered(records_for(klass), options).first
      end
    
      def find_last(klass, options)
        records_for(klass).last
      end
    
      def find_every(klass, options)
        #FIXME: This doesn't filter yet
        
        
        records = filtered(records_for(klass), options)
        options[:limit] ? records[0...options[:limit]] : records
      end
    
      def find_from_ids(klass, args, options)
        #FIXME: not filtering or using options
        results = args.map do |the_id|
          records_for(klass).detect{|record| record.id.to_s == the_id.to_s}
          
        end.compact
        results.size <= 1 ? results.first : results
      end
    
      def records_for(klass)
        hash = table_for(klass)
        (hash ? hash.values : []).map{|attributes| instantiate(klass, attributes)}
      end
      
      private
      
        def instantiate(klass, attributes)
          returning o = klass.new do
            attributes.stringify_keys!
            # the_id = attributes.delete("id")
            # attributes.each do |key, value|              
            #   o.send("#{key}=", value)
            # end
            # if the_id
            #   o.instance_variable_get("@attributes")["id"] = the_id
            # end
            o.instance_variable_set("@attributes", attributes)
            o.instance_variable_set("@new_record", false)
          end
        end
        
        def filtered(result_set, options)
          if options[:conditions]
            result_set.select do |result|
              result_matches?(options[:conditions], result)
            end
          else
            result_set
          end
        end
      
        def result_matches?(conditions, result)
          conditions.all? do |attribute, value| 
            value.kind_of?(Hash) ? result.send(attribute).all?{|r| result_matches?(value, r)} : result.send(attribute) == value
          end
        end
        
        def table_for(klass)
          mock_database[klass.name.downcase.pluralize] ||= {}
        end
        
        def id_for(attributes)
          #Just a hack to get us through
          #FIXME: not really safe
          Digest::SHA1.hexdigest(attributes.inspect)
        end
    end
    SCHEMA = {
      "Tag" => ["id", "name"],
      "Tagging" => ["id", "taggable_type", "taggable_id", "tag_id"],
      "Picture" => ["id",
                  :title,
                  :person_id,
                  :mime_type,
                  :created_at,
                  :updated_at,
                  :data,
                  :filename,
                  :album_id,
                  :thumbnail_data],
      "Person" => ["id", 
                  "first_name",
                  "last_name",
                  "password",
                  "salt",
                  "created_at",
                  "updated_at",
                  "email"],

      "Album" => ["id", "name"],
      "Comment" => 
                ["body",
                "person_id",
                "picture_id",
                "created_at",
                "updated_at"]
      
    
    }    
  end
  
  def create
    the_id = self.class.create(@attributes)
  end

  # def marshal_load(a)
  #   require 'ruby-debug'; debugger 
  #   instance_variable_set("@attributes", a)
  # end
  # 
  # def marshal_dump
  #   @attributes
  # end
  
  class << self
    def db
      FileDB
    end
    # This is mostly just placeholer stuff
    def columns
      db.columns(self.name)
    # rescue => e
    #   raise "#{self.name}: #{e}"
    end 
    
    def primary_key
      "id"
    end
    
    def count(*args)
      db.count(self, *args)
    end
    
    def all(*args)
      find(:all, *args)
    end
    
    def first(*args)
      find(:first, *args)
    end
    
    def find(*args)
      db.find(self, *args)
    end
    
    def find_all_by_person_id(person_id, options = {})
      all(options).select{|object| object.person_id.to_s == person_id.to_s}
    end    
   
    def find_or_create_by_name(name, options = {})
       all(options).detect{|object| object.name == name} || db.create(self, options.merge(:name => name))
    end
    
    def create(*args)
      db.create(self, *args)
    end
  end
end