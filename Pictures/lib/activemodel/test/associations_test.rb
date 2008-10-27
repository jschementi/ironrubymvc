require File.expand_path(File.join(File.dirname(__FILE__), 'test_helper'))
require 'ostruct'
class AssociationsTest < ActiveModel::TestCase

  class StubPersistenceDriver
    def columns(klass)
      []
    end
    def primary_key
      "stub_primary_key"
    end
  end
  
  class ActiveModel::Base
    def self.primary_key
      "id"
    end
  end
  
  class Something < ActiveModel::Base
    def self.tada
      "TADA!"
    end
  end
     
  class Thing < ActiveModel::Base
  end 
  
  class Example < ActiveModel::Base

    has_many :somethings
    belongs_to :thing
  end


  test "Model supports has_many class-level call" do
    assert_nothing_raised do
      Class.new(ActiveModel::Base) do
        has_many :somethings
      end
    end
  end
  
  test "has_many creates a reflection which can be inspected" do
    assert_not_nil Example.reflect_on_association(:somethings)
  end
  
  test "has_many generates accessor methods based on reflection name" do
    assert_respond_to Example.new, :somethings
    assert_respond_to Example.new, :somethings=
    assert_respond_to stubbed_example_model.somethings, :each
  end
  
  test "can do .class (without blowing up)" do
    stubbed_example_model.somethings.class
  end
  
  test "proxies missing methods through to the model which the association is configured for" do
    the_proxy = stubbed_example_model.somethings
    assert_equal "TADA!", the_proxy.tada
  end
  
  
  test "belongs_to generates accessor methods" do
    assert_respond_to Example.new, :thing
    assert_respond_to Example.new, :thing=
  end
  
  test "belongs_to accessor methods can be used to set and get property" do
    assert_nothing_raised do
      thing = stubbed_thing_model
      e = Example.new
      e.thing = thing
      assert_equal thing, e.thing
    end
  end
  
  
  private
    def stubbed_example_model
      returning example = Example.new do
        example.stubs(:persistence_driver).returns(stub('persistence driver', :new_record? => false, :find => []))
      end
    end
    
    def stubbed_thing_model
      returning thing = Thing.new do
        thing.stubs(:persistence_driver).returns(stub('persistence driver',:new_record? => false))
      end
    end
end