require File.expand_path(File.join(File.dirname(__FILE__), 'test_helper'))
require 'ostruct'
class AssociationsTest < ActiveModel::TestCase
  class ExampleModel < ActiveModel::Base
    class << self
      def primary_key
        "test"
      end
      def columns
        ['foo', 'bar'].map{|name| ActiveModel::SchemaDefinitions::Column.new(name, nil)}
      end
    end
  end
  
  test "Attributes in model generate methods" do
    model = ExampleModel.new
    assert_nil model.foo
    assert_nil model.bar
    model.foo = "Test"
    assert_equal "Test", model.foo
  end
end