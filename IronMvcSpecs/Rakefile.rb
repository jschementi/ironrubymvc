require 'ftools'

def tgt_dir
 File.dirname(__FILE__) + "/bin"
end 
  
def mono?
  !ENV['mono'].nil? && ENV['mono'] > 0
end

desc "The default task is to run all the specs"
task :default => :spec

desc "Runs all the specs"
task :spec => [:copy_binaries, :workarounds, 'spec:extensions']

namespace :spec do 
  desc "runs the specs for the extensions"
  task :extensions do
    puts "starting extension specs"
    system "ibacon #{Dir.glob('extensions/**/*_spec.rb').join(' ')}"
#    specs = Dir.glob('extensions/**/*_spec.rb')
#    require 'rubygems'
#    require 'bacon'
#    Bacon.extend Bacon.const_get('SpecDoxOutput')
#    Bacon.summary_on_exit
#    
#    specs.each { |spec|
#      load File.dirname(__FILE__) + "/#{spec}"
#    }
  end
end

desc "Copies the binaries from System.Web.Mvc.IronRuby to the specs folder"
task :copy_binaries do
  src_dir = File.expand_path(File.dirname(__FILE__) + "/../IronRubyMvc/bin/Debug")
  files = Dir.glob("#{src_dir}/*")
  files.each { |f| File.copy("#{f}", "#{tgt_dir}/#{File.basename(f)}") }
end

desc "Compiles the workarounds"
task :workarounds do
  Dir.chdir(File.dirname(__FILE__))
  files = Dir.glob("workarounds/*.cs").collect { |f| f.gsub(/\//, "\\")  }.join(" ")
  system "csc /noconfig /target:library /debug+ /debug:full /out:bin\\BugWorkarounds.dll /reference:bin\\IronRuby.dll /reference:bin\\Microsoft.Scripting.dll /reference:bin\\Microsoft.Scripting.Core.dll /reference:bin\\System.Web.Mvc.IronRuby.dll #{files}"
end
