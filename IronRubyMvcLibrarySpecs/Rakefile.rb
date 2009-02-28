


namespace :spec do 
  desc "runs the specs for the extensions"
  task :extensions do
    system "ibacon #{Dir.glob('extensions/**/*_spec.rb').join(' ')}"
  end
end