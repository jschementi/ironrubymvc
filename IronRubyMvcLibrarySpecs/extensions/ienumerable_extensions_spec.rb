require File.dirname(__FILE__) + "/extensions.rb"

describe "IEnumerableExtensions" do
  
  before do
    @collection = [1, 2, 3, 4, 5, 6, 7, 8]
    @generic_collection = System::Collections::Generic::List.of(System::Int32).new
    @generic_collection.add 1
    @generic_collection.add 2
    @generic_collection.add 3
    @generic_collection.add 5
    @generic_collection.add 6
    @generic_collection.add 7
    @generic_collection.add 8
    @generic_collection.add 9
  end
  
  it "should iterate over an untyped collection" do
    counter, result = 0, 0
    iterator = System::Action.of(System::Object).new do |item|
      counter += 1
      result += item
    end
    IEnumerableExtensions.for_each(@collection, iterator)
    counter.should.be 8
    result.should.be 36
  end
  
end