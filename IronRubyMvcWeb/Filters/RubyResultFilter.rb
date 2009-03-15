class RubyResultFilter < ResultFilter      
  
  def on_result_executing(context)
    # put before result filtering code here
  end
  
  def on_result_executed(context)
    # put after result filtering code here
  end
end