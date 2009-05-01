require File.dirname(__FILE__) + '/extensions.rb'

describe "ObjectExtensions" do 
  
  describe "when asking for null" do
    it "should return true for a null value" do
       Workarounds.is_null(nil).should.be.true?
    end
    
    it "should return false for an object instance" do
      ObjectExtensions.is_null(System::Object.new).should.be.false?
    end
  end
  
  describe "when asking for not null" do
    it "should return false for a null value" do
      Workarounds.is_not_null(nil).should.be.false?
    end
    
    it "should return true for an object instance" do
      ObjectExtensions.is_not_null(Object.new).should.be.true?
    end
  end
 
  
end