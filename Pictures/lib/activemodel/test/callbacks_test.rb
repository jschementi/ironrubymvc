require File.expand_path(File.join(File.dirname(__FILE__), 'test_helper'))

class ModelForTesting < ActiveModel::Base
  
  attr_reader :logged_calls
  attr_accessor :persistence_driver

  def log_call(call_name)
    @logged_calls ||= []
    @logged_calls << call_name
  end
end

class WithNamedCallback < ModelForTesting
  before_save :log_before_save
   def log_before_save
     log_call(:before_save)
   end
end

class WithHardcodedCallback < ModelForTesting
  def before_save
    log_call(:before_save_in_method)
  end
end

class AllThreeWays < ModelForTesting
  def before_save
    log_call(:as_method)
  end
  
  before_save do |record|
    record.log_call(:as_proc)
  end
  
  before_save :named_callback
  def named_callback
    log_call(:named_callback)
  end
end

class VariousHookedEvents < ModelForTesting
  before_save :saved
  before_destroy :destroyed
  [:saved, :destroyed].each do |name|
    define_method(name) do
      log_call(name)
    end
  end
end

class ThreeSaves < ModelForTesting
  before_save :first
  before_save :second
  before_save :third
  def method_missing(method_name, *args)
    log_call(method_name)
  end
end

class CallbacksTest < ActiveModel::TestCase
  uses_mocha "Callback tests" do
    test "Calls before save defined as method" do
      model = model_of(WithNamedCallback)
      model.save
      assert_equal [:before_save], model.logged_calls
    end
  
    test "Calls before save callback methods" do
      model = model_of(WithHardcodedCallback)
      model.save
      assert_equal [:before_save_in_method], model.logged_calls
    end
  

    test "Callbacks are added in the order they're specified" do
      model = model_of(ThreeSaves)
      model.save
      assert_equal [:first, :second, :third], model.logged_calls
    end
  
    test "Callbacks can be specified as methods, procs, or named callbacks" do
      model = model_of(AllThreeWays)
      model.save
      assert ([:as_method, :as_proc, :named_callback] - model.logged_calls).blank?, "Should have defined callbacks all 3 possible ways."
    end
  
  
    test "Can mix callbacks of different types" do
      model = model_of(VariousHookedEvents, [:new_record?, :update, :destroy])
      model.save
      model.destroy
    end
  end
  
  private
  def model_of(klass, messages_to_expect = [:new_record?, :update])
    returning (model = klass.new) do
#      model.persistence_driver = persistence_driver = stub
#      messages_to_expect.each{|message| persistence_driver.expects(message)}
    end
  end
end