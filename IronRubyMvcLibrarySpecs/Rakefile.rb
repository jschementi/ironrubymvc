


def mono?
  !ENV['mono'].nil? && ENV['mono'] > 0
end

desc "Runs all the specs"
task :spec => ['spec:extensions']

namespace :spec do 
  desc "runs the specs for the extensions"
  task :extensions do
    system "ibacon #{Dir.glob('extensions/**/*_spec.rb').join(' ')}"
  end
end

