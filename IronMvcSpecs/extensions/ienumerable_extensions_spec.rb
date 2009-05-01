require File.dirname(__FILE__) + "/extensions.rb"

describe "IEnumerableExtensions" do
  
  before do
    @collection = [1, 2, 3, 4, 5, 6, 7, 8]
    @generic_collection = System::Collections::Generic::List.of(Fixnum).new
    @generic_collection.add 1
    @generic_collection.add 2
    @generic_collection.add 3
    @generic_collection.add 4
    @generic_collection.add 5
    @generic_collection.add 6
    @generic_collection.add 7
    @generic_collection.add 8
    @generic_collection.add 9
  end
  
  describe "iteration" do
    it "should iterate over an untyped collection" do
      counter, result = 0, 0
      p = Proc.new {|item| counter += 1; result += item }
      iterator = Workarounds.wrap_proc(p)
      IEnumerableExtensions.for_each(@collection, iterator)
      counter.should == 8
      result.should == 36
    end
    
    it "should iterate over a generic collection" do
      counter, result = 0, 0
      p = Proc.new {|item| counter += 1; result += item }
      iterator = Workarounds.method(:wrap_proc).of(Fixnum).call(p)
      IEnumerableExtensions.method(:for_each).of(Fixnum).call(@generic_collection, iterator)
      counter.should == 9
      result.should == 45
    end
  end
  
  describe "empty checking" do
  
    it "should raise an ArgumentException for a non-generic method call with null" do
      should.raise(System::NullReferenceException){ Workarounds.is_empty(nil) }
    end
    
    it "should raise an ArgumentException for a non-generic method call with null" do
      should.raise(System::NullReferenceException){ Workarounds.method(:is_empty).of(System::String).call(nil) }
    end
    
    it "should be true for an empty non-generic collection" do
      IEnumerableExtensions.is_empty([]).should.be.true?
    end
    
    it "should be true for an empty generic collection" do
      coll = System::Collections::Generic::List.of(String).new
      IEnumerableExtensions.method(:is_empty).of(String).call(coll).should.be.true?
    end
    
    it "should be false for an non-empty non-generic collection" do
      IEnumerableExtensions.is_empty([3]).should.be.false?
    end
    
    it "should be false for a non-empty generic collection" do
      coll = System::Collections::Generic::List.of(String).new
      coll.add "a string"
      IEnumerableExtensions.method(:is_empty).of(String).call(coll).should.be.false?
    end
  
  end 
  
end