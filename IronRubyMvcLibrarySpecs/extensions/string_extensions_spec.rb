require File.dirname(__FILE__) + '/extensions.rb'

describe "StringExtensions" do 
  
  describe "when asking for null or blank" do
    it "should return true for a null value" do
      StringExtensions.is_null_or_blank(nil).should.be.true?
    end
    
    it "should return true for an empty value" do
      StringExtensions.is_null_or_blank("").should.be.true?
    end
    
    it "should return true when the string contains only spaces" do
      StringExtensions.is_null_or_blank("      ").should.be.true?
    end
    
    it "should return false when the string contains a value" do
      StringExtensions.is_null_or_blank("a string value").should.be.false?
    end
        
  end
  
  describe "when asking for not null or blank" do
    
    it "should return false when value is null" do
      StringExtensions.is_not_null_or_blank(nil).should.be.false?
    end
    
    it "should return false for an empty value" do
      StringExtensions.is_not_null_or_blank("").should.be.false?
    end
    
    it "should return false when the string contains only spaces" do
      StringExtensions.is_not_null_or_blank("      ").should.be.false?
    end
    
    it "should return true when the string contains a value" do
      StringExtensions.is_not_null_or_blank("a string value").should.be.true?
    end
    
  end
  
  describe "when asking to format a string" do 
    
    it "should return a properly formatted string" do
      expected = "This is the 1 and only Format test at #{System::DateTime.now.to_short_date_string}".to_clr_string
      actual = StringExtensions.formatted_with("This is the {0} and only {1} test at {2}".to_clr_string, 1, "Format", System::DateTime.now.to_short_date_string)
      
      expected.should == actual
    end
    
  end

end