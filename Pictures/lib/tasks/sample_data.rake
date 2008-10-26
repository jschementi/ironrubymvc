require 'erb'
namespace :sample_data do
  desc "Load sample data from sample_data/ fixture files into the current environment's database.  Load specific fixtures using FIXTURES=x,y"
  task :load => [:environment,"db:migrate"] do
    
    require 'active_record/fixtures'
    
    ActiveRecord::Base.establish_connection(RAILS_ENV.to_sym)
    (ENV['FIXTURES'] ? ENV['FIXTURES'].split(/,/) : Dir.glob(File.join(RAILS_ROOT, 'sample_data', '*.{yml}'))).each do |fixture_file|
      data_file = File.join(RAILS_ROOT, 'sample_data', File.basename(fixture_file))
      if File.exists?(data_file)
        Fixtures.create_fixtures('sample_data', File.basename(fixture_file, '.*')) if File.exists?(data_file)
        puts "Loaded #{File.basename(fixture_file, '.*').camelcase.singularize.constantize.count} #{File.basename(fixture_file, '.*').camelcase}" rescue nil
      end
    end
    Dir.entries("sample_data/albums").each do |album_name|
      album = Album.find_by_name(album_name)
      if album
        Dir.glob("sample_data/albums/#{album.name}/*.jpg").each do |image|
          next if image.include?("_small")
          puts "Loading #{File.basename(image)} into #{album.name}"
          data = File.open(image, "rb") {|f| f.read}
          thumbnail_data = File.exist?(image.gsub(/\.jpg/, "_small.jpg")) ? File.open(image.gsub(/\.jpg/, "_small.jpg"), "rb") {|f| f.read} : ""
          album.pictures.create :title => File.basename(image)[0...-4].humanize, :mime_type => "image/jpg", :data => data, :thumbnail_data => thumbnail_data, :person => album.person, :filename => File.basename(image)
        end
      end
    end
  end
  
  desc "Load sample data from sample_data/ fixture files into the current environment's database.  Load specific fixtures using FIXTURES=x,y"
  task :marshal => [:environment,"db:migrate"] do
    
    require 'active_record/fixtures'
    BASE_FOR_MARSHAL = File.join(RAILS_ROOT, "sample_data", "marshal")
    FileUtils.mkdir_p BASE_FOR_MARSHAL
    ActiveRecord::Base.establish_connection(RAILS_ENV.to_sym)
    (ENV['FIXTURES'] ? ENV['FIXTURES'].split(/,/) : Dir.glob(File.join(RAILS_ROOT, 'sample_data', '*.{yml}'))).each do |fixture_file|
      data_file = File.join(RAILS_ROOT, 'sample_data', File.basename(fixture_file))
      if File.exists?(data_file)
puts data_file        
         data = YAML.load(ERB.new(File.read(data_file)).result)
         marshal_file_name = File.basename(data_file).sub(/yml$/, "dat")
puts marshal_file_name         
         File.open(File.join(BASE_FOR_MARSHAL, marshal_file_name), "wb") {|f| f.write Marshal.dump(data)}
#        Fixtures.create_fixtures('sample_data', File.basename(fixture_file, '.*')) if File.exists?(data_file)
#        puts "Loaded #{File.basename(fixture_file, '.*').camelcase.singularize.constantize.count} #{File.basename(fixture_file, '.*').camelcase}" rescue nil
      end
    end
    pictures = {}
#     Dir.entries("sample_data/albums").each do |album_name|
puts "Loading albums and pictures"
     Album.all.each do |album|
       # album = Album.find(:first, :conditions => {:name => album_name})
puts album  .name
       if album
         Dir.glob("sample_data/albums/#{album.name}/*.jpg").each do |image|
           next if image.include?("_small")
           puts "Loading #{File.basename(image)} into #{album.name}"
           data = File.open(image, "rb") {|f| f.read}
           thumbnail_data = File.exist?(image.gsub(/\.jpg/, "_small.jpg")) ? File.open(image.gsub(/\.jpg/, "_small.jpg"), "rb") {|f| f.read} : ""
           picture = album.pictures.create :title => File.basename(image)[0...-4].humanize, :mime_type => "image/jpg", :data => data, :thumbnail_data => thumbnail_data, :person_id => album.person_id, :filename => File.basename(image)           
           pictures[picture.id] = picture.instance_variable_get("@attributes").merge("id" => picture.id)
         end
       end
     end
     File.open(File.join(BASE_FOR_MARSHAL, "pictures.dat"), "wb") {|f| f.write Marshal.dump(pictures)}
  end
  
end
