$:.unshift "#{File.dirname(__FILE__)}/../lib"
#$:.unshift "#{File.dirname(__FILE__)}/../../activesupport/lib"
$:.unshift File.dirname(__FILE__)

require 'test/unit'
require 'active_model'
require 'active_model/state_machine'
require 'active_support/callbacks' # needed by ActiveModel::TestCase
require 'active_support/test_case'

def uses_gem(gem_name, test_name, version = '> 0')
  require 'rubygems'
  gem gem_name.to_s, version
  require gem_name.to_s
  yield
rescue LoadError
  $stderr.puts "Skipping #{test_name} tests. `gem install #{gem_name}` and try again."
end

# Wrap tests that use Mocha and skip if unavailable.
unless defined? uses_mocha
  def uses_mocha(test_name, &block)
    uses_gem('mocha', test_name, '>= 0.5.5', &block)
  end
end

begin
  require 'rubygems'
  require 'ruby-debug'
  Debugger.start
rescue LoadError
end

ActiveSupport::TestCase.send :include, ActiveSupport::Testing::Default

require 'active_support/version'
module ActiveModel
  class TestCase < ActiveSupport::TestCase
    include ActiveSupport::Testing::Default
    if ActiveSupport::VERSION::STRING == "2.1.1"
      #backported from edge
      def self.test(name, &block)
        test_name = "test_#{name.gsub(/\s+/,'_')}".to_sym
        defined = instance_method(test_name) rescue false
        raise "#{test_name} is already defined in #{self}" if defined
        if block_given?
          define_method(test_name, &block)
        else
          define_method(test_name) do
            flunk "No implementation provided for #{name}"
          end
        end
      end
    end
  end
end
