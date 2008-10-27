ruby_lib = "C:/Users/jimmysch/Projects/vsl1/Merlin/External/Languages/Ruby/ruby-1.8.6/lib"
ironruby_lib = "C:/Users/jimmysch/Projects/vsl1/Merlin/Main/Languages/Ruby/Libs"
$: << ironruby_lib
$: << "#{ruby_lib}/ruby/site_ruby/1.8"
$: << "#{ruby_lib}/ruby/site_ruby"
$: << "#{ruby_lib}/ruby/1.8"
$: << "."
$: << File.dirname(__FILE__) + "/../lib/activesupport/lib"
$: << File.dirname(__FILE__) + "/../lib/activerecord/lib"
$: << File.dirname(__FILE__) + "/../lib/activemodel/lib"

require "active_model"
require 'active_record'
require 'in_memory_model'
require 'album'
require 'comment'
require 'follow'
require 'person'
require 'picture'
require 'tag'
require 'tagging'
